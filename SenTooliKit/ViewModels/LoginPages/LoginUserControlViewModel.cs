using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SenTooliKit.IServices.IServices;

namespace SenTooliKit.ViewModels.LoginPages
{
    public partial class LoginUserControlViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _stateText = "未登录";

        [ObservableProperty]
        private string _qrCodePath = "";

        [ObservableProperty]
        private Visibility _isQrCodeShow = Visibility.Visible;
        
        private readonly IBilibiliAuthService _bilibiliAuthService;
        
        public LoginUserControlViewModel(IBilibiliAuthService bilibiliAuthService)
        {
            _bilibiliAuthService = bilibiliAuthService;
            _ = InitAsync();
        }
        
        private async Task InitAsync()
        { 
            string qrCodePathAsync = await _bilibiliAuthService.GetQrCodePathAsync();
            QrCodePath = qrCodePathAsync;
            IsQrCodeShow = Visibility.Visible;
            if (await _bilibiliAuthService.Login())
            {
                StateText = "已登录";
                QrCodePath = "";
                IsQrCodeShow = Visibility.Collapsed;
            }
            else
            {
                StateText = "登录失败";
                QrCodePath = "";
                IsQrCodeShow = Visibility.Collapsed;
            }
        }

        [RelayCommand]
        public async Task LoginAgain()
        {
            StateText = "未登录（请重新扫码）";
            
            await _bilibiliAuthService.Logout();
            await InitAsync();
        }
    }
}