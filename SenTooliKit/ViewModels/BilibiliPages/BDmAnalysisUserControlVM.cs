using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ScottPlot;
using ScottPlot.WPF;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;

using FontFamily = System.Windows.Media.FontFamily;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BDmAnalysisUserControlVM : ViewModelBase
    {
        private readonly IBilibiliDmService _bilibiliDmService;
        private readonly IBilibiliInfoService _bilibiliInfoService;

        [ObservableProperty] private string _url;
        [ObservableProperty] private WpfPlot _plot1 = new();
        [ObservableProperty] private WpfPlot _piePlot = new();
        private List<DmDetailInfo> _dmDetailInfos = new();
        private int _intervalSeconds = 1;

        public BDmAnalysisUserControlVM(IBilibiliDmService bilibiliDmService, IBilibiliInfoService bilibiliInfoService)
        {
            _bilibiliDmService = bilibiliDmService;
            _bilibiliInfoService = bilibiliInfoService;
        }

        [RelayCommand]
        private async Task SearchAndAnalysis()
        {
            try
            {
                _dmDetailInfos = await GetDmDetailInfos(Url);
                if (_dmDetailInfos is null) return;
                if (_dmDetailInfos.Count > 0)
                {
                    DrawPlot();
                    DrawPiePlot();
                }
            }catch (Exception ex)
            {
                MessageBox.Show($"获取弹幕出现错误：\n{ex.Message}");
            }
        }

        private async Task<List<DmDetailInfo>> GetDmDetailInfos(string url)
        {
            try
            {
                var res = new List<DmDetailInfo>();
                var bvid = BvHelper.GetBvFromUrl(url);
                var cids = await _bilibiliInfoService.GetCidsAsync(bvid);
                foreach (var cid in cids)
                {
                    var times = await _bilibiliDmService
                        .GetDmTimesByOidAsync(cid.ToString(), DateTime.Now);
                    var time = times.Last();
                    var dmSegMobileReply = await _bilibiliDmService
                        .GetDmSegMobileReplyByDateAsync(cid, time);
                    foreach (var e in dmSegMobileReply.Elems)
                    {
                        res.Add(new DmDetailInfo(e));
                    }
                }

                return res;
            }
            catch (Exception ex)
            {

                throw new Exception($"{ex.Message}");
            }
        }

        private void DrawPlot()
        {
            try
            {
                if (_dmDetailInfos.Count == 0) return;

                // 找到最小值和最大值（单位：秒）
                int minSec = (int)(_dmDetailInfos.Min(x => x.Progress) / 1000);
                int maxSec = (int)(_dmDetailInfos.Max(x => x.Progress) / 1000);

                // 统计并补齐区间
                var counts = CountDanmakuByInterval(_dmDetailInfos, _intervalSeconds, minSec, maxSec);

                if (counts is null)
                {
                    return;
                }

                double[] xs = counts.Keys.Select(k => k * _intervalSeconds).Select(x => (double)x).ToArray();
                double[] ys = counts.Values.Select(v => (double)v).ToArray();
            
                var plt = Plot1.Plot;
                plt.Clear();
                plt.Add.Bars(xs, ys);

                // 设置坐标轴标签
                plt.XLabel("Time");
                plt.YLabel("Danmu Counts");
                plt.Title("弹幕分布");
            

                // 设置横坐标范围
                plt.Axes.SetLimitsX(minSec, maxSec);

                // ✅ ScottPlot 5 的刻度格式化方式
                plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic()
                {
                    LabelFormatter = sec =>
                    {
                        int minutes = (int)sec / 60;
                        int seconds = (int)sec % 60;
                        return $"{minutes:D2}:{seconds:D2}";
                    }
                };
                Plot1.Plot.Font.Automatic();
                Plot1.Plot.Axes.AutoScale();
                Plot1.Refresh();
            }catch (Exception ex)
            {
                throw new Exception($"绘制弹幕分布图出现错误：{ex.Message}");
            }
        }

        // 改造的统计函数：补齐缺失的区间
        private Dictionary<int, int> CountDanmakuByInterval(List<DmDetailInfo> infos, int intervalSeconds, int minSec,
            int maxSec)
        {
            try
            {
                var grouped = infos
                    .Select(x => (int)(x.Progress / 1000)) // 毫秒转秒
                    .GroupBy(sec => sec / intervalSeconds)
                    .ToDictionary(g => g.Key, g => g.Count());

                var dict = new Dictionary<int, int>();
                for (int i = minSec / intervalSeconds; i <= maxSec / intervalSeconds; i++)
                {
                    dict[i] = grouped.ContainsKey(i) ? grouped[i] : 0;
                }

                return dict;
            }
            catch (Exception ex)
            {
                throw new Exception($"统计弹幕出现错误：{ex.Message}");
            }
        }

        private void DrawPiePlot()
        {
            if (_dmDetailInfos.Count == 0) return;
            try
            {
                // 分词并统计
                var allWords = new List<string>();
                foreach (var d in _dmDetailInfos)
                    allWords.AddRange(JiebaHelper.CutFiltered(d.Content));

                var wordCounts = allWords
                    .GroupBy(w => w)
                    .Select(g => new { Word = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10) // 取前10个高频词，可按需调整
                    .ToList();

                if (wordCounts.Count == 0) return;

                double total = wordCounts.Sum(x => x.Count);
                
                var palette = new[]
                {
                    ScottPlot.Colors.Red, ScottPlot.Colors.Orange, ScottPlot.Colors.Gold, ScottPlot.Colors.Green,
                    ScottPlot.Colors.Blue, ScottPlot.Colors.Purple, ScottPlot.Colors.Cyan, ScottPlot.Colors.Brown,
                    ScottPlot.Colors.Magenta, ScottPlot.Colors.Lime
                };
                
                var slices = new List<PieSlice>();
                for (int i = 0; i < wordCounts.Count; i++)
                {
                    var wc = wordCounts[i];
                    var pct = wc.Count / total;
                    var label = $"{wc.Word} ({wc.Count}, {pct:P0})"; // e.g. "hello (12, 24%)"
                    var color = palette[i % palette.Length];

                    slices.Add(new PieSlice()
                    {
                        Value = wc.Count,
                        FillColor = color,
                        Label = label, // 扇区上的标签
                        LegendText = wc.Word // 图例文本
                    });
                }

                // 绘图
                var plt = PiePlot.Plot;
                plt.Clear();
                var pie = plt.Add.Pie(slices);

                pie.ExplodeFraction = 0.05; // 分片分离一点点
                pie.SliceLabelDistance = 1.2; // 标签距离半径的倍率（根据需求调整）

                plt.ShowLegend();
                plt.Axes.Frameless();
                plt.HideGrid();
                plt.Axes.AutoScale();
                PiePlot.Plot.Font.Automatic();
                PiePlot.Refresh();
            }
            catch (Exception ex)
            {
                throw new Exception($"绘制饼图出现错误：{ex.Message}");
            }
        }
    }
}