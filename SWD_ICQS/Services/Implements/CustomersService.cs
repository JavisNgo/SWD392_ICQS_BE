using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Diagnostics.Contracts;

namespace SWD_ICQS.Services.Implements
{
    public class CustomersService : ICustomersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Accounts? GetAccountByCustomers(Customers customer)
        {
            try
            {
                var account = _unitOfWork.AccountRepository.Find(a => a.Id == customer.AccountId).FirstOrDefault();
                return account;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Accounts? GetAccountByUsername(string username)
        {
            try
            {
                var account = _unitOfWork.AccountRepository.Find(a => a.Username == username).FirstOrDefault();
                return account;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Customers>? GetAllCustomers()
        {
            try
            {
                var customers = _unitOfWork.CustomerRepository.Get().ToList();
                return customers;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Customers? GetCustomersByAccount(Accounts account)
        {
            try
            {
                var customer = _unitOfWork.CustomerRepository.Find(a => a.AccountId == account.Id).FirstOrDefault();
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Customers? GetCustomersById(int id)
        {
            try
            {
                var customer = _unitOfWork.CustomerRepository.GetByID(id);
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<CustomersView>? GetCustomersView(List<Customers> customers)
        {
            try
            {
                List<CustomersView> customersViews = new List<CustomersView>();
                foreach (Customers customer in customers)
                {
                    CustomersView customersView = _mapper.Map<CustomersView>(customer);
                    customersViews.Add(customersView);
                }
                return customersViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CustomersView? GetCustomersViewById(Customers customer)
        {
            try
            {
                var customersView = _mapper.Map<CustomersView>(customer); 
                return customersView;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsChangedStatusCustomerById(int id, Accounts account)
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsUpdateCustomer(string username, CustomersView customersView, Accounts account, Customers customer)
        {
            try
            {
                customer.Name = customersView.Name;
                customer.Email = customersView.Email;
                customer.PhoneNumber = customersView.PhoneNumber;
                customer.Address = customersView.Address;
                _unitOfWork.CustomerRepository.Update(customer);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
