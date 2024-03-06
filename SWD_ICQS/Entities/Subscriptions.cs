using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Subscriptions
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Duration { get; set; }
        public bool? Status { get; set; }

        // Relationship
        public ICollection<Orders>? Orders { get; set; }
        public ICollection<Contractors>? Contractors { get; set; }
    }
}
