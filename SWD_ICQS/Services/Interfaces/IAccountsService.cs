using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IAccountsService
    {
        AccountsView? AuthenticateUser(AccountsView loginInfo);
        string? HashPassword(string password);
        (string accessToken, string refreshToken) GenerateTokens(AccountsView account);
        public string? GenerateToken(Accounts account);
        Token? GetRefreshTokenByAccountId(int AccountId);
        (bool isValid, string username) ValidateRefreshToken(string refreshToken);
        Accounts GetAccountById(int AccountId);
        bool IsExistedEmail (string email);
        Accounts? GetAccountByUsername(string username);
        bool CreateAccountCustomer (AccountsView newAccount);
        bool CreateAccountContractor(AccountsView newAccount);
        string? GetAccountRole(string username, Accounts account);
        Contractors? GetContractorByAccount(Accounts account);
        ContractorsView? GetContractorInformation(string username, Accounts account, Contractors contractor);
        Customers? GetCustomerByUsername(Accounts account);
        CustomersView? GetCustomersInformation(string username, Accounts account, Customers customer);
        (double? TotalRevenue, double? TotalDepositRevenue, int TotalCustomers, int TotalFilterdCustomer, int TotalContractors, int TotalRequests, int TotalSignedRequests, int TotalOnGoingRequests, int TotalRejectedRequests, int TotalConstructs, int TotalProducts) GetPlatformStats();
        (double? TotalRevenue, int TotalRequests, int TotalSignedRequests, int TotalOnGoingRequests, int TotalRejectedRequests, int TotalConstructs, int TotalProducts) GetContractorStats(int contractorId);
    }
}
