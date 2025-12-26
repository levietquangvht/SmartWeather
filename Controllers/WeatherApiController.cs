using Microsoft.AspNetCore.Mvc;
using SmartWeather.Services;

namespace SmartWeather.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherApiController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherApiController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // =================================================
        // API CŨ – LẤY THỜI TIẾT THEO TÊN THÀNH PHỐ (GIỮ NGUYÊN)
        // GET: /api/weather?city=HaNoi
        // =================================================
        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("Vui lòng nhập tên thành phố.");

            var data = await _weatherService.GetWeatherAsync(city);

            if (data == null)
                return NotFound("Không tìm thấy thông tin thời tiết.");

            return Ok(data);
        }

        // =================================================
        // ⭐ API MỚI – AUTOCOMPLETE THÀNH PHỐ (GEOCODING)
        // GET: /api/weather/cities?q=ha
        // =================================================
        [HttpGet("cities")]
        public async Task<IActionResult> GetCities([FromQuery] string q)
        {
            var cities = await _weatherService.SearchCityAsync(q);
            return Ok(cities);
        }
    }
}
