using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Text;

namespace SWD_ICQS.Services.Implements
{
    public class OrdersService : IOrdersService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public OrdersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public OrdersView AddOrder(OrdersView ordersView)
        {
            try
            {
                var checkContractorId = unitOfWork.ContractorRepository.GetByID(ordersView.ContractorId);
                var checkSubscriptionId = unitOfWork.SubscriptionRepository.GetByID(ordersView.SubscriptionId);

                if (checkContractorId == null || checkSubscriptionId == null)
                {
                    throw new Exception("ContractorId or SubscriptionId not found");
                }

                

                var order = new Orders
                {
                    SubscriptionId = ordersView.SubscriptionId,
                    ContractorId = ordersView.ContractorId,
                    OrderPrice = checkSubscriptionId.Price,
                    OrderDate = DateTime.Now,
                    Status = true,
                    TransactionCode = GenerateRandomCode(10)
                };

                unitOfWork.OrderRepository.Insert(order);
                unitOfWork.Save();

                checkContractorId.SubscriptionId = ordersView.SubscriptionId;
                checkContractorId.ExpiredDate = DateTime.Now.AddDays((double)checkSubscriptionId.Duration);
                unitOfWork.ContractorRepository.Update(checkContractorId);
                unitOfWork.Save();

                var result = _mapper.Map<OrdersView>(order);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the order. Error message: {ex.Message}");

            }
        }
        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        public IEnumerable<OrdersView> getOrders()
        {
            try
            {
                var orderList = unitOfWork.OrderRepository.Get();

                if (!orderList.Any())
                {
                    return null;
                }

                List<OrdersView> ordersViews = new List<OrdersView>();

                foreach (var item in orderList)
                {
                    ordersViews.Add(_mapper.Map<OrdersView>(item));
                }
                return ordersViews;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        public OrdersView GetOrderById(int id)
        {
            try
            {
                var order = unitOfWork.OrderRepository.GetByID(id);

                if (order == null)
                {
                    return null;
                }

                var orderView = _mapper.Map<OrdersView>(order);

                return orderView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the order. Error message: {ex.Message}");
            }
        }

        public IEnumerable<OrdersView> GetOrdersByContractorId(int contractorId)
        {
            try
            {
                var order = unitOfWork.OrderRepository.Find(o => o.ContractorId == contractorId).ToList();

                if (order == null)
                {
                    return null;
                }

                List<OrdersView> ordersViews = new List<OrdersView>();

                foreach (var item in order)
                {
                    ordersViews.Add(_mapper.Map<OrdersView>(item));
                }

                return ordersViews;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the order. Error message: {ex.Message}");
            }
        }
    }
}
