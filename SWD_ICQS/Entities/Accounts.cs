using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Accounts
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? Status { get; set; }
        public AccountsRoleEnum? Role {  get; set; }
        public enum AccountsRoleEnum
        {
            ADMIN,
            CONTRACTOR,
            CUSTOMER
        }

        // Relationship
        public Contractors? Contractor { get; set; }
        public Customers? Customer { get; set; }
        public IEnumerable<Token>? Tokens { get; set; }
    }
}
