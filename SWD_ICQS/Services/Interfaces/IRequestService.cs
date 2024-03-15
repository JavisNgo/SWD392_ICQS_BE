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

        IEnumerable<RequestViewForGet> GetRequestsByContractorId(int contractorId);
        IEnumerable<RequestViewForGet> GetRequestsByCustomerId(int customerId);
        Task<IActionResult> AddRequest(RequestView requestView);
        Task<IActionResult> UpdateRequest(int id, RequestView requestView);
        IActionResult AcceptRequest(int id);
        IActionResult MarkMeetingAsCompleted(int id);

    }
}
