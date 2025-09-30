using System.Windows.Controls;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BCommentInfoUserControl : UserControl
    {
        public BCommentInfoUserControl(BCommentInfoUserControlVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}