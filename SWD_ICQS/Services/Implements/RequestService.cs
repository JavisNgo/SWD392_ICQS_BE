using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class RequestService : IRequestService
    {

        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public RequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
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


        public Boolean AcceptRequest(int id)
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
                unitOfWork.Save();

                var request = unitOfWork.RequestRepository.GetByID(existingAppointment.RequestId);
                if (request == null)
                {
                    throw new Exception("Request not found");
                }

                request.TimeOut = DateTime.Now.AddDays(14);
                request.Status = Requests.RequestsStatusEnum.COMPLETED;
                unitOfWork.RequestRepository.Update(request);
                unitOfWork.Save();
               return true;
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
                request.Status = 0;
                request.TimeIn = DateTime.Now;
                request.TimeOut = DateTime.Now.AddDays(7);

                unitOfWork.RequestRepository.Insert(request);
                unitOfWork.Save();

                return requestView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the request. Error message: {ex.Message}");
            }
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
