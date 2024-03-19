using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Transactions;

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

        public bool UpdateStatus(int id)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.GetByID(id);
                if (depositOrder != null)
                {
                    if(depositOrder.TransactionCode != null)
                    {
                        depositOrder.Status = DepositOrders.DepositOrderStatusEnum.COMPLETED;
                        _unitOfWork.DepositOrdersRepository.Update(depositOrder);
                        _unitOfWork.Save();
                        return true;
                    }
                    
                    
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateTransactionCode(int id, string transactionCode)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.GetByID(id);
                if(depositOrder != null)
                {
                    var checkContract = _unitOfWork.ContractRepository.Find(c => c.RequestId == depositOrder.RequestId).FirstOrDefault();
                    if(checkContract == null) {
                        return false;
                    }
                    var checkRequest = _unitOfWork.RequestRepository.Find(r => r.Id == depositOrder.RequestId).FirstOrDefault();
                    if(checkRequest == null)
                    {
                        return false;
                    }
                    if(depositOrder.TransactionCode == null && checkContract.Status == 1 && checkRequest.Status == Requests.RequestsStatusEnum.SIGNED)
                    {
                        checkRequest.Status = Requests.RequestsStatusEnum.DEPOSITED;
                        _unitOfWork.RequestRepository.Update(checkRequest);
                        depositOrder.DepositDate = DateTime.Now;
                        depositOrder.TransactionCode = transactionCode;
                        depositOrder.Status = DepositOrders.DepositOrderStatusEnum.PROCESSING;
                        _unitOfWork.DepositOrdersRepository.Update(depositOrder);
                        _unitOfWork.Save();
                        return true;
                    }
                    
                }
                
                return false;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
