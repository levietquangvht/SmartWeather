using System.Text.Json.Serialization;

namespace SmartWeather.Models
{
    public class CityDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        // Bang / Tỉnh (rất hữu ích khi trùng tên thành phố)
        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}
