using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using SenTooliKit.Common.Helpers;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Services.Services
{
    public class BilibiliInfoService : IBilibiliInfoService
    {
        private readonly HttpClient _httpClient = HttpFactory.GetHttpClient();

        /// <summary>
        /// 获取cid
        /// </summary>
        /// <param name="bvId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<long[]> GetCidsAsync(string bvId)
        {
            if (string.IsNullOrWhiteSpace(bvId))
                throw new ArgumentException("BV号不能为空", nameof(bvId));

            string url = $"https://api.bilibili.com/x/web-interface/wbi/view?bvid={bvId}";
            string response = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
            var res = JsonValueHelper.GetJsonValue<long[]>(response, "$.data.pages..cid");
            return res;
        }

        /// <summary>
        /// 获取直播信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<string> GetStreamInfoAsync(long cid)
        {
            var apiUrl = $"https://api.live.bilibili.com/room/v1/Room/get_info?room_id={cid}";
            var res = await
                _httpClient.GetStringAsync(apiUrl);
            using var doc = JsonDocument.Parse(res);
            string formatted = JsonSerializer.Serialize(
                doc,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    
                }
            );
            return formatted;
        }

        /// <summary>
        /// 获取wbiKey
        /// </summary>
        /// <returns></returns>
        public async Task<(string, string)> GetWbiKey()
        {
            var baseUrl = "https://api.bilibili.com/x/web-interface/nav";
            var resp = await _httpClient
                .GetStringAsync(baseUrl);
            var imagKeyUrl = JsonValueHelper
                .GetJsonValue<string>(resp, "img_url");
            var imagKey = BilibiliWbiHelper.ExtractKeyFromUrl(imagKeyUrl);
            var subKeyUrl = JsonValueHelper
                .GetJsonValue<string>(resp, "sub_url");
            var subKey = BilibiliWbiHelper.ExtractKeyFromUrl(subKeyUrl);
            return (imagKey, subKey);
            
        }
    }
}