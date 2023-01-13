using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DesktopWeather.Util
{
    internal static class DegreeConvert
    {
        static double degreeConvert(string raw)
        {
            MatchCollection parts = Regex.Matches(raw, @"\d+");
            double value = Convert.ToDouble(parts[0].Value) + Convert.ToDouble(parts[1].Value) /60 + Convert.ToDouble(parts[2].Value) /3600;
            
            return Math.Round(value, 6);
        }

        public static string convert(List<string> raw)
        {
            return $"{degreeConvert(raw[0])},{degreeConvert(raw[1])}";
        }

        public static string calcCenter(string raw)
        {
            string[] points = raw.Split(';');
            double x = (Convert.ToDouble(points[0].Split(',')[0]) + Convert.ToDouble(points[1].Split(',')[0])) / 2;
            double y = (Convert.ToDouble(points[0].Split(',')[1]) + Convert.ToDouble(points[1].Split(',')[1])) / 2;

            return $"{Math.Round(x, 6)},{Math.Round(y, 6)}";
        }
    }
}
