using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    public class RequestDetailController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public RequestDetailController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/RequestDetails")]
        public async Task<IActionResult> getAllRequestDetails()
        {
            try
            {
                var requestDetailList = unitOfWork.RequestDetailRepository.Get();
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
                var requestDetail = unitOfWork.RequestDetailRepository.GetByID(id);

                if (requestDetail == null)
                {
                    return NotFound($"RequestDetail with ID {id} not found.");
                }

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
                var checkingProductID = unitOfWork.ProductRepository.GetByID(requestDetailView.ProductId);
                var checkingRequestId = unitOfWork.RequestRepository.GetByID(requestDetailView.RequestId);
                if (checkingProductID == null || checkingRequestId == null)
                {
                    return NotFound("ProductID or RequestID not found");
                }
                RequestDetails requestDetail = _mapper.Map<RequestDetails>(requestDetailView);
                unitOfWork.RequestDetailRepository.Insert(requestDetail);
                unitOfWork.Save();

                return Ok(requestDetailView);
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
                var existingrequestDetail = unitOfWork.RequestDetailRepository.GetByID(id);

                if (existingrequestDetail == null)
                {
                    return NotFound($"RequestDetail with ID {id} not found.");
                }
                var checkingProductID = unitOfWork.ProductRepository.GetByID(requestDetailView.ProductId);
                var checkingRequestId = unitOfWork.RequestRepository.GetByID(requestDetailView.RequestId);
                if (checkingProductID == null || checkingRequestId == null)
                {
                    return NotFound("ProductID or RequestID not found");
                }

                _mapper.Map(requestDetailView, existingrequestDetail);

                unitOfWork.RequestDetailRepository.Update(existingrequestDetail);
                unitOfWork.Save();

                return Ok(requestDetailView);
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
                var requestDetail = unitOfWork.RequestDetailRepository.GetByID(id);

                if (requestDetail == null)
                {
                    return NotFound($"Request Detail with ID {id} not found.");
                }

                unitOfWork.RequestDetailRepository.Delete(id);
                unitOfWork.Save();

                return Ok($"Request Detail with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the Request Detail. Error message: {ex.Message}");
            }
        }
    }
}
