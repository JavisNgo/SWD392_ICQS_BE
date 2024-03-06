using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Customers
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        // Relationship
        public ICollection<Messages>? Messages { get; set; }
        public ICollection<Requests>? Requests { get; set; } 
        public ICollection<Appointments>? Appointments { get; set; }
        public Accounts? Account { get; set; }
    }
}
