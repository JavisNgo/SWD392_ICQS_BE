using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IAppointmentService
    {
        IEnumerable<AppointmentView> GetAllAppointment();
        IEnumerable<AppointmentView> GetAppointmentByContractorId(int contractorId);
        IEnumerable<AppointmentView> GetAppointmentByCustomerId(int customerId);
    }
}
