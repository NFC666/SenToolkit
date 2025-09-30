using System.Windows;
using System.Windows.Input;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BCommentWindow : Window
    {
        public BCommentWindow(BCommentWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BCommentWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}