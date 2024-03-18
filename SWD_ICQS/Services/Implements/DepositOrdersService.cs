using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class DepositOrdersService : IDepositOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepositOrdersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<DepositOrders>? GetDepositOrders()
        {
            try
            {
                var DepositOrders = _unitOfWork.DepositOrdersRepository.Get();

                return DepositOrders;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<DepositOrders>? GetDepositOrdersByCustomerId(int CustomerId)
        {
            try
            {
                List<DepositOrders> depositOrders = new List<DepositOrders>();
                var Requests = _unitOfWork.RequestRepository.Find(r => r.CustomerId == CustomerId).ToList();
                if (Requests != null)
                {
                    if (Requests.Any())
                    {
                        foreach(var Request in Requests)
                        {
                            var depositOrder = _unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == Request.Id).FirstOrDefault();
                            if(depositOrder != null)
                            {
                                depositOrders.Add(depositOrder);
                            }
                        }
                    }
                }
                return depositOrders;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DepositOrders? GetDepositOrdersById(int id)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.GetByID(id);

                return depositOrder;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DepositOrders? GetDepositOrdersByRequestId(int RequestsId)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == RequestsId).FirstOrDefault();
                return depositOrder;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
