namespace SenTooliKit.IServices.IServices
{
    public interface IBilibiliInfoService 
    {
        Task<long[]> GetCidsAsync(string bvId);

        Task<string> GetStreamInfoAsync(long cid);
        
        Task<(string,string)> GetWbiKey();
    }
}