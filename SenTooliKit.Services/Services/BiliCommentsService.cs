using System.Text.Json;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Services.Services
{
    public class BiliCommentsService : IBiliCommentsService
    {
        private readonly HttpClient _httpClient = HttpFactory.GetBiliHttpClient();

        private readonly IBilibiliInfoService _bilibiliInfoService = new BilibiliInfoService();

        /// <summary>
        /// 获取视频的评论
        /// </summary>
        /// <param name="bvid"></param>
        /// <returns></returns>
        public async Task<List<Reply>> GetCommentsAsync(string bvid)
        {
            var baseUrl = "https://api.bilibili.com/x/v2/reply/wbi/main";
            var parameter = new Dictionary<string, string> { { "type", "1" }, { "oid", bvid }, };
            (var imgKey, var subKey) = await _bilibiliInfoService.GetWbiKey();
            var signedParams = BilibiliWbiHelper.EncWbi(parameter, imgKey, subKey);
            string query = string.Join("&", signedParams.Select(p => $"{p.Key}={p.Value}"));
            var resp = await _httpClient.GetStringAsync($"{baseUrl}?{query}");
            var res = JsonValueHelper.GetJsonValue<List<Reply>>(resp, "$.data.replies");
            return res;
        }


        /// <summary>
        /// 获取视频的所有基本评论
        /// </summary>
        /// <param name="bvid"></param>
        /// <returns></returns>
        public async Task<List<Reply>> GetBaseAllCommentsAsync(string bvid)
        {
            var baseUrl = "https://api.bilibili.com/x/v2/reply/wbi/main";
            var allReplies = new List<Reply>();
            string? nextOffset = null;
            var random = new Random();

            do
            {
                // 基础参数
                var parameter = new Dictionary<string, string>
                {
                    { "oid", $"{bvid}" },
                    { "type", "1" },
                    { "mode", "3" },
                    { "plat", "1" },
                    { "web_location", "1315875" },
                };

                // 如果有 nextOffset，则生成 pagination_str
                if (!string.IsNullOrEmpty(nextOffset))
                {
                    parameter["pagination_str"] = $"{{\"offset\":\"{nextOffset}\"}}";
                }

                // 获取 WBI 签名
                (var imgKey, var subKey) = await _bilibiliInfoService.GetWbiKey();
                var signedParams = BilibiliWbiHelper.EncWbi(parameter, imgKey, subKey);
                string query = await new FormUrlEncodedContent(signedParams).ReadAsStringAsync();
                // 发送请求
                var resp = await _httpClient.GetStringAsync($"{baseUrl}?{query}");

                // 解析评论
                var replies = JsonValueHelper.GetJsonValue<List<Reply>>(resp, "$.data.replies");
                if (replies != null)
                    allReplies.AddRange(replies);
                try
                {
                    // 获取下一页 offset
                    nextOffset =
                        JsonValueHelper
                            .GetJsonValue<string>(resp, "$.data.cursor.pagination_reply.next_offset");
                }
                catch (Exception)
                {
                    nextOffset = null;
                }

                // 随机延迟 ，避免风控
                await Task.Delay(random.Next(500, 1000));
            } while (!string.IsNullOrEmpty(nextOffset));

            return allReplies;
        }

        /// <summary>
        /// 获取回复
        /// </summary>
        /// <param name="rpid"></param>
        /// <param name="bvid"></param>
        /// <param name="pn"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public async Task<List<Reply>> GetCommentsByReplyAsync(long rpid
            , string bvid, int pn = 1, int ps = 5)
        {
            var random = new Random();
            var res = new List<Reply>();
            var baseUrl = "https://api.bilibili.com/x/v2/reply/reply";

            try
            {
                for (int i = 1; i < 100; i++)
                {
                    var parameter = new Dictionary<string, string>
                    {
                        { "oid", $"{bvid}" },
                        { "type", "1" },
                        { "root", $"{rpid}" },
                        { "pn", $"{pn}" },
                        { "ps", $"{ps}" },
                    };
                    res.AddRange(await JsonValueHelper
                        .GetJsonValueWithQueryAsync<List<Reply>>(
                            baseUrl, "$.data.replies", parameter));
                    pn++;
                    await Task.Delay(random.Next(500, 1000));
                }
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return res;
            }
        }
        /// <summary>
        /// 获取所有回复
        /// </summary>
        /// <param name="bvid"></param>
        /// <returns></returns>
        public async Task<List<Reply>> GetAllCommentsByReplyAsync(string bvid)
        { 
            var random = new Random();
            var res = await GetBaseAllCommentsAsync(bvid);
            foreach (Reply r in res)
            {
                if (r.Replies is null || r.Replies.Count == 0)
                {
                    continue; 
                }
                r.Replies = await GetCommentsByReplyAsync(r.RpId, bvid);
            }
            return res;
        }
    }
}