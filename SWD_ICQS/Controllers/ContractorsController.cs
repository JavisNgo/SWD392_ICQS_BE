using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorsController : ControllerBase
    {
        private readonly IContractorsService _contractorsService;

        public ContractorsController(IContractorsService contractorsService)
        {
            _contractorsService = contractorsService;
        }

        // GET: api/Contractors
        [AllowAnonymous]
        [HttpGet("/api/v1/contractors")]
        public ActionResult<IEnumerable<ContractorsView>> GetContractors()
        {
            var contractors = _contractorsService.GetAllContractor();
            if (contractors == null)
            {
                return NotFound("No contractor in database");
            }
            var contractorsViews = _contractorsService.GetContractorsView(contractors);
            return Ok(contractorsViews);
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contractors/id={id}")]
        public ActionResult<ContractorsView> GetContractor(int id)
        {
            var contractors = _contractorsService.GetAllContractor();
            if (contractors == null)
            {
                return NotFound("No contractor in database");
            }
            var contractor = _contractorsService.GetContractorById(id);
            if (contractor == null)
            {
                return NotFound($"No contractor found with id {id}");
            }
            var contractorsView = _contractorsService.GetContractorViewById(contractor);
            return Ok(contractorsView);
        }

        // PUT: api/Contractors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPut("/api/v1/contractors/username={username}")]
        public IActionResult UpdateContractor(string username, ContractorsView contractorsView)
        { 
            var account = _contractorsService.GetAccountByUsername(username);
            if (account == null)
            {
                return NotFound($"No account with username {username} found");

            }
            var contractor = _contractorsService.GetContractorByAccount(account);
            if (contractor == null)
            {
                return NotFound("No contractor found");
            }
            if (_contractorsService.IsUpdateContractor(username, contractorsView, account, contractor))
            {
                return Ok("Update successfully");
            }
            else
            {
                return BadRequest("Update failed");
            }
        }

        // DELETE: api/Contractors/5
        [AllowAnonymous]
        [HttpPut("/api/v1/contractors/status/id={id}")]
        public async Task<IActionResult> SetStatusContractor(int id)
        {
            var contractors = _contractorsService.GetAllContractor();
            if (contractors == null)
            {
                return NotFound("No contractor in database to disable");
            }
            var contractor = _contractorsService.GetContractorById(id);
            if (contractor == null)
            {
                return NotFound($"No contractor with id {id} found");
            }
            var account = _contractorsService.GetAccountByContractor(contractor);
            if (account == null)
            {
                return NotFound("No account found");
            }
            if (_contractorsService.IsChangedStatusContractorById(id, account))
            {
                return Ok("Change status success");
            }
            else
            {
                return BadRequest("Change status failed");
            }
        }
    }
}
