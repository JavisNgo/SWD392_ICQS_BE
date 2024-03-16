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
        private readonly IConstructService _constructService;

        public ConstructController(IConstructService constructService)
        {
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
            if (string.IsNullOrEmpty(constructsView.Name))
            {
                return BadRequest("Construct's name is required");
            }
            if(constructsView.EstimatedPrice < 0)
            {
                return BadRequest("Estimate Price must larger than 0 ");
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
            if (string.IsNullOrEmpty(constructsView.Name))
            {
                return BadRequest("Construct's name is required");
            }
            if (constructsView.EstimatedPrice < 0)
            {
                return BadRequest("Estimate Price must larger than 0 ");
            }
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
            var existingConstruct = _constructService.GetConstructsById(id);
            if (existingConstruct == null)
            {
                return NotFound($"Construct with ID : {id} not found");
            }
            bool IsChanged = _constructService.IsChangedStatusConstruct(existingConstruct);
            if(IsChanged)
            {
                return Ok("Changed status successfully");
            } else
            {
                return BadRequest("Change status failed");
            }
        }

        [HttpDelete("/api/v1/constructs/delete/id={id}")]
        public IActionResult DeleteConstructById(int id)
        {
            var existingConstruct = _constructService.GetConstructsById(id);
            if (existingConstruct == null)
            {
                return NotFound($"Construct with ID : {id} not found");
            }
            bool IsDelete = _constructService.IsDeleteConstruct(existingConstruct);
            if(IsDelete)
            {
                return Ok("Delete construct successfully");
            }
            else
            {
                return BadRequest("Delete construct failed");
            }
        }

    }
}
