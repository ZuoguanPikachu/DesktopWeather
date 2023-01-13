using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DesktopWeather.Util
{
    internal static class GetIP
    {
        public static string getIP(Requests requests)
        {
            string url = "https://2023.ip138.com";
            HttpContent response = requests.get(url);
            string ip = Regex.Match(response.ReadAsStringAsync().Result, @"<title>的IP地址是：(.*?)</title>").Groups[1].Value;

            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }

            url = "https://ip.900cha.com";
            response = requests.get(url);
            ip = Regex.Match(response.ReadAsStringAsync().Result, @"我的IP: ((\d+).(\d+).(\d+).(\d+))", RegexOptions.Multiline).Groups[1].Value;

            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }

            url = "https://ip.negui.com";
            response = requests.get(url);
            ip = Regex.Match(response.ReadAsStringAsync().Result, @"<td><span>((\d+).(\d+).(\d+).(\d+))</span></td>").Groups[1].Value;

            return ip;
        }
    }
}
