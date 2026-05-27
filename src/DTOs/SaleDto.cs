using System;

namespace TableCharm.DTOs
{
    /// <summary>
    /// DTO for Sale read operations
    /// </summary>
    public class SaleDto
    {
        public int SaleId { get; set; }
        public int DistributorId { get; set; }
        public string DistributorName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DTO for creating a new sale
    /// </summary>
    public class CreateSaleDto
    {
        public int DistributorId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime SaleDate { get; set; }
        public string TransactionId { get; set; }
    }

    /// <summary>
    /// DTO for updating a sale
    /// </summary>
    public class UpdateSaleDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DTO for sales summary by distributor
    /// </summary>
    public class SalesSummaryDto
    {
        public int DistributorId { get; set; }
        public string DistributorName { get; set; }
        public int Level { get; set; }
        public decimal TotalSales { get; set; }
        public int SaleCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
