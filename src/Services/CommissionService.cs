using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TableCharm.Data;
using TableCharm.DTOs;
using TableCharm.Models;

namespace TableCharm.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly TableCharmDbContext _context;
        private readonly ILogger<CommissionService> _logger;

        public CommissionService(TableCharmDbContext context, ILogger<CommissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CommissionDto>> GetAllCommissionsAsync()
        {
            try
            {
                var commissions = await _context.Commissions
                    .Include(c => c.Distributor)
                    .ToListAsync();

                return commissions.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all commissions: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<CommissionDto> GetCommissionByIdAsync(int id)
        {
            try
            {
                var commission = await _context.Commissions
                    .Include(c => c.Distributor)
                    .FirstOrDefaultAsync(c => c.CommissionId == id);

                if (commission == null)
                {
                    _logger.LogWarning($"Commission with ID {id} not found");
                    return null;
                }

                return MapToDto(commission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commission {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<CommissionCalculationResultDto> CalculateCommissionAsync(int distributorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                decimal totalCommission = 0m;
                decimal level0Commission = 0m;
                decimal level1Commission = 0m;
                decimal level2Commission = 0m;

                // Get the main distributor (Level 0)
                var mainDistributor = await _context.Distributors
                    .Include(d => d.DirectDownline)
                        .ThenInclude(d => d.DirectDownline)
                    .FirstOrDefaultAsync(d => d.DistributorId == distributorId);

                if (mainDistributor == null)
                {
                    throw new ArgumentException($"Distributor with ID {distributorId} not found");
                }

                // Level 0: Calculate 5% commission on self sales
                var selfSales = await _context.Sales
                    .Where(s => s.DistributorId == distributorId && 
                                s.SaleDate >= startDate && 
                                s.SaleDate <= endDate)
                    .SumAsync(s => s.Amount);

                level0Commission = selfSales * 0.05m;
                totalCommission += level0Commission;

                // Level 1: Calculate 3% commission on direct downline's sales
                if (mainDistributor.DirectDownline != null && mainDistributor.DirectDownline.Any())
                {
                    var level1DistributorIds = mainDistributor.DirectDownline.Select(d => d.DistributorId).ToList();

                    var level1Sales = await _context.Sales
                        .Where(s => level1DistributorIds.Contains(s.DistributorId) && 
                                    s.SaleDate >= startDate && 
                                    s.SaleDate <= endDate)
                        .SumAsync(s => s.Amount);

                    level1Commission = level1Sales * 0.03m;
                    totalCommission += level1Commission;
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

                    level2Commission = level2Sales * 0.01m;
                    totalCommission += level2Commission;
                }

                _logger.LogInformation($"Commission calculated for distributor {distributorId}: Total={totalCommission}");

                return new CommissionCalculationResultDto
                {
                    DistributorId = distributorId,
                    DistributorName = mainDistributor.Name,
                    Level0Commission = level0Commission,
                    Level1Commission = level1Commission,
                    Level2Commission = level2Commission,
                    TotalCommission = totalCommission,
                    StartDate = startDate,
                    EndDate = endDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating commission for distributor {distributorId}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<CommissionDto>> GetCommissionsByDistributorIdAsync(int distributorId)
        {
            try
            {
                var commissions = await _context.Commissions
                    .Where(c => c.DistributorId == distributorId)
                    .Include(c => c.Distributor)
                    .ToListAsync();

                return commissions.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commissions for distributor {distributorId}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<CommissionDto>> GetCommissionsByStatusAsync(string status)
        {
            try
            {
                var commissions = await _context.Commissions
                    .Where(c => c.Status == status)
                    .Include(c => c.Distributor)
                    .ToListAsync();

                return commissions.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commissions with status {status}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<decimal> GetTotalCommissionAsync(int distributorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await CalculateCommissionAsync(distributorId, startDate, endDate);
                return result.TotalCommission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting total commission for distributor {distributorId}: {ex.Message}", ex);
                throw;
            }
        }

        private CommissionDto MapToDto(Commission commission)
        {
            var levelDescriptions = new Dictionary<int, string>
            {
                { 0, "Self" },
                { 1, "Direct Downline" },
                { 2, "Second-level Downline" }
            };

            return new CommissionDto
            {
                CommissionId = commission.CommissionId,
                DistributorId = commission.DistributorId,
                DistributorName = commission.Distributor?.Name,
                Level = commission.Level,
                LevelDescription = levelDescriptions.ContainsKey(commission.Level) ? levelDescriptions[commission.Level] : "Unknown",
                Amount = commission.Amount,
                StartDate = commission.StartDate,
                EndDate = commission.EndDate,
                Status = commission.Status,
                DatePaid = commission.DatePaid,
                DateCreated = commission.DateCreated,
                Notes = commission.Notes
            };
        }
    }
}
