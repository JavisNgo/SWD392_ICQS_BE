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

        public ContractorsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Contractors
        [AllowAnonymous]
        [HttpGet("/api/v1/contractors")]
        public ActionResult<IEnumerable<ContractorsView>> GetContractors()
        {
            if (_unitOfWork.ContractorRepository.Get() == null)
            {
                return NotFound("No contractor found");
            }
            List<Contractors> contractors = _unitOfWork.ContractorRepository.Get().ToList();
            List<ContractorsView> contractorsViews = new List<ContractorsView>();
            foreach(Contractors contractor in contractors)
            {
                ContractorsView contractorsView = _mapper.Map<ContractorsView>(contractor);
                contractorsViews.Add(contractorsView);
            }
            return Ok(contractorsViews);
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contractors/id={id}")]
        public ActionResult<ContractorsView> GetContractor(int id)
        {
            if (_unitOfWork.ContractorRepository.Get() == null)
            {
                return NotFound("No contractor found");
            }
            var contractor = _unitOfWork.ContractorRepository.GetByID(id);
            if (contractor == null)
            {
                return NotFound("No contractor found");
            }
            var contractorsView = new ContractorsView
            {
                Email = contractor.Email,
                Name    = contractor.Name,
                PhoneNumber = contractor.PhoneNumber,
                Address = contractor.Address,
                AvatarBin = Convert.ToBase64String(contractor.AvatarBin),
                SubscriptionId = contractor.SubscriptionId,
                ExpiredDate = contractor.ExpiredDate,
                AccountId = contractor.AccountId
            };
            return Ok(contractorsView);
        }

        // PUT: api/Contractors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPut("/api/v1/contractors/username={username}")]
        public IActionResult UpdateContractor(string username, ContractorsView contractorsView)
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
            try
            {
                // image string to bin
                byte[] imageData = Convert.FromBase64String(contractorsView.AvatarBin);

                contractor.Name = contractorsView.Name;
                contractor.Email = contractorsView.Email;
                contractor.PhoneNumber = contractorsView.PhoneNumber;
                contractor.Address = contractorsView.Address;
                contractor.AvatarBin = imageData;
                _unitOfWork.ContractorRepository.Update(contractor);
                _unitOfWork.Save();
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
            if(_unitOfWork.ContractorRepository.Get() == null)
            {
                return NotFound("No contractor in database to disable");
            }
            var contractor = _unitOfWork.ContractorRepository.GetByID(id);
            if (contractor == null)
            {
                return NotFound("No contractor found");
            }
            var account = _unitOfWork.AccountRepository.Find(a => a.Id == contractor.AccountId).FirstOrDefault();
            if(account == null)
            {
                return NotFound("No account found");
            }
            try
            {
                if(account.Status == true)
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
                return BadRequest(ex.Message);
            }
            return BadRequest("Something went wrong!");
        }
    }
}
