using Microsoft.AspNetCore.Mvc;
using WeatherApp.Core.DTOs;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.IService;
using WeatherApp.Core.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(IAlertService alertService, ILogger<AlertsController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlertDto>> GetById(int id)
        {
            try
            {
                var alert = await _alertService.GetByIdAsync(id);
                return Ok(alert);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alert {AlertId}", id);
                return StatusCode(500, "An error occurred while retrieving the alert");
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAllActive()
        {
            try
            {
                var alerts = await _alertService.GetAllActiveAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active alerts");
                return StatusCode(500, "An error occurred while retrieving alerts");
            }
        }

        [HttpGet("city/{cityId}")]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetByCityId(int cityId)
        {
            try
            {
                var alerts = await _alertService.GetByCityIdAsync(cityId);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alerts for city {CityId}", cityId);
                return StatusCode(500, "An error occurred while retrieving alerts");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AlertDto>> Create([FromBody] CreateAlertDto dto)
        {
            try
            {
                var alert = await _alertService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = alert.Id }, alert);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert");
                return StatusCode(500, "An error occurred while creating the alert");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AlertDto>> Update(int id, [FromBody] UpdateAlertDto dto)
        {
            try
            {
                var alert = await _alertService.UpdateAsync(id, dto);
                return Ok(alert);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating alert {AlertId}", id);
                return StatusCode(500, "An error occurred while updating the alert");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _alertService.DeleteAsync(id);
                if (!result)
                    return NotFound($"Alert with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert {AlertId}", id);
                return StatusCode(500, "An error occurred while deleting the alert");
            }
        }
    }
}