using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class BlogImages
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string ImageUrl { get; set; }

        // Relationship
        public Blogs? Blog { get; set; }
    }
}
