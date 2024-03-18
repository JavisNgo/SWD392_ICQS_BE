using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IDepositOrdersService
    {
        IEnumerable<DepositOrders>? GetDepositOrders();
        DepositOrders? GetDepositOrdersById(int id);
        DepositOrders? GetDepositOrdersByRequestId(int RequestsId);
        IEnumerable<DepositOrders>? GetDepositOrdersByCustomerId(int CustomerId);

        bool UpdateTransactionCode(int id, string transactionCode);

        bool UpdateStatus(int id);
        
    }
}
