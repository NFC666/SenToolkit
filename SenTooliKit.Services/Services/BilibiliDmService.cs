using Bilibili.Community.Service.Dm.V1;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Services.Services
{
    public class BilibiliDmService : IBilibiliDmService
    {
        private readonly HttpClient _httpClient = HttpFactory.GetHttpClient();

        /// <summary>
        /// 获取有人发弹幕的日期
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public Task<string[]> GetDmTimesByOidAsync(string oid, DateTime startTime)
        {
            string baseUrl = "https://api.bilibili.com/x/v2/dm/history/index";
            var query = new Dictionary<string, string>
            {
                ["oid"] = oid,
                ["type"] = "1",
                ["month"] = startTime.ToString("yyyy-MM")
            };
            var res = JsonValueHelper
                .GetJsonValueWithQueryAsync<string[]>(baseUrl, "data", query);
            return res;
        }

        /// <summary>
        /// 获取DmSegMobileReply弹幕形式
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task<DmSegMobileReply> GetDmSegMobileReplyByDateAsync(long oid, string time)
        {
            string url = $"https://api.bilibili.com/x/v2/dm/web/history/seg.so?type=1&oid={oid}&date={time}";
            
            var response = await _httpClient.GetAsync(url);
            
            response.EnsureSuccessStatusCode();
            
            byte[] rawBytes = await response.Content.ReadAsByteArrayAsync();
            
            // 使用 protobuf 反序列化
            var reply = DmSegMobileReply.Parser.ParseFrom(rawBytes);
            
            return reply;
        }
        /// <summary>
        /// 获取弹幕
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task<List<DmInfo>> GetDmInfoAsync(long cid, string time)
        {
            var dmInfos = new List<DmInfo>();
            var reply = await GetDmSegMobileReplyByDateAsync(cid, time);
            foreach (var e in reply.Elems)
            {
                var dmInfo = new DmInfo
                {
                    DmId = e.Id,
                    DmContent = e.Content,
                    SendTime = FormatTime(e.Progress),
                    SendId = e.MidHash
                };
                dmInfos.Add(dmInfo);
            }
            return dmInfos;
        }
        private static string FormatTime(long milliseconds)
        {
            long totalSeconds = milliseconds / 1000;
            long minutes = totalSeconds / 60;
            long seconds = totalSeconds % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}