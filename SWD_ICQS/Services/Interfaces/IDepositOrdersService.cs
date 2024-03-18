using SWD_ICQS.Entities;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IDepositOrdersService
    {
        IEnumerable<DepositOrders>? GetDepositOrders();
        DepositOrders? GetDepositOrdersById(int id);
        DepositOrders? GetDepositOrdersByRequestId(int RequestsId);
        IEnumerable<DepositOrders>? GetDepositOrdersByCustomerId(int CustomerId);
        
    }
}
