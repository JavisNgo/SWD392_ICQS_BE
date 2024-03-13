using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Blogs
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Code { get; set; }
        public int ContractorId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? PostTime { get; set; }
        public DateTime? EditTime { get; set; }
        public bool? Status { get; set; }

        // Relationship
        public Contractors? Contractor { get; set; }
        public ICollection<BlogImages>? BlogImages { get; set; }
    }
}
