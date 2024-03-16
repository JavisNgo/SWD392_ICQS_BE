using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Diagnostics.Contracts;
using System.Security.Principal;

namespace SWD_ICQS.Services.Implements
{
    public class ContractorsService : IContractorsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public ContractorsService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "contractorAvatar");
        }

        public Accounts? GetAccountByUsername(string username)
        {
            try
            {
                var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
                return account;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Contractors>? GetAllContractor()
        {
            try
            {
                var contractors = _unitOfWork.ContractorRepository.Get().ToList();
                return contractors;
                
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Contractors? GetContractorByAccount(Accounts account)
        {
            try
            {
                var contractor = _unitOfWork.ContractorRepository.Find(a => a.AccountId == account.Id).FirstOrDefault();
                return contractor;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Contractors? GetContractorById(int id)
        {
            try
            {
                var contractor = _unitOfWork.ContractorRepository.GetByID(id);
                return contractor;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ContractorsView? GetContractorViewById(Contractors contractor)
        {
            try
            {
                string url = null;
                if (!String.IsNullOrEmpty(contractor.AvatarUrl))
                {
                    url = $"https://localhost:7233/img/contractorAvatar/{contractor.AvatarUrl}";
                }
                var contractorsView = new ContractorsView
                {
                    Id = contractor.Id,
                    Email = contractor.Email,
                    Name = contractor.Name,
                    PhoneNumber = contractor.PhoneNumber,
                    Address = contractor.Address,
                    AvatarUrl = url,
                    AccountId = contractor.AccountId
                };
                return contractorsView;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ContractorsView>? GetContractorsView(List<Contractors> contractors)
        {
            try
            {
                List<ContractorsView> contractorsViews = new List<ContractorsView>();
                foreach (Contractors contractor in contractors)
                {
                    ContractorsView contractorsView = _mapper.Map<ContractorsView>(contractor);
                    contractorsViews.Add(contractorsView);
                }
                return contractorsViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsUpdateContractor(string username, ContractorsView contractorsView, Accounts account, Contractors contractor)
        {
            try
            {
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
                if (contractorsView.AvatarUrl == null && !String.IsNullOrEmpty(tempString))
                {
                    string imagePath = Path.Combine(_imagesDirectory, tempString);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                return true;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Accounts? GetAccountByContractor(Contractors contractor)
        {
            try
            {
                var account = _unitOfWork.AccountRepository.Find(a => a.Id == contractor.AccountId).FirstOrDefault();
                return account;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsChangedStatusContractorById(int id, Accounts account)
        {
            try
            {
                if (account.Status == true)
                {
                    account.Status = false;
                }
                else if (account.Status == false)
                {
                    account.Status = true;
                }
                _unitOfWork.AccountRepository.Update(account);
                _unitOfWork.Save();
                return true;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
