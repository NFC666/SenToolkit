namespace SenTooliKit.IServices.IServices
{
    public interface IBilibiliStreamService
    {
        Task<string[]> GetStreamSourceAsync(long cid, string quality = "4");

        Task<string> DownloadStreamToFileAsync(long cid
            , string saveDic
            , CancellationToken cancellationToken
            , string quality = "4"
        );
    }
}