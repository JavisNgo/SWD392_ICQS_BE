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

        private Accounts AuthenticateUser(AccountsView loginInfo)
        {
            Accounts _account = null;
            string hashedPassword = HashPassword(loginInfo.Password);
            Accounts? account = _unitOfWork.AccountRepository.Find(a => a.Username == loginInfo.Username && a.Password == hashedPassword).FirstOrDefault();
            if (account != null)
            {
                switch (account.Role)
                {
                    case 1:
                        _account = new Accounts();
                        _account.Id = account.Id;
                        _account.Username = account.Username;
                        _account.Status = account.Status;
                        _account.Role = account.Role;
                        break;
                    case 2:
                        _account = new Accounts();
                        Contractors? contractor = _unitOfWork.ContractorRepository.Find(c => c.Account.Id == account.Id).FirstOrDefault();
                        _account.Id = account.Id;
                        _account.Username = account.Username;
                        _account.Status = account.Status;
                        _account.Role = account.Role;
                        _account.Contractor = new Contractors();
                        _account.Contractor.Id = contractor.Id;
                        _account.Contractor.Name = contractor.Name;
                        break;
                    case 3:
                        _account = new Accounts();
                        Customers? customer = _unitOfWork.CustomerRepository.Find(c => c.Account.Id == account.Id).FirstOrDefault();
                        _account.Id = account.Id;
                        _account.Username = account.Username;
                        _account.Status = account.Status;
                        _account.Role = account.Role;
                        _account.Customer = new Customers();
                        _account.Customer.Id = customer.Id;
                        _account.Customer.Name = customer.Name;
                        break;
                }
            }
            return _account;
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

        private string GenerateToken(Accounts account)
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
                return BadRequest("No account found");
            }
            return response;
        }
    }

}
