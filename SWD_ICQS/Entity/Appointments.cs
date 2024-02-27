using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entity
{
    public class Appointments
    {
        // Completed entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public int ContractId { get; set; }
        public DateTime MeetingDate { get; set; }
        public bool? Status { get; set; }

        // Relationship
        public Customers Customer { get; set; }
        public Contractors Contractor { get; set; }
        public Contracts Contract { get; set; }
    }
}