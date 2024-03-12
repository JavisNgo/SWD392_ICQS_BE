using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Customers
        [AllowAnonymous]
        [HttpGet("/api/v1/customers")]
        public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
        {
            if (_unitOfWork.CustomerRepository.Get() == null)
            {
                return NotFound("No customer found");
            }
            List<Customers> customers = _unitOfWork.CustomerRepository.Get().ToList();
            List<CustomersView> customersViews = new List<CustomersView>();
            foreach (Customers customer in customers)
            {
                CustomersView customersView = _mapper.Map<CustomersView>(customer);
                customersViews.Add(customersView);
            }
            return Ok(customersViews);
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/customers/id={id}")]
        public ActionResult<CustomersView> GetCustomer(int id)
        {
            if (_unitOfWork.CustomerRepository.Get() == null)
            {
                return NotFound("No customer found");
            }
            var customer = _unitOfWork.CustomerRepository.GetByID(id);
            if (customer == null)
            {
                return NotFound("No customer found");
            }
            CustomersView customersView = _mapper.Map<CustomersView>(customer);
            return Ok(customersView);
        }

        // PUT: api/Contractors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPut("/api/v1/customers/username={username}")]
        public IActionResult UpdateCustomer(string username, CustomersView customersView)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            if (account == null)
            {
                return NotFound("No account found in database");
            }
            if (account.Id != customersView.AccountId)
            {
                return BadRequest("Your current loged in session is not valid, please log in right account to update");
            }
            var customer = _unitOfWork.CustomerRepository.Find(a => a.AccountId == account.Id).FirstOrDefault();
            if (customer == null)
            {
                return NotFound("No customer found");
            }
            try
            {
                customer.Name = customersView.Name;
                customer.Email = customersView.Email;
                customer.PhoneNumber = customersView.PhoneNumber;
                customer.Address = customersView.Address;
                _unitOfWork.CustomerRepository.Update(customer);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Update successfully");
        }

        // DELETE: api/Contractors/5
        [AllowAnonymous]
        [HttpPut("/api/v1/customers/status/id={id}")]
        public async Task<IActionResult> SetStatusCustomer(int id)
        {
            if (_unitOfWork.CustomerRepository.Get() == null)
            {
                return NotFound("No customer in database to enable/disable");
            }
            var customer = _unitOfWork.CustomerRepository.GetByID(id);
            if (customer == null)
            {
                return NotFound("No customer found");
            }
            var account = _unitOfWork.AccountRepository.Find(a => a.Id == customer.AccountId).FirstOrDefault();
            if (account == null)
            {
                return NotFound("No account found");
            }
            try
            {
                if (account.Status == true)
                {
                    account.Status = false;
                    _unitOfWork.AccountRepository.Update(account);
                    _unitOfWork.Save();
                    return Ok("Disable successfully");
                }
                else if (account.Status == false)
                {
                    account.Status = true;
                    _unitOfWork.AccountRepository.Update(account);
                    _unitOfWork.Save();
                    return Ok("Enable successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Something went wrong!");
        }
    }
}
