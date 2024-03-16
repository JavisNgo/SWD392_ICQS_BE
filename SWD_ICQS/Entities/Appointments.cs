using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Appointments
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public int RequestId { get; set; }
        public DateTime? MeetingDate { get; set; }
        public AppointmentsStatusEnum? Status { get; set; }
        public enum AppointmentsStatusEnum
        {
            PENDING,
            COMPLETED,
            SIGNED,
            CANCELLED

        }

        // Relationship
        public Contractors? Contractor { get; set; }
        public Customers? Customer { get; set; }
        public Requests? Request { get; set; }
    }
}