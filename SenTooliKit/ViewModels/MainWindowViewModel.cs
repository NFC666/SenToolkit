using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using SenTooliKit.Common.Message;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Views.BilibiliPages;
using SenTooliKit.Views.LoginPages;

namespace SenTooliKit.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserControl _currentPage;

        [ObservableProperty]
        private List<string> _items;
        [ObservableProperty]
        private string _selectedItem;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedItem))
            {
                switch (SelectedItem)
                {
                    case "登录页面":
                        CurrentPage = App.Service.GetRequiredService<LoginUserControl>();
                        break;
                    case "B站工具箱":
                        CurrentPage = App.Service.GetRequiredService<BiliTkHomeUserControl>();
                        break;
                }
            }
        }


        public MainWindowViewModel()
        {
            Items = new List<string>()
            {
                "登录页面",
                "B站工具箱",
            };
            CurrentPage = App.Service.GetRequiredService<LoginUserControl>();
        }
    }
}