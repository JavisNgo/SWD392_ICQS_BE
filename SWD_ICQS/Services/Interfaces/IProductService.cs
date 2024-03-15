using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductsView> GetProducts();

        IEnumerable<ProductsView> getAllProductsByContractorId(int contractorid);

        ProductsView getProductByID(int id);

        ProductsView AddProduct(ProductsView productsView);

        ProductsView UpdateProduct(ProductsView productsView);

        Products ChangeStatusProduct(int id);
    }
}
