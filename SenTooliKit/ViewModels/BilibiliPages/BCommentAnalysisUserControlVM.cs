using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ScottPlot;
using ScottPlot.WPF;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BCommentAnalysisUserControlVM : ViewModelBase
    {
        private readonly IBiliCommentsService _biliCommentsService = new BiliCommentsService();
        private static List<Reply> _replies = new();

        [ObservableProperty] private WpfPlot _barPlot;
        [ObservableProperty] private WpfPlot _piePlot = new();
        [ObservableProperty] private string _url = "";
        [ObservableProperty] private bool _isLoading;

        [RelayCommand]
        public async Task GetCommentDetailInfo()
        {
            try
            {
                SetLoadingStatus();
                _replies = await GetCommentsToAnalysisAsync(Url);

                if (_replies.Count > 0)
                {
                    DrawPiePloy();
                }
                else
                {
                    MessageBox.Show("未获取到评论数据。");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}");
            }
            finally
            {
                ResetLoadingStatus();
            }
        }

        private async Task<List<Reply>> GetCommentsToAnalysisAsync(string url)
        {
            var res = new List<Reply>();
            try
            {
                string bv = BvHelper.GetBvFromUrl(url);
                var allComments = await _biliCommentsService.GetAllCommentsByReplyAsync(bv);

                foreach (Reply c in allComments)
                {
                    if (c.Replies != null && c.Replies.Count > 0)
                    {
                        res.AddRange(c.Replies);
                    }

                    res.Add(c);
                }

                return res;
            }
            catch (Exception e)
            {
                throw new Exception($"获取评论出现问题：{e.Message}");
            }
        }

        private void DrawPiePloy()
        {
            if (_replies.Count == 0) return;

            try
            {
                // 1. 分词并统计
                var allWords = new List<string>();
                foreach (var r in _replies)
                {
                    if (!string.IsNullOrWhiteSpace(r.Content?.Message))
                        allWords.AddRange(JiebaHelper.CutFiltered(r.Content.Message));
                }

                var wordCounts = allWords
                    .GroupBy(w => w)
                    .Select(g => new { Word = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10) // 取前10个高频词
                    .ToList();

                if (wordCounts.Count == 0) return;

                double total = wordCounts.Sum(x => x.Count);

                // 2. 配色方案
                var palette = new[]
                {
                    ScottPlot.Colors.Red, ScottPlot.Colors.Orange, ScottPlot.Colors.Gold, ScottPlot.Colors.Green,
                    ScottPlot.Colors.Blue, ScottPlot.Colors.Purple, ScottPlot.Colors.Cyan, ScottPlot.Colors.Brown,
                    ScottPlot.Colors.Magenta, ScottPlot.Colors.Lime
                };

                // 3. 构造扇区数据
                var slices = new List<PieSlice>();
                for (int i = 0; i < wordCounts.Count; i++)
                {
                    var wc = wordCounts[i];
                    var pct = wc.Count / total;
                    var label = $"{wc.Word} ({wc.Count}, {pct:P0})"; // e.g. "你好 (12, 24%)"
                    var color = palette[i % palette.Length];

                    slices.Add(new PieSlice()
                    {
                        Value = wc.Count,
                        FillColor = color,
                        Label = label, // 扇区内部标签
                        LegendText = wc.Word // 图例
                    });
                }

                // 4. 绘图
                var plt = PiePlot.Plot;
                plt.Clear();

                var pie = plt.Add.Pie(slices);
                pie.ExplodeFraction = 0.05; // 分离效果
                pie.SliceLabelDistance = 1.2; // 标签半径倍率（越大越往外）

                plt.Title("评论词频饼图");
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
        
        private void SetLoadingStatus()
        { 
            IsLoading = true;
        }
        
        private void ResetLoadingStatus()
        {
            IsLoading = false;
        }
    }
}