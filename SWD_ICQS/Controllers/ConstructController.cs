using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ConstructController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("/api/constructs")]
        public async Task<IActionResult> GetAllConstruct()
        {
            try
            {
                var constructs = unitOfWork.ConstructRepository.Get();
                return Ok(constructs);
            }catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/constructs/{id}")]
        public IActionResult GetConstructByID(int id)
        {
            try
            {
                var construct = unitOfWork.ConstructRepository.GetByID(id);
                if(construct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }
                return  Ok(construct);
            }catch(Exception ex)
            {
                return BadRequest($"An error occurred while getting the construct. Error message: {ex.Message}");
            }
        }

        [HttpPost("/constructs")]
        public IActionResult AddConstruct([FromBody] ConstructsView constructsView)
        {
            try
            {
                var checkingContractor = unitOfWork.ContractorRepository.GetByID(constructsView.ContractorId);
                if(checkingContractor == null)
                {
                    return NotFound("Contractor not found");
                }
                var checkingCategory = unitOfWork.CategoryRepository.GetByID(constructsView.CategoryId);
                if(checkingCategory == null)
                {
                    return NotFound("Category not found");
                }
                if(!constructsView.EstimatedPrice.HasValue && constructsView.EstimatedPrice.Value < 0)
                {
                    return BadRequest("EstimatedPrice must be greater than 1 and has value");
                }
                var construct = _mapper.Map<Constructs>(constructsView);
                construct.Status = true;
                unitOfWork.ConstructRepository.Insert(construct);
                unitOfWork.Save();
                return Ok(constructsView);

            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the construct. Error message: {ex.Message}");
            }
        }
        [HttpPut("/constructs")]
        public IActionResult UpdateConstruct(int id, [FromBody] ConstructsView constructsView)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }
                var checkingContractor = unitOfWork.ContractorRepository.GetByID(constructsView.ContractorId);
                if (checkingContractor == null)
                {
                    return NotFound("Contractor not found");
                }
                var checkingCategory = unitOfWork.CategoryRepository.GetByID(constructsView.CategoryId);
                if (checkingCategory == null)
                {
                    return NotFound("Category not found");
                }
                if (!constructsView.EstimatedPrice.HasValue && constructsView.EstimatedPrice.Value < 0)
                {
                    return BadRequest("EstimatedPrice must be greater than 1 and has value");
                }
                _mapper.Map(constructsView, existingConstruct);
                unitOfWork.ConstructRepository.Update(existingConstruct);
                unitOfWork.Save();
                return Ok(constructsView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the construct. Error message: {ex.Message}");
            }
        }
        [HttpDelete("/constructs/{id}")]
        public IActionResult DeleteConstruct(int id)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }
                unitOfWork.ConstructRepository.Delete(id);
                unitOfWork.Save();
                return Ok($"Construct with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the construct. Error message: {ex.Message}");
            }
        }

    }
}
