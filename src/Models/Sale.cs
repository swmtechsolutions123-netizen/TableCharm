using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableCharm.Models
{
    /// <summary>
    /// Represents a sale made by a distributor
    /// </summary>
    [Table("Sales")]
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        [Required]
        public int DistributorId { get; set; }

        [ForeignKey(nameof(DistributorId))]
        public virtual Distributor Distributor { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string TransactionId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Completed"; // Completed, Pending, Cancelled
    }
}
