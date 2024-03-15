using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IRequestService
    {
        bool checkExistedRequestId(int id);
        RequestView GetRequestView(int id);
    }
}
