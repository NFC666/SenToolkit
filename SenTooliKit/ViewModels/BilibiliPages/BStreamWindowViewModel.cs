using System.Windows;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SenTooliKit.Common.Helpers;
using SenTooliKit.IServices.IServices;

using MessageBox = System.Windows.Forms.MessageBox;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BStreamWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string _url = "";
        [ObservableProperty] private string _savePath = "";
        [ObservableProperty] private string _streamDetailInfo = "";
        [ObservableProperty] private bool _isDownloading;
        [ObservableProperty] private Visibility _downloadButtonVisibility = Visibility.Visible;


        private string? _saveDic;
        private CancellationTokenSource _cts = new();
        private CancellationToken _cancellationToken;


        private readonly IBilibiliStreamService _bilibiliStreamService;
        private readonly IBilibiliInfoService _bilibiliInfoService;

        public BStreamWindowViewModel(
            IBilibiliStreamService bilibiliStreamService,
            IBilibiliInfoService bilibiliInfoService)
        {
            _bilibiliStreamService = bilibiliStreamService;
            _bilibiliInfoService = bilibiliInfoService;
        }

        [RelayCommand]
        private async Task DownloadStreamAndShowInfo()
        {
            _cts = new CancellationTokenSource();
            _cancellationToken = _cts.Token;
            try
            {
                SetDownloadStatus();
                var cid = GetCid(Url);
                StreamDetailInfo = await ShowStreamInfo(cid);
                _saveDic = GetSaveDic();
                await DownloadStream(cid, _saveDic, _cancellationToken);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}(手动停止录制请无视)");
            }
            finally
            {
                ResetDownloadStatus();
            }
        }

        [RelayCommand]
        private async Task StopStream()
        {
            await _cts.CancelAsync();
            ResetDownloadStatus();
        }

        private long GetCid(string url)
        {
            try
            {
                return StreamCidHelper.GetCidFromUrl(url);
            }
            catch (Exception e)
            {
                throw new Exception($"视频链接识别失败，请检查链接是否正确。\n详情：{e.Message}");
            }
        }

        private async Task<string> ShowStreamInfo(long cid)
        {
            try
            {
                return await _bilibiliInfoService.GetStreamInfoAsync(cid);
            }
            catch (Exception e)
            {
                return $"加载直播信息失败，请稍后再试。\n详情：{e.Message}";
            }
        }

        private async Task DownloadStream(long cid, string saveDic, CancellationToken token)
        {
            try
            {
                await _bilibiliStreamService.DownloadStreamToFileAsync(cid, saveDic, token);
            }
            catch (Exception e)
            {
                throw new Exception($"下载失败，请检查网络或稍后再试。\n详情：{e.Message}");
            }
        }

        private void SetDownloadStatus()
        {
            IsDownloading = true;
            DownloadButtonVisibility = Visibility.Collapsed;
        }
        private void ResetDownloadStatus()
        {
            IsDownloading = false;
            DownloadButtonVisibility = Visibility.Visible;
        }


        private string GetSaveDic()
        {
            using var dialog = new FolderBrowserDialog { Description = "请选择保存目录" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            throw new Exception("没有选择保存目录，请重新尝试。");
        }
    }
}