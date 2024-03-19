using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Requests
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public RequestsStatusEnum? Status { get; set; }
        public enum RequestsStatusEnum
        {
            PENDING,
            ACCEPTED,
            COMPLETED,
            SIGNED,
            DEPOSITED,
            REJECTED,

        }

        // Relationship
        public ICollection<RequestDetails>? RequestDetails { get; set; }
        public ICollection<Appointments>? Appointments { get; set; }
        public Customers? Customer { get; set; }
        public Contractors? Contractor { get; set; }
        public Contracts? Contract { get; set; }
        public DepositOrders? DepositOrder {  get; set; }
    }
}
