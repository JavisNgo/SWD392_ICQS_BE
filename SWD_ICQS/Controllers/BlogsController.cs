using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Reflection.Metadata;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : Controller
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public BlogsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/Blogs")]
        public async Task<IActionResult> getAllBlogs()
        {
            try
            {
                var blogsList = unitOfWork.BlogRepository.Get();
                return Ok(blogsList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/Blogs/{id}")]
        public IActionResult GetBlogById(int id)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.GetByID(id);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {id} not found.");
                }

                return Ok(blog);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/Blogs")]
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
                blog.PostTime = DateTime.Now;
                unitOfWork.BlogRepository.Insert(blog);
                unitOfWork.Save();

                return Ok(blogView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the blog. Error message: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpPut("/Blogs/{id}")]
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
                _mapper.Map(blogView, existingBlog);
                unitOfWork.BlogRepository.Update(existingBlog);
                unitOfWork.Save();
                return Ok(blogView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the Blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("/BlogStatus/{id}")]
        public IActionResult ChangeStatusBlog(int id)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.GetByID(id);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {id} not found.");
                }

                // Chỉ đặt thuộc tính Status là false thay vì xóa hoàn toàn
                blog.Status = false;

                unitOfWork.BlogRepository.Update(blog);
                unitOfWork.Save();

                return Ok("Set Status to false successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while changing status the blog. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpDelete("/Blogs/{id}")]
        public IActionResult DeleteBlog(int id)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.GetByID(id);

                if (blog == null)
                {
                    return NotFound($"Blog with ID {id} not found.");
                }

                // Chỉ đặt thuộc tính Status là false thay vì xóa hoàn toàn
                

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
