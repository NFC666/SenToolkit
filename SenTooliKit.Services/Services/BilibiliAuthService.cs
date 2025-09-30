using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text.Json;

using Newtonsoft.Json.Linq;

using QRCoder;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Services.Services
{
    public class BilibiliAuthService : IBilibiliAuthService
    {
        private static string QrCodePath = PathHelper.GetQrCOdePath();
        private static string CookieSavePath = PathHelper.GetCookieSavePath();
        private static readonly string BasePassportUrl = "https://passport.bilibili.com/x/passport-login/web/qrcode";
        private string _qrCodeKey;
        private readonly HttpClient _httpClient = HttpFactory.GetHttpClient();


        public async Task<string> GetQrCodePathAsync()
        {
            await GetQrCodeAsync();
            return QrCodePath;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Login()
        {
            if (!await SetCookie())
            {
                bool res = await LoginByQrCodeAsync();
                return res;
            }
            return true;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public Task<bool> Logout()
        {

            if (File.Exists(CookieSavePath))
            {
                File.Delete(CookieSavePath);
            }
            return Task.FromResult(true);
        }


        private async Task GetQrCodeAsync()
        {
            string url = $"{BasePassportUrl}/generate";
            string response = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
            string qrUrl = JsonValueHelper.GetJsonValue<string>(response, "url");
            _qrCodeKey = JsonValueHelper.GetJsonValue<string>(response, "qrcode_key");

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);

            using Bitmap qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(QrCodePath, ImageFormat.Png);
        }


        private async Task<bool> LoginByQrCodeAsync(CancellationToken cancellationToken = default)
        {
            var timeout = TimeSpan.FromMinutes(3);
            var startTime = DateTime.UtcNow;

            while (!cancellationToken.IsCancellationRequested &&
                   DateTime.UtcNow - startTime < timeout)
            {
                await Task.Delay(3000, cancellationToken).ConfigureAwait(false);

                string pollUrl = $"{BasePassportUrl}/poll?qrcode_key={_qrCodeKey}";
                string response = await _httpClient.GetStringAsync(pollUrl).ConfigureAwait(false);
                int code = JsonValueHelper.GetJsonValue<int>(response, "$.data.code");

                switch (code)
                {
                    case 0: // 登录成功
                        SaveCookie();
                        return true;
                    case 86038: // 二维码过期
                        return false;
                }
            }

            return false; // 超时或被取消
        }

        private void SaveCookie()
        {
            CookieCollection cookie =
                HttpFactory.CookieContainer.GetCookies(new Uri("https://www.bilibili.com"));
            var cookieList = cookie.Cast<Cookie>().Select(c => new SerializableCookie(c)).ToList();
            string json = JsonSerializer.Serialize(cookieList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CookieSavePath, json);
        }

        private async Task<bool> SetCookie()
        {
            if (!File.Exists(CookieSavePath))
            {
                return false;
            }
            var cookieTest = await File.ReadAllTextAsync(CookieSavePath);
            if (File.Exists(CookieSavePath)
                && cookieTest != "[]"
                && !string.IsNullOrEmpty(cookieTest))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(CookieSavePath);
                    var savedCookies = JsonSerializer.Deserialize<List<Cookie>>(json);
                    foreach (var c in savedCookies)
                    {
                        HttpFactory.CookieContainer.Add(new Uri("https://www.bilibili.com"), c);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }

            return false;
        }
    }
}