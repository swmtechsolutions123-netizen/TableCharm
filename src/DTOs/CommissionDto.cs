using System;

namespace TableCharm.DTOs
{
    /// <summary>
    /// DTO for Commission read operations
    /// </summary>
    public class CommissionDto
    {
        public int CommissionId { get; set; }
        public int DistributorId { get; set; }
        public string DistributorName { get; set; }
        public int Level { get; set; }
        public string LevelDescription { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime DateCreated { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO for commission calculation request
    /// </summary>
    public class CalculateCommissionDto
    {
        public int DistributorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// DTO for commission calculation response
    /// </summary>
    public class CommissionCalculationResultDto
    {
        public int DistributorId { get; set; }
        public string DistributorName { get; set; }
        public decimal Level0Commission { get; set; }
        public decimal Level1Commission { get; set; }
        public decimal Level2Commission { get; set; }
        public decimal TotalCommission { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
