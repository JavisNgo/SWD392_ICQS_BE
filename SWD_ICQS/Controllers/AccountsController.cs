using AutoMapper;
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
            bool checkExist = _accountsService.checkExistedAccount(newAccount.Username);
            if(!checkExist)
            {
                _accountsService.CreateAccount(newAccount);
                return Ok("Create success");
            } else
            {
                return BadRequest("Existed username");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/accounts/get/username={username}")]
        public ActionResult GetAccountInfo(string username)
        {
            if(!_accountsService.checkExistedAccount(username))
            {
                return NotFound("No account found in database");
            }
            string StringRole = _accountsService.GetAccountRole(username);
            if (StringRole.Equals("CONTRACTOR"))
            {
                bool checkContractor = _accountsService.checkExistedContractor(username);
                if(checkContractor)
                {
                    ContractorsView contractorsView = _accountsService.GetContractorInformation(username);
                    return Ok(contractorsView);
                } else
                {
                    return NotFound("Account existed but no contractor found, please contact Administrator");
                }
            } else if (StringRole.Equals("CUSTOMER"))
            {
                bool checkCustomer = _accountsService.checkExistedCustomer(username);
                if(checkCustomer)
                {
                    CustomersView customersView = _accountsService.GetCustomersInformation(username);
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
