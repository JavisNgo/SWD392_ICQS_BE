using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IRequestDetailService
    {
        IEnumerable<RequestDetails> GetRequestDetails();
        RequestDetailView GetRequestById(int id);

        RequestDetailView AddRequestDetail(RequestDetailView requestDetailView);

        bool UpdateRequestDetail(int id, RequestDetailView requestView);
        bool DeleteRequestDetail(int id );

    }
}
