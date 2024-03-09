﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class Orders
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int ContractorId { get; set; }
        public double? OrderPrice { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool? Status { get; set; }
        public string? TransactionCode { get; set; }

        // Relationship
        public Subscriptions? Subscription { get; set; }
        public Contractors? Contractor { get; set; }
    }
}