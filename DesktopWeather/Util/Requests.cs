using System.Net.Http;

namespace DesktopWeather.Util
{
    internal class Requests
    {
        private HttpClient client;

        public Requests()
        {
            HttpClientHandler handler = new HttpClientHandler();
            client = new HttpClient(handler);
        }

        public HttpContent get(string url)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");

            HttpResponseMessage response = client.GetAsync(url).Result;
            HttpContent content = response.Content;

            return content;
        }
    }
}
