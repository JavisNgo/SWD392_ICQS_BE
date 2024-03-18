using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Implements;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructProductController : ControllerBase
    {
        private readonly IConstructProductService _constructProductService;



        public ConstructProductController(IConstructProductService constructProductService)
        {
            _constructProductService = constructProductService;
        }

        [HttpGet("/api/constructProducts")]
        public async Task<IActionResult> GetAllConstructProducts()
        {
            try
            {
                var constructProducts = _constructProductService.GetConstructProducts();
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
                var constructProduct = _constructProductService.GetConstructProductByID(id);
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

        [HttpPost("/api/constructProducts")]
        public IActionResult AddConstructProduct([FromBody] ConstructProductsView constructProductsView)
        {
            if(constructProductsView.Quantity < 1)
            {
                return BadRequest("Quantity must larger than 1 unit");
            }
            try
            {
                var constructProduct = _constructProductService.AddConstructProduct(constructProductsView);
                
                return Ok(constructProduct);

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the constructProduct. Error message: {ex.Message}");
            }
        }
        [HttpPut("/api/constructProducts/{id}")]
        public IActionResult UpdateConstructProduct(int id, [FromBody] ConstructProductsView constructProductsView)
        {
            if (constructProductsView.Quantity <= 1)
            {
                return BadRequest("Quantity must larger than 1 unit");
            }
            try
            {
                var constructProduct = _constructProductService.UpdateConstructProduct(id, constructProductsView);
                
                return Ok(constructProduct);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the constructProduct. Error message: {ex.Message}");
            }
        }
        [HttpDelete("/api/constructProducts/{id}")]
        public IActionResult DeleteConstructProduct(int id)
        {
            try
            {
                var checkDelete =  _constructProductService.DeleteConstructProduct(id);
                if (checkDelete.Equals(true))
                {
                    return Ok($"ConstructProduct with ID: {id} has been successfully deleted.");
                }
                
                
                return BadRequest($"ConstructProduct with ID: {id} deleted fail.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the constructProduct. Error message: {ex.Message}");
            }
        }
    }
}
