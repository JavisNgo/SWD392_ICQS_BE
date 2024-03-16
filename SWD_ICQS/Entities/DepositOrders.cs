﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entities
{
    public class DepositOrders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public double DepositPrice { get; set; }
        public DateTime? DepositDate { get; set; }
        public string? TransactionCode { get; set; }
        public DepositOrderStatusEnum? Status { get; set; }
        public enum DepositOrderStatusEnum
        {
            PENDING,
            COMPLETED,
            REJECTED
        }

        public Requests? Request { get; set; }
    }
}