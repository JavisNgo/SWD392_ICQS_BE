using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("/api/v1/products/get")]
        public async Task<IActionResult> getAllProducts()
        {
            try
            {
                
                var productsList = _productService.GetProducts();
                
                return Ok(productsList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/v1/products/get/contractorid={contractorid}")]
        public async Task<IActionResult> getAllProductsByContractorId(int contractorid)
        {
            try
            {
                var productsList = _productService.getAllProductsByContractorId(contractorid);
                
                if (productsList != null)
                {
                    return Ok(productsList);
                } else
                {
                    return NotFound($"No product that created by this contractor");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/v1/products/get/id={id}")]
        public IActionResult getProductByID(int id)
        {
            try
            {
                var product = _productService.getProductByID(id);
                if(product == null)
                {
                    return NotFound($"Product with id: {id} not found");
                }

                

                return Ok(product);
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred while getting the product. Error message: {ex.Message}");
            }
        }

        [HttpPost("/api/v1/products/post")]
        public IActionResult AddProduct([FromBody] ProductsView productsView)
        {
            if (string.IsNullOrEmpty(productsView.Name))
            {
                return BadRequest("Construct's name is required");
            }
            if (productsView.Price < 0)
            {
                return BadRequest("Estimate Price must larger than 0 ");
            }
            try
            {
                var product = _productService.AddProduct(productsView); 
                if(product == null)
                {
                    return BadRequest("An error occurred while adding the product");
                }
                

                return Ok("Added successfully");

            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the product. Error message: {ex.Message}");
            }
        }

        

        [HttpPut("/api/v1/products/put")]
        public IActionResult UpdateProduct([FromBody] ProductsView productsView)
        {
            if (string.IsNullOrEmpty(productsView.Name))
            {
                return BadRequest("Construct's name is required");
            }
            if (productsView.Price < 0)
            {
                return BadRequest("Estimate Price must larger than 0 ");
            }
            try
            {
                var product = _productService.UpdateProduct(productsView);

                if (product == null)
                {
                    return NotFound($"Product with ID {productsView.Id} not found.");
                }
                

                return Ok("Update successfully"); 
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the category. Error message: {ex.Message}");
            }
        }

        [HttpPut("/api/v1/products/put/status/id={id}")]
        public IActionResult ChangeStatusProduct(int id)
        {
            try
            {
                var product = _productService.ChangeStatusProduct(id);
                if (product == null)
                {
                    return NotFound($"Product with ID: {id} not found");
                }
                if(product.Status == true)
                {
                    return Ok($"Product with ID {id} set status to true successfully.");
                } else
                {
                    return Ok($"Product with ID {id} set status to false successfully.");
                }
                
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred while changing status the product. Error message: {ex.Message}");
            }
        }

        //private bool IsValidName(string name)
        //{
        //    // Use a regular expression to check if the name contains letters, spaces, and numbers
        //    return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9\s]+$");
        //}

    }



}

