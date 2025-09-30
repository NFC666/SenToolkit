using SenTooliKit.Common.Models;

namespace SenTooliKit.IServices.IServices
{
    public interface IBilibiliVideoService
    {
        Task<string?> DownloadVideoAsync(string bvid, long cid, string savePath, int qn = 112);
        
        Task<string?> DownloadCoverAsync(string bvid);
        
        Task<VideoDetailInfo> GetVideoDetailInfoAsync(string bvid);
    }
}