using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableCharm.Models
{
    /// <summary>
    /// Represents a commission earned by a distributor
    /// </summary>
    [Table("Commissions")]
    public class Commission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommissionId { get; set; }

        [Required]
        public int DistributorId { get; set; }

        [ForeignKey(nameof(DistributorId))]
        public virtual Distributor Distributor { get; set; }

        /// <summary>
        /// 0 = Self, 1 = Direct Downline, 2 = Second-level Downline
        /// </summary>
        [Required]
        public int Level { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Earned"; // Earned, Paid, Pending

        public DateTime? DatePaid { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string Notes { get; set; }
    }
}
