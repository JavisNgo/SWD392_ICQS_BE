using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Diagnostics.Contracts;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contracts")]
        public ActionResult<IEnumerable<ContractViewForGet>> GetContracts()
        {
            try
            {
                var contract = _contractService.GetAllContract();

                return Ok(contract);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("/api/v1/contracts/{contractId}")]
        public ActionResult<ContractViewForGet> GetContractById(int contractId)
        {
            try
            {
              var contract= _contractService.GetContractById(contractId);
                return Ok(contract);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet("/api/v1/contracts/contractor/{contractorId}")]
        public ActionResult<IEnumerable<ContractViewForGet>> GetContractsByContractorId(int contractorId)
        {
            try
            {

                var contract = _contractService.GetContractsByContractorId(contractorId);

                return Ok(contract);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contracts/customer/{customerId}")]
        public ActionResult<IEnumerable<ContractViewForGet>> GetContractsByCustomerId(int customerId)
        {
            try
            {
                var contract = _contractService.GetContractsByCustomerId(customerId);
                return Ok(contract);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [AllowAnonymous]
        [HttpPost("/Contracts")]
        public IActionResult AddContract([FromBody] ContractsView contractView)
        {
            try
            {
                var contract = _contractService.AddContract(contractView);
                return Ok(contract);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the contract. Error message: {ex.Message}");
            }
        }


        [HttpPut("UpdateProgress/{id}")]
        public IActionResult UploadContractProgress(int id, [FromBody] ContractsView contractsView)
        {
            try
            {
                bool contract = _contractService.UploadContractProgress(id, contractsView);
                if (contract)
                {
                    return Ok("Contract updated successfully.");
                }
                else
                {
                    return NotFound($"Contract with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the contract. Error message: {ex.Message}");
            }
        }

        [HttpPut("/Contracts/{id}")]
        public IActionResult UpdateContract(int id, [FromBody] ContractsView contractsView)
        {
            try
            {
               var contract = _contractService.UpdateContract(id, contractsView);
                return Ok(contract);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the contract. Error message: {ex.Message}");
            }
        }
    }
}
