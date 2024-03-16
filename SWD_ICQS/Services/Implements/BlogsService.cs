using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Text;

namespace SWD_ICQS.Services.Implements
{
    public class BlogsService : IBlogsService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public BlogsService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "blogImage");
        }
        public BlogsView AddBlog(BlogsView blogView)
        {
            try
            {
                var checkingContractorID = unitOfWork.ContractorRepository.GetByID(blogView.ContractorId);
                if (checkingContractorID == null)
                {
                    return null;
                }

                Blogs blog = _mapper.Map<Blogs>(blogView);

                string code = $"B_{blog.ContractorId}_{GenerateRandomCode(10)}";
                bool checking = true;
                while (checking)
                {
                    if (unitOfWork.ProductRepository.Find(p => p.Code == code).FirstOrDefault() != null)
                    {
                        code = $"P_{blogView.ContractorId}_{GenerateRandomCode(10)}";
                    }
                    else
                    {
                        checking = false;
                    }
                };
                blog.Code = code;
                DateTime postTime = DateTime.Now;
                blog.PostTime = postTime;
                unitOfWork.BlogRepository.Insert(blog);
                unitOfWork.Save();

                var blogCreated = unitOfWork.BlogRepository.Find(b => b.ContractorId == blogView.ContractorId && b.Content == blogView.Content && b.Title == blogView.Title && b.PostTime == postTime && b.Code == code).FirstOrDefault();

                if(blogView.blogImagesViews != null && blogCreated != null)
                {
                    if (blogView.blogImagesViews.Any())
                    {
                        foreach (var item in blogView.blogImagesViews)
                        {
                            if (!String.IsNullOrEmpty(item.ImageUrl))
                            {
                                string randomString = GenerateRandomString(15);
                                byte[] imageBytes = Convert.FromBase64String(item.ImageUrl);
                                string filename = $"BlogImage_{blogCreated.Id}_{randomString}.png";
                                string imagePath = Path.Combine(_imagesDirectory, filename);
                                System.IO.File.WriteAllBytes(imagePath, imageBytes);
                                var blogImage = new BlogImages
                                {
                                    BlogId = blogCreated.Id,
                                    ImageUrl = filename
                                };
                                unitOfWork.BlogImageRepository.Insert(blogImage);
                                unitOfWork.Save();
                            }
                        }
                    }
                }

                return blogView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the blog. Error message: {ex.Message}");
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

        public Blogs ChangeStatusBlog(string? code)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.Find(b => b.Code == code).FirstOrDefault();

                if (blog == null)
                {
                    return null;
                }

                if (blog.Status == true)
                {
                    blog.Status = false;
                    unitOfWork.BlogRepository.Update(blog);
                    unitOfWork.Save();
                    return blog;
                }
                else
                {
                    blog.Status = true;
                    unitOfWork.BlogRepository.Update(blog);
                    unitOfWork.Save();

                    return blog;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while changing status the blog. Error message: {ex.Message}");
            }
        }

        public Blogs DeleteBlog(string? code)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.Find(b => b.Code == code).FirstOrDefault();

                if (blog == null)
                {
                    return null;
                }

                var blogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == blog.Id).ToList();

                foreach (var image in blogImages)
                {
                    if (!String.IsNullOrEmpty(image.ImageUrl))
                    {
                        string imagePath = Path.Combine(_imagesDirectory, image.ImageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    unitOfWork.BlogImageRepository.Delete(image.Id);
                    unitOfWork.Save();
                }

                unitOfWork.BlogRepository.Delete(blog.Id);
                unitOfWork.Save();

                return blog;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the blog. Error message: {ex.Message}");
            }
        }

        public IEnumerable<BlogsView> getAllBlogsOfContractor(int contractorid)
        {
            try
            {
                var blogsList = unitOfWork.BlogRepository.Find(c => c.ContractorId == contractorid).ToList();
                if (blogsList.Any())
                {
                    List<BlogsView> blogsViews = new List<BlogsView>();

                    foreach (var blog in blogsList)
                    {
                        var blogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == blog.Id).ToList();
                        var blogsView = _mapper.Map<BlogsView>(blog);

                        if (blogImages.Any())
                        {
                            blogsView.blogImagesViews = new List<BlogImagesView>();
                            foreach (var image in blogImages)
                            {
                                image.ImageUrl = $"https://localhost:7233/img/blogImage/{image.ImageUrl}";
                                blogsView.blogImagesViews.Add(_mapper.Map<BlogImagesView>(image));
                            }
                        }
                        blogsViews.Add(blogsView);
                    }
                    return blogsViews;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        public BlogsView GetBlogByCode(string? code)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.Find(b => b.Code == code).FirstOrDefault();

                if (blog == null)
                {
                    return null;
                }

                var blogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == blog.Id).ToList();

                var blogsView = _mapper.Map<BlogsView>(blog);

                if (blogImages.Any())
                {
                    blogsView.blogImagesViews = new List<BlogImagesView>();
                    foreach (var image in blogImages)
                    {
                        image.ImageUrl = $"https://localhost:7233/img/blogImage/{image.ImageUrl}";
                        blogsView.blogImagesViews.Add(_mapper.Map<BlogImagesView>(image));
                    }
                }

                return blogsView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the blog. Error message: {ex.Message}");
            }
        }

        public IEnumerable<BlogsView> GetBlogs()
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
                            image.ImageUrl = $"https://localhost:7233/img/blogImage/{image.ImageUrl}";
                            blogsView.blogImagesViews.Add(_mapper.Map<BlogImagesView>(image));
                        }
                    }
                    blogsViews.Add(blogsView);
                }

                return blogsViews;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        public BlogsView UpdateBlog(BlogsView blogView)
        {
            try
            {
                var existingBlog = unitOfWork.BlogRepository.Find(b => b.Code == blogView.Code).FirstOrDefault();
                if (existingBlog == null)
                {
                    return null;
                }

                var currentBlogImages = unitOfWork.BlogImageRepository.Find(b => b.BlogId == existingBlog.Id).ToList();

                existingBlog.Title = blogView.Title;
                existingBlog.Content = blogView.Content;
                existingBlog.PostTime = blogView.PostTime;
                existingBlog.EditTime = DateTime.Now;

                unitOfWork.BlogRepository.Update(existingBlog);
                unitOfWork.Save();

                if(blogView.blogImagesViews != null)
                {
                    int countUrl = 0;
                    foreach (var image in blogView.blogImagesViews)
                    {
                        if (!String.IsNullOrEmpty(image.ImageUrl))
                        {
                            if (image.ImageUrl.Contains("https://localhost:7233/img/blogImage/"))
                            {
                                countUrl++;
                            }
                        }
                    }

                    if (countUrl == currentBlogImages.Count)
                    {
                        foreach (var image in blogView.blogImagesViews)
                        {
                            if (!image.ImageUrl.Contains("https://localhost:7233/img/blogImage/") && !String.IsNullOrEmpty(image.ImageUrl))
                            {
                                string randomString = GenerateRandomString(15);
                                byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                                string filename = $"BlogImage_{existingBlog.Id}_{randomString}.png";
                                string imagePath = Path.Combine(_imagesDirectory, filename);
                                System.IO.File.WriteAllBytes(imagePath, imageBytes);
                                var blogImage = new BlogImages
                                {
                                    BlogId = existingBlog.Id,
                                    ImageUrl = filename
                                };
                                unitOfWork.BlogImageRepository.Insert(blogImage);
                                unitOfWork.Save();
                            }
                        }
                    }
                    else if (countUrl < currentBlogImages.Count)
                    {
                        List<BlogImages> tempList = currentBlogImages;
                        foreach (var image in blogView.blogImagesViews)
                        {
                            if (!image.ImageUrl.Contains("https://localhost:7233/img/blogImage/") && !String.IsNullOrEmpty(image.ImageUrl))
                            {
                                string randomString = GenerateRandomString(15);
                                byte[] imageBytes = Convert.FromBase64String(image.ImageUrl);
                                string filename = $"BlogImage_{existingBlog.Id}_{randomString}.png";
                                string imagePath = Path.Combine(_imagesDirectory, filename);
                                System.IO.File.WriteAllBytes(imagePath, imageBytes);
                                var blogImage = new BlogImages
                                {
                                    BlogId = existingBlog.Id,
                                    ImageUrl = filename
                                };
                                unitOfWork.BlogImageRepository.Insert(blogImage);
                                unitOfWork.Save();
                            }
                            else if (image.ImageUrl.Contains("https://localhost:7233/img/blogImage/"))
                            {
                                for (int i = tempList.Count - 1; i >= 0; i--)
                                {
                                    string url = $"https://localhost:7233/img/blogImage/{tempList[i].ImageUrl}";
                                    if (url.Equals(image.ImageUrl))
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
                }

                return blogView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the Blog. Error message: {ex.Message}");
            }
        }
    }
}
