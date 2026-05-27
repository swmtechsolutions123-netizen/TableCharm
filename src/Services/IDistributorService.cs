using System.Collections.Generic;
using System.Threading.Tasks;
using TableCharm.DTOs;

namespace TableCharm.Services
{
    public interface IDistributorService
    {
        Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync();
        Task<DistributorDto> GetDistributorByIdAsync(int id);
        Task<DistributorHierarchyDto> GetDistributorHierarchyAsync(int id);
        Task<DistributorDto> CreateDistributorAsync(CreateDistributorDto createDto);
        Task<DistributorDto> UpdateDistributorAsync(int id, UpdateDistributorDto updateDto);
        Task<bool> DeleteDistributorAsync(int id);
        Task<IEnumerable<DistributorDto>> GetDirectDownlineAsync(int parentId);
        Task<IEnumerable<DistributorDto>> GetDistributorsByStateAsync(string state);
    }
}
