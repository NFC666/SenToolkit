using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;

using MessageBox = System.Windows.MessageBox;


// 以下代码需要重构
namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BDmInfoUserControlVM : ViewModelBase
    {
        private readonly IBilibiliDmService _bilibiliDmService;
        private readonly IBilibiliInfoService _bilibiliInfoService;

        public BDmInfoUserControlVM(
            IBilibiliInfoService bilibiliInfoService,
            IBilibiliDmService bilibiliDmService)
        {
            _bilibiliInfoService = bilibiliInfoService;
            _bilibiliDmService = bilibiliDmService;
        }

        #region Observable Properties
        [ObservableProperty] private string _saveDirectory = string.Empty;
        [ObservableProperty] private string _url = string.Empty;
        [ObservableProperty] private string _selectedTime = string.Empty;
        [ObservableProperty] private VideoPart _selectedCid;
        [ObservableProperty] private DateTime _month = DateTime.Now;
        [ObservableProperty] private string _errorText = string.Empty;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private double _progress;
        [ObservableProperty] private DmInfo _selectedBulletInfo;
        [ObservableProperty] private bool _isIndeterminate;
        [ObservableProperty] private string _keyword;

        #endregion

        #region Collections
        [ObservableProperty] private ObservableCollection<string> _bulletTimes = new();
        [ObservableProperty] private ObservableCollection<DmInfo> _bulletInfos = new();
        [ObservableProperty] private ObservableCollection<VideoPart> _cids = new();
        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedCid))
            {
                _ = TaskLoadTimesWhenCidChangeAsync();
            }
        }

        #region Command Methods
        
        [RelayCommand]
        private void Copy()
        { 
            if (SelectedBulletInfo == null) return;
            Clipboard.SetText(SelectedBulletInfo.DmContent);
        }
        [RelayCommand]
        private async Task GetSenderId()
        {
            if (SelectedBulletInfo == null) return;
            IsIndeterminate = true;
            await GetId();
            IsIndeterminate = false;
        }

        [RelayCommand]
        private void Search()
        {
            var res = new List<DmInfo>();
            if (string.IsNullOrWhiteSpace(Keyword))
            {
                return;
            }
            foreach (var item in BulletInfos)
            {
                if (item.DmContent.Contains(Keyword) 
                    || item.SendId.Contains(Keyword) 
                    || item.DmId.ToString() == Keyword)
                {
                    res.Add(item);
                }
            }
            BulletInfos = new ObservableCollection<DmInfo>(res);
        }



        [RelayCommand]
        private async Task LoadBulletTimesAndCidsAsync()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                SetError("请输入正确的视频链接");
                return;
            }

            try
            {
                IsLoading = true;
                ClearError();

                // 获取分P
                Cids.Clear();
                var cids = await SafeGetCidsAsync();
                if (cids.Length == 0)
                {
                    SetError("未获取到任何分P信息");
                    return;
                }

                foreach (var item in cids.Select((cid, idx) => new VideoPart($"P{idx + 1}", cid)))
                {
                    Cids.Add(item);
                }
                SelectedCid = Cids[0];

                // 获取时间段
                await LoadTimesForSelectedCidAsync();
            }
            catch (Exception ex)
            {
                SetError("加载失败", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadBulletsAsync()
        {
            if (SelectedCid.Cid == 0 || string.IsNullOrEmpty(SelectedTime))
            {
                SetError("请先选择分P和时间段");
                return;
            }

            try
            {
                IsLoading = true;
                ClearError();
                BulletInfos.Clear();

                var dmList = await _bilibiliDmService.GetDmInfoAsync(
                    SelectedCid.Cid, SelectedTime);

                foreach (var item in dmList)
                {
                    BulletInfos.Add(new DmInfo
                    {
                        DmId = item.DmId,
                        SendTime = item.SendTime,
                        DmContent = item.DmContent,
                        SendId = item.SendId
                    });
                }
            }
            catch (Exception ex)
            {
                SetError("加载弹幕失败", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DownloadAllDmAsync()
        {
            if (!GetSaveDirectory()) return;

            try
            {
                IsLoading = true;
                ClearError();

                await ExecuteSaveBySingleVideoAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Helpers

        private async Task GetId()
        {
            // 在后台线程执行耗时计算
            var userId = await Task.Run(() => MidHashHelper.FindUserId(SelectedBulletInfo.SendId));

            if (userId == -1)
            {
                SetError("未找到发送者ID");
                return;
            }
            OpenSendHomePage(userId);
            // 这里回到UI线程，可以正常调用UI相关方法
            MessageBox.Show($"发送者ID为：{userId}, 已保存到你的粘贴板");

            Clipboard.SetText(userId.ToString());
        }

        private void OpenSendHomePage(int userId)
        {
            string url = $"https://space.bilibili.com/{userId}";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        
        private async Task LoadTimesForSelectedCidAsync()
        {
            if (SelectedCid?.Cid == 0 || string.IsNullOrWhiteSpace(Url))
                return;

            try
            {
                BulletTimes.Clear();
                var times = await SafeGetTimesAsync(SelectedCid.Cid);
                foreach (var time in times)
                {
                    BulletTimes.Add(time);
                }
                if (BulletTimes.Count > 0)
                    SelectedTime = BulletTimes[0];
            }
            catch (Exception ex)
            {
                SetError("加载时间段失败", ex);
            }
        }

        private async Task TaskLoadTimesWhenCidChangeAsync()
        {
            try
            {
                if (SelectedCid is null)
                    return;
                var times = await SafeGetTimesAsync(SelectedCid.Cid);
                BulletTimes.Clear();
                foreach (var time in times)
                {
                    BulletTimes.Add(time);
                }

                if (BulletTimes.Count > 0)
                    SelectedTime = BulletTimes[0];
            }
            catch (Exception ex)
            {
                SetError("获取弹幕时间失败", ex);
            }
        }

        private bool GetSaveDirectory()
        {
            using var dialog = new FolderBrowserDialog { Description = "请选择保存目录" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveDirectory = dialog.SelectedPath;
                return true;
            }
            return false;
        }

        private async Task<long[]> SafeGetCidsAsync()
        {
            try
            {
                return await _bilibiliInfoService.GetCidsAsync(BvHelper.GetBvFromUrl(Url));
            }
            catch (Exception ex)
            {
                SetError("获取分P失败", ex);
                return Array.Empty<long>();
            }
        }

        private async Task<string[]> SafeGetTimesAsync(long oid)
        {
            try
            {
                return await _bilibiliDmService.GetDmTimesByOidAsync(oid.ToString(), Month);
            }
            catch (Exception ex)
            {
                SetError("获取弹幕时间失败", ex);
                return Array.Empty<string>();
            }
        }

        private async Task<List<DmDetailInfo>> SafeGetDmInfoAsync(long oid, string time)
        {
            try
            {
                var res = await _bilibiliDmService.GetDmSegMobileReplyByDateAsync(oid, time);
                return res.Elems.Select(e => new DmDetailInfo(e)).ToList();
            }
            catch (Exception ex)
            {
                SetError("获取弹幕信息失败", ex);
                return new List<DmDetailInfo>();
            }
        }

        private async Task ExecuteSaveBySingleVideoAsync()
        {
            if (string.IsNullOrEmpty(SaveDirectory))
            {
                SetError("请选择保存路径");
                return;
            }

            var cids = await SafeGetCidsAsync();
            if (cids.Length == 0) return;

            var cid = SelectedCid;
            var times = await SafeGetTimesAsync(cid.Cid);
            if (times.Length == 0)
            {
                SetError("未获取到任何弹幕时间");
                return;
            }

            var allDmInfos = new List<DmDetailInfo>();
            int total = times.Length;
            int count = 0;

            foreach (var t in times)
            {
                var infos = await SafeGetDmInfoAsync(cid.Cid, t);
                allDmInfos.AddRange(infos);
                count++;
                Progress = count * 100.0 / total; // 更新进度
            }

            var result = SaveDmInfoToExcel(allDmInfos, $"{BvHelper.GetBvFromUrl(Url)}.xlsx");
            SetError(result);

            Progress = 0; // 重置进度
        }


        private string SaveDmInfoToExcel(List<DmDetailInfo> allDmInfos, string fileName)
        {
            try
            {
                var path = Path.Combine(SaveDirectory, fileName);
                using var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Danmaku"
                });

                var props = typeof(DmDetailInfo).GetProperties();

                // 写表头
                var headerRow = new Row();
                foreach (var p in props)
                    headerRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(p.Name) });
                sheetData.AppendChild(headerRow);

                // 写数据
                foreach (var item in allDmInfos)
                {
                    var row = new Row();
                    foreach (var p in props)
                        row.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(p.GetValue(item)?.ToString() ?? "") });
                    sheetData.AppendChild(row);
                }

                workbookPart.Workbook.Save();
                return $"Excel 保存成功: {path}";
            }
            catch (Exception ex)
            {
                return $"Excel 保存失败: {ex.Message}";
            }
        }

        private void SetError(string message, Exception? ex = null)
        {
            ErrorText = ex == null ? message : $"{message}: {ex.Message}";
        }

        private void ClearError() => ErrorText = string.Empty;

        #endregion
    }

    public record VideoPart(string Label, long Cid);
}
