namespace SmartWeather.Models
{
    public class WeatherResponse
    {
        public MainInfo Main { get; set; }
        public List<WeatherInfo> Weather { get; set; }
        public string Name { get; set; }
    }

    public class MainInfo
    {
        public float Temp { get; set; }
        public float Humidity { get; set; }
    }

    public class WeatherInfo
    {
        public string Description { get; set; }
    }
}
