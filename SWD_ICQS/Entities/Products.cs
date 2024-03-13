using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Products
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Code { get; set; }
        public int ContractorId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public bool? Status { get; set; }

        // Relationship
        public ICollection<ProductImages>? ProductImages { get; set; }
        public ICollection<ConstructProducts>? ConstructProducts { get; set; }
        public ICollection<RequestDetails>? RequestDetails { get; set; }
        public Contractors? Contractor { get; set; }
    }
}
