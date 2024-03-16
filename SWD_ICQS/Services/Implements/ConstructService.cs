using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Text;

namespace SWD_ICQS.Services.Implements
{
    public class ConstructService : IConstructService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public ConstructService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "constructImage");
        }

        public List<ConstructsView>? GetAllConstruct()
        {
            try
            {
                var constructsList = unitOfWork.ConstructRepository.Get();
                List<ConstructsView>? constructsViews = new List<ConstructsView>();
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
                return constructsViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Constructs>? GetConstructsByContractorId(int contractorId)
        {
            try
            {
                var constructsList = unitOfWork.ConstructRepository.Find(c => c.ContractorId == contractorId).ToList();
                return constructsList;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ConstructsView>? GetConstructsViewsByContractorId(List<Constructs> constructsList)
        {
            try
            {
                List<ConstructsView>? constructsViews = new List<ConstructsView>();
                if (constructsList.Any())
                {
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
                }
                return constructsViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Constructs? GetConstructsById(int id)
        {
            try
            {
                var construct = unitOfWork.ConstructRepository.GetByID(id);
                return construct;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ConstructsView? GetConstructsViewByConstruct(int id, Constructs construct)
        {
            try
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

                return constructsView;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public Contractors? GetContractorById(int id)
        {
            try
            {
                var contractor = unitOfWork.ContractorRepository.GetByID(id);
                return contractor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Categories? GetCategoryById(int id)
        {
            try
            {
                var category = unitOfWork.CategoryRepository.GetByID(id);
                return category;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        public bool IsCreateConstruct(ConstructsView constructsView)
        {
            try
            {
                bool status = false;

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
                bool checking = true;
                while (checking)
                {
                    if (unitOfWork.ProductRepository.Find(p => p.Code == code).FirstOrDefault() != null)
                    {
                        code = $"P_{constructsView.ContractorId}_{GenerateRandomCode(10)}";
                    }
                    else
                    {
                        checking = false;
                    }
                };
                var construct = new Constructs
                {
                    Code = code,
                    ContractorId = constructsView.ContractorId,
                    CategoryId = constructsView.CategoryId,
                    Name = constructsView.Name,
                    Description = constructsView.Description,
                    EstimatedPrice = constructsView.EstimatedPrice,
                    Status = true
                };
                unitOfWork.ConstructRepository.Insert(construct);
                unitOfWork.Save();

                var createdConstruct = unitOfWork.ConstructRepository.Find(c => c.Code == code).FirstOrDefault();

                if(constructsView.constructImagesViews != null && createdConstruct != null)
                {
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
                }
                status = true;

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

                return status;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsUpdateConstruct(ConstructsView constructsView, Constructs construct)
        {
            try {
                bool status = false;

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

                construct.CategoryId = constructsView.CategoryId;
                construct.Name = constructsView.Name;
                construct.Description = constructsView.Description;
                construct.EstimatedPrice = constructsView.EstimatedPrice;
                unitOfWork.ConstructRepository.Update(construct);
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
                            string filename = $"ConstructImage_{construct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var constructImage = new ConstructImages
                            {
                                ConstructId = construct.Id,
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
                            string filename = $"ConstructImage_{construct.Id}_{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var constructImage = new ConstructImages
                            {
                                ConstructId = construct.Id,
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

                status = true;

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

                return status;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsChangedStatusConstruct(Constructs construct)
        {
            try
            {
                bool status = false;
                if (construct.Status == true)
                {
                    construct.Status = false;
                }
                else
                {
                    construct.Status = true;
                }
                unitOfWork.ConstructRepository.Update(construct);
                unitOfWork.Save();
                status = true;

                return status;
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }

        public bool IsDeleteConstruct(Constructs construct)
        {
            try
            {
                bool status = false;
                var constructProducts = unitOfWork.ConstructProductRepository.Find(c => c.ConstructId == construct.Id).ToList();
                if (constructProducts.Any())
                {
                    foreach (var cp in constructProducts)
                    {
                        unitOfWork.ConstructProductRepository.Delete(cp.Id);
                        unitOfWork.Save();
                    }
                }

                var constructImage = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == construct.Id).ToList();
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

                unitOfWork.ConstructRepository.Delete(construct);
                unitOfWork.Save();
                status = true;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
