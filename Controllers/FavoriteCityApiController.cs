using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWeather.Services;
using System.Security.Claims;

namespace SmartWeather.Controllers
{
    [ApiController]
    [Route("api/favorites")]
    [Authorize] // 🔐 Bắt buộc có JWT
    public class FavoriteCityApiController : ControllerBase
    {
        private readonly UserService _userService;

        public FavoriteCityApiController(UserService userService)
        {
            _userService = userService;
        }

        // Lấy username từ JWT
        private string Username =>
            User.FindFirstValue(ClaimTypes.Name)!;

        // =========================
        // GET: /api/favorites
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var cities = await _userService.GetFavoriteCities(Username);
            return Ok(cities);
        }

        // =========================
        // ➕ POST: /api/favorites/add?city=HaNoi
        // =========================
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City không hợp lệ");

            await _userService.AddFavoriteCity(Username, city);
            return Ok($"Đã thêm {city}");
        }

        // =========================
        // ❌ DELETE: /api/favorites/remove?city=HaNoi
        // =========================
        [HttpDelete("remove")]
        public async Task<IActionResult> Remove([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City không hợp lệ");

            await _userService.RemoveFavoriteCity(Username, city);
            return Ok($"Đã xoá {city}");
        }
    }
}
