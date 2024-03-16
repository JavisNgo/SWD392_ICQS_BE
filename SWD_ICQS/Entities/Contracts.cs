using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Contracts
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string? ContractUrl { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? EditDate { get; set; }
        public int? Status { get; set; }
        public string? Progress { get; set; }

        // Relationship
        public Requests? Request { get; set; }
    }
}