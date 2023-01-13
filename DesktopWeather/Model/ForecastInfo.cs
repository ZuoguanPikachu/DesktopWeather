namespace DesktopWeather.Model
{
    internal class DailyForecastInfo
    {
        public string? MaxTemperature { get; set; }
        public string? MinTemperature { get; set; }
        public string? WeatherIcon { get; set; }
        public string? Date { get; set; }
    }

    internal class HourlyForecastInfo
    {
        public string? Temperature { get; set; }
        public string? WeatherIcon { get; set; }
        public string? Time { get; set; }
    }
}
