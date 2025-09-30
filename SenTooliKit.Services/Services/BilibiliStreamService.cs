using SenTooliKit.Common.Helpers;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Services.Services
{
    public class BilibiliStreamService : IBilibiliStreamService
    {
        private readonly HttpClient _httpClient = HttpFactory
            .CreateNormalHttpClient();
        public async Task<string[]> GetStreamSourceAsync
            (long cid,string quality = "4")
        {

            string baseUrl =
                "https://api.live.bilibili.com/room/v1/Room/playUrl";
            Dictionary<string, string> query = new()
            {
                { "cid", cid.ToString() },
                { "platform", "web" },
                { "quality", quality },
            };
            var res = await JsonValueHelper
                .GetJsonValueWithQueryAsync<string[]>
                (baseUrl, "url", query);
            return res;
        }

        /// <summary>
        /// 下载直播流
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="saveDic"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<string> DownloadStreamToFileAsync(
            long cid, 
            string saveDic, 
            CancellationToken cancellationToken = default,
            string quality = "4" 
            )
        {
            var savePath = Path.Combine(saveDic, $"{cid}.flv");
            string[] source = await GetStreamSourceAsync(cid, quality);

            if (source == null || source.Length == 0)
                throw new Exception("未获取到直播源");

            string url = source[0];
            
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync(cancellationToken))
                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                        // 每次写入都检查取消
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new OperationCanceledException("用户取消任务");
                        }
                    }
                }
            }

            return savePath;
        }

    }
}