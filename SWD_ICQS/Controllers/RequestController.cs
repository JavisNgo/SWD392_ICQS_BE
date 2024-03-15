using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Threading;

namespace SWD_ICQS.Controllers
{
    public class RequestController : ControllerBase
    {

        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRequestService _requestService;

        public RequestController(IUnitOfWork unitOfWork, IMapper mapper, IRequestService requestService)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _requestService = requestService;
        }

        [AllowAnonymous]
        [HttpGet("/Requests")]
        public async Task<IActionResult> getAllRequests()
        {
            try
            {
                var requestsList = _requestService.GetAllRequests();
                return Ok(requestsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting all requests. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/Requests/{id}")]
        public IActionResult GetRequestById(int id)
        {
            bool checkRequest = _requestService.checkExistedRequestId(id);
            if(checkRequest)
            {
                RequestViewForGet requestView = _requestService.GetRequestView(id);


                return Ok(requestView);
            } else
            {
                return NotFound($"No request that have id {id}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/requests/contractor/{contractorId}")]
        public ActionResult<IEnumerable<RequestViewForGet>> GetRequestByContractorId(int contractorId)
        {
            try
            {
                var requests = _requestService.GetRequestsByContractorId(contractorId);
                return Ok(requests);
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
                var requests = _requestService.GetRequestsByCustomerId(customerId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("/Requests")]
        public async Task<IActionResult> AddRequest([FromBody] RequestView requestView)
        {
            return await _requestService.AddRequest(requestView);
        }


        [AllowAnonymous]
        [HttpPut("/Requests/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] RequestView requestView)
        {
            var result = await _requestService.UpdateRequest(id, requestView);
            return result;
        }

        [HttpPut("/RequestAccepted/{id}")]
        public IActionResult AcceptRequest(int id)
        {
            return _requestService.AcceptRequest(id);
        }

        [HttpPut("/IsMeeting/{id}/")]
        public IActionResult IsMeeting(int id)
        {
            return _requestService.MarkMeetingAsCompleted(id);

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

