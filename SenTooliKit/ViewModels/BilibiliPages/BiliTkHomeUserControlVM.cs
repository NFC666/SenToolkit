using System.Collections.ObjectModel;

using MaterialDesignThemes.Wpf;

using SenTooliKit.IServices.IServices;
using SenTooliKit.Manager;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BiliTkHomeUserControlVM : ViewModelBase
    {
        private readonly IBilibiliAuthService _bilibiliAuthService;
        private readonly WindowManager _windowManager;

        public ObservableCollection<ToolItemViewModel> Tools { get; set; }
            = new ObservableCollection<ToolItemViewModel>();

        public BiliTkHomeUserControlVM(WindowManager windowManager, IBilibiliAuthService bilibiliAuthService)
        {
            _windowManager = windowManager;
            ConfigTools();
        }

        private void ConfigTools()
        {
            Tools.Add(new ToolItemViewModel(_windowManager)
            {
                Title = "弹幕获取", Icon = PackIconKind.CommentEye, WindowKey = "BDmMainWindow"
            });
            Tools.Add(new ToolItemViewModel(_windowManager)
            {
                Title = "视频下载", Icon = PackIconKind.Video, WindowKey = "BVideoWindow"
            });
            Tools.Add(new ToolItemViewModel(_windowManager)
            {
                Title = "直播工具", Icon = PackIconKind.ViewStream, WindowKey = "BStreamWindow"
            });
            Tools.Add(new ToolItemViewModel(_windowManager)
            {
                Title = "评论工具", Icon = PackIconKind.Comments, WindowKey = "BCommentWindow"
            });
        }
    }
}