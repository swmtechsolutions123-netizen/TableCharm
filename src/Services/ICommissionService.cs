using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableCharm.DTOs;

namespace TableCharm.Services
{
    public interface ICommissionService
    {
        Task<IEnumerable<CommissionDto>> GetAllCommissionsAsync();
        Task<CommissionDto> GetCommissionByIdAsync(int id);
        Task<CommissionCalculationResultDto> CalculateCommissionAsync(int distributorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<CommissionDto>> GetCommissionsByDistributorIdAsync(int distributorId);
        Task<IEnumerable<CommissionDto>> GetCommissionsByStatusAsync(string status);
        Task<decimal> GetTotalCommissionAsync(int distributorId, DateTime startDate, DateTime endDate);
    }
}
