using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface ICustomersService
    {
        List<Customers>? GetAllCustomers();
        List<CustomersView>? GetCustomersView(List<Customers> customers);
        Customers? GetCustomersById(int id);
        CustomersView? GetCustomersViewById(Customers customer);
        Customers? GetCustomersByAccount(Accounts account);
        Accounts? GetAccountByUsername(string username);
        Accounts? GetAccountByCustomers(Customers customer);
        bool IsUpdateCustomer(string username, CustomersView customersView, Accounts account, Customers customer);
        bool IsChangedStatusCustomerById(int id, Accounts account);
    }
}
