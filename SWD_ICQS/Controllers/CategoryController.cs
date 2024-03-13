using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        [HttpGet("/api/v1/categories/get")]
        public async Task<IActionResult> getAllCategories()
        {
            try
            {
                var categoriesList = unitOfWork.CategoryRepository.Get();
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
                var category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                var categoriesView = _mapper.Map<CategoriesView>(category);

                var constructLists = unitOfWork.ConstructRepository.Find(c => c.CategoryId == id).ToList();

                if(constructLists.Any())
                {
                    categoriesView.constructsViewList = new List<ConstructsView>();
                    foreach (var construct in constructLists)
                    {
                        categoriesView.constructsViewList.Add(_mapper.Map<ConstructsView>(construct));
                    }
                    foreach (var construct in categoriesView.constructsViewList)
                    {
                        construct.constructImagesViews = new List<ConstructImagesView>();
                        var imageLists = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == construct.Id).ToList();
                        foreach (var image in imageLists)
                        {
                            image.ImageUrl = $"https://localhost:7233/img/constructImage/{image.ImageUrl}";
                            construct.constructImagesViews.Add(_mapper.Map<ConstructImagesView>(image));
                        }
                    }
                }

                return Ok(categoriesView);
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
                var category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                
                var existingConstruct = unitOfWork.ConstructRepository.Find(c => c.CategoryId == category.Id).ToList();
                if (existingConstruct.Any())
                {
                    return BadRequest("Please delete or change category of construct that contain this category");
                }

                unitOfWork.CategoryRepository.Delete(id);
                unitOfWork.Save();

                // You can return a custom response message 
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
