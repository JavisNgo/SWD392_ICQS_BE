using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IContractorsService
    {
        List<Contractors>? GetAllContractor();
        List<ContractorsView>? GetContractorsView(List<Contractors> contractors);
        Contractors? GetContractorById(int id);
        ContractorsView? GetContractorViewById(Contractors contractor);
        Contractors? GetContractorByAccount(Accounts account);
        Accounts? GetAccountByUsername(string username);
        Accounts? GetAccountByContractor(Contractors contractor);
        bool IsUpdateContractor(string username, ContractorsView contractorsView, Accounts account, Contractors contractor);   
        bool IsChangedStatusContractorById(int id, Accounts account);
    }
}
