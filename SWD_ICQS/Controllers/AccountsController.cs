﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        
        private readonly IAccountsService _accountsService;

        public AccountsController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }


        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/login")]
        public IActionResult Login([FromBody] AccountsView loginInfo)
        {
            IActionResult response = Unauthorized();
            var account_ = _accountsService.AuthenticateUser(loginInfo);
            if (account_ == null)
            {
                return NotFound("No account found");
            }
            if (account_.Status == true)
            {
                var token = _accountsService.GenerateToken(account_);
                response = Ok(new { accessToken = token, account_ });
            }
            else
            {
                return BadRequest("Your account is locked");
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/register")]
        public IActionResult Register([FromBody] AccountsView newAccount)
        {
            var account = _accountsService.GetAccountByUsername(newAccount.Username);
            if(account == null)
            {
                bool checkRegister = _accountsService.CreateAccount(newAccount);
                if(checkRegister)
                {
                    return Ok("Create success");
                } else
                {
                    return BadRequest("Not correct role");
                }
            } else
            {
                return BadRequest("Existed username");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/accounts/get/username={username}")]
        public ActionResult GetAccountInfo(string username)
        {
            var account = _accountsService.GetAccountByUsername(username);
            if(account == null)
            {
                return NotFound($"No account with username {username} found");
            }
            var StringRole = _accountsService.GetAccountRole(username, account);
            if(StringRole == null)
            {
                return BadRequest("Some problems occur, cannot find your role!");
            }
            if (StringRole.Equals("CONTRACTOR"))
            {
                var contractor = _accountsService.GetContractorByAccount(account);
                if(contractor != null)
                {
                    var contractorsView = _accountsService.GetContractorInformation(username, account, contractor);
                    return Ok(contractorsView);
                } else
                {
                    return NotFound("Account existed but no contractor found, please contact Administrator");
                }
            } else if (StringRole.Equals("CUSTOMER"))
            {
                var customer = _accountsService.GetCustomerByUsername(account);
                if(customer != null)
                {
                    var customersView = _accountsService.GetCustomersInformation(username, account, customer);
                    return Ok(customersView);
                } else
                {
                    return NotFound("Account existed but no customer found, please contact Administrator");
                }
            } else
            {
                return BadRequest($"You dont have permission to access {username} information");
            }
        }
    }
}
