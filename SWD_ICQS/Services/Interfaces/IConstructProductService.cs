using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IConstructProductService
    {
        IEnumerable<ConstructProducts> GetConstructProducts();
        ConstructProducts GetConstructProductByID(int id);

        ConstructProductsView AddConstructProduct(ConstructProductsView constructProductsView);

        ConstructProductsView UpdateConstructProduct(int id, ConstructProductsView constructProductsView);

        bool DeleteConstructProduct(int id);
    }
}
