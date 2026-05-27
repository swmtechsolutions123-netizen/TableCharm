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
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesService salesService, ILogger<SalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// Get all sales
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll()
        {
            try
            {
                var sales = await _salesService.GetAllSalesAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving sales" });
            }
        }

        /// <summary>
        /// Get sale by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetById(int id)
        {
            try
            {
                var sale = await _salesService.GetSaleByIdAsync(id);
                if (sale == null)
                    return NotFound(new { error = "Sale not found" });

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sale {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the sale" });
            }
        }

        /// <summary>
        /// Create a new sale
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SaleDto>> Create([FromBody] CreateSaleDto createDto)
        {
            try
            {
                if (createDto == null || createDto.DistributorId <= 0 || createDto.Amount <= 0)
                    return BadRequest(new { error = "Valid DistributorId and Amount are required" });

                var sale = await _salesService.CreateSaleAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = sale.SaleId }, sale);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating sale: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while creating the sale" });
            }
        }

        /// <summary>
        /// Update a sale
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<SaleDto>> Update(int id, [FromBody] UpdateSaleDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return BadRequest(new { error = "Update data is required" });

                var sale = await _salesService.UpdateSaleAsync(id, updateDto);
                return Ok(sale);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating sale {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while updating the sale" });
            }
        }

        /// <summary>
        /// Delete a sale
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _salesService.DeleteSaleAsync(id);
                if (!result)
                    return NotFound(new { error = "Sale not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting sale {id}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while deleting the sale" });
            }
        }

        /// <summary>
        /// Get sales by distributor ID
        /// </summary>
        [HttpGet("distributor/{distributorId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetByDistributorId(int distributorId)
        {
            try
            {
                var sales = await _salesService.GetSalesByDistributorIdAsync(distributorId);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales for distributor {distributorId}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving sales" });
            }
        }

        /// <summary>
        /// Get sales by date range
        /// </summary>
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new { error = "Start date must be before end date" });

                var sales = await _salesService.GetSalesByDateRangeAsync(startDate, endDate);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales for date range: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving sales" });
            }
        }

        /// <summary>
        /// Get sales summary for a distributor
        /// </summary>
        [HttpGet("summary/{distributorId}")]
        public async Task<ActionResult<SalesSummaryDto>> GetSummary(int distributorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new { error = "Start date must be before end date" });

                var summary = await _salesService.GetSalesSummaryAsync(distributorId, startDate, endDate);
                return Ok(summary);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving sales summary for distributor {distributorId}: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while retrieving the summary" });
            }
        }
    }
}
