using System.Windows;
using System.Windows.Input;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BStreamWindow : Window
    {
        public BStreamWindow(BStreamWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BStreamWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}