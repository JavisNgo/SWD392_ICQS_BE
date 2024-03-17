using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using SimpleEmailApp.Models;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;

namespace SWD_ICQS.Services.Implements
{
    public class RequestService : IRequestService
    {

        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public RequestService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        public bool checkExistedRequestId(int id)
        {
            try
            {
                var request = unitOfWork.RequestRepository.GetByID(id);

                if (request != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public RequestViewForGet GetRequestView(int id)
        {
            var request = unitOfWork.RequestRepository.GetByID(id);
            var requestView = _mapper.Map<RequestViewForGet>(request);

            try
            {
                var requestDetail = unitOfWork.RequestDetailRepository.Find(r => r.RequestId == requestView.Id);
                if (requestDetail.Any())
                {
                    requestView.requestDetailViews = new List<RequestDetailView>();
                    foreach (var item in requestDetail)
                    {
                        requestView.requestDetailViews.Add(_mapper.Map<RequestDetailView>(item));
                    }

                    if (requestView.requestDetailViews.Any())
                    {
                        foreach (var item in requestView.requestDetailViews)
                        {
                            var product = unitOfWork.ProductRepository.GetByID(item.ProductId);
                            item.ProductView = _mapper.Map<ProductsView>(product);

                            var productImages = unitOfWork.ProductImageRepository.Find(p => p.ProductId == product.Id).ToList();
                            if (productImages.Any())
                            {
                                item.ProductView.productImagesViews = new List<ProductImagesView>();
                                foreach (var image in productImages)
                                {
                                    image.ImageUrl = $"https://localhost:7233/img/productImage/{image.ImageUrl}";
                                    item.ProductView.productImagesViews.Add(_mapper.Map<ProductImagesView>(image));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return requestView;
        }

        public IEnumerable<RequestViewForGet> GetRequestsByContractorId(int contractorId)
        {
            var requests = unitOfWork.RequestRepository.Get(filter: c => c.ContractorId == contractorId).ToList();
            var requestViews = new List<RequestViewForGet>();

            foreach (var request in requests)
            {
                var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);
                if (contractor == null)
                {
                    continue;
                }

                var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                var requestView = _mapper.Map<RequestViewForGet>(request);
                requestView.ContractorName = contractor.Name;
                requestView.CustomerName = customer != null ? customer.Name : null;

                requestViews.Add(requestView);
            }

            return requestViews;
        }

        public IEnumerable<Requests> GetAllRequests()
        {
            return unitOfWork.RequestRepository.Get().ToList();
        }

        public IEnumerable<RequestViewForGet> GetRequestsByCustomerId(int customerId)
        {
            var requests = unitOfWork.RequestRepository.Get(filter: c => c.CustomerId == customerId).ToList();
            var requestViews = new List<RequestViewForGet>();
            foreach (var request in requests)
            {
                var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);
                if (customer == null)
                {
                    continue;
                }
                var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                var requestView = _mapper.Map<RequestViewForGet>(request);
                requestView.CustomerName = customer.Name;
                requestView.ContractorName = contractor != null ? contractor.Name : null;

                requestViews.Add(requestView);
            }

            return requestViews;
        }


        public bool AcceptRequest(int id)
        {
            try
            {

                var existingRequest = unitOfWork.RequestRepository.GetByID(id);
                if (existingRequest == null)
                {
                    throw new Exception($"Request with ID : {id} not found");
                }

                existingRequest.Status = Requests.RequestsStatusEnum.ACCEPTED;
                existingRequest.TimeOut = DateTime.Now.AddDays(14);
                unitOfWork.RequestRepository.Update(existingRequest);
                unitOfWork.Save();

                var appointment = new Appointments
                {
                    CustomerId = existingRequest.CustomerId,
                    ContractorId = existingRequest.ContractorId,
                    RequestId = existingRequest.Id,
                    MeetingDate = DateTime.Now.AddDays(7),
                    Status = Appointments.AppointmentsStatusEnum.PENDING
                };
                unitOfWork.AppointmentRepository.Insert(appointment);
                unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                 throw new Exception($"An error occurred while accepting the request. Error message: {ex.Message}");
            }
        }

        public bool MarkMeetingAsCompleted(int id)
        {
            try
            {
                var existingAppointment = unitOfWork.AppointmentRepository.GetByID(id);
                if (existingAppointment == null)
                {
                    throw new Exception($"Appointment with ID : {id} not found");
                }

                existingAppointment.Status = Appointments.AppointmentsStatusEnum.COMPLETED;
                unitOfWork.AppointmentRepository.Update(existingAppointment);
                
                var request = unitOfWork.RequestRepository.GetByID(existingAppointment.RequestId);
                if (request == null)
                {
                    throw new Exception("Request not found");
                }

                request.TimeOut = DateTime.Now.AddDays(14);
                request.Status = Requests.RequestsStatusEnum.COMPLETED;
                unitOfWork.RequestRepository.Update(request);
                

                var deposit = unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == request.Id).FirstOrDefault();
                if (deposit != null)
                {
                    throw new Exception("Deposit has been created");
                }
                if (request.Status == Requests.RequestsStatusEnum.COMPLETED &&
                    existingAppointment.Status == Appointments.AppointmentsStatusEnum.COMPLETED)
                {
                    if(request.TotalPrice.HasValue && request.TotalPrice > 0)
                    {
                        var newDeposit = new DepositOrders
                        {
                            RequestId = request.Id,
                            DepositPrice = (request.TotalPrice.Value * 2/10),
                            Status = DepositOrders.DepositOrderStatusEnum.PENDING

                        };
                        unitOfWork.DepositOrdersRepository.Insert(newDeposit);
                        unitOfWork.Save();
                        return true;
                    }
                    
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while marking meeting as completed. Error message: {ex.Message}");
            }
        }

        

        public RequestView AddRequest(RequestView requestView)
        {
           try
            {
                var contractor = unitOfWork.ContractorRepository.GetByID(requestView.ContractorId);
                var customer = unitOfWork.CustomerRepository.GetByID(requestView.CustomerId);

                if (contractor == null || customer == null)
                {
                    throw new Exception("ContractorID or CustomerID not found");
                }

                if (requestView.TotalPrice < 0)
                {
                    throw new Exception("Price must be larger than 0");
                }
                
                var request = _mapper.Map<Requests>(requestView);
                string code = $"P_{requestView.CustomerId}_{requestView.ContractorId}_{GenerateRandomCode(10)}";
                bool checking = true;
                while (checking)
                {
                    if (unitOfWork.ProductRepository.Find(p => p.Code == code).FirstOrDefault() != null)
                    {
                        code = $"P_{requestView.CustomerId}_{requestView.ContractorId}_{GenerateRandomCode(10)}";
                    }
                    else
                    {
                        checking = false;
                    }
                };
                request.Code = code;
                request.Status = 0;
                request.TimeIn = DateTime.Now;
                request.TimeOut = DateTime.Now.AddDays(7);

                unitOfWork.RequestRepository.Insert(request);
                unitOfWork.Save();

                var createdRequest = unitOfWork.RequestRepository.Find(r => r.Code == code).FirstOrDefault();

                if(requestView.requestDetailViews != null && createdRequest != null)
                {
                    if (requestView.requestDetailViews.Any())
                    {
                        foreach (var rd in requestView.requestDetailViews)
                        {
                            var existingProduct = unitOfWork.ProductRepository.Find(p => p.Id == rd.ProductId).FirstOrDefault();
                            if (existingProduct != null)
                            {
                                var requestDetails = new RequestDetails
                                {
                                    RequestId = createdRequest.Id,
                                    ProductId = rd.ProductId,
                                    Quantity = rd.Quantity
                                };
                                unitOfWork.RequestDetailRepository.Insert(requestDetails);
                                unitOfWork.Save();
                            }
                        }
                    }
                }
                var existingContractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);
                var existingCustomer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);
                if(existingContractor != null)
                {
                    if (existingCustomer != null)
                    {
                        EmailDto email = new EmailDto()
                        {
                            From = existingCustomer.Email,
                            To = existingContractor.Email,
                            Subject = request.Note,
                            Body = request.Note,
                            Price = (double)request.TotalPrice
                        };

                        SendEmail(email);
                    }
                }
                

                return requestView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the request. Error message: {ex.Message}");
            }
        }
        public void SendEmail(EmailDto request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(request.From));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Text) { Text = request.Body };

                using var smtp = new SmtpClient();
                smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(request.To, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
            }catch
            {
                throw new Exception("Error while send email");
            }
            
        }
        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        public RequestView UpdateRequest(int id, RequestView requestView)
        {
            try
            {
                var existingRequest = unitOfWork.RequestRepository.GetByID(id);
                if (existingRequest == null)
                {
                    throw new Exception($"Request with ID : {id} not found");
                }
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(requestView.ContractorId);
                var checkingCustomerId = unitOfWork.CustomerRepository.GetByID(requestView.CustomerId);
                if (checkingContractorID == null || checkingCustomerId == null)
                {
                    throw new Exception("ContractorID or CustomerID not found");
                }
                if (requestView.TotalPrice < 0)
                {
                    throw new Exception("Price must be larger than 0");
                }
                _mapper.Map(requestView, existingRequest);
                unitOfWork.RequestRepository.Update(existingRequest);
                unitOfWork.Save();
                return requestView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the constructProduct. Error message: {ex.Message}");
            }
        }
    }
}
