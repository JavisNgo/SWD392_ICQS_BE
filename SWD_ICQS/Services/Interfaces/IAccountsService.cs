using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IAccountsService
    {
        AccountsView AuthenticateUser(AccountsView loginInfo);
        string HashPassword(string password);
        string GenerateToken(AccountsView account);
        bool checkExistedAccount(string username);
        void CreateAccount (AccountsView newAccount);
        string GetAccountRole(string username);
        bool checkExistedContractor(string username);
        ContractorsView GetContractorInformation(string username);
        bool checkExistedCustomer(string username);
        CustomersView GetCustomersInformation(string username);
    }
}
