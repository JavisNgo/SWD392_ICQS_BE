﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entity
{
    public class RequestDetails
    {
        // Completed entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int ProductId { get; set; }

        // Relationship
        public Products? Product { get; set; }
        public Requests? Request { get; set; }
    }
}
