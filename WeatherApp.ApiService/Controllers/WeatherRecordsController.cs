using Microsoft.AspNetCore.Mvc;
using WeatherApp.Core.Dtos;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherRecordsController : ControllerBase
    {
        private readonly IWeatherRecordService _weatherRecordService;
        private readonly ILogger<WeatherRecordsController> _logger;

        public WeatherRecordsController(
            IWeatherRecordService weatherRecordService,
            ILogger<WeatherRecordsController> logger)
        {
            _weatherRecordService = weatherRecordService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherRecordDto>> GetById(int id)
        {
            try
            {
                var record = await _weatherRecordService.GetByIdAsync(id);
                return Ok(record);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather record {RecordId}", id);
                return StatusCode(500, "An error occurred while retrieving the weather record");
            }
        }

        [HttpGet("city/{cityId}")]
        public async Task<ActionResult<IEnumerable<WeatherRecordDto>>> GetByCityId(int cityId)
        {
            try
            {
                var records = await _weatherRecordService.GetByCityIdAsync(cityId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather records for city {CityId}", cityId);
                return StatusCode(500, "An error occurred while retrieving weather records");
            }
        }

        [HttpGet("city/{cityId}/latest")]
        public async Task<ActionResult<WeatherRecordDto>> GetLatestByCityId(int cityId)
        {
            try
            {
                var record = await _weatherRecordService.GetLatestByCityIdAsync(cityId);
                if (record == null)
                    return NotFound($"No weather records found for city {cityId}");

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest weather record for city {CityId}", cityId);
                return StatusCode(500, "An error occurred while retrieving the weather record");
            }
        }

        [HttpPost]
        public async Task<ActionResult<WeatherRecordDto>> Create([FromBody] CreateWeatherRecordDto dto)
        {
            try
            {
                var record = await _weatherRecordService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
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
                _logger.LogError(ex, "Error creating weather record");
                return StatusCode(500, "An error occurred while creating the weather record");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _weatherRecordService.DeleteAsync(id);
                if (!result)
                    return NotFound($"Weather record with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting weather record {RecordId}", id);
                return StatusCode(500, "An error occurred while deleting the weather record");
            }
        }
    }
}