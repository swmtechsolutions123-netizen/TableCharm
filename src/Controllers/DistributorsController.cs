using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TableCharm.DTOs;
using TableCharm.Services;

namespace TableCharm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistributorsController : ControllerBase
    {
        private readonly IDistributorService _distributorService;
        private readonly ILogger<DistributorsController> _logger;

        public DistributorsController(IDistributorService distributorService, ILogger<DistributorsController> logger)
        {
            _distributorService = distributorService;
            _logger = logger;
        }

        /// <summary>
        /// Get all distributors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistributorDto>>> GetAll()
        {
            try
            {
                var distributors = await _distributorService.GetAllDistributorsAsync();
                return Ok(distributors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving distributors: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving distributors" });
            }
        }

        /// <summary>
        /// Get distributor by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DistributorDto>> GetById(int id)
        {
            try
            {
                var distributor = await _distributorService.GetDistributorByIdAsync(id);
                if (distributor == null)
                    return NotFound(new { error = "Distributor not found" });

                return Ok(distributor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving distributor {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the distributor" });
            }
        }

        /// <summary>
        /// Get distributor hierarchy
        /// </summary>
        [HttpGet("{id}/hierarchy")]
        public async Task<ActionResult<DistributorHierarchyDto>> GetHierarchy(int id)
        {
            try
            {
                var hierarchy = await _distributorService.GetDistributorHierarchyAsync(id);
                if (hierarchy == null)
                    return NotFound(new { error = "Distributor not found" });

                return Ok(hierarchy);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving hierarchy for distributor {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the hierarchy" });
            }
        }

        /// <summary>
        /// Create a new distributor
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DistributorDto>> Create([FromBody] CreateDistributorDto createDto)
        {
            try
            {
                if (createDto == null || string.IsNullOrWhiteSpace(createDto.Name) || string.IsNullOrWhiteSpace(createDto.Email))
                    return BadRequest(new { error = "Name and Email are required" });

                var distributor = await _distributorService.CreateDistributorAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = distributor.DistributorId }, distributor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating distributor: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while creating the distributor" });
            }
        }

        /// <summary>
        /// Update a distributor
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DistributorDto>> Update(int id, [FromBody] UpdateDistributorDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return BadRequest(new { error = "Update data is required" });

                var distributor = await _distributorService.UpdateDistributorAsync(id, updateDto);
                return Ok(distributor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating distributor {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while updating the distributor" });
            }
        }

        /// <summary>
        /// Delete a distributor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _distributorService.DeleteDistributorAsync(id);
                if (!result)
                    return NotFound(new { error = "Distributor not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting distributor {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while deleting the distributor" });
            }
        }

        /// <summary>
        /// Get direct downline for a distributor
        /// </summary>
        [HttpGet("{id}/downline")]
        public async Task<ActionResult<IEnumerable<DistributorDto>>> GetDirectDownline(int id)
        {
            try
            {
                var downline = await _distributorService.GetDirectDownlineAsync(id);
                return Ok(downline);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving downline for distributor {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the downline" });
            }
        }

        /// <summary>
        /// Get distributors by state
        /// </summary>
        [HttpGet("state/{state}")]
        public async Task<ActionResult<IEnumerable<DistributorDto>>> GetByState(string state)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(state))
                    return BadRequest(new { error = "State is required" });

                var distributors = await _distributorService.GetDistributorsByStateAsync(state);
                return Ok(distributors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving distributors for state {state}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving distributors" });
            }
        }
    }
}
