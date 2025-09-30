using System.Text.Json.Nodes;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Services.Services
{
    public class BilibiliVideoService : IBilibiliVideoService
    {
        private static string DonwloadPath = PathHelper.GetDownloadPath();

        
        public async Task<string?> DownloadVideoAsync(string bvid, long cid,
            string? savaPath = null, int qn = 112)
        { 
            var baseUrl = $"https://api.bilibili.com/x/player/playurl";
            Dictionary<string, string> query = new()
            {
                ["bvid"] = bvid,
                ["cid"] = cid.ToString(),
                ["qn"] = qn.ToString(),
                ["fnval"] = "0",
                ["fnver"] = "0",
                ["fourk"] = "1"
            };
            var streamUrl = await JsonValueHelper
                .GetJsonValueWithQueryAsync<string>(baseUrl, "url", query);

            if (!string.IsNullOrEmpty(savaPath) && Directory.Exists(Path.GetDirectoryName(savaPath)))
            {
                await SaveStreamToFile(streamUrl, savaPath);
                return savaPath;
            }
            await SaveStreamToFile(streamUrl, DonwloadPath);
            return DonwloadPath;
            
        }

        /// <summary>
        /// 获取视频封面
        /// </summary>
        /// <param name="bvid"></param>
        /// <returns></returns>
        public async Task<string?> DownloadCoverAsync(string bvid)
        {
            var baseUrl = $"https://api.bilibili.com/x/web-interface/wbi/view";
            Dictionary<string, string> query = new()
            {
                ["bvid"] = bvid
            };
            var picUrl = await JsonValueHelper
                .GetJsonValueWithQueryAsync<string>(baseUrl, "pic", query);
            return await SaveBytesToFile(picUrl, PathHelper.GetCoverImgPath(bvid));
        }

        /// <summary>
        /// 获取视频详情
        /// </summary>
        /// <param name="bvid"></param>
        /// <returns></returns>
        public async Task<VideoDetailInfo> GetVideoDetailInfoAsync(string bvid)
        {
            var baseUrl = $"https://api.bilibili.com/x/web-interface/wbi/view";
            Dictionary<string, string> query = new()
            {
                ["bvid"] = bvid
            };
            var videoDetail = await JsonValueHelper
                .GetJsonValueWithQueryAsync<VideoDetailInfo>(baseUrl, "data", query);
            return videoDetail;
        }

        /// <summary>
        /// 保存视频流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        private async Task SaveStreamToFile(string url, string filePath)
        { 
            var httpClient = HttpFactory.GetBiliHttpClient();
            using var response = await httpClient.GetAsync(url);
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);
        }
        private async Task<string> SaveBytesToFile(string url, string filePath)
        { 
            var biliHttpClient = HttpFactory.GetBiliHttpClient();
            var imagesBytes = await biliHttpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, imagesBytes);
            return filePath;
        }
    }
}