using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entity
{
    public class Contracts
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string? ContractUrl { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? EditDate { get; set; }
        public int? Status { get; set; }

        // Relationship
        public Appointments? Appointment { get; set; }
    }
}