using Bilibili.Community.Service.Dm.V1;

using SenTooliKit.Common.Models;

namespace SenTooliKit.IServices.IServices
{
    public interface IBilibiliDmService
    {
        Task<string[]> GetDmTimesByOidAsync(string Oid,DateTime startTime);
        
        Task<DmSegMobileReply> GetDmSegMobileReplyByDateAsync(long Oid,string time);
        
        Task<List<DmInfo>> GetDmInfoAsync(long Oid,string time);
        
    }
}