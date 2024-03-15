using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IOrdersService
    {
        IEnumerable<OrdersView> getOrders();

        IEnumerable<OrdersView> GetOrdersByContractorId(int contractorId);

        OrdersView GetOrderById(int id);

        OrdersView AddOrder(OrdersView ordersView);


    }
}
