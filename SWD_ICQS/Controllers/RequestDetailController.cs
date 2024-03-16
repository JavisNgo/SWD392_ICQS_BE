using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    public class RequestDetailController : ControllerBase
    {
        private readonly IRequestDetailService _requestDetailService;

        public RequestDetailController(IRequestDetailService requestDetailService)
        {
            _requestDetailService = requestDetailService;
        }

        [AllowAnonymous]
        [HttpGet("/RequestDetails")]
        public async Task<IActionResult> getAllRequestDetails()
        {
            try
            {
                var requestDetailList = _requestDetailService.GetRequestDetails();
                return Ok(requestDetailList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/RequestDetails/{id}")]
        public IActionResult GetRequestDetailsById(int id)
        {
            try
            {
                var requestDetail = _requestDetailService.GetRequestById(id);

                return Ok(requestDetail);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the request detail. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/RequestDetails")]
        public IActionResult AddRequestDetail([FromBody] RequestDetailView requestDetailView)
        {
            try
            {
                var requestDetail = _requestDetailService.AddRequestDetail(requestDetailView);
                return Ok(requestDetail);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the request detail. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("/RequestDetails/{id}")]
        public IActionResult UpdateRequestDetail(int id, [FromBody] RequestDetailView requestDetailView)
        {
            try
            {
                var success = _requestDetailService.UpdateRequestDetail(id, requestDetailView);
                if (success)
                {
                    return Ok("RequestDetail updated successfully.");
                }
                else
                {
                    return BadRequest("Failed to update RequestDetail.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the RequestDetail. Error message: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpDelete("/RequestDetails/{id}")]
        public IActionResult DeleteRequestDetails(int id)
        {
            try
            {
                bool isDeleted = _requestDetailService.DeleteRequestDetail(id);
                if (isDeleted)
                {
                    return Ok($"Request Detail with ID: {id} has been successfully deleted.");
                }
                else
                {
                    return NotFound($"Request Detail with ID: {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the Request Detail. Error message: {ex.Message}");
            }
        }

    }
}
