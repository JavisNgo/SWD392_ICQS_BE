using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entity
{
    public class Contractors
    {
        // Completed entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int SubcriptionId { get; set; }
        public DateTime ExpiredDate { get; set; }

        // Relationship
        public Accounts Account { get; set; }
        public Subscriptions Subscription { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public ICollection<Blogs> Blogs { get; set; }
        public ICollection<Messages> Messages { get; set; }
        public ICollection<Products> Products { get; set; }
        public ICollection<Constructs> Constructs { get; set; }
        public ICollection<Requests> Requests { get; set; }
        public ICollection<Appointments> Appointments { get; set; }
    }
}
