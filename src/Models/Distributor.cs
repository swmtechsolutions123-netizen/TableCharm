using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableCharm.Models
{
    /// <summary>
    /// Represents a distributor in the direct selling network
    /// </summary>
    [Table("Distributors")]
    public class Distributor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistributorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        /// <summary>
        /// Parent distributor ID for hierarchical relationship (null for Level 0 - self)
        /// </summary>
        public int? ParentDistributorId { get; set; }

        [ForeignKey(nameof(ParentDistributorId))]
        public virtual Distributor ParentDistributor { get; set; }

        /// <summary>
        /// Direct downline distributors (Level 1)
        /// </summary>
        public virtual ICollection<Distributor> DirectDownline { get; set; } = new List<Distributor>();

        /// <summary>
        /// Sales made by this distributor
        /// </summary>
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

        /// <summary>
        /// Commission records for this distributor
        /// </summary>
        public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
