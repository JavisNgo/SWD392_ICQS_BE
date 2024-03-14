using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Reflection.Metadata;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public ConstructController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "constructImage");
        }

        [HttpGet("/api/v1/constructs/get")]
        public async Task<IActionResult> GetAllConstruct()
        {
            try
            {
                var constructsList = unitOfWork.ConstructRepository.Get();
                List<ConstructsView> constructsViews = new List<ConstructsView>();
                foreach (var construct in constructsList)
                {
                    var constructImages = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == construct.Id).ToList();
                    var constructsView = _mapper.Map<ConstructsView>(construct);

                    constructsView.CategoriesView = _mapper.Map<CategoriesView>(unitOfWork.CategoryRepository.GetByID(constructsView.CategoryId));
                    
                    if (constructImages.Any())
                    {
                        constructsView.constructImagesViews = new List<ConstructImagesView>();
                        foreach (var image in constructImages)
                        {
                            image.ImageUrl = $"https://localhost:7233/img/constructImage/{image.ImageUrl}";
                            constructsView.constructImagesViews.Add(_mapper.Map<ConstructImagesView>(image));
                        }
                    }
                    constructsViews.Add(constructsView);
                }

                return Ok(constructsViews);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/v1/constructs/get/contractorid={contractorid}")]
        public async Task<IActionResult> GetAllConstructByContractorId(int contractorid)
        {
            try
            {
                var constructsList = unitOfWork.ConstructRepository.Find(c => c.ContractorId == contractorid).ToList();
                if(unitOfWork.ContractorRepository.GetByID(contractorid) == null)
                {
                    return NotFound($"Contractor with id {contractorid} doesn't exist");
                }
                if (constructsList.Any())
                {
                    List<ConstructsView> constructsViews = new List<ConstructsView>();
                    foreach (var construct in constructsList)
                    {
                        var constructImages = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == construct.Id).ToList();
                        var constructsView = _mapper.Map<ConstructsView>(construct);

                        constructsView.CategoriesView = _mapper.Map<CategoriesView>(unitOfWork.CategoryRepository.GetByID(constructsView.CategoryId));

                        if (constructImages.Any())
                        {
                            constructsView.constructImagesViews = new List<ConstructImagesView>();
                            foreach (var image in constructImages)
                            {
                                image.ImageUrl = $"https://localhost:7233/img/constructImage/{image.ImageUrl}";
                                constructsView.constructImagesViews.Add(_mapper.Map<ConstructImagesView>(image));
                            }
                        }
                        constructsViews.Add(constructsView);
                    }

                    return Ok(constructsViews);
                } else
                {
                    return NotFound("No construct owned by this contractor");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/v1/constructs/get/id={id}")]
        public IActionResult GetConstructByID(int id)
        {
            try
            {
                var construct = unitOfWork.ConstructRepository.GetByID(id);
                if(construct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }
                var constructImages = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == construct.Id).ToList();

                var constructsView = _mapper.Map<ConstructsView>(construct);

                constructsView.CategoriesView = _mapper.Map<CategoriesView>(unitOfWork.CategoryRepository.GetByID(constructsView.CategoryId));

                if (constructImages.Any())
                {
                    constructsView.constructImagesViews = new List<ConstructImagesView>();
                    foreach (var image in constructImages)
                    {
                        image.ImageUrl = $"https://localhost:7233/img/constructImage/{image.ImageUrl}";
                        constructsView.constructImagesViews.Add(_mapper.Map<ConstructImagesView>(image));
                    }
                }

                var constructsProducts = unitOfWork.ConstructProductRepository.Find(c => c.ConstructId == id).ToList();
                if (constructsProducts.Any())
                {
                    constructsView.constructProductsViews = new List<ConstructProductsView>();
                    foreach (var cp in constructsProducts)
                    {
                        constructsView.constructProductsViews.Add(_mapper.Map<ConstructProductsView>(cp));
                    }
                    foreach (var cpv in constructsView.constructProductsViews)
                    {
                        var product = unitOfWork.ProductRepository.GetByID(cpv.ProductId);
                        if (product != null)
                        {
                            var productImages = unitOfWork.ProductImageRepository.Find(p => p.ProductId == product.Id).ToList();
                            cpv.ProductsView = _mapper.Map<ProductsView>(product);
                            if (productImages.Any())
                            {
                                cpv.ProductsView.productImagesViews = new List<ProductImagesView>();
                                foreach (var image in productImages)
                                {
                                    image.ImageUrl = $"https://localhost:7233/img/productImage/{image.ImageUrl}";
                                    cpv.ProductsView.productImagesViews.Add(_mapper.Map<ProductImagesView>(image));
                                }
                            }
                        }
                    }
                }

                return Ok(constructsView);
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/v1/constructs/post")]
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
                //if (constructsView.constructProductsViews.Any())
                //{
                //    foreach(var cp in constructsView.constructProductsViews)
                //    {
                //        var existingProduct = unitOfWork.ProductRepository.Find(c => c.Id == cp.ProductId).FirstOrDefault();
                //        if (existingProduct == null)
                //        {
                //            return NotFound($"No product found with id {cp.ProductId}");
                //        }
                //    }
                //}
                //else
                //{
                //    return BadRequest("You need to add at least 1 product to construct");
                //}

                string code = $"C_{constructsView.ContractorId}_{GenerateRandomCode(10)}";

                var construct = new Constructs
                {
                    Code = code,
                    ContractorId = constructsView.ContractorId,
                    CategoryId = constructsView.CategoryId,
                    Name = constructsView.Name,
                    EstimatedPrice = constructsView.EstimatedPrice,
                    Status = true
                };
                unitOfWork.ConstructRepository.Insert(construct);
                unitOfWork.Save();

                var createdConstruct = unitOfWork.ConstructRepository.Find(c => c.Code == code).FirstOrDefault();

                if (constructsView.constructImagesViews.Any())
                {
                    foreach (var image in constructsView.constructImagesViews)
                    {
                        if (!String.IsNullOrEmpty(image.ImageUrl))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                            string filename = $"ConstructImage_{createdConstruct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var constructImage = new ConstructImages
                            {
                                ConstructId = createdConstruct.Id,
                                ImageUrl = filename
                            };
                            unitOfWork.ConstructImageRepository.Insert(constructImage);
                            unitOfWork.Save();
                        }
                    }
                }

                //if (constructsView.constructProductsViews.Any())
                //{
                //    foreach(var cp in constructsView.constructProductsViews)
                //    {
                //        var existingProduct = unitOfWork.ProductRepository.Find(c => c.Id == cp.ProductId).FirstOrDefault();
                //        if(existingProduct != null)
                //        {
                //            var constructProduct = new ConstructProducts
                //            {
                //                ConstructId = createdConstruct.Id,
                //                ProductId = cp.ProductId
                //            };
                //            unitOfWork.ConstructProductRepository.Insert(constructProduct);
                //            unitOfWork.Save();
                //        }
                //    }
                //}

                return Ok("Create successfully");

            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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

        [HttpPut("/api/v1/constructs/put")]
        public IActionResult UpdateConstruct([FromBody] ConstructsView constructsView)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(constructsView.Id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {constructsView.Id} not found");
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
                //if (constructsView.constructProductsViews.Any())
                //{
                //    foreach (var cp in constructsView.constructProductsViews)
                //    {
                //        var existingProduct = unitOfWork.ProductRepository.Find(c => c.Id == cp.ProductId).FirstOrDefault();
                //        if (existingProduct == null)
                //        {
                //            return NotFound($"No product found with id {cp.ProductId}");
                //        }
                //    }
                //} else
                //{
                //    return BadRequest("You need to add at least 1 product to update construct");
                //}

                var currentConstructImages = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == constructsView.Id).ToList();

                existingConstruct.CategoryId = constructsView.CategoryId;
                existingConstruct.Name = constructsView.Name;
                existingConstruct.EstimatedPrice = constructsView.EstimatedPrice;
                unitOfWork.ConstructRepository.Update(existingConstruct);
                unitOfWork.Save();

                int countUrl = 0;
                foreach (var image in constructsView.constructImagesViews)
                {
                    if (!String.IsNullOrEmpty(image.ImageUrl))
                    {
                        if (image.ImageUrl.Contains("https://localhost:7233/img/constructImage/"))
                        {
                            countUrl++;
                        }
                    }
                }

                if (countUrl == currentConstructImages.Count)
                {
                    foreach (var image in constructsView.constructImagesViews)
                    {
                        if (!image.ImageUrl.Contains("https://localhost:7233/img/constructImage/") && !String.IsNullOrEmpty(image.ImageUrl))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                            string filename = $"ConstructImage_{existingConstruct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var constructImage = new ConstructImages
                            {
                                ConstructId = existingConstruct.Id,
                                ImageUrl = filename
                            };
                            unitOfWork.ConstructImageRepository.Insert(constructImage);
                            unitOfWork.Save();
                        }
                    }
                }
                else if (countUrl < currentConstructImages.Count)
                {
                    List<ConstructImages> tempList = currentConstructImages;
                    foreach (var image in constructsView.constructImagesViews)
                    {
                        if (!image.ImageUrl.Contains("https://localhost:7233/img/constructImage/") && !String.IsNullOrEmpty(image.ImageUrl))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                            string filename = $"ConstructImage_{existingConstruct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var constructImage = new ConstructImages
                            {
                                ConstructId = existingConstruct.Id,
                                ImageUrl = filename
                            };
                            unitOfWork.ConstructImageRepository.Insert(constructImage);
                            unitOfWork.Save();
                        }
                        else if (image.ImageUrl.Contains("https://localhost:7233/img/constructImage/"))
                        {
                            for (int i = tempList.Count - 1; i >= 0; i--)
                            {
                                string url = $"https://localhost:7233/img/constructImage/{tempList[i].ImageUrl}";
                                if (url.Equals(image.ImageUrl))
                                {
                                    tempList.RemoveAt(i);
                                }
                            }
                        }
                    }
                    foreach (var temp in tempList)
                    {
                        unitOfWork.ConstructImageRepository.Delete(temp);
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

                //var currentConstructProducts = unitOfWork.ConstructProductRepository.Find(c => c.ConstructId == existingConstruct.Id).ToList();
                //if (currentConstructProducts.Any())
                //{
                //    foreach(var cpp in currentConstructProducts)
                //    {
                //        unitOfWork.ConstructProductRepository.Delete(cpp);
                //        unitOfWork.Save();
                //    }
                //    foreach(var cpv in constructsView.constructProductsViews)
                //    {
                //        var constructProduct = new ConstructProducts
                //        {
                //            ConstructId = existingConstruct.Id,
                //            ProductId = cpv.ProductId
                //        };
                //        unitOfWork.ConstructProductRepository.Insert(constructProduct);
                //        unitOfWork.Save();
                //    }
                //} else
                //{
                //    foreach (var cpv in constructsView.constructProductsViews)
                //    {
                //        var constructProduct = new ConstructProducts
                //        {
                //            ConstructId = existingConstruct.Id,
                //            ProductId = cpv.ProductId
                //        };
                //        unitOfWork.ConstructProductRepository.Insert(constructProduct);
                //        unitOfWork.Save();
                //    }
                //}
                
                return Ok("Update successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/api/v1/constructs/put/status/id={id}")]
        public IActionResult SetStatusConstruct(int id)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }

                if(existingConstruct.Status == true)
                {
                    existingConstruct.Status = false;
                    unitOfWork.ConstructRepository.Update(existingConstruct);
                    unitOfWork.Save();
                    return Ok($"Construct with ID {id} set status to false successfully.");
                } else
                {
                    existingConstruct.Status = true;
                    unitOfWork.ConstructRepository.Update(existingConstruct);
                    unitOfWork.Save();
                    return Ok($"Construct with ID {id} set status to true successfully.");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/api/v1/constructs/delete/id={id}")]
        public IActionResult DeleteConstructById(int id)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }

                var constructProducts = unitOfWork.ConstructProductRepository.Find(c => c.ConstructId == existingConstruct.Id).ToList();
                if(constructProducts.Any())
                {
                    foreach (var cp in constructProducts)
                    {
                        unitOfWork.ConstructProductRepository.Delete(cp.Id);
                        unitOfWork.Save();
                    }
                }

                var constructImage = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == existingConstruct.Id).ToList();
                foreach (var image in constructImage)
                {
                    if (!String.IsNullOrEmpty(image.ImageUrl))
                    {
                        string imagePath = Path.Combine(_imagesDirectory, image.ImageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    unitOfWork.ConstructImageRepository.Delete(image.Id);
                    unitOfWork.Save();
                }

                unitOfWork.ConstructRepository.Delete(existingConstruct);
                unitOfWork.Save();

                return Ok($"Construct with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
