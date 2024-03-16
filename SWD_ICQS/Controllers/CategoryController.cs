using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Net.WebSockets;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        
        private readonly ICategoriesService _categoriesService; 

        public CategoryController(ICategoriesService categoriesService) {
            _categoriesService = categoriesService;
        }
        [AllowAnonymous]
        [HttpGet("/api/v1/categories/get")]
        public async Task<IActionResult> getAllCategories()
        {
            try
            {
                var categoriesList = _categoriesService.getAllCategories();
                return Ok(categoriesList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/categories/get/id={id}")]
        public IActionResult GetCategoryById(int id)
        {
            try
            {
                var category = _categoriesService.GetCategoryById(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("/api/v1/categories/post")]
        public IActionResult AddCategory([FromBody] CategoriesView categoryView)
        {
            if (string.IsNullOrEmpty(categoryView.Name))
            {
                return BadRequest("Category name is required");
            }
            try
            {
                // Validate the name using a regular expression
                if (!IsValidName(categoryView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters.");
                }

                var category = _categoriesService.AddCategory(categoryView);
                if(category == null)
                {
                    return BadRequest();
                }
                return Ok("Create successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPut("/api/v1/categories/put/id={id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoriesView updatedCategoryView)
        {
            if (string.IsNullOrEmpty(updatedCategoryView.Name))
            {
                return BadRequest("Category name is required");
            }
            try
            {
                // Validate the name using a regular expression
                if (!IsValidName(updatedCategoryView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters.");
                }
                var existingCategory = _categoriesService.UpdateCategory(id, updatedCategoryView);

                if (existingCategory == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                

                return Ok("Update successfully"); // Return the updated category
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpDelete("/api/v1/categories/delete/id={id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var category = _categoriesService.DeleteCategory(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                
                
                return Ok(new { Message = $"Category with ID {id} has been successfully deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        private bool IsValidName(string name)
        {
            // Use a regular expression to check if the name contains only letters and spaces
            return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s]+$");
        }

    }
}
