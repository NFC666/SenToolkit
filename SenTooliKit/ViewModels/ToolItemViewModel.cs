using System.Windows;

using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using SenTooliKit.Manager;
using SenTooliKit.Views.BilibiliPages;

namespace SenTooliKit.ViewModels
{
    public partial class ToolItemViewModel : ViewModelBase
    {
        private readonly WindowManager _windowManager;
        public ToolItemViewModel(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        
        public PackIconKind Icon { get; set; }
        public string Title { get; set; }
        
        public string WindowKey { get; set; }
        
        [RelayCommand]
        private void OpenWindow()
        {
            switch (WindowKey)
            {
                case "BDmMainWindow":
                    _windowManager.ShowChromeWindow<BDmMainWindow>();
                    break;
                case "BVideoWindow":
                    _windowManager.ShowChromeWindow<BVideoWindow>();
                    break;
                case "BStreamWindow":
                    _windowManager.ShowChromeWindow<BStreamWindow>();
                    break;
                case "BCommentWindow":
                    _windowManager.ShowChromeWindow<BCommentWindow>();
                    break;
            }
            
        }
    }
}