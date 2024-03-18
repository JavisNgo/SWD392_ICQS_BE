using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class RequestDetailService : IRequestDetailService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public RequestDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public RequestDetailView AddRequestDetail(RequestDetailView requestDetailView)
        {
            var checkingProductID = unitOfWork.ProductRepository.GetByID(requestDetailView.ProductId);
            var checkingRequestId = unitOfWork.RequestRepository.GetByID(requestDetailView.RequestId);
            if (checkingProductID == null || checkingRequestId == null)
            {
                throw new Exception("ProductID or RequestID not found");
            }
            if(requestDetailView.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than 0");
            }
            RequestDetails requestDetail = _mapper.Map<RequestDetails>(requestDetailView);
            unitOfWork.RequestDetailRepository.Insert(requestDetail);
            unitOfWork.Save();

            return requestDetailView;
        }


        public RequestDetailView GetRequestById(int id)
        {
            var requestDetail = unitOfWork.RequestDetailRepository.GetByID(id);

            if (requestDetail == null)
            {
                throw new Exception($"RequestDetail with ID {id} not found.");
            }
            var requestDetailView = new RequestDetailView
            {
              ProductId = requestDetail.ProductId,
              RequestId = requestDetail.RequestId,
              Quantity = requestDetail.Quantity,
            };
            return requestDetailView;
        }

        public IEnumerable<RequestDetails> GetRequestDetails()
        {
            var requestDetailList = unitOfWork.RequestDetailRepository.Get().ToList();
            return requestDetailList;
        }


        public bool UpdateRequestDetail(int id, RequestDetailView requestDetailView)
        {
            var existingRequestDetail = unitOfWork.RequestDetailRepository.GetByID(id);

            if (existingRequestDetail == null)
            {
                throw new Exception($"RequestDetail with ID {id} not found.");
            }

            var checkingProductID = unitOfWork.ProductRepository.GetByID(requestDetailView.ProductId);
            var checkingRequestId = unitOfWork.RequestRepository.GetByID(requestDetailView.RequestId);
            if (checkingProductID == null || checkingRequestId == null)
            {
                throw new Exception("ProductID or RequestID not found");
            }

            if (requestDetailView.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than 0");
            }

            _mapper.Map(requestDetailView, existingRequestDetail);

            unitOfWork.RequestDetailRepository.Update(existingRequestDetail);
            unitOfWork.Save();

            return true;
        }



        public bool DeleteRequestDetail(int id)
        {
            var requestDetail = unitOfWork.RequestDetailRepository.GetByID(id);

            if (requestDetail == null)
            {
                throw new Exception($"Request Detail with ID {id} not found.");
            }

            unitOfWork.RequestDetailRepository.Delete(id);
            unitOfWork.Save();

            // Kiểm tra xem requestDetail còn tồn tại trong database sau khi xóa
            var isDeleted = unitOfWork.RequestDetailRepository.GetByID(id) == null;

            return isDeleted;
        }

    }
}
