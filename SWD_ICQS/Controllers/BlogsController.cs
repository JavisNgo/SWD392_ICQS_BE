using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
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
        private readonly IBlogsService _blogService;

        public BlogsController(IBlogsService blogService)
        {
            _blogService = blogService;
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/blogs/get")]
        public async Task<IActionResult> getAllBlogs()
        {
            try
            {
                
                var blogsList = _blogService.GetBlogs();
                return Ok(blogsList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/blogs/get/contractorid={contractorid}")]
        public async Task<IActionResult> getAllBlogsOfContractor(int contractorid)
        {
            try
            {
                var blogsList = _blogService.getAllBlogsOfContractor(contractorid);
                if (blogsList != null)
                {
                    return Ok(blogsList);
                } else
                {
                    return NotFound("No blogs that posted by this contractor");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/blogs/get/code={code}")]
        public IActionResult GetBlogByCode(string? code)
        {
            try
            {
                var blog = _blogService.GetBlogByCode(code);

                if (blog == null)
                {
                    return NotFound($"Blog with Code: {code} not found.");
                }

                

                return Ok(blog);
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
            if (string.IsNullOrEmpty(blogView.Title))
            {
                return BadRequest("Title is required");
            }
            if(string.IsNullOrEmpty(blogView.Content))
            {
                return BadRequest("Content is required");
            }
            try
            {
                var blog = _blogService.AddBlog(blogView);
                if (blog == null)
                {
                    return BadRequest("An error occurred while adding the blog");
                }

                

                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the blog. Error message: {ex.Message}");
            }
        }

        


        [AllowAnonymous]
        [HttpPut("/api/v1/blogs/put")]
        public IActionResult UpdateBlog([FromBody] BlogsView blogView)
        {
            try
            {
                if (string.IsNullOrEmpty(blogView.Title))
                {
                    return BadRequest("Title is required");
                }
                if (string.IsNullOrEmpty(blogView.Content))
                {
                    return BadRequest("Content is required");
                }
                var existingBlog = _blogService.UpdateBlog(blogView);
                if (existingBlog == null)
                {
                    return NotFound($"Blog with Code : {blogView.Code} not found");
                }

                
                
                return Ok("Update successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the Blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("/api/v1/blogs/put/status/code={code}")]
        public IActionResult ChangeStatusBlog(string? code)
        {
            try
            {
                var blog = _blogService.ChangeStatusBlog(code);

                if (blog == null)
                {
                    return NotFound($"Blog with Code {code} not found.");
                }

                if(blog.Status == false)
                {
                    
                    return Ok("Set Status to false successfully.");
                } else { 
                    

                    return Ok("Set Status to true successfully.");
                }            
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while changing status the blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpDelete("/api/v1/blogs/delete/code={code}")]
        public IActionResult DeleteBlog(string? code)
        {
            try
            {
                var blog = _blogService.DeleteBlog(code);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {code} not found.");
                }

                

                return Ok($"Blog with ID: {code} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the blog. Error message: {ex.Message}");
            }
        }

    }
}
