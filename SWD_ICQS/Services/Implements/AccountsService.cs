using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace SWD_ICQS.Services.Implements
{
    public class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountsService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public AccountsView AuthenticateUser(AccountsView loginInfo)
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

        public string HashPassword(string password)
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

        public string GenerateToken(AccountsView account)
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

        public bool checkExistedAccount(string username)
        {
            var account_ = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            if (account_ != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateAccount(AccountsView newAccount)
        {
            try
            {
                newAccount.Password = HashPassword(newAccount.Password);
                newAccount.Status = true;
                var account = _mapper.Map<Accounts>(newAccount);
                _unitOfWork.AccountRepository.Insert(account);
                _unitOfWork.Save();
                var insertedAccount = _unitOfWork.AccountRepository.Find(a => a.Username == newAccount.Username).FirstOrDefault();
                if (insertedAccount != null)
                {
                    if (Enum.Parse(typeof(Accounts.AccountsRoleEnum), newAccount.Role).Equals(Accounts.AccountsRoleEnum.CONTRACTOR))
                    {
                        var contractor = new Contractors
                        {
                            AccountId = insertedAccount.Id,
                            Name = newAccount.Name,
                            Email = newAccount.Email,
                            PhoneNumber = newAccount.PhoneNumber,
                            Address = newAccount.Address,
                            SubscriptionId = 1,
                            ExpiredDate = DateTime.ParseExact("20991231 23:59", "yyyyMMdd HH:mm", null)
                        };
                        _unitOfWork.ContractorRepository.Insert(contractor);
                        _unitOfWork.Save();
                    }
                    else if (Enum.Parse(typeof(Accounts.AccountsRoleEnum), newAccount.Role).Equals(Accounts.AccountsRoleEnum.CUSTOMER))
                    {
                        var customer = new Customers
                        {
                            AccountId = insertedAccount.Id,
                            Name = newAccount.Name,
                            Email = newAccount.Email,
                            PhoneNumber = newAccount.PhoneNumber,
                            Address = newAccount.Address
                        };
                        _unitOfWork.CustomerRepository.Insert(customer);
                        _unitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                var insertedAccount = _unitOfWork.AccountRepository.Find(a => a.Username == newAccount.Username).FirstOrDefault();
                if (insertedAccount != null)
                {
                    _unitOfWork.AccountRepository.Delete(insertedAccount);
                    _unitOfWork.Save();
                }
                Console.WriteLine(ex.Message);
            }
        }

        public string GetAccountRole(string username)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            if (account.Role == Accounts.AccountsRoleEnum.CONTRACTOR)
            {
                return "CONTRACTOR";
            } else if (account.Role == Accounts.AccountsRoleEnum.CUSTOMER)
            {
                return "CUSTOMER";
            } else
            {
                return "ADMIN";
            }
        }

        public ContractorsView GetContractorInformation(string username)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            var contractor = _unitOfWork.ContractorRepository.Find(c => c.AccountId == account.Id).FirstOrDefault();
            ContractorsView contractorsView = _mapper.Map<ContractorsView>(contractor);
            string url = null;
            if (!String.IsNullOrEmpty(contractor.AvatarUrl))
            {
                url = $"https://localhost:7233/img/contractorAvatar/{contractor.AvatarUrl}";
            }
            contractorsView.AvatarUrl = url;
            return contractorsView;
        }

        public CustomersView GetCustomersInformation(string username)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            var customer = _unitOfWork.CustomerRepository.Find(c => c.AccountId == account.Id).FirstOrDefault();
            CustomersView customersView = _mapper.Map<CustomersView>(customer);
            return customersView;
        }

        public bool checkExistedContractor(string username)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            var contractor = _unitOfWork.ContractorRepository.Find(c => c.AccountId == account.Id).FirstOrDefault();
            if(contractor != null)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool checkExistedCustomer(string username)
        {
            var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
            var customer = _unitOfWork.CustomerRepository.Find(c => c.AccountId == account.Id).FirstOrDefault();
            if (customer != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
