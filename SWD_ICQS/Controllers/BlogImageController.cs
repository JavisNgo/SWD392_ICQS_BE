using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Diagnostics.Contracts;
namespace SWD_ICQS.Controllers
{
    public class BlogImageController : Controller
    {
        //private IUnitOfWork unitOfWork;
        //private readonly IMapper _mapper;
        //private readonly string _imagesDirectory;

        //public BlogImageController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        //{
        //    this.unitOfWork = unitOfWork;
        //    _mapper = mapper;
        //    _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "contractorAvatar");
        //}

        //[AllowAnonymous]
        //[HttpGet("/BlogImages/{id}")]
        //public IActionResult GetBlogImageById(int id)
        //{
        //    try
        //    {
        //        var blogImage = unitOfWork.BlogImageRepository.Find(b => b.BlogId == id).ToList();

        //        if (blogImage == null)
        //        {
        //            return NotFound($"BlogImage with ID {id} not found.");
        //        }

        //        List<BlogImagesView> images = new List<BlogImagesView>();

        //        foreach (var item in blogImage)
        //        {
        //            BlogImagesView blogImagesView = new BlogImagesView();
        //            blogImagesView = _mapper.Map<BlogImagesView>(item);
        //            images.Add(blogImagesView);
        //        }

        //        foreach (var item in images)
        //        {
        //            item.ImageBin = $"https://localhost:7233/img/contractorAvatar/{item.ImageBin}";
        //        }
                
        //        return Ok(images);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while getting the blog image. Error message: {ex.Message}");
        //    }
        //}

        ////[AllowAnonymous]
        ////[HttpPost("/BlogImages")]
        ////public IActionResult AddBlogImage([FromBody] BlogImagesView blogImageView)
        ////{
        ////    try
        ////    {
        ////        var checkingBlogId = unitOfWork.BlogImageRepository.GetByID(blogImageView.BlogId);
        ////        if (checkingBlogId == null)
        ////        {
        ////            return NotFound("BlogId not found");
        ////        }
        ////        var blogImage = _mapper.Map<BlogImages>(blogImageView);
        ////        unitOfWork.BlogImageRepository.Insert(blogImage);
        ////        unitOfWork.Save();

        ////        return Ok(blogImage);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return BadRequest($"An error occurred while adding the blog image. Error message: {ex.Message}");
        ////    }
        ////}

        ////[AllowAnonymous]
        ////[HttpPut("/BlogImages/{id}")]
        ////public IActionResult AddImageToBlog(int id, IFormFile formFile)
        ////{
        ////    try
        ////    {
        ////        var existingBlog = unitOfWork.BlogImageRepository.GetByID(id);
        ////        if (existingBlog == null)
        ////        {
        ////            return NotFound($"BLog with ID {id} not found.");
        ////        }

        ////        // Check if a file is uploaded
        ////        if (formFile != null && formFile.Length > 0)
        ////        {
        ////            using (var memoryStream = new MemoryStream())
        ////            {
        ////                formFile.CopyTo(memoryStream);
        ////                existingBlog.ImageBin = memoryStream.ToArray();
        ////            }
        ////        }


        ////        // Insert the new product into the database
        ////        unitOfWork.BlogImageRepository.Update(existingBlog);
        ////        unitOfWork.Save();
        ////        return Ok("Add Image Successfully");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return BadRequest($"An error occurred while save the blog. Error message: {ex.Message}");
        ////    }
        ////}

        //[AllowAnonymous]
        //[HttpDelete("/BlogImages/{id}")]
        //public IActionResult DeleteBlogImage(int id)
        //{
        //    try
        //    {
        //        var blogImage = unitOfWork.BlogImageRepository.GetByID(id);

        //        if (blogImage == null)
        //        {
        //            return NotFound($"BlogImage with ID {id} not found.");
        //        }

        //        unitOfWork.BlogImageRepository.Delete(id);
        //        unitOfWork.Save();

        //        return Ok("Delete successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while deleting the blog image. Error message: {ex.Message}");
        //    }
        //}
    }
}
