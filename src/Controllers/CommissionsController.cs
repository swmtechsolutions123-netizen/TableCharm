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
    public class CommissionsController : ControllerBase
    {
        private readonly ICommissionService _commissionService;
        private readonly ILogger<CommissionsController> _logger;

        public CommissionsController(ICommissionService commissionService, ILogger<CommissionsController> logger)
        {
            _commissionService = commissionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all commissions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommissionDto>>> GetAll()
        {
            try
            {
                var commissions = await _commissionService.GetAllCommissionsAsync();
                return Ok(commissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commissions: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving commissions" });
            }
        }

        /// <summary>
        /// Get commission by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommissionDto>> GetById(int id)
        {
            try
            {
                var commission = await _commissionService.GetCommissionByIdAsync(id);
                if (commission == null)
                    return NotFound(new { error = "Commission not found" });

                return Ok(commission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commission {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the commission" });
            }
        }

        /// <summary>
        /// Calculate commission for a distributor
        /// </summary>
        [HttpPost("calculate")]
        public async Task<ActionResult<CommissionCalculationResultDto>> Calculate([FromBody] CalculateCommissionDto calculateDto)
        {
            try
            {
                if (calculateDto == null || calculateDto.DistributorId <= 0)
                    return BadRequest(new { error = "Valid DistributorId is required" });

                if (calculateDto.StartDate > calculateDto.EndDate)
                    return BadRequest(new { error = "Start date must be before end date" });

                var result = await _commissionService.CalculateCommissionAsync(
                    calculateDto.DistributorId, 
                    calculateDto.StartDate, 
                    calculateDto.EndDate);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating commission: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while calculating the commission" });
            }
        }

        /// <summary>
        /// Get commissions by distributor ID
        /// </summary>
        [HttpGet("distributor/{distributorId}")]
        public async Task<ActionResult<IEnumerable<CommissionDto>>> GetByDistributorId(int distributorId)
        {
            try
            {
                var commissions = await _commissionService.GetCommissionsByDistributorIdAsync(distributorId);
                return Ok(commissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commissions for distributor {distributorId}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving commissions" });
            }
        }

        /// <summary>
        /// Get commissions by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<CommissionDto>>> GetByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest(new { error = "Status is required" });

                var commissions = await _commissionService.GetCommissionsByStatusAsync(status);
                return Ok(commissions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving commissions with status {status}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving commissions" });
            }
        }

        /// <summary>
        /// Get total commission for a distributor
        /// </summary>
        [HttpGet("total/{distributorId}")]
        public async Task<ActionResult<decimal>> GetTotal(int distributorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new { error = "Start date must be before end date" });

                var total = await _commissionService.GetTotalCommissionAsync(distributorId, startDate, endDate);
                return Ok(new { total });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving total commission for distributor {distributorId}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the total" });
            }
        }
    }
}
