using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Ookii.Dialogs.Wpf;

using SenTooliKit.Common.Helpers;
using SenTooliKit.IServices.IServices;

using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BVideoViewModel : ViewModelBase
    {
        // 保存路径
        [ObservableProperty] private string? _savePath = PathHelper.GetDownloadPath();

        // 视频质量选项（显示文本 + qn值）
        public List<(string Name, int Qn)> QualityOptions { get; } = new()
        {
            ("240P 极速", 6),
            ("360P 流畅", 16),
            ("480P 清晰", 32),
            ("720P 高清", 64),
            ("720P60 高帧率", 74),
            ("1080P 高清", 80),
            ("1080P+ 高码率", 112),
            ("1080P60 高帧率", 116),
            ("4K 超清", 120),
            ("HDR 真彩色", 125),
            ("杜比视界", 126),
            ("8K 超高清", 127),
            ("HDR Vivid", 129)
        };

        [ObservableProperty] private (string Name, int Qn) _selectedQuality;

        [ObservableProperty] private List<(string p, long cid)> _cids = new();
        [ObservableProperty] private (string p, long cid) _cid;

        [ObservableProperty] private bool _isDownloading;

        [ObservableProperty] private bool _isGetCids;

        [ObservableProperty] private string _videoUrl;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(VideoUrl))
            {
                _ = GetCids();
            }
        }

        private async Task GetCids()
        {
            try
            {
                Cids.Clear();
                IsGetCids = true;
                var res = await _bilibiliInfoService.GetCidsAsync(BvHelper.GetBvFromUrl(VideoUrl));
                IsGetCids = false;
                int index = 1;
                foreach (long c in res)
                {
                    Cids.Add(($"p{index}", c));
                    index++;
                }
            }catch (Exception e)
            {
                MessageBox.Show("获取分批失败！"+e.Message);
            }
        }

        [ObservableProperty] private string _downInfo;


        private readonly IBilibiliVideoService _bilibiliService;
        private readonly IBilibiliInfoService _bilibiliInfoService;

        public BVideoViewModel(IBilibiliVideoService bilibiliService,
            IBilibiliInfoService bilibiliInfoService)
        {
            _bilibiliService = bilibiliService;
            _bilibiliInfoService = bilibiliInfoService;
            _selectedQuality = QualityOptions[3]; // 默认 720P 高清
        }

        [RelayCommand]
        private void BrowsePath()
        {
            try
            {
                SavePath = OpenBrowserPath();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        [RelayCommand]
        private async Task Download()
        {
            try
            {
                StartDownloading();
                await DownloadFromUrl(VideoUrl, Cid.cid, SavePath);
                MessageBox.Show($"下载完成！\n保存地址：{SavePath}");
            }
            catch (Exception e)
            {
                MessageBox.Show("请检查保存路径或者分批选择,"+e.Message);
            }
            finally
            {
                StopDownloading();
            }
        }

        private async Task<string?> DownloadFromUrl(string url, long cid, string? savePath)
        {
            try
            {
                string bv = BvHelper.GetBvFromUrl(url);
                string? filePath = await _bilibiliService.DownloadVideoAsync(
                    bv,
                    cid,
                    savePath
                );
                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"下载失败：{ex.Message}");
            }
        }

        private string? OpenBrowserPath()
        {
            try
            {
                string? savePath = null;
                var dialog = new SaveFileDialog
                {
                    Title = "选择或输入文件名", Filter = "所有文件|*.*", FileName = "default.mp4" // 可选，提供一个默认文件名
                };

                if (dialog.ShowDialog() == true)
                {
                    savePath = dialog.FileName; // 这里就是完整的文件路径（可能不存在）
                }

                return savePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"选择路径失败：{ex.Message}");
            }
        }

        private void StartDownloading()
        {
            IsDownloading = true;
        }

        private void StopDownloading()
        {
            IsDownloading = false;
        }
    }
}