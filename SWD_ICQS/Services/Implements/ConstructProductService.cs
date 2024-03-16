using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class ConstructProductService : IConstructProductService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ConstructProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public ConstructProductsView AddConstructProduct(ConstructProductsView constructProductsView)
        {
            try
            {
                var checkingConstruct = unitOfWork.ConstructRepository.GetByID(constructProductsView.ConstructId);
                if (checkingConstruct == null)
                {
                    throw new Exception("Construct not found");
                }
                var checkingProduct = unitOfWork.ProductRepository.GetByID(constructProductsView.ProductId);
                if (checkingProduct == null)
                {
                    throw new Exception("Product not found");
                }

                var constructProduct = _mapper.Map<ConstructProducts>(constructProductsView);

                unitOfWork.ConstructProductRepository.Insert(constructProduct);
                unitOfWork.Save();
                return constructProductsView;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the constructProduct. Error message: {ex.Message}");
            }
        }

        public bool DeleteConstructProduct(int id)
        {
            try
            {
                var existingConstructProduct = unitOfWork.ConstructProductRepository.GetByID(id);
                if (existingConstructProduct == null)
                {
                    throw new Exception($"ConstructProduct with ID : {id} not found");
                }
                unitOfWork.ConstructProductRepository.Delete(id);
                unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the constructProduct. Error message: {ex.Message}");
            }
        }

        public IEnumerable<ConstructProducts> GetConstructProducts()
        {
            try
            {
                var constructProducts = unitOfWork.ConstructProductRepository.Get();
                return constructProducts;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        public ConstructProducts GetConstructProductByID(int id)
        {
            try
            {
                var constructProduct = unitOfWork.ConstructProductRepository.GetByID(id);
                if (constructProduct == null)
                {
                    return null;
                }
                return constructProduct;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the constructProduct. Error message: {ex.Message}");
            }
        }

        public ConstructProductsView UpdateConstructProduct(int id, ConstructProductsView constructProductsView)
        {
            try
            {
                var existingConstructProduct = unitOfWork.ConstructProductRepository.GetByID(id);
                if (existingConstructProduct == null)
                {
                    throw new Exception($"ConstructProduct with ID : {id} not found");
                }
                var checkingConstruct = unitOfWork.ConstructRepository.GetByID(constructProductsView.ConstructId);
                if (checkingConstruct == null)
                {
                    throw new Exception("Construct not found");
                }
                var checkingProduct = unitOfWork.ProductRepository.GetByID(constructProductsView.ProductId);
                if (checkingProduct == null)
                {
                    throw new Exception("Product not found");
                }
                _mapper.Map(constructProductsView, existingConstructProduct);
                unitOfWork.ConstructProductRepository.Update(existingConstructProduct);
                unitOfWork.Save();
                return constructProductsView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the constructProduct. Error message: {ex.Message}");
            }
        }
    }
}
