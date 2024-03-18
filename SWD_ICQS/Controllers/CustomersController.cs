using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
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
using SWD_ICQS.Services.Implements;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService _customersService;
        private readonly IAccountsService _accountsService;

        public CustomersController(ICustomersService customersService, IAccountsService accountsService)
        {
            _customersService = customersService;
            _accountsService = accountsService;
        }

        // GET: api/Customers
        [AllowAnonymous]
        [HttpGet("/api/v1/customers")]
        public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
        {
            var customers = _customersService.GetAllCustomers();
            if(customers == null)
            {
                return NotFound("No customer found");
            }
            var customersViews = _customersService.GetCustomersView(customers);
            return Ok(customersViews);
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/customers/id={id}")]
        public ActionResult<CustomersView> GetCustomer(int id)
        {
            var customers = _customersService.GetAllCustomers();
            if (customers == null)
            {
                return NotFound("No customer found");
            }
            var customer = _customersService.GetCustomersById(id);
            if (customer == null)
            {
                return NotFound($"No customer with id {id} found");
            }
            var customersView = _customersService.GetCustomersViewById(customer);
            return Ok(customersView);
        }

        // PUT: api/Contractors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPut("/api/v1/customers/username={username}")]
        public IActionResult UpdateCustomer(string username, CustomersView customersView)
        {
            if (string.IsNullOrEmpty(customersView.Name))
            {
                return BadRequest("Full Name is required");
            }
            if (customersView.Name.Length > 100)
            {
                return BadRequest("Full Name must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(customersView.Email))
            {
                return BadRequest("Email is required");
            }
            if (!IsValidEmail(customersView.Email))
            {
                return BadRequest("Invalid email address");
            }

            if (string.IsNullOrEmpty(customersView.Address))
            {
                return BadRequest("Address is required");
            }
            if (customersView.Address.Length > 100)
            {
                return BadRequest("Address must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(customersView.PhoneNumber))
            {
                return BadRequest("Phone number is required");
            }
            if (!Regex.IsMatch(customersView.PhoneNumber, @"^[0-9]{10}$"))
            {
                return BadRequest("Phone number must be exactly 10 digits");
            }
            if (_accountsService.IsExistedEmail(customersView.Email))
            {
                return BadRequest("Email you entered has already existed");
            }
            var account = _customersService.GetAccountByUsername(username);
            if (account == null)
            {
                return NotFound($"No account with username {username} found");

            }
            var customer = _customersService.GetCustomersByAccount(account);
            if (customer == null)
            {
                return NotFound("No customer found");
            }
            if (_customersService.IsUpdateCustomer(username, customersView, account, customer))
            {
                return Ok("Update successfully");
            }
            else
            {
                return BadRequest("Update failed");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // DELETE: api/Contractors/5
        [AllowAnonymous]
        [HttpPut("/api/v1/customers/status/id={id}")]
        public async Task<IActionResult> SetStatusCustomer(int id)
        {
            var customers = _customersService.GetAllCustomers();
            if (customers == null)
            {
                return NotFound("No customer in database to disable");
            }
            var customer = _customersService.GetCustomersById(id);
            if (customer == null)
            {
                return NotFound($"No customer with id {id} found");
            }
            var account = _customersService.GetAccountByCustomers(customer);
            if (account == null)
            {
                return NotFound("No account found");
            }
            if (_customersService.IsChangedStatusCustomerById(id, account))
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
