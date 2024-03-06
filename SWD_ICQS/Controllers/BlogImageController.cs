﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
namespace SWD_ICQS.Controllers
{
    public class BlogImageController : Controller
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public BlogImageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/BlogImages")]
        public async Task<IActionResult> getAllBlogImages()
        {
            try
            {
                var blogImageList = unitOfWork.BlogImageRepository.Get();
                return Ok(blogImageList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }
        [AllowAnonymous]
        [HttpGet("/BlogImages/{id}")]
        public IActionResult GetBlogImageById(int id)
        {
            try
            {
                var blogImage = unitOfWork.BlogImageRepository.GetByID(id);

                if (blogImage == null)
                {
                    return NotFound($"BlogImage with ID {id} not found.");
                }

                return Ok(blogImage);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the blog image. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/BlogImages")]
        public IActionResult AddBlogImage([FromBody] BlogImagesView blogImageView)
        {
            try
            {
                var blogImage = _mapper.Map<BlogImages>(blogImageView);

                unitOfWork.BlogImageRepository.Insert(blogImage);
                unitOfWork.Save();

                return Ok(blogImage);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the blog image. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPut("/BlogImages/{id}")]
        public IActionResult UpdateBlogImage(int id, [FromBody] BlogImagesView updatedBlogImageView)
        {
            try
            {
                var existingBlogImage = unitOfWork.BlogImageRepository.GetByID(id);

                if (existingBlogImage == null)
                {
                    return NotFound($"BlogImage with ID {id} not found.");
                }

                _mapper.Map(updatedBlogImageView, existingBlogImage);

                unitOfWork.BlogImageRepository.Update(existingBlogImage);
                unitOfWork.Save();

                return Ok(existingBlogImage);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the blog image. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpDelete("/BlogImages/{id}")]
        public IActionResult DeleteBlogImage(int id)
        {
            try
            {
                var blogImage = unitOfWork.BlogImageRepository.GetByID(id);

                if (blogImage == null)
                {
                    return NotFound($"BlogImage with ID {id} not found.");
                }

                unitOfWork.BlogImageRepository.Delete(id);
                unitOfWork.Save();

                return Ok("Delete successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the blog image. Error message: {ex.Message}");
            }
        }

    }
}
