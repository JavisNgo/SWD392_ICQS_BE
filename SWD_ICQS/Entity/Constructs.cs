﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_ICQS.Entity
{
    public class Constructs
    {
        // Completed entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ContractorId { get; set; }
        public int CategoryId { get; set; }
        public double EstimatedPrice { get; set; }
        public bool Status { get; set; }
        
        // Relationship
        public Contractors Contractor { get; set; }
        public Categories Category { get; set; }
        public ICollection<ConstructImages> ConstructImages { get; set; }
        public ICollection<ConstructProducts> ConstructProducts { get; set; }
    }
}