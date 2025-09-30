using System.Windows;
using System.Windows.Input;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BVideoWindow : Window
    {
        public BVideoWindow(BVideoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BVideoWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}