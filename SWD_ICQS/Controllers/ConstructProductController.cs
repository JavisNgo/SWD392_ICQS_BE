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
    public class ConstructProductController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ConstructProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("/api/constructProducts")]
        public async Task<IActionResult> GetAllConstructProducts()
        {
            try
            {
                var constructProducts = unitOfWork.ConstructProductRepository.Get();
                return Ok(constructProducts);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/constructProducts/{id}")]
        public IActionResult GetConstructProductByID(int id)
        {
            try
            {
                var constructProduct = unitOfWork.ConstructProductRepository.GetByID(id);
                if (constructProduct == null)
                {
                    return NotFound($"ConstructProduct with ID : {id} not found");
                }
                return Ok(constructProduct);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the constructProduct. Error message: {ex.Message}");
            }
        }

        [HttpPost("api/constructProducts")]
        public IActionResult AddConstructProduct([FromBody] ConstructProductsView constructProductsView)
        {
            try
            {
                var checkingConstruct = unitOfWork.ConstructRepository.GetByID(constructProductsView.ConstructId);
                if (checkingConstruct == null)
                {
                    return NotFound("Construct not found");
                }
                var checkingProduct = unitOfWork.ProductRepository.GetByID(constructProductsView.ProductId);
                if (checkingProduct == null)
                {
                    return NotFound("Product not found");
                }
                
                var constructProduct = _mapper.Map<ConstructProducts>(constructProductsView);
                
                unitOfWork.ConstructProductRepository.Insert(constructProduct);
                unitOfWork.Save();
                return Ok(constructProductsView);

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the constructProduct. Error message: {ex.Message}");
            }
        }
        [HttpPut("api/constructProducts")]
        public IActionResult UpdateConstructProduct(int id, [FromBody] ConstructProductsView constructProductsView)
        {
            try
            {
                var existingConstructProduct = unitOfWork.ConstructProductRepository.GetByID(id);
                if (existingConstructProduct == null)
                {
                    return NotFound($"ConstructProduct with ID : {id} not found");
                }
                var checkingConstruct = unitOfWork.ConstructRepository.GetByID(constructProductsView.ConstructId);
                if (checkingConstruct == null)
                {
                    return NotFound("Construct not found");
                }
                var checkingProduct = unitOfWork.ProductRepository.GetByID(constructProductsView.ProductId);
                if (checkingProduct == null)
                {
                    return NotFound("Product not found");
                }
                _mapper.Map(constructProductsView, existingConstructProduct);
                unitOfWork.ConstructProductRepository.Update(existingConstructProduct);
                unitOfWork.Save();
                return Ok(constructProductsView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the constructProduct. Error message: {ex.Message}");
            }
        }
        [HttpDelete("api/constructProducts/{id}")]
        public IActionResult DeleteConstructProduct(int id)
        {
            try
            {
                var existingConstructProduct = unitOfWork.ConstructProductRepository.GetByID(id);
                if (existingConstructProduct == null)
                {
                    return NotFound($"ConstructProduct with ID : {id} not found");
                }
                unitOfWork.ConstructProductRepository.Delete(id);
                unitOfWork.Save();
                return Ok($"ConstructProduct with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the constructProduct. Error message: {ex.Message}");
            }
        }
    }
}
