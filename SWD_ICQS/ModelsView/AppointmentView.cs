using static SWD_ICQS.Entities.Appointments;

namespace SWD_ICQS.ModelsView
{
    public class AppointmentView
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public string? CustomerName { get; set; }
        public string? ContractorName { get; set; }
        public int RequestId { get; set; }
        public DateTime? MeetingDate { get; set; }
        public AppointmentsStatusEnum? Status { get; set; }

    }
}
