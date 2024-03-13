using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructImageController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ConstructImageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/ConstructImages")]
        public async Task<IActionResult> getAllConstructImages()
        {
            
            try
            {
                var constructImages = unitOfWork.ConstructImageRepository.Get();
                return Ok(constructImages);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }
        [AllowAnonymous]
        [HttpGet("/ConstructImages/{id}")]
        public IActionResult GetConstructImageById(int id)
        {
            try
            {
                var constructImage = unitOfWork.ConstructImageRepository.GetByID(id);

                if (constructImage == null)
                {
                    return NotFound($"ConstructImage with ID {id} not found.");
                }

                return Ok(constructImage);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the construct image. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/ConstructImages")]
        public IActionResult AddConstructImage([FromBody] ConstructImagesView ConstructImageView)
        {
            try
            {
                var constructImage = _mapper.Map<ConstructImages>(ConstructImageView);
                unitOfWork.ConstructImageRepository.Insert(constructImage);
                unitOfWork.Save();

                return Ok(constructImage);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the Construct image. Error message: {ex.Message}");
            }
        }

        

        [AllowAnonymous]
        [HttpDelete("/ConstructImages/{id}")]
        public IActionResult DeleteConstructImage(int id)
        {
            try
            {
                var constructImage = unitOfWork.ConstructImageRepository.GetByID(id);

                if (constructImage == null)
                {
                    return NotFound($"ConstructImage with ID {id} not found.");
                }

                unitOfWork.ConstructImageRepository.Delete(id);
                unitOfWork.Save();

                return Ok("Delete successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the construct image. Error message: {ex.Message}");
            }
        }
    }
}
