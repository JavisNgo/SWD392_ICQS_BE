using AutoMapper;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class RequestService : IRequestService
    {

        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public RequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool checkExistedRequestId(int id)
        {
            try
            {
                var request = unitOfWork.RequestRepository.GetByID(id);

                if (request != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public RequestView GetRequestView(int id)
        {
            var request = unitOfWork.RequestRepository.GetByID(id);
            var requestView = _mapper.Map<RequestView>(request);

            try
            {
                var requestDetail = unitOfWork.RequestDetailRepository.Find(r => r.RequestId == requestView.Id);
                if (requestDetail.Any())
                {
                    requestView.requestDetailViews = new List<RequestDetailView>();
                    foreach (var item in requestDetail)
                    {
                        requestView.requestDetailViews.Add(_mapper.Map<RequestDetailView>(item));
                    }

                    if (requestView.requestDetailViews.Any())
                    {
                        foreach (var item in requestView.requestDetailViews)
                        {
                            var product = unitOfWork.ProductRepository.GetByID(item.ProductId);
                            item.ProductView = _mapper.Map<ProductsView>(product);

                            var productImages = unitOfWork.ProductImageRepository.Find(p => p.ProductId == product.Id).ToList();
                            if (productImages.Any())
                            {
                                item.ProductView.productImagesViews = new List<ProductImagesView>();
                                foreach (var image in productImages)
                                {
                                    image.ImageUrl = $"https://localhost:7233/img/productImage/{image.ImageUrl}";
                                    item.ProductView.productImagesViews.Add(_mapper.Map<ProductImagesView>(image));
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return requestView;
        }
    }
}
