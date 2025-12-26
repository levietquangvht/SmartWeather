using Microsoft.AspNetCore.Mvc;
using SmartWeather.Services;
using SmartWeather.Models;
using System.Collections.Generic;

namespace SmartWeather.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;
        private readonly UserService _userService;

        public WeatherController(WeatherService weatherService, UserService userService)
        {
            _weatherService = weatherService;
            _userService = userService;
        }

        // =========================
        // GET: /Weather/Index
        // =========================
        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("User");
            ViewBag.Username = username;

            if (username != null)
            {
                var favoriteCities = await _userService.GetFavoriteCities(username);
                var favoriteWeathers =
                    await _weatherService.GetWeatherForCitiesAsync(favoriteCities);

                ViewBag.FavoriteWeathers = favoriteWeathers;
            }

            return View("Index");
        }

        // =========================
        // POST: /Weather/GetWeather
        // =========================
        [HttpPost]
        public async Task<IActionResult> GetWeather(string city)
        {
            var username = HttpContext.Session.GetString("User");
            ViewBag.Username = username;
            ViewBag.City = city;

            // Load favorite trước để dùng cho mọi case
            if (username != null)
            {
                var favoriteCities = await _userService.GetFavoriteCities(username);
                ViewBag.FavoriteWeathers =
                    await _weatherService.GetWeatherForCitiesAsync(favoriteCities);
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Vui lòng nhập tên thành phố.";
                return View("Index");
            }

            WeatherResponse? data;
            try
            {
                data = await _weatherService.GetWeatherAsync(city);
            }
            catch
            {
                ViewBag.Error = "Có lỗi xảy ra khi gọi API thời tiết.";
                return View("Index");
            }

            if (data == null)
            {
                ViewBag.Error =
                    "❌ Không tìm thấy dữ liệu thời tiết. Vui lòng chọn lại từ danh sách gợi ý.";
                return View("Index");
            }

            // Thành công
            return View("Index", data);
        }

        // =========================
        // POST: /Weather/AddFavorite
        // =========================
        [HttpPost]
        public async Task<IActionResult> AddFavorite(string city)
        {
            var username = HttpContext.Session.GetString("User");
            if (username == null)
                return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(city))
            {
                await _userService.AddFavoriteCity(username, city);
            }

            return RedirectToAction("Index");
        }

        // =========================
        // POST: /Weather/RemoveFavorite
        // =========================
        [HttpPost]
        public async Task<IActionResult> RemoveFavorite(string city)
        {
            var username = HttpContext.Session.GetString("User");
            if (username == null)
                return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(city))
            {
                await _userService.RemoveFavoriteCity(username, city);
            }

            return RedirectToAction("Index");
        }
    }
}
