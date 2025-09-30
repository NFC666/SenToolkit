using System.Net;
using System.Net.Http;

namespace SenTooliKit.Repository.Factory
{
    public class HttpFactory
    {
        public static CookieContainer CookieContainer { get; } = new CookieContainer();

        private static HttpClientHandler HttpClientHandler { get; } = new HttpClientHandler()
        {
            CookieContainer = CookieContainer,
            UseCookies = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        private static HttpClient HttpClient { get; } = CreateNormalHttpClient();

        // 带 Bilibili Referer 的 HttpClient
        private static HttpClient HttpClientWithBiliReferer { get; } = CreateBiliClient();

        private static HttpClient CreateBiliClient()
        {
            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer,
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/58.0.3029.110 Safari/537.36");

            client.DefaultRequestHeaders.Add("Referer", "https://www.bilibili.com");

            return client;
        }

        public static HttpClient GetHttpClient()
        {
            return HttpClient;
        }
        public static HttpClient CreateNormalHttpClient()
        {
            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer,
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/58.0.3029.110 Safari/537.36");

            client.DefaultRequestHeaders.Add("Referer", "https://www.bilibili.com");

            return client;
        }

        public static HttpClient GetBiliHttpClient()
        {
            return HttpClientWithBiliReferer;
        }
    }
}