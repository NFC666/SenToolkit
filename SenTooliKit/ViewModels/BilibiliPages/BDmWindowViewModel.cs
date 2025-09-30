using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SenTooliKit.Common.Models;
using SenTooliKit.Views.BilibiliPages;
using Microsoft.Extensions.DependencyInjection;

namespace SenTooliKit.ViewModels.BilibiliPages
{
    public partial class BDmWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private ObservableCollection<BulletPageItem>
            _bulletPages = new ObservableCollection<BulletPageItem>();
        [ObservableProperty]
        private UserControl _currentPage;
        [ObservableProperty]
        private BulletPageItem _selectedItem;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(SelectedItem))
            {
                OpenPage(SelectedItem.WindowKey);
            }
        }

        public BDmWindowViewModel()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            BulletPages.Add(new BulletPageItem
            {
                Title = "弹幕信息",
                WindowKey = "BDmInfoUserControl"
            });
            BulletPages.Add(new BulletPageItem
            {
                Title = "弹幕分析",
                WindowKey = "DmAnalysis"
            });
        }
        
        [RelayCommand]
        public void OpenPage(string windowKey)
        {
            switch (windowKey)
            {
                case "BDmInfoUserControl":
                    CurrentPage = App.Service.GetService<BDmInfoUserControl>();
                    break;
                case "DmAnalysis":
                    CurrentPage = App.Service.GetService<BDmAnalysisUserControl>();
                    break;
            }
        }
    }
}