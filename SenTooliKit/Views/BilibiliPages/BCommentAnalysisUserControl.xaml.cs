using System.Windows.Controls;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BCommentAnalysisUserControl : UserControl
    {
        public BCommentAnalysisUserControl(BCommentAnalysisUserControlVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}