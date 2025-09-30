namespace SenTooliKit.IServices.IServices
{
    public interface IBilibiliAuthService
    {
        Task<string> GetQrCodePathAsync();
        Task<bool> Login();
        Task<bool> Logout();
    }
}