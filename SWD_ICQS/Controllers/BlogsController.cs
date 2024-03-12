using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : Controller
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public BlogsController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "blogImage");
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/blogs")]
        public async Task<IActionResult> getAllBlogs()
        {
            try
            {
                List<BlogsView> blogsViews = new List<BlogsView>();
                var blogsList = unitOfWork.BlogRepository.Get();

                foreach (var blog in blogsList)
                {
                    var blogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == blog.Id).ToList();
                    var blogsView = _mapper.Map<BlogsView>(blog);

                    if (blogImages.Any())
                    {
                        blogsView.blogImagesViews = new List<BlogImagesView>();
                        foreach (var image in blogImages)
                        {
                            image.ImageBin = $"https://localhost:7233/img/blogImage/{image.ImageBin}";
                            blogsView.blogImagesViews.Add(_mapper.Map<BlogImagesView>(image));
                        }
                    }
                    blogsViews.Add(blogsView);
                }

                return Ok(blogsViews);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/blogs/id={id}")]
        public IActionResult GetBlogById(int id)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.GetByID(id);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {id} not found.");
                }

                var blogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == blog.Id).ToList();

                var blogsView = _mapper.Map<BlogsView>(blog);

                if (blogImages.Any())
                {
                    blogsView.blogImagesViews = new List<BlogImagesView>();
                    foreach (var image in blogImages)
                    {
                        image.ImageBin = $"https://localhost:7233/img/blogImage/{image.ImageBin}";
                        blogsView.blogImagesViews.Add(_mapper.Map<BlogImagesView>(image));
                    }
                }

                return Ok(blogsView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/blogs/post")]
        public IActionResult AddBlog([FromBody] BlogsView blogView)
        {
            try
            {
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(blogView.ContractorId);
                if (checkingContractorID == null)
                {
                    return NotFound("ContractorID not found");
                }

                Blogs blog = _mapper.Map<Blogs>(blogView);
                DateTime postTime = DateTime.Now;
                blog.PostTime = postTime;
                unitOfWork.BlogRepository.Insert(blog);
                unitOfWork.Save();

                var blogCreated = unitOfWork.BlogRepository.Find(b => b.ContractorId == blogView.ContractorId && b.Content == blogView.Content && b.Title == blogView.Title && b.PostTime == postTime).FirstOrDefault();

                if(blogView.blogImagesViews.Any())
                {
                    foreach (var item in blogView.blogImagesViews)
                    {
                        if (!String.IsNullOrEmpty(item.ImageBin))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(item.ImageBin);
                            string filename = $"BlogImage-{blogCreated.Id}-{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var blogImage = new BlogImages
                            {
                                BlogId = blogCreated.Id,
                                ImageBin = filename
                            };
                            unitOfWork.BlogImageRepository.Insert(blogImage);
                            unitOfWork.Save();
                        }
                    }
                }

                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the blog. Error message: {ex.Message}");
            }
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


        [AllowAnonymous]
        [HttpPut("/api/v1/blogs/update/id={id}")]
        public IActionResult UpdateBlog(int id, [FromBody] BlogsView blogView)
        {
            try
            {
                var existingBlog = unitOfWork.BlogRepository.GetByID(id);
                if (existingBlog == null)
                {
                    return NotFound($"BLog with ID : {id} not found");
                }
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(blogView.ContractorId);
                if (checkingContractorID == null)
                {
                    return NotFound("ContractorID not found");
                }

                var currentBlogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == id).ToList();

                existingBlog.ContractorId = blogView.ContractorId;
                existingBlog.Title = blogView.Title;
                existingBlog.Content = blogView.Content;
                existingBlog.PostTime = blogView.PostTime;
                existingBlog.EditTime = DateTime.Now;
                existingBlog.Status = blogView.Status;

                unitOfWork.BlogRepository.Update(existingBlog);
                unitOfWork.Save();

                int countUrl = 0;
                foreach (var image in blogView.blogImagesViews)
                {
                    if (!String.IsNullOrEmpty(image.ImageBin))
                    {
                        if (image.ImageBin.Contains("https://localhost:7233/img/blogImage/")) {
                            countUrl++;
                        }
                    }
                }

                if(countUrl == currentBlogImages.Count)
                {
                    foreach (var image in blogView.blogImagesViews)
                    {
                        if (!image.ImageBin.Contains("https://localhost:7233/img/blogImage/") && !String.IsNullOrEmpty(image.ImageBin))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageBin);
                            string filename = $"BlogImage-{id}-{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var blogImage = new BlogImages
                            {
                                BlogId = id,
                                ImageBin = filename
                            };
                            unitOfWork.BlogImageRepository.Insert(blogImage);
                            unitOfWork.Save();
                        }
                    }
                } else if(countUrl < currentBlogImages.Count)
                {
                    List<BlogImages> tempList = currentBlogImages;
                    foreach (var image in blogView.blogImagesViews)
                    {
                        if (!image.ImageBin.Contains("https://localhost:7233/img/blogImage/") && !String.IsNullOrEmpty(image.ImageBin))
                        {
                            string randomString = GenerateRandomString(15);
                            byte[] imageBytes = Convert.FromBase64String(image.ImageBin);
                            string filename = $"BlogImage-{id}-{randomString}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            var blogImage = new BlogImages
                            {
                                BlogId = id,
                                ImageBin = filename
                            };
                            unitOfWork.BlogImageRepository.Insert(blogImage);
                            unitOfWork.Save();
                        }
                        else if (image.ImageBin.Contains("https://localhost:7233/img/blogImage/"))
                        {
                            for (int i = tempList.Count - 1; i >= 0; i--)
                            {
                                string url = $"https://localhost:7233/img/blogImage/{tempList[i].ImageBin}";
                                if (url.Equals(image.ImageBin))
                                {
                                    tempList.RemoveAt(i);
                                }
                            }
                        }
                    }
                    foreach (var temp in tempList)
                    {
                        unitOfWork.BlogImageRepository.Delete(temp);
                        unitOfWork.Save();
                        if (!String.IsNullOrEmpty(temp.ImageBin))
                        {
                            string imagePath = Path.Combine(_imagesDirectory, temp.ImageBin);
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
                return BadRequest($"An error occurred while updating the Blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("/api/v1/blogs/status/id={id}")]
        public IActionResult ChangeStatusBlog(int id)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.GetByID(id);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {id} not found.");
                }

                if(blog.Status == true)
                {
                    blog.Status = false;
                    unitOfWork.BlogRepository.Update(blog);
                    unitOfWork.Save();
                    return Ok("Set Status to false successfully.");
                } else { 
                    blog.Status = true;
                    unitOfWork.BlogRepository.Update(blog);
                    unitOfWork.Save();

                    return Ok("Set Status to true successfully.");
                }            
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while changing status the blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpDelete("/api/v1/blogs/delete/id={id}")]
        public IActionResult DeleteBlog(int id)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.GetByID(id);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {id} not found.");
                }

                unitOfWork.BlogRepository.Delete(id);
                unitOfWork.Save();

                return Ok($"Blog with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the blog. Error message: {ex.Message}");
            }
        }

    }
}
