using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IRequestService
    {
        bool checkExistedRequestId(int id);
        IEnumerable<Requests> GetAllRequests();

        RequestViewForGet GetRequestView(int id);
        RequestView AddRequest(RequestView requestView);
        bool IsOnGoingCustomerRequestExisted(int CustomerId, int ContractorId);
        IEnumerable<RequestViewForGet>? GetRequestsByContractorId(int contractorId);
        IEnumerable<RequestViewForGet>? GetRequestsByCustomerId(int customerId);
        RequestView UpdateRequest(int id, RequestView requestView);
        bool MarkMeetingAsCompleted(int id);
        bool AcceptRequest(int id);

    }
}
