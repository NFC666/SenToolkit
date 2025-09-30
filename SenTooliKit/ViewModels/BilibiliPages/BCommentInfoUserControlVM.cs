using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Message;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BCommentInfoUserControlVM : ViewModelBase
    {
        [ObservableProperty] private string _url = "";

        [ObservableProperty] private bool _isLoading;

        [ObservableProperty] private Reply _selecReply;
        [ObservableProperty] private ObservableCollection<Reply> _replies = new();
        

        [ObservableProperty] private int _replyNum;

        private readonly IBiliCommentsService _bilibiliCommentService;

        public BCommentInfoUserControlVM(IBiliCommentsService bilibiliCommentService)
        {
            _bilibiliCommentService = bilibiliCommentService;
        }

        [RelayCommand]
        private async Task GetComments()
        {
            try
            {
                StartLoading();
                var bvid = GetBvid(Url);
                Replies = new ObservableCollection<Reply>(await GetCommentsAsync(bvid));
                ReplyNum = Replies.Count;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}");
            }
            finally
            {
                StopLoading();
            }
        }

        private async Task<List<Reply>> GetCommentsAsync(string bvid)
        {
            try
            {
                return await _bilibiliCommentService.GetAllCommentsByReplyAsync(bvid);
            }
            catch (ExternalException e)
            {
                throw new Exception($"获取弹幕失败，请检查网络连接。\n详情：{e.Message}");
            }
        }

        private string GetBvid(string url)
        {
            try
            {
                return BvHelper.GetBvFromUrl(url);
            }
            catch (Exception e)
            {
                throw new Exception($"视频链接识别失败，请检查链接是否正确。\n详情：{e.Message}");
            }
        }

        private void StartLoading()
        {
            IsLoading = true;
        }

        private void StopLoading()
        {
            IsLoading = false;
        }
    }
}