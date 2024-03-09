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

        [HttpGet("/api/messages")]
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

        [HttpGet("/api/messages/{id}")]
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

        [HttpPost("/message")]
        public IActionResult AddConstruct([FromBody] ConstructsView constructsView)
        {
            try
            {
                var checkingContractor = unitOfWork.ContractRepository.GetByID(constructsView.ContractorId);
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
                unitOfWork.ConstructRepository.Insert(construct);
                unitOfWork.Save();
                return Ok(constructsView);

            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the construct. Error message: {ex.Message}");
            }
        }
        

    }
}
