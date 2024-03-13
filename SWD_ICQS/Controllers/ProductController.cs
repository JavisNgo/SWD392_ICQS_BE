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
        private readonly string _imagesDirectory;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "blogImage");
        }
        [HttpGet("/api/v1/products/get")]
        public async Task<IActionResult> getAllProducts()
        {
            try
            {
                List<ProductsView> productsViews = new List<ProductsView>();
                var productsList = unitOfWork.ProductRepository.Get();
                foreach (var product in productsList)
                {
                    var productImages = unitOfWork.ProductImageRepository.Find(p => p.ProductId == product.Id).ToList();
                    var productsView = _mapper.Map<ProductsView>(product);

                    if (productImages.Any())
                    {
                        productsView.productImagesViews = new List<ProductImagesView>();
                        foreach(var image in productImages)
                        {
                            image.ImageUrl = $"https://localhost:7233/img/productImage/{image.ImageUrl}";
                            productsView.productImagesViews.Add(_mapper.Map<ProductImagesView>(image));
                        }
                    }
                    productsViews.Add(productsView);
                }
                return Ok(productsViews);
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
                var product = unitOfWork.ProductRepository.GetByID(id);
                if(product == null)
                {
                    return NotFound($"Product with ID: {id} not found");
                }

                var productImages = unitOfWork.ProductImageRepository.Find(p => p.ProductId == product.Id).ToList();

                var productsView = _mapper.Map<ProductsView>(product);

                if(productImages.Any())
                {
                    productsView.productImagesViews = new List<ProductImagesView>();
                    foreach(var image in productImages)
                    {
                        image.ImageUrl = $"https://localhost:7233/img/productImage/{image.ImageUrl}";
                        productsView.productImagesViews.Add(_mapper.Map<ProductImagesView>(image));
                    }
                }

                return Ok(productsView);
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred while getting the product. Error message: {ex.Message}");
            }
        }

        [HttpPost("/api/v1/products/post")]
        public IActionResult AddProduct([FromBody] ProductsView productsView)
        {
            try
            {
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(productsView.ContractorId);
                if(checkingContractorID == null)
                {
                    return NotFound("Contractor not found");
                }
                if (!IsValidName(productsView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters and number.");
                }

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
        public IActionResult UpdateProduct(int id, [FromBody] ProductsView productsView)
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
                    return BadRequest("Invalid name. It should only contain letters and number.");
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

        [HttpPut("/api/v1/products/status/id={id}")]
        public IActionResult ChangeStatusProduct(int id)
        {
            try
            {
                var product = unitOfWork.ProductRepository.GetByID(id);
                if (product == null)
                {
                    return NotFound($"Product with ID: {id} not found");
                }
                if(product.Status == true)
                {
                    product.Status = false;
                } else if(product.Status == false)
                {
                    product.Status = true;
                }
                unitOfWork.ProductRepository.Update(product);
                unitOfWork.Save();
                return Ok($"Product with ID {id} set status to false successfully.");
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred while changing status the product. Error message: {ex.Message}");
            }
        }

        private bool IsValidName(string name)
        {
            // Use a regular expression to check if the name contains letters, spaces, and numbers
            return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9\s]+$");
        }

    }



}

