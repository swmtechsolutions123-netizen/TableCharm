using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TableCharm.Services
{
    /// <summary>
    /// Commission calculation service for multi-level distributor commissions
    /// Handles commission calculations for Level 0 (self), Level 1 (direct downline), 
    /// and Level 2 (second-level downline) distributors
    /// </summary>
    public class CommissionCalculationService
    {
        private readonly TableCharmDbContext _context;
        private readonly ILogger<CommissionCalculationService> _logger;

        public CommissionCalculationService(TableCharmDbContext context, ILogger<CommissionCalculationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Calculates commission for a distributor based on multi-level sales structure
        /// </summary>
        /// <param name="distributorId">The distributor ID to calculate commission for</param>
        /// <param name="startDate">Start date for sales aggregation</param>
        /// <param name="endDate">End date for sales aggregation</param>
        /// <returns>Total commission amount</returns>
        public async Task<decimal> CalculateCommissionAsync(int distributorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                decimal totalCommission = 0m;

                // Get the main distributor (Level 0)
                var mainDistributor = await _context.Distributors
                    .Include(d => d.DirectDownline)
                        .ThenInclude(d => d.DirectDownline)
                    .FirstOrDefaultAsync(d => d.DistributorId == distributorId);

                if (mainDistributor == null)
                {
                    _logger.LogWarning($"Distributor {distributorId} not found");
                    return 0m;
                }

                // Level 0: Calculate 5% commission on self sales
                var selfSales = await _context.Sales
                    .Where(s => s.DistributorId == distributorId && 
                                s.SaleDate >= startDate && 
                                s.SaleDate <= endDate)
                    .SumAsync(s => s.Amount);

                decimal level0Commission = selfSales * 0.05m;
                totalCommission += level0Commission;

                _logger.LogInformation($"Level 0 Commission for Distributor {distributorId}: {level0Commission}");

                // Level 1: Calculate 3% commission on direct downline's sales
                if (mainDistributor.DirectDownline != null && mainDistributor.DirectDownline.Any())
                {
                    var level1DistributorIds = mainDistributor.DirectDownline.Select(d => d.DistributorId).ToList();

                    var level1Sales = await _context.Sales
                        .Where(s => level1DistributorIds.Contains(s.DistributorId) && 
                                    s.SaleDate >= startDate && 
                                    s.SaleDate <= endDate)
                        .SumAsync(s => s.Amount);

                    decimal level1Commission = level1Sales * 0.03m;
                    totalCommission += level1Commission;

                    _logger.LogInformation($"Level 1 Commission for Distributor {distributorId}: {level1Commission}");
                }

                // Level 2: Calculate 1% commission on second-level downline's sales
                var level2Distributors = mainDistributor.DirectDownline?
                    .SelectMany(d => d.DirectDownline ?? new List<Distributor>())
                    .ToList();

                if (level2Distributors != null && level2Distributors.Any())
                {
                    var level2DistributorIds = level2Distributors.Select(d => d.DistributorId).ToList();

                    var level2Sales = await _context.Sales
                        .Where(s => level2DistributorIds.Contains(s.DistributorId) && 
                                    s.SaleDate >= startDate && 
                                    s.SaleDate <= endDate)
                        .SumAsync(s => s.Amount);

                    decimal level2Commission = level2Sales * 0.01m;
                    totalCommission += level2Commission;

                    _logger.LogInformation($"Level 2 Commission for Distributor {distributorId}: {level2Commission}");
                }

                _logger.LogInformation($"Total Commission for Distributor {distributorId}: {totalCommission}");
                return totalCommission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating commission for distributor {distributorId}: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Calculates commission for multiple distributors concurrently
        /// </summary>
        /// <param name="distributorIds">List of distributor IDs</param>
        /// <param name="startDate">Start date for sales aggregation</param>
        /// <param name="endDate">End date for sales aggregation</param>
        /// <returns>Dictionary of distributor ID to commission amount</returns>
        public async Task<Dictionary<int, decimal>> CalculateCommissionsAsync(List<int> distributorIds, DateTime startDate, DateTime endDate)
        {
            var commissions = new Dictionary<int, decimal>();
            var tasks = distributorIds.Select(id => CalculateCommissionAsync(id, startDate, endDate)).ToList();

            var results = await Task.WhenAll(tasks);

            for (int i = 0; i < distributorIds.Count; i++)
            {
                commissions[distributorIds[i]] = results[i];
            }

            return commissions;
        }
    }
}
