using Microsoft.AspNetCore.Mvc;
using WeatherApp.Core.Dtos;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ILogger<CitiesController> _logger;

        public CitiesController(ICityService cityService, ILogger<CitiesController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAll()
        {
            try
            {
                var cities = await _cityService.GetAllAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all cities");
                return StatusCode(500, "An error occurred while retrieving cities");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetById(int id)
        {
            try
            {
                var city = await _cityService.GetByIdAsync(id);
                return Ok(city);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving city {CityId}", id);
                return StatusCode(500, "An error occurred while retrieving the city");
            }
        }

        [HttpGet("{id}/weather")]
        public async Task<ActionResult<CityDto>> GetWithWeather(int id)
        {
            try
            {
                var city = await _cityService.GetCityWithWeatherAsync(id);
                return Ok(city);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving city {CityId} with weather", id);
                return StatusCode(500, "An error occurred while retrieving the city");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CityDto>> Create([FromBody] CreateCityDto dto)
        {
            try
            {
                var city = await _cityService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = city.Id }, city);
            }
            catch (DuplicateEntityException ex)
            {
                return Conflict(ex.Message);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating city");
                return StatusCode(500, "An error occurred while creating the city");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CityDto>> Update(int id, [FromBody] UpdateCityDto dto)
        {
            try
            {
                var city = await _cityService.UpdateAsync(id, dto);
                return Ok(city);
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
                _logger.LogError(ex, "Error updating city {CityId}", id);
                return StatusCode(500, "An error occurred while updating the city");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _cityService.DeleteAsync(id);
                if (!result)
                    return NotFound($"City with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting city {CityId}", id);
                return StatusCode(500, "An error occurred while deleting the city");
            }
        }
    }
}