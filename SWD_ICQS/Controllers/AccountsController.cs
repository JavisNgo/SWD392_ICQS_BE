using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Implements;
using SWD_ICQS.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            if(loginInfo.Username.IsNullOrEmpty())
            {
                return BadRequest("Username is required");
            } else if(loginInfo.Password.IsNullOrEmpty())
            {
                return BadRequest("Password is required");
            }
            IActionResult response = Unauthorized();
            var account_ = _accountsService.AuthenticateUser(loginInfo);
            if (account_ == null)
            {
                return NotFound("No account found");
            }
            if (account_.Status == true)
            {
                var token = _accountsService.GenerateTokens(account_);
                response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken});
            }
            else
            {
                return BadRequest("Your account is locked");
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenView refreshTokenRequest)
        {
            var refreshToken = refreshTokenRequest.RefreshToken;

            // Validate the refresh token
            var (isValid, AccountId) = _accountsService.ValidateRefreshToken(refreshToken);

            if (!isValid)
            {
                return Unauthorized("Invalid refresh token");
            }

            // Get the user account associated with the refresh token
            var account = _accountsService.GetAccountById(int.Parse(AccountId));

            // Generate a new access token
            var accessToken = _accountsService.GenerateToken(account);

            return Ok(new { accessToken });
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/register")]
        public IActionResult Register([FromBody] AccountsView newAccount)
        {
            if (string.IsNullOrEmpty(newAccount.Name))
            {
                return BadRequest("Full Name is required");
            }
            if (newAccount.Name.Length > 100)
            {
                return BadRequest("Full Name must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(newAccount.Email))
            {
                return BadRequest("Email is required");
            }
            if (!IsValidEmail(newAccount.Email))
            {
                return BadRequest("Invalid email address");
            }

            if (string.IsNullOrEmpty(newAccount.Address))
            {
                return BadRequest("Address is required");
            }
            if (newAccount.Address.Length > 100)
            {
                return BadRequest("Address must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(newAccount.PhoneNumber))
            {
                return BadRequest("Phone number is required");
            }
            if (!Regex.IsMatch(newAccount.PhoneNumber, @"^[0-9]{10}$"))
            {
                return BadRequest("Phone number must be exactly 10 digits");
            }

            if (string.IsNullOrEmpty(newAccount.Username))
            {
                return BadRequest("Username is required");
            }

            if (newAccount.Username.Length > 100)
            {
                return BadRequest("Username must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(newAccount.Password))
            {
                return BadRequest("Password is required");
            }
            if (newAccount.Password.Length < 10 || !Regex.IsMatch(newAccount.Password, @"(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])"))
            {
                return BadRequest("Password must be at least 10 characters long and contain at least one uppercase letter, one lowercase letter, and one number");
            }
            if(_accountsService.IsExistedEmail(newAccount.Email))
            {
                return BadRequest("Email you entered has already existed");
            }
            var account = _accountsService.GetAccountByUsername(newAccount.Username);
            if(account == null)
            {
                bool checkRegister = _accountsService.CreateAccountCustomer(newAccount);
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
        [HttpPost("/api/v1/accounts/register/contractor")]
        public IActionResult RegisterContractor([FromBody] AccountsView newAccount)
        {
            if (string.IsNullOrEmpty(newAccount.Name))
            {
                return BadRequest("Full Name is required");
            }
            if (newAccount.Name.Length > 100)
            {
                return BadRequest("Full Name must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(newAccount.Email))
            {
                return BadRequest("Email is required");
            }
            if (!IsValidEmail(newAccount.Email))
            {
                return BadRequest("Invalid email address");
            }

            if (string.IsNullOrEmpty(newAccount.Address))
            {
                return BadRequest("Address is required");
            }
            if (newAccount.Address.Length > 100)
            {
                return BadRequest("Address must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(newAccount.PhoneNumber))
            {
                return BadRequest("Phone number is required");
            }
            if (!Regex.IsMatch(newAccount.PhoneNumber, @"^[0-9]{10}$"))
            {
                return BadRequest("Phone number must be exactly 10 digits");
            }

            if (string.IsNullOrEmpty(newAccount.Username))
            {
                return BadRequest("Username is required");
            }

            if (newAccount.Username.Length > 100)
            {
                return BadRequest("Username must be 100 characters or less");
            }

            if (string.IsNullOrEmpty(newAccount.Password))
            {
                return BadRequest("Password is required");
            }
            if (newAccount.Password.Length < 10 || !Regex.IsMatch(newAccount.Password, @"(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])"))
            {
                return BadRequest("Password must be at least 10 characters long and contain at least one uppercase letter, one lowercase letter, and one number");
            }
            if (_accountsService.IsExistedEmail(newAccount.Email))
            {
                return BadRequest("Email you entered has already existed");
            }
            var account = _accountsService.GetAccountByUsername(newAccount.Username);
            if (account == null)
            {
                bool checkRegister = _accountsService.CreateAccountContractor(newAccount);
                if (checkRegister)
                {
                    return Ok("Create success");
                }
                else
                {
                    return BadRequest("Not correct role");
                }
            }
            else
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

        [AllowAnonymous]
        [HttpGet("/api/v1/admin/stats")]
        public IActionResult GetPlatfromStatistics()
        {
            var stats = _accountsService.GetPlatformStats();
            return Ok(new
            {
                TotalRevenue = stats.TotalRevenue,
                TotalDepositRevenue = stats.TotalDepositRevenue,
                TotalCustomers = stats.TotalCustomers,
                TotalRequestedCustomer = stats.TotalFilterdCustomer,
                TotalContractors = stats.TotalContractors,
                TotalRequests = stats.TotalRequests,
                TotalSignedRequests = stats.TotalSignedRequests,
                TotalOnGoingRequests = stats.TotalOnGoingRequests,
                TotalRejectedRequests = stats.TotalRejectedRequests,
                TotalConstructs = stats.TotalConstructs,
                TotalProducts = stats.TotalProducts
            });
        }

        [AllowAnonymous]
        [HttpGet("{contractorId}/stats")]
        public IActionResult GetContractorStats(int contractorId)
        {
            try
            {
                var stats = _accountsService.GetContractorStats(contractorId);
                return Ok(new
                {
                    TotalRevenue = stats.TotalRevenue,
                    TotalRequests = stats.TotalRequests,
                    TotalSignedRequests = stats.TotalSignedRequests,
                    TotalOnGoingRequests = stats.TotalOnGoingRequests,
                    TotalRejectedRequests = stats.TotalRejectedRequests,
                    TotalConstructs = stats.TotalConstructs,
                    TotalProducts = stats.TotalProducts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
    }
}
