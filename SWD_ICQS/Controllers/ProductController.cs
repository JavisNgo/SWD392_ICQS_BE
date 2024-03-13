using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "productImage");
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

        [HttpGet("/api/v1/products/get/contractorid={contractorid}")]
        public async Task<IActionResult> getAllProductsByContractorId(int contractorid)
        {
            try
            {
                var productsList = unitOfWork.ProductRepository.Find(p => p.ContractorId == contractorid).ToList();
                if (unitOfWork.ContractorRepository.GetByID(contractorid) == null)
                {
                    return NotFound($"Contractor with id {contractorid} doesn't exist");
                }
                if (productsList.Any())
                {
                    List<ProductsView> productsViews = new List<ProductsView>();
                    foreach (var product in productsList)
                    {
                        var productImages = unitOfWork.ProductImageRepository.Find(p => p.ProductId == product.Id).ToList();
                        var productsView = _mapper.Map<ProductsView>(product);

                        if (productImages.Any())
                        {
                            productsView.productImagesViews = new List<ProductImagesView>();
                            foreach (var image in productImages)
                            {
                                image.ImageUrl = $"https://localhost:7233/img/productImage/{image.ImageUrl}";
                                productsView.productImagesViews.Add(_mapper.Map<ProductImagesView>(image));
                            }
                        }
                        productsViews.Add(productsView);
                    }
                    return Ok(productsViews);
                } else
                {
                    return NotFound("No product owned by this contractor");
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
                var product = unitOfWork.ProductRepository.GetByID(id);
                if(product == null)
                {
                    return NotFound($"Product with id: {id} not found");
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

                string code = $"P_{productsView.ContractorId}_{GenerateRandomCode(10)}";
                var product = new Products
                {
                    Code = code,
                    ContractorId = productsView.ContractorId,
                    Name = productsView.Name,
                    Price = productsView.Price,
                    Description = productsView.Description,
                    Status = productsView.Status
                };

                unitOfWork.ProductRepository.Insert(product);
                unitOfWork.Save();

                var createdProduct = unitOfWork.ProductRepository.Find(p => p.Code == code).FirstOrDefault();

                if (productsView.productImagesViews.Any())
                {
                    foreach(var image in productsView.productImagesViews)
                    {
                        if (!String.IsNullOrEmpty(image.ImageUrl))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                            string filename = $"ProductImage_{createdProduct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var productImage = new ProductImages
                            {
                                ProductId = createdProduct.Id,
                                ImageUrl = filename
                            };
                            unitOfWork.ProductImageRepository.Insert(productImage);
                            unitOfWork.Save();
                        }
                    }
                }

                return Ok("Added successfully");

            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the product. Error message: {ex.Message}");
            }
        }

        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        [HttpPut("/api/v1/products/put")]
        public IActionResult UpdateProduct([FromBody] ProductsView productsView)
        {
            try
            {
                var existingProduct = unitOfWork.ProductRepository.GetByID(productsView.Id);

                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {productsView.Id} not found.");
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

                var currentProductImages = unitOfWork.ProductImageRepository.Find(b => b.ProductId == existingProduct.Id).ToList();

                existingProduct.Name = productsView.Name;
                existingProduct.Price = productsView.Price;
                existingProduct.Description = productsView.Description;

                unitOfWork.ProductRepository.Update(existingProduct);
                unitOfWork.Save();

                int countUrl = 0;
                foreach (var image in productsView.productImagesViews)
                {
                    if (!String.IsNullOrEmpty(image.ImageUrl))
                    {
                        if (image.ImageUrl.Contains("https://localhost:7233/img/productImage/"))
                        {
                            countUrl++;
                        }
                    }
                }

                if (countUrl == currentProductImages.Count)
                {
                    foreach (var image in productsView.productImagesViews)
                    {
                        if (!image.ImageUrl.Contains("https://localhost:7233/img/productImage/") && !String.IsNullOrEmpty(image.ImageUrl))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                            string filename = $"ProductImage_{existingProduct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var productImage = new ProductImages
                            {
                                ProductId = existingProduct.Id,
                                ImageUrl = filename
                            };
                            unitOfWork.ProductImageRepository.Insert(productImage);
                            unitOfWork.Save();
                        }
                    }
                }
                else if (countUrl < currentProductImages.Count)
                {
                    List<ProductImages> tempList = currentProductImages;
                    foreach (var image in productsView.productImagesViews)
                    {
                        if (!image.ImageUrl.Contains("https://localhost:7233/img/productImage/") && !String.IsNullOrEmpty(image.ImageUrl))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                            string filename = $"ProductImage_{existingProduct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var productImage = new ProductImages
                            {
                                ProductId = existingProduct.Id,
                                ImageUrl = filename
                            };
                            unitOfWork.ProductImageRepository.Insert(productImage);
                            unitOfWork.Save();
                        }
                        else if (image.ImageUrl.Contains("https://localhost:7233/img/productImage/"))
                        {
                            for (int i = tempList.Count - 1; i >= 0; i--)
                            {
                                string url = $"https://localhost:7233/img/productImage/{tempList[i].ImageUrl}";
                                if (url.Equals(image.ImageUrl))
                                {
                                    tempList.RemoveAt(i);
                                }
                            }
                        }
                    }
                    foreach (var temp in tempList)
                    {
                        unitOfWork.ProductImageRepository.Delete(temp);
                        unitOfWork.Save();
                        if (!String.IsNullOrEmpty(temp.ImageUrl))
                        {
                            string imagePath = Path.Combine(_imagesDirectory, temp.ImageUrl);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                    }
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
                var product = unitOfWork.ProductRepository.GetByID(id);
                if (product == null)
                {
                    return NotFound($"Product with ID: {id} not found");
                }
                if(product.Status == true)
                {
                    product.Status = false;
                    unitOfWork.ProductRepository.Update(product);
                    unitOfWork.Save();
                    return Ok($"Product with ID {id} set status to false successfully.");
                } else
                {
                    product.Status = true;
                    unitOfWork.ProductRepository.Update(product);
                    unitOfWork.Save();
                    return Ok($"Product with ID {id} set status to true successfully.");
                }
                
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

