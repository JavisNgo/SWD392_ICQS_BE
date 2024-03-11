using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet("/products")]
        public async Task<IActionResult> getAllProducts()
        {
            try
            {
                var products = unitOfWork.ProductRepository.Get();
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/products/{id}")]
        public IActionResult getProductByID(int id)
        {
            try
            {
                var product = unitOfWork.ProductRepository.GetByID(id);
                if(product == null)
                {
                    return NotFound($"Product with ID: {id} not found");
                }
                return Ok(product);
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred while getting the product. Error message: {ex.Message}");
            }
        }

        [HttpPost("/products")]
        public IActionResult AddProduct([FromBody] ProductsView productsView)
        {
            try
            {
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(productsView.ContractorId);
                if(checkingContractorID == null)
                {
                    return NotFound("Contractor not found");
                }
                //if (!IsValidName(productsView.Name))
                //{
                //    return BadRequest("Invalid name. It should only contain letters and spaces.");
                //}

                // Validate the Price
                if (productsView.Price.HasValue && productsView.Price < 0)
                {
                    return BadRequest("Invalid Price. It should be a non-negative number.");
                }
                var product = _mapper.Map<Products>(productsView);
                product.Status = true;
                unitOfWork.ProductRepository.Insert(product);
                unitOfWork.Save();
                return Ok(productsView);

            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the product. Error message: {ex.Message}");
            }
        }
        [HttpPut("/Products/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] ProductsView productsView)
        {
            try
            {
                var existingProduct = unitOfWork.ProductRepository.GetByID(id);

                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(productsView.ContractorId);
                if (checkingContractorID == null)
                {
                    return NotFound("Contractor not found");
                }
                if (!IsValidName(productsView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters and spaces.");
                }

                
                if (productsView.Price.HasValue && productsView.Price < 0)
                {
                    return BadRequest("Invalid Price. It should be a non-negative number.");
                }

                
                _mapper.Map(productsView, existingProduct);

                
                unitOfWork.ProductRepository.Update(existingProduct);
                unitOfWork.Save();

                return Ok(productsView); 
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the category. Error message: {ex.Message}");
            }
        }

        [HttpDelete("/product/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = unitOfWork.ProductRepository.GetByID(id);
                if (product == null)
                {
                    return NotFound($"Product with ID: {id} not found");
                }
                unitOfWork.ProductRepository.Delete(id);
                unitOfWork.Save();
                return Ok($"Product with ID {id} has been successfully deleted.");
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred while deleting the product. Error message: {ex.Message}");
            }
        }

        private bool IsValidName(string name)
        {
            // Use a regular expression to check if the name contains only letters and spaces
            return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s]+$");
        }
    }

    

}

