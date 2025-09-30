using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;

using SenTooliKit.Common.Helpers;
using SenTooliKit.Common.Models;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Views.BilibiliPages;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BCommentWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private UserControl _currentPage;
        [ObservableProperty]
        private List<CommentPage> _commentPages = new();
        [ObservableProperty]
        private CommentPage _selectPage;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectPage))
            {
                ConfirmPage(SelectPage.WindowKey);
            }
        }

        void ConfirmPage(string windowKey)
        {
            switch (windowKey)
            {
                case nameof(BCommentInfoUserControl):
                    CurrentPage = App.Service.GetRequiredService<BCommentInfoUserControl>();
                    break;
                case nameof(BCommentAnalysisUserControl):
                    CurrentPage = App.Service.GetRequiredService<BCommentAnalysisUserControl>();
                    break;
            }
        }


        public BCommentWindowViewModel()
        {
            ConfigPages();
        }

        void ConfigPages()
        {
            CommentPages = new List<CommentPage>
            {
                new CommentPage("评论信息", nameof(BCommentInfoUserControl)),
                new CommentPage("评论分析", nameof(BCommentAnalysisUserControl)),
            };
            
        }
        
        
        
        
    }

    public record CommentPage(string Title, string WindowKey);
}