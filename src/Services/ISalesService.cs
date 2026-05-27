using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableCharm.DTOs;

namespace TableCharm.Services
{
    public interface ISalesService
    {
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<SaleDto> GetSaleByIdAsync(int id);
        Task<SaleDto> CreateSaleAsync(CreateSaleDto createDto);
        Task<SaleDto> UpdateSaleAsync(int id, UpdateSaleDto updateDto);
        Task<bool> DeleteSaleAsync(int id);
        Task<IEnumerable<SaleDto>> GetSalesByDistributorIdAsync(int distributorId);
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<SalesSummaryDto> GetSalesSummaryAsync(int distributorId, DateTime startDate, DateTime endDate);
    }
}
