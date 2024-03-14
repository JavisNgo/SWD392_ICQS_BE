using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using System.Threading;

namespace SWD_ICQS.Controllers
{
    public class RequestController : ControllerBase
    {

        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public RequestController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/Requests")]
        public async Task<IActionResult> getAllRequests()
        {
            try
            {
                var requestsList = unitOfWork.RequestRepository.Get();
                return Ok(requestsList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/Requests/{id}")]
        public IActionResult GetRequestById(int id)
        {
            try
            {
                var request = unitOfWork.RequestRepository.GetByID(id);

                if (request == null)
                {
                    return NotFound($"Request with ID {id} not found.");
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the request. Error message: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("/api/v1/requests/contractor/{contractorId}")]
        public ActionResult<IEnumerable<RequestViewForGet>> GetRequestByContractorId(int contractorId)
        {
            try
            {
                // Lấy tất cả các hợp đồng có liên quan đến nhà thầu có contractorId cung cấp
                var requests = unitOfWork.RequestRepository.Get(filter: c => c.ContractorId == contractorId).ToList();
                var requestViews = new List<RequestViewForGet>();

                foreach (var request in requests)
                {

                    // Lấy thông tin của Customer từ Appointment
                    var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                    if (contractor == null)
                    {
                        // Nếu không tìm thấy thông tin của Contractor, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Customer từ Appointment
                    var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                    var requestView = _mapper.Map<RequestViewForGet>(request);

                    // Gán tên của Contractor và Customer vào ContractViewForGet
                    requestView.ContractorName = contractor.Name;
                    requestView.CustomerName = customer != null ? customer.Name : null;

                    requestViews.Add(requestView);
                }

                return Ok(requestViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("/api/v1/requests/customer/{customerId}")]
        public ActionResult<IEnumerable<RequestViewForGet>> GetRequestByCustomerId(int customerId)
        {
            try
            {
                // Lấy tất cả các hợp đồng có liên quan đến nhà thầu có customerId cung cấp
                var requests = unitOfWork.RequestRepository.Get(filter: c => c.CustomerId == customerId).ToList();
                var requestViews = new List<RequestViewForGet>();

                foreach (var request in requests)
                {

                    // Lấy thông tin của Customer từ Appointment
                    var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                    if (customer == null)
                    {
                        // Nếu không tìm thấy thông tin của Contractor, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Customer từ Appointment
                    var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                    var requestView = _mapper.Map<RequestViewForGet>(request);

                    // Gán tên của Contractor và Customer vào ContractViewForGet
                    requestView.CustomerName = customer.Name;
                    requestView.ContractorName = contractor != null ? contractor.Name : null;

                    requestViews.Add(requestView);
                }

                return Ok(requestViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("/Requests")]
        public IActionResult AddRequest([FromBody] RequestView requestView)
        {
            try
            {
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(requestView.ContractorId);
                var checkingCustomerId = unitOfWork.CustomerRepository.GetByID(requestView.CustomerId);
                if (checkingContractorID == null || checkingCustomerId == null)
                {
                    return NotFound("ContractorID or CustomerID not found");
                }
                if(requestView.TotalPrice < 0)
                {
                    return BadRequest("Price must be larger than 0");
                }
                Requests request = _mapper.Map<Requests>(requestView);
                request.Status = 0;
                request.TimeIn = DateTime.Now;
                if (request.TimeIn.HasValue)
                {
                    request.TimeOut = request.TimeIn.Value.AddDays(7); // Set TimeOut to TimeIn + 7 days
                }
                unitOfWork.RequestRepository.Insert(request);
                unitOfWork.Save();

                return Ok(requestView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the request. Error message: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpPut("/Requests/{id}")]
        public IActionResult UpdateRequest(int id, [FromBody] RequestView requestView)
        {
            try
            {
                var existingRequest = unitOfWork.RequestRepository.GetByID(id);
                if (existingRequest == null)
                {
                    return NotFound($"Request with ID : {id} not found");
                }
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(requestView.ContractorId);
                var checkingCustomerId = unitOfWork.CustomerRepository.GetByID(requestView.CustomerId);
                if (checkingContractorID == null || checkingCustomerId == null)
                {
                    return NotFound("ContractorID or CustomerID not found");
                }
                if (requestView.TotalPrice < 0)
                {
                    return BadRequest("Price must be larger than 0");
                }
                _mapper.Map(requestView, existingRequest);
                unitOfWork.RequestRepository.Update(existingRequest);
                unitOfWork.Save();
                return Ok(requestView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the constructProduct. Error message: {ex.Message}");
            }
        }

        [HttpPut("/RequestAccepted/{id}")]
        public IActionResult AcceptRequest(int id)
        {
            try
            {
                var existingRequest = unitOfWork.RequestRepository.GetByID(id);
                if (existingRequest == null)
                {
                    return NotFound($"Request with ID : {id} not found");
                }
                existingRequest.Status = (Requests.RequestsStatusEnum?)2;
                existingRequest.TimeOut = DateTime.Now.AddDays(14);
                unitOfWork.RequestRepository.Update(existingRequest);
                unitOfWork.Save();
                var appointment = new Appointments
                {
                    CustomerId = existingRequest.CustomerId,
                    ContractorId = existingRequest.ContractorId,
                    RequestId = existingRequest.Id,
                    MeetingDate = DateTime.Now.AddDays(7),
                    Status = (Appointments.AppointmentsStatusEnum?)0
                };
                unitOfWork.AppointmentRepository.Insert(appointment);
                unitOfWork.Save();
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest($"An error occurred while accept request flow. Error message: {ex.Message}");
            }

        }

        [HttpPut("/IsMeeting/{id}/")]
        public IActionResult IsMeeting(int id)
        {
            try
            {
                var existingAppointment = unitOfWork.AppointmentRepository.GetByID(id);
                if (existingAppointment == null)
                {
                    return NotFound($"Appointment with ID : {id} not found");
                }
                existingAppointment.Status = (Appointments.AppointmentsStatusEnum?)2;
                unitOfWork.AppointmentRepository.Update(existingAppointment);
                unitOfWork.Save();
                var request = unitOfWork.RequestRepository.GetByID(existingAppointment.RequestId);
                if(request == null)
                {
                    return NotFound("Request not found");
                }
                request.TimeOut = DateTime.Now.AddDays(14);
                request.Status = (Requests.RequestsStatusEnum?)3;
                unitOfWork.RequestRepository.Update(request);
                unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while accept request flow. Error message: {ex.Message}");
            }
        }

        //[HttpPut("/ContractsUploaded/{id}/")]
        //public IActionResult ContractsUploaded(int id)
        //{
        //    try
        //    {
        //        var existingAppointment = unitOfWork.AppointmentRepository.GetByID(id);
        //        if (existingAppointment == null)
        //        {
        //            return NotFound($"Appointment with ID : {id} not found");
        //        }
        //        existingAppointment.Status = (Appointments.AppointmentsStatusEnum?)3;

        //        existingAppointment.Request.Status = (Requests.RequestsStatusEnum?)4;
        //        if (existingAppointment.Status.Equals(0))
        //        {

        //        }


        //        unitOfWork.AppointmentRepository.Update(existingAppointment);
        //        unitOfWork.Save();
        //        var contract = new Contracts
        //        {
        //            AppointmentId = existingAppointment.Id,
        //            UploadDate = DateTime.Now,
        //            Status = 1

        //        };
        //        unitOfWork.ContractRepository.Insert(contract);
        //        unitOfWork.Save();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while accept request flow. Error message: {ex.Message}");
        //    }
        //}

    }
}

