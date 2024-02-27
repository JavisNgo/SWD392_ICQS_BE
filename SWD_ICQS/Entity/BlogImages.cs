using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entity
{
    public class BlogImages
    {
        // Completed entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public byte[]? ImageBin { get; set; }

        // Relationship
        public Blogs Blog { get; set; }
    }
}
