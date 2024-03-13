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

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public ContractorsController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "contractorAvatar");
        }

        // GET: api/Contractors
        [AllowAnonymous]
        [HttpGet("/api/v1/contractors")]
        public ActionResult<IEnumerable<ContractorsView>> GetContractors()
        {
            try
            {
                if (_unitOfWork.ContractorRepository.Get() == null)
                {
                    return NotFound("No contractor found");
                }
                List<Contractors> contractors = _unitOfWork.ContractorRepository.Get().ToList();
                List<ContractorsView> contractorsViews = new List<ContractorsView>();
                foreach (Contractors contractor in contractors)
                {
                    ContractorsView contractorsView = _mapper.Map<ContractorsView>(contractor);
                    contractorsViews.Add(contractorsView);
                }
                return Ok(contractorsViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contractors/id={id}")]
        public ActionResult<ContractorsView> GetContractor(int id)
        {
            try {
                if (_unitOfWork.ContractorRepository.Get() == null)
                {
                    return NotFound("No contractor found");
                }
                var contractor = _unitOfWork.ContractorRepository.GetByID(id);
                if (contractor == null)
                {
                    return NotFound("No contractor found");
                }
                string url = null;
                if (!String.IsNullOrEmpty(contractor.AvatarUrl)) {
                    url = $"https://localhost:7233/img/contractorAvatar/{contractor.AvatarUrl}";
                }
                var contractorsView = new ContractorsView
                {
                    Email = contractor.Email,
                    Name = contractor.Name,
                    PhoneNumber = contractor.PhoneNumber,
                    Address = contractor.Address,
                    SubscriptionId = contractor.SubscriptionId,
                    ExpiredDate = contractor.ExpiredDate,
                    AvatarUrl = url,
                    AccountId = contractor.AccountId
                };
                return Ok(contractorsView);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Contractors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPut("/api/v1/contractors/username={username}")]
        public IActionResult UpdateContractor(string username, ContractorsView contractorsView)
        { 
            try
            {
                var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
                if (account == null)
                {
                    return NotFound("No account found in database");
                }
                if (account.Id != contractorsView.AccountId)
                {
                    return BadRequest("Your current loged in session is not valid, please log in right account to update");
                }
                var contractor = _unitOfWork.ContractorRepository.Find(a => a.AccountId == account.Id).FirstOrDefault();
                if (contractor == null)
                {
                    return NotFound("No contractor found");
                }
                string filename = null;
                string? tempString = contractor.AvatarUrl;
                if (!String.IsNullOrEmpty(contractorsView.AvatarUrl))
                {
                    byte[] imageBytes = Convert.FromBase64String(contractorsView.AvatarUrl);
                    filename = $"ContractorAvatar_{contractor.Id}.png";
                    string imagePath = Path.Combine(_imagesDirectory, filename);
                    System.IO.File.WriteAllBytes(imagePath, imageBytes);
                }
                contractor.Name = contractorsView.Name;
                contractor.Email = contractorsView.Email;
                contractor.PhoneNumber = contractorsView.PhoneNumber;
                contractor.Address = contractorsView.Address;
                contractor.AvatarUrl = filename;
                _unitOfWork.ContractorRepository.Update(contractor);
                _unitOfWork.Save();
                if(contractorsView.AvatarUrl == null && !String.IsNullOrEmpty(tempString))
                {
                    string imagePath = Path.Combine(_imagesDirectory, tempString);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Update successfully");
        }

        // DELETE: api/Contractors/5
        [AllowAnonymous]
        [HttpPut("/api/v1/contractors/status/id={id}")]
        public async Task<IActionResult> SetStatusContractor(int id)
        {
            try
            {
                if (_unitOfWork.ContractorRepository.Get() == null)
                {
                    return NotFound("No contractor in database to disable");
                }
                var contractor = _unitOfWork.ContractorRepository.GetByID(id);
                if (contractor == null)
                {
                    return NotFound("No contractor found");
                }
                var account = _unitOfWork.AccountRepository.Find(a => a.Id == contractor.AccountId).FirstOrDefault();
                if (account == null)
                {
                    return NotFound("No account found");
                }
                if (account.Status == true)
                {
                    account.Status = false;
                    _unitOfWork.AccountRepository.Update(account);
                    _unitOfWork.Save();
                    return Ok("Disable successfully");
                } else if (account.Status == false)
                {
                    account.Status = true;
                    _unitOfWork.AccountRepository.Update(account);
                    _unitOfWork.Save();
                    return Ok("Enable successfully");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return BadRequest("Something went wrong!");
        }
    }
}
