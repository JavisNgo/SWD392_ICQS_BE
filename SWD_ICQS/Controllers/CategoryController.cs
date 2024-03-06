using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
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
        

    }
}
