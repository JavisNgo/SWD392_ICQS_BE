using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface ICategoriesService
    {
        IEnumerable<Categories> getAllCategories();

        CategoriesView GetCategoryById(int id);

        CategoriesView AddCategory(CategoriesView categoryView);

        CategoriesView UpdateCategory(int id, CategoriesView updatedCategoryView);

        Categories DeleteCategory(int id);

    }
}
