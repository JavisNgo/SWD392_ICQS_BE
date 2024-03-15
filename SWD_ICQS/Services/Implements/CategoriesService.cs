using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class CategoriesService : ICategoriesService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public CategoriesView AddCategory(CategoriesView categoryView)
        {
            try
            {
                

                var category = _mapper.Map<Categories>(categoryView);
                unitOfWork.CategoryRepository.Insert(category);
                unitOfWork.Save();
                return categoryView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the category. Error message: {ex.Message}");
            }
        }

        public Categories DeleteCategory(int id)
        {

            try
            {
                var category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                {
                    return null;
                }

                var existingConstruct = unitOfWork.ConstructRepository.Find(c => c.CategoryId == category.Id).ToList();
                if (existingConstruct.Any())
                {
                    throw new Exception("Please delete or change category of constructs that contain this category before deleting.");
                }
            

                unitOfWork.CategoryRepository.Delete(id);
                unitOfWork.Save();

                // You can return a custom response message 
                return category;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the category. Error message: {ex.Message}");
            }
        }
        

        public IEnumerable<Categories> getAllCategories()
        {
            try
            {
                var categoriesList = unitOfWork.CategoryRepository.Get();
                return categoriesList;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occur while get.");
            }
        }

        public CategoriesView GetCategoryById(int id)
        {
            try
            {
                var category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                {
                    return null;
                }

                var categoriesView = _mapper.Map<CategoriesView>(category);

                var constructLists = unitOfWork.ConstructRepository.Find(c => c.CategoryId == id).ToList();

                if (constructLists.Any())
                {
                    categoriesView.constructsViewList = new List<ConstructsView>();
                    foreach (var construct in constructLists)
                    {
                        categoriesView.constructsViewList.Add(_mapper.Map<ConstructsView>(construct));
                    }
                    foreach (var construct in categoriesView.constructsViewList)
                    {
                        construct.constructImagesViews = new List<ConstructImagesView>();
                        var imageLists = unitOfWork.ConstructImageRepository.Find(c => c.ConstructId == construct.Id).ToList();
                        foreach (var image in imageLists)
                        {
                            image.ImageUrl = $"https://localhost:7233/img/constructImage/{image.ImageUrl}";
                            construct.constructImagesViews.Add(_mapper.Map<ConstructImagesView>(image));
                        }
                    }
                }

                return categoriesView;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occur while get.");
            }
        }

        public CategoriesView UpdateCategory(int id, CategoriesView updatedCategoryView)
        {
            try
            {
                var existingCategory = unitOfWork.CategoryRepository.GetByID(id);

                if (existingCategory == null)
                {
                    return null;
                }

                

                // Map the properties from updatedCategoryView to existingCategory using AutoMapper
                _mapper.Map(updatedCategoryView, existingCategory);

                // Mark the entity as modified
                unitOfWork.CategoryRepository.Update(existingCategory);
                unitOfWork.Save();

                return updatedCategoryView; // Return the updated category
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the category. Error message: {ex.Message}");
            }
        }
    }
}
