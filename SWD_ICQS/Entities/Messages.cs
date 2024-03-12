using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Messages
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public string? Content { get; set; }
        public DateTime? SendAt { get; set; }
        public bool? Status { get; set; }
        public string? ImageUrl { get; set; }

        // Relationship
        public Contractors? Contractor { get; set; }
        public Customers? Customer { get; set; }
    }
}
