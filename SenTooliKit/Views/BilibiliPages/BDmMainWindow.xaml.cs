using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BDmMainWindow : Window
    {
        public BDmMainWindow(BDmWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BulletMainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}