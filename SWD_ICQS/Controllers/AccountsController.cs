using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountsController(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private AccountsView AuthenticateUser(AccountsView loginInfo)
        {
            AccountsView accountsView = null;
            string hashedPassword = HashPassword(loginInfo.Password);
            Accounts? account = _unitOfWork.AccountRepository.Find(a => a.Username == loginInfo.Username && a.Password == hashedPassword).FirstOrDefault();
            if (account != null)
            {
                accountsView = new AccountsView();
                accountsView.Username = account.Username;
                accountsView.Status = account.Status;
                accountsView.Role = account.Role.ToString();
            }
            return accountsView;
        }

        private string HashPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                // Compute hash from the password bytes
                byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a hexadecimal string
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private string GenerateToken(AccountsView account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expirationTime = DateTime.UtcNow.AddMinutes(60);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, account.Role.ToString())
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: expirationTime, signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/login")]
        public IActionResult Login([FromBody] AccountsView loginInfo)
        {
            IActionResult response = Unauthorized();
            var account_ = AuthenticateUser(loginInfo);
            if (account_ != null)
            {
                if (account_.Status == true)
                {
                    var token = GenerateToken(account_);
                    response = Ok(new { accessToken = token, account_});
                }
                else
                {
                    return BadRequest("Your account is locked");
                }
            }
            else
            {
                return NotFound("No account found");
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/register")]
        public IActionResult Register([FromBody] AccountsView registerInfo)
        {
            var account_ = _unitOfWork.AccountRepository.Find(a => a.Username == registerInfo.Username).FirstOrDefault();
            if (account_ == null) {
                try
                {
                    registerInfo.Password = HashPassword(registerInfo.Password);
                    registerInfo.Status = true;
                    var account = _mapper.Map<Accounts>(registerInfo);
                    _unitOfWork.AccountRepository.Insert(account);
                    _unitOfWork.Save();
                    var insertedAccount = _unitOfWork.AccountRepository.Find(a => a.Username == registerInfo.Username).FirstOrDefault();
                    if (insertedAccount != null)
                    {
                        if (Enum.Parse(typeof(Accounts.AccountsRoleEnum), registerInfo.Role).Equals(Accounts.AccountsRoleEnum.CONTRACTOR))
                        {
                            var contractor = new Contractors
                            {
                                AccountId = insertedAccount.Id,
                                Name = registerInfo.Name,
                                Email = registerInfo.Email,
                                PhoneNumber = registerInfo.PhoneNumber,
                                Address = registerInfo.Address,
                                SubscriptionId = 1,
                                ExpiredDate = DateTime.ParseExact("20991231 23:59", "yyyyMMdd HH:mm", null)
                        };
                            _unitOfWork.ContractorRepository.Insert(contractor);
                            _unitOfWork.Save();
                        }
                        else if (Enum.Parse(typeof(Accounts.AccountsRoleEnum), registerInfo.Role).Equals(Accounts.AccountsRoleEnum.CUSTOMER))
                        {
                            var customer = new Customers
                            {
                                AccountId = insertedAccount.Id,
                                Name = registerInfo.Name,
                                Email = registerInfo.Email,
                                PhoneNumber = registerInfo.PhoneNumber,
                                Address = registerInfo.Address
                            };
                            _unitOfWork.CustomerRepository.Insert(customer);
                            _unitOfWork.Save();
                        }
                    }
                } catch (Exception ex)
                {
                    var insertedAccount = _unitOfWork.AccountRepository.Find(a => a.Username == registerInfo.Username).FirstOrDefault();
                    if(insertedAccount != null)
                    {
                        _unitOfWork.AccountRepository.Delete(insertedAccount);
                        _unitOfWork.Save();
                    }
                    return BadRequest(ex.Message);
                }
            } else
            {
                return BadRequest("Existed username");
            }
            return Ok("Create success");
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/accounts/get/username={username}")]
        public ActionResult GetAccountInfo(string username)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            if(account == null)
            {
                return NotFound("No account found in database");
            }
            if(account.Role == Accounts.AccountsRoleEnum.CONTRACTOR)
            {
                if (_unitOfWork.ContractorRepository.Get() == null)
                {
                    return NotFound("No contractor found in database");
                }
                var contractor = _unitOfWork.ContractorRepository.Find(c => c.AccountId == account.Id).FirstOrDefault();

                if (contractor == null)
                {
                    return NotFound($"No contractor contains username = {username} in database");
                }
                ContractorsView contractorsView = _mapper.Map<ContractorsView>(contractor);
                return Ok(contractorsView);
            } else if(account.Role == Accounts.AccountsRoleEnum.CUSTOMER)
            {
                if(_unitOfWork.CustomerRepository.Get() == null)
                {
                    return NotFound("No customer found in database");
                }
                var customer = _unitOfWork.CustomerRepository.Find(c => c.AccountId == account.Id).FirstOrDefault();
                if (customer == null)
                {
                    return NotFound($"No customer contains username = {username} in database");
                }
                CustomersView customersView = _mapper.Map<CustomersView>(customer);
                return Ok(customersView);
            } else if(account.Role == Accounts.AccountsRoleEnum.ADMIN)
            {
                return Ok("You are the administrator, no information to get");
            }
            return BadRequest("Something went wrong!");
        }
    }
}
