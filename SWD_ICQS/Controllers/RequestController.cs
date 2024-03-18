using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Net.WebSockets;
using System.Threading;

namespace SWD_ICQS.Controllers
{
    public class RequestController : ControllerBase
    {

        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
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
            if (requestView.requestDetailViews == null)
            {
                return BadRequest("Request must contain at least 1 product");
            }
            if (!requestView.requestDetailViews.Any())
            {
                return BadRequest("Request must contain at least 1 product");
            } else
            {
                foreach(var view in requestView.requestDetailViews)
                {
                    if(view.Quantity < 1)
                    {
                        return BadRequest("Quantity cannot < 1");
                    }
                }
            }
            if (_requestService.IsOnGoingCustomerRequestExisted(requestView.CustomerId, requestView.ContractorId))
            {
                return BadRequest("You have already sent request to this contractor, please wait for processing");
            }
            try
            {
            var request = _requestService.AddRequest(requestView);
            if(request == null)
            {
                 return BadRequest("Error while add request");
            }
            return Ok(request);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpPut("/Requests/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] RequestView requestView)
        {
            if(requestView.requestDetailViews == null)
            {
                return BadRequest("Request must contain at least 1 product");
            }
            if (!requestView.requestDetailViews.Any())
            {
                return BadRequest("Request must contain at least 1 product");
            }
            try
            {
                var result = _requestService.UpdateRequest(id, requestView);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut("/RequestAccepted/{id}")]
        public IActionResult AcceptRequest(int id)
        {
            try
            {
                var result = _requestService.AcceptRequest(id);
                if (result == true)
                {
                    return Ok("Accept successfully");
                }
                return BadRequest("Accept Fail");
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpPut("/IsMeeting/{id}/")]
        public IActionResult IsMeeting(int id)
        {
            try
            {
                var request = _requestService.MarkMeetingAsCompleted(id);
               if(request == true)
                {
                    return Ok("Successfully");
                }
                return BadRequest("Fail");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
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

