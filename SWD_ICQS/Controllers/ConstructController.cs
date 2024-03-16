using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Reflection.Metadata;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;
        private readonly IConstructService _constructService;

        public ConstructController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env, IConstructService constructService)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "constructImage");
            _constructService = constructService;
        }

        [HttpGet("/api/v1/constructs/get")]
        public async Task<IActionResult> GetAllConstruct()
        {
            var constructsViews = _constructService.GetAllConstruct();
            return Ok(constructsViews);
        }

        [HttpGet("/api/v1/constructs/get/contractorid={contractorid}")]
        public async Task<IActionResult> GetAllConstructByContractorId(int contractorid)
        {
            var constructsList = _constructService.GetConstructsByContractorId(contractorid);
            var constructsViewList = _constructService.GetConstructsViewsByContractorId(constructsList);
            return Ok(constructsViewList);
        }

        [HttpGet("/api/v1/constructs/get/id={id}")]
        public IActionResult GetConstructByID(int id)
        {
            var construct = _constructService.GetConstructsById(id);
            if (construct == null)
            {
                return NotFound($"Construct with ID : {id} not found");
            }
            var constructsView = _constructService.GetConstructsViewByConstruct(id, construct);
            return Ok(constructsView);
        }

        [HttpPost("/api/v1/constructs/post")]
        public IActionResult AddConstruct([FromBody] ConstructsView constructsView)
        {
            var checkingContractor = _constructService.GetContractorById(constructsView.ContractorId);
            if (checkingContractor == null)
            {
                return NotFound("Contractor not found");
            }
            var checkingCategory = _constructService.GetCategoryById(constructsView.CategoryId);
            if (checkingCategory == null)
            {
                return NotFound("Category not found");
            }
            if (!constructsView.EstimatedPrice.HasValue && constructsView.EstimatedPrice.Value < 0)
            {
                return BadRequest("EstimatedPrice must be greater than 1 and has value");
            }
            bool isCreated = _constructService.IsCreateConstruct(constructsView);
            if(isCreated)
            {
                return Ok("Create successfully");
            } else
            {
                return BadRequest("Create failed");
            }
        }

        

        [HttpPut("/api/v1/constructs/put")]
        public IActionResult UpdateConstruct([FromBody] ConstructsView constructsView)
        {
            var existingConstruct = _constructService.GetConstructsById(constructsView.Id);
            if (existingConstruct == null)
            {
                return NotFound($"Construct with ID : {constructsView.Id} not found");
            }
            var checkingContractor = _constructService.GetContractorById(constructsView.ContractorId);
            if (checkingContractor == null)
            {
                return NotFound("Contractor not found");
            }
            var checkingCategory = _constructService.GetCategoryById(constructsView.CategoryId);
            if (checkingCategory == null)
            {
                return NotFound("Category not found");
            }
            if (!constructsView.EstimatedPrice.HasValue && constructsView.EstimatedPrice.Value < 0)
            {
                return BadRequest("EstimatedPrice must be greater than 1 and has value");
            }
            bool IsUpdate = _constructService.IsUpdateConstruct(constructsView, existingConstruct);
            if (IsUpdate)
            {
                return Ok("Update success");
            }
            else
            {
                return BadRequest("Update failed");
            }
        }

        [HttpPut("/api/v1/constructs/put/status/id={id}")]
        public IActionResult SetStatusConstruct(int id)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }

                if(existingConstruct.Status == true)
                {
                    existingConstruct.Status = false;
                    unitOfWork.ConstructRepository.Update(existingConstruct);
                    unitOfWork.Save();
                    return Ok($"Construct with ID {id} set status to false successfully.");
                } else
                {
                    existingConstruct.Status = true;
                    unitOfWork.ConstructRepository.Update(existingConstruct);
                    unitOfWork.Save();
                    return Ok($"Construct with ID {id} set status to true successfully.");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/api/v1/constructs/delete/id={id}")]
        public IActionResult DeleteConstructById(int id)
        {
            try
            {
                var existingConstruct = unitOfWork.ConstructRepository.GetByID(id);
                if (existingConstruct == null)
                {
                    return NotFound($"Construct with ID : {id} not found");
                }

                var constructProducts = unitOfWork.ConstructProductRepository.Find(c => c.ConstructId == existingConstruct.Id).ToList();
                if(constructProducts.Any())
                {
                    foreach (var cp in constructProducts)
                    {
                        unitOfWork.ConstructProductRepository.Delete(cp.Id);
                        unitOfWork.Save();
                    }
                }

                var constructImage = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == existingConstruct.Id).ToList();
                foreach (var image in constructImage)
                {
                    if (!String.IsNullOrEmpty(image.ImageUrl))
                    {
                        string imagePath = Path.Combine(_imagesDirectory, image.ImageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    unitOfWork.ConstructImageRepository.Delete(image.Id);
                    unitOfWork.Save();
                }

                unitOfWork.ConstructRepository.Delete(existingConstruct);
                unitOfWork.Save();

                return Ok($"Construct with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
