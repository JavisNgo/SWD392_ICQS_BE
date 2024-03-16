using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IConstructService
    {
        List<ConstructsView>? GetAllConstruct();
        List<Constructs>? GetConstructsByContractorId(int contractorId);
        List<ConstructsView>? GetConstructsViewsByContractorId(List<Constructs> constructsList);
        Constructs? GetConstructsById(int id);
        ConstructsView? GetConstructsViewByConstruct(int id, Constructs construct);
        Contractors? GetContractorById(int id);
        Categories? GetCategoryById(int id);
        bool IsCreateConstruct(ConstructsView constructsView);
        bool IsUpdateConstruct(ConstructsView constructsView, Constructs construct);
        bool IsChangedStatusConstruct(Constructs construct);
        bool IsDeleteConstruct(Constructs construct);
    }
}
