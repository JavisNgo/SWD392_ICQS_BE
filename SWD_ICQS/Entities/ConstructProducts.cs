using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class ConstructProducts
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ConstructId { get; set; }
        public int Quantity { get; set; }

        // Relationship
        public Products? Product { get; set; }
        public Constructs? Construct { get; set; }
    }
}