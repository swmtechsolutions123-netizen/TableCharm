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
    public class DistributorService : IDistributorService
    {
        private readonly TableCharmDbContext _context;
        private readonly ILogger<DistributorService> _logger;

        public DistributorService(TableCharmDbContext context, ILogger<DistributorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync()
        {
            try
            {
                var distributors = await _context.Distributors
                    .Include(d => d.ParentDistributor)
                    .ToListAsync();

                return distributors.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all distributors: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<DistributorDto> GetDistributorByIdAsync(int id)
        {
            try
            {
                var distributor = await _context.Distributors
                    .Include(d => d.ParentDistributor)
                    .FirstOrDefaultAsync(d => d.DistributorId == id);

                if (distributor == null)
                {
                    _logger.LogWarning($"Distributor with ID {id} not found");
                    return null;
                }

                return MapToDto(distributor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving distributor {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<DistributorHierarchyDto> GetDistributorHierarchyAsync(int id)
        {
            try
            {
                var distributor = await _context.Distributors
                    .Include(d => d.DirectDownline)
                    .FirstOrDefaultAsync(d => d.DistributorId == id);

                if (distributor == null)
                {
                    return null;
                }

                return await MapToHierarchyDtoAsync(distributor, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving distributor hierarchy for {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<DistributorDto> CreateDistributorAsync(CreateDistributorDto createDto)
        {
            try
            {
                // Validate parent distributor if specified
                if (createDto.ParentDistributorId.HasValue)
                {
                    var parentExists = await _context.Distributors
                        .AnyAsync(d => d.DistributorId == createDto.ParentDistributorId);

                    if (!parentExists)
                    {
                        throw new ArgumentException($"Parent distributor with ID {createDto.ParentDistributorId} not found");
                    }
                }

                var distributor = new Distributor
                {
                    Name = createDto.Name,
                    Email = createDto.Email,
                    PhoneNumber = createDto.PhoneNumber,
                    Address = createDto.Address,
                    City = createDto.City,
                    State = createDto.State,
                    ZipCode = createDto.ZipCode,
                    ParentDistributorId = createDto.ParentDistributorId,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Distributors.Add(distributor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Distributor {distributor.DistributorId} created successfully");

                return MapToDto(distributor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating distributor: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<DistributorDto> UpdateDistributorAsync(int id, UpdateDistributorDto updateDto)
        {
            try
            {
                var distributor = await _context.Distributors.FindAsync(id);

                if (distributor == null)
                {
                    throw new ArgumentException($"Distributor with ID {id} not found");
                }

                distributor.Name = updateDto.Name ?? distributor.Name;
                distributor.PhoneNumber = updateDto.PhoneNumber ?? distributor.PhoneNumber;
                distributor.Address = updateDto.Address ?? distributor.Address;
                distributor.City = updateDto.City ?? distributor.City;
                distributor.State = updateDto.State ?? distributor.State;
                distributor.ZipCode = updateDto.ZipCode ?? distributor.ZipCode;
                distributor.IsActive = updateDto.IsActive;
                distributor.DateModified = DateTime.UtcNow;

                _context.Distributors.Update(distributor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Distributor {id} updated successfully");

                return MapToDto(distributor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating distributor {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteDistributorAsync(int id)
        {
            try
            {
                var distributor = await _context.Distributors.FindAsync(id);

                if (distributor == null)
                {
                    return false;
                }

                _context.Distributors.Remove(distributor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Distributor {id} deleted successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting distributor {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<DistributorDto>> GetDirectDownlineAsync(int parentId)
        {
            try
            {
                var downline = await _context.Distributors
                    .Where(d => d.ParentDistributorId == parentId)
                    .Include(d => d.ParentDistributor)
                    .ToListAsync();

                return downline.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving downline for distributor {parentId}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<DistributorDto>> GetDistributorsByStateAsync(string state)
        {
            try
            {
                var distributors = await _context.Distributors
                    .Where(d => d.State == state && d.IsActive)
                    .Include(d => d.ParentDistributor)
                    .ToListAsync();

                return distributors.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving distributors for state {state}: {ex.Message}", ex);
                throw;
            }
        }

        private DistributorDto MapToDto(Distributor distributor)
        {
            return new DistributorDto
            {
                DistributorId = distributor.DistributorId,
                Name = distributor.Name,
                Email = distributor.Email,
                PhoneNumber = distributor.PhoneNumber,
                Address = distributor.Address,
                City = distributor.City,
                State = distributor.State,
                ZipCode = distributor.ZipCode,
                ParentDistributorId = distributor.ParentDistributorId,
                ParentDistributorName = distributor.ParentDistributor?.Name,
                IsActive = distributor.IsActive,
                DateCreated = distributor.DateCreated,
                DateModified = distributor.DateModified
            };
        }

        private async Task<DistributorHierarchyDto> MapToHierarchyDtoAsync(Distributor distributor, int level)
        {
            var dto = new DistributorHierarchyDto
            {
                DistributorId = distributor.DistributorId,
                Name = distributor.Name,
                Email = distributor.Email,
                Level = level
            };

            if (distributor.DirectDownline != null && distributor.DirectDownline.Any())
            {
                foreach (var downline in distributor.DirectDownline)
                {
                    var childDto = await MapToHierarchyDtoAsync(downline, level + 1);
                    dto.DirectDownline.Add(childDto);
                }
            }

            return dto;
        }
    }
}
