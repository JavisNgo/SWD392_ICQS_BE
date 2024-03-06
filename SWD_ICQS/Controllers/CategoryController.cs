using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Net.WebSockets;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper) {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet("/Categories")]
        public async Task<IActionResult> getAllCategories()
        {
            try
            {
                var categoriesList = unitOfWork.CategoryRepository.Get();
                return Ok(categoriesList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/Categories/{id}")]
        public IActionResult GetCategoryById(int id)
        {
            try
            {
                var category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the category. Error message: {ex.Message}");
            }
        }

        [HttpPost("/Categories")]
        public IActionResult AddCategory([FromBody] CategoriesView categoryView)
        {
            try
            {
                // Validate the name using a regular expression
                if (!IsValidName(categoryView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters.");
                }

                var category = _mapper.Map<Categories>(categoryView);
                unitOfWork.CategoryRepository.Insert(category);
                unitOfWork.Save();
                return Ok(categoryView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the category. Error message: {ex.Message}");
            }
        }

        [HttpPut("/Categories/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoriesView updatedCategoryView)
        {
            try
            {
                var existingCategory = unitOfWork.CategoryRepository.GetByID(id);

                if (existingCategory == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                // Validate the name using a regular expression
                if (!IsValidName(updatedCategoryView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters.");
                }

                // Map the properties from updatedCategoryView to existingCategory using AutoMapper
                _mapper.Map(updatedCategoryView, existingCategory);

                // Mark the entity as modified
                unitOfWork.CategoryRepository.Update(existingCategory);
                unitOfWork.Save();

                return Ok(existingCategory); // Return the updated category
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the category. Error message: {ex.Message}");
            }
        }



        [HttpDelete("/Categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                unitOfWork.CategoryRepository.Delete(id);
                unitOfWork.Save();

                // You can return a custom response message 
                return Ok(new { Message = $"Category with ID {id} has been successfully deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the category. Error message: {ex.Message}");
            }
        }


        private bool IsValidName(string name)
        {
            // Use a regular expression to check if the name contains only letters and spaces
            return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s]+$");
        }

    }
}
