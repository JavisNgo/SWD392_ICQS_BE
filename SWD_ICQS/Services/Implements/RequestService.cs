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
using System.Net.Mail;
using System.Net;

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
                if (existingRequest.Status == Requests.RequestsStatusEnum.PENDING)
                {
                    existingRequest.Status = Requests.RequestsStatusEnum.ACCEPTED;
                    existingRequest.TimeOut = DateTime.Now.AddDays(14);
                    unitOfWork.RequestRepository.Update(existingRequest);
                    //unitOfWork.Save();
                    if (existingRequest != null)
                    {
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
                        var existingCustomer = unitOfWork.CustomerRepository.GetByID(existingRequest.CustomerId);

                        if (existingCustomer != null)
                        {

                            EmailDto email = new EmailDto()
                            {

                                To = existingCustomer.Email,
                                Subject = "Contractor accept your request",
                                Body = emailBodyForAccpept(existingCustomer, existingRequest, appointment)

                            };

                            SendMail(email);
                            return true;
                        }
                    }
                }

                return false;
                
            }
            catch (Exception ex)
            {
                 throw new Exception($"An error occurred while accepting the request. Error message: {ex.Message}");
            }
        }
        public string emailBodyForAccpept(Customers customer, Requests request, Appointments appointment)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Contractor accept your request</h1>
        <p>Dear {customer.Name},</p>
        <p>Contractor have been accepted your request</p>        
        <p>Note: {request.Note}</p>
        <p>Total price: {request.TotalPrice}</p>
        <p>Created date: {request.TimeIn}</p>        
        <p>Expired date: {request.TimeOut}</p>

        <p>Please check the appointment and meeting before expired appointment date.</p></br></br>
        <p>Expired appointment date: {appointment.MeetingDate}</p>
        <p>Best regards,<br/>[Admin from ICQS]</p>
    </div>
</body>
</html>
";
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
                if (existingAppointment.Status == Appointments.AppointmentsStatusEnum.PENDING)
                {
                    existingAppointment.Status = Appointments.AppointmentsStatusEnum.COMPLETED;
                    unitOfWork.AppointmentRepository.Update(existingAppointment);

                    var request = unitOfWork.RequestRepository.GetByID(existingAppointment.RequestId);
                    if (request == null)
                    {
                        throw new Exception("Request not found");
                    }
                    if(request.Status == Requests.RequestsStatusEnum.ACCEPTED) { 
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
                            if (request.TotalPrice.HasValue && request.TotalPrice > 0)
                            {
                                var newDeposit = new DepositOrders
                                {
                                    RequestId = request.Id,
                                    DepositPrice = (request.TotalPrice.Value * 2 / 10),
                                    Status = DepositOrders.DepositOrderStatusEnum.PENDING

                                };
                                unitOfWork.DepositOrdersRepository.Insert(newDeposit);
                                var newContract = new Contracts
                                {
                                    RequestId = request.Id,
                                    Status = 0
                                };
                                unitOfWork.ContractRepository.Insert(newContract);
                                unitOfWork.Save();
                                var existingCustomer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                                if (existingCustomer != null)
                                {

                                    EmailDto email = new EmailDto()
                                    {

                                        To = existingCustomer.Email,
                                        Subject = "The appoinment completed",
                                        Body = emailBodyForCompleteAppoinment(existingCustomer, request)

                                    };

                                    SendMail(email);
                                    return true;
                                }

                            }
                        }
                    }
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while marking meeting as completed. Error message: {ex.Message}");
            }
        }
        public string emailBodyForCompleteAppoinment(Customers customer, Requests request)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>The appointment completed</h1>
        <p>Dear {customer.Name},</p>
        <p>Contractor have been marked your appointment status as completed</p>        
        

        <p>Please check the deposit order and paying before expired date.</p></br></br>
        <p>Expired deposit order date: {request.TimeOut}</p>
        <p>Best regards,<br/>[Admin from ICQS]</p>
    </div>
</body>
</html>
";
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

                if (requestView.TotalPrice <= 0)
                {
                    throw new Exception("Price must be larger than 0");
                }
                foreach (var requestDetail in requestView.requestDetailViews)
                {
                    var product = unitOfWork.ProductRepository.Find(p => p.Id == requestDetail.ProductId && p.ContractorId == requestView.ContractorId).FirstOrDefault();

                    if (product == null)
                    {
                        throw new Exception($"Product with ID {requestDetail.ProductId} does not belong to contractor with ID {requestView.ContractorId}");
                    }
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
                    var existingContractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                    if (existingContractor != null)
                    {

                        EmailDto email = new EmailDto()
                        {

                            To = existingContractor.Email,
                            Subject = "New request from customer",
                            Body = emailBodyForRequest(existingContractor, createdRequest)

                        };

                        SendMail(email);

                    }
                    return requestView;
                }


                return null;

                
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the request. Error message: {ex.Message}");
            }
        }

        public string emailBodyForRequest(Contractors contractor, Requests request)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>New request from customer</h1>
        <p>Dear {contractor.Name},</p>
        <p>You have a new request from customer</p>        
        <p>Note: {request.Note}</p>
        <p>Total price: {request.TotalPrice}</p>
        <p>Created date: {request.TimeIn}</p>        
        <p>Expired date: {request.TimeOut}</p>

        <p>Please process request before expired.</p></br></br>

        <p>Best regards,<br/>[Admin from ICQS]</p>
    </div>
</body>
</html>
";
        }
        

        public void SendMail(EmailDto request)
        {
            try
            {
                string fromMail = _config.GetSection("EmailUsername").Value;
                string fromPassword = _config.GetSection("EmailPassword").Value;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = request.Subject;
                message.To.Add(new MailAddress(request.To));
                message.Body = request.Body;
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_config.GetSection("EmailHost").Value)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.Send(message);
            }catch
            
            {
                throw new Exception("Error while send mail");
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

        public bool IsOnGoingCustomerRequestExisted(int CustomerId, int ContractorId)
        {
            try
            {
                bool status = true;
                var request = unitOfWork.RequestRepository.Find(r => r.CustomerId == CustomerId && r.ContractorId == ContractorId && (r.Status == Requests.RequestsStatusEnum.PENDING || r.Status == Requests.RequestsStatusEnum.ACCEPTED || r.Status == Requests.RequestsStatusEnum.COMPLETED)).FirstOrDefault();
                if (request == null)
                {
                    status = false;
                }

                return status;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
