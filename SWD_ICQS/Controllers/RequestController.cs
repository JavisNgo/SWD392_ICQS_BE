using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
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

    }
}
