using System.Collections.Generic;

namespace DesktopWeather.Model
{
    internal class Config
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<string>? Location { get; set; }
        public string? ConfigLocation { get; set; }
        public string? WeatherToken { get; set; }
        public string? AmapToken { get; set; }
        public string? EditorPath { get; set; }
        public bool LocationCorrection { get; set; }
    }
}
