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
    public class SalesService : ISalesService
    {
        private readonly TableCharmDbContext _context;
        private readonly ILogger<SalesService> _logger;

        public SalesService(TableCharmDbContext context, ILogger<SalesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            try
            {
                var sales = await _context.Sales
                    .Include(s => s.Distributor)
                    .ToListAsync();

                return sales.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all sales: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<SaleDto> GetSaleByIdAsync(int id)
        {
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.Distributor)
                    .FirstOrDefaultAsync(s => s.SaleId == id);

                if (sale == null)
                {
                    _logger.LogWarning($"Sale with ID {id} not found");
                    return null;
                }

                return MapToDto(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sale {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto createDto)
        {
            try
            {
                // Validate distributor exists
                var distributorExists = await _context.Distributors
                    .AnyAsync(d => d.DistributorId == createDto.DistributorId);

                if (!distributorExists)
                {
                    throw new ArgumentException($"Distributor with ID {createDto.DistributorId} not found");
                }

                var sale = new Sale
                {
                    DistributorId = createDto.DistributorId,
                    Amount = createDto.Amount,
                    Description = createDto.Description,
                    SaleDate = createDto.SaleDate,
                    TransactionId = createDto.TransactionId,
                    DateCreated = DateTime.UtcNow,
                    Status = "Completed"
                };

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Reload with distributor info
                await _context.Entry(sale).Reference(s => s.Distributor).LoadAsync();

                _logger.LogInformation($"Sale {sale.SaleId} created successfully");

                return MapToDto(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating sale: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<SaleDto> UpdateSaleAsync(int id, UpdateSaleDto updateDto)
        {
            try
            {
                var sale = await _context.Sales.FindAsync(id);

                if (sale == null)
                {
                    throw new ArgumentException($"Sale with ID {id} not found");
                }

                sale.Amount = updateDto.Amount > 0 ? updateDto.Amount : sale.Amount;
                sale.Description = updateDto.Description ?? sale.Description;
                sale.Status = updateDto.Status ?? sale.Status;

                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();

                await _context.Entry(sale).Reference(s => s.Distributor).LoadAsync();

                _logger.LogInformation($"Sale {id} updated successfully");

                return MapToDto(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating sale {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteSaleAsync(int id)
        {
            try
            {
                var sale = await _context.Sales.FindAsync(id);

                if (sale == null)
                {
                    return false;
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Sale {id} deleted successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting sale {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDistributorIdAsync(int distributorId)
        {
            try
            {
                var sales = await _context.Sales
                    .Where(s => s.DistributorId == distributorId)
                    .Include(s => s.Distributor)
                    .ToListAsync();

                return sales.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales for distributor {distributorId}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var sales = await _context.Sales
                    .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                    .Include(s => s.Distributor)
                    .ToListAsync();

                return sales.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales for date range: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<SalesSummaryDto> GetSalesSummaryAsync(int distributorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var distributor = await _context.Distributors.FindAsync(distributorId);

                if (distributor == null)
                {
                    throw new ArgumentException($"Distributor with ID {distributorId} not found");
                }

                var sales = await _context.Sales
                    .Where(s => s.DistributorId == distributorId && s.SaleDate >= startDate && s.SaleDate <= endDate)
                    .ToListAsync();

                return new SalesSummaryDto
                {
                    DistributorId = distributorId,
                    DistributorName = distributor.Name,
                    Level = 0,
                    TotalSales = sales.Sum(s => s.Amount),
                    SaleCount = sales.Count,
                    StartDate = startDate,
                    EndDate = endDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales summary for distributor {distributorId}: {ex.Message}", ex);
                throw;
            }
        }

        private SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                SaleId = sale.SaleId,
                DistributorId = sale.DistributorId,
                DistributorName = sale.Distributor?.Name,
                Amount = sale.Amount,
                Description = sale.Description,
                SaleDate = sale.SaleDate,
                DateCreated = sale.DateCreated,
                TransactionId = sale.TransactionId,
                Status = sale.Status
            };
        }
    }
}
