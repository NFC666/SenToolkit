using SenTooliKit.Common.Models;

namespace SenTooliKit.IServices.IServices
{
    public interface IBiliCommentsService
    {
        Task<List<Reply>> GetCommentsAsync(string bvid);
        Task<List<Reply>> GetBaseAllCommentsAsync(string bvid);

        Task<List<Reply>> GetCommentsByReplyAsync(long rpid
            , string bvid, int pn = 1, int ps = 5);
        Task<List<Reply>> GetAllCommentsByReplyAsync(string bvid);
    }
}