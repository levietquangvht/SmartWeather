using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SmartWeather.Models;
using System.Linq;

namespace SmartWeather.Services
{
    public class WeatherService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public WeatherService(IConfiguration configuration)
        {
            _http = new HttpClient();
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentException("API key OpenWeatherMap chưa được cấu hình trong appsettings.json");
            }
        }

        // =================================================
        // LẤY THỜI TIẾT 1 THÀNH PHỐ (GIỮ NGUYÊN)
        // =================================================
        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return null;

            try
            {
                string url =
                    $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric&lang=vi";

                var httpResponse = await _http.GetAsync(url);

                if (!httpResponse.IsSuccessStatusCode)
                    return null;

                var responseString = await httpResponse.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<WeatherResponse>(responseString, options);
            }
            catch
            {
                return null;
            }
        }

        // =================================================
        // ⭐ LẤY THỜI TIẾT CHO NHIỀU THÀNH PHỐ YÊU THÍCH
        // =================================================
        public async Task<List<WeatherResponse>> GetWeatherForCitiesAsync(List<string> cities)
        {
            var result = new List<WeatherResponse>();

            if (cities == null || cities.Count == 0)
                return result;

            foreach (var city in cities)
            {
                var weather = await GetWeatherAsync(city);
                if (weather != null)
                {
                    result.Add(weather);
                }
            }

            return result;
        }

        // =================================================
        // ⭐ AUTOCOMPLETE THÀNH PHỐ (GEOCODING API) - ĐÃ TỐI ƯU
        // =================================================
        public async Task<List<CityDto>> SearchCityAsync(string query)
        {
            var result = new List<CityDto>();

            if (string.IsNullOrWhiteSpace(query))
                return result;

            try
            {
                string url =
                    $"https://api.openweathermap.org/geo/1.0/direct?q={Uri.EscapeDataString(query)}&limit=5&appid={_apiKey}";

                var httpResponse = await _http.GetAsync(url);

                if (!httpResponse.IsSuccessStatusCode)
                    return result;

                var responseString = await httpResponse.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cities = JsonSerializer.Deserialize<List<CityDto>>(responseString, options);

                if (cities == null)
                    return result;

                // ⭐ TỐI ƯU UX:
                // 1. Ưu tiên tên bắt đầu bằng query
                // 2. Ưu tiên Việt Nam (VN)
                var sorted = cities
                    .OrderByDescending(c =>
                        c.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .ThenByDescending(c => c.Country == "VN")
                    .ToList();

                return sorted;
            }
            catch
            {
                return result;
            }
        }
    }
}
