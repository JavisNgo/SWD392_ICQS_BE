using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IContractService
    {
        IEnumerable<ContractViewForGet> GetAllContract();
        ContractViewForGet GetContractById(int contractId);
        IEnumerable<ContractViewForGet>? GetContractsByContractorId(int contractorId);
        IEnumerable<ContractViewForGet>? GetContractsByCustomerId(int customerId);
        ContractsView AddContract(ContractsView contractView);
        bool UploadContractProgress(int id, ContractsView contractView);
        ContractsView UpdateContract(int id, ContractsView contractsView);

    }
}
