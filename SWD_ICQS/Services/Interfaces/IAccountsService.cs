using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IAccountsService
    {
        AccountsView? AuthenticateUser(AccountsView loginInfo);
        string? HashPassword(string password);
        string? GenerateToken(AccountsView account);
        Accounts? GetAccountByUsername(string username);
        bool CreateAccount (AccountsView newAccount);
        string? GetAccountRole(string username, Accounts account);
        Contractors? GetContractorByAccount(Accounts account);
        ContractorsView? GetContractorInformation(string username, Accounts account, Contractors contractor);
        Customers? GetCustomerByUsername(Accounts account);
        CustomersView? GetCustomersInformation(string username, Accounts account, Customers customer);
    }
}
