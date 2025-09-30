using System.Windows.Controls;

using SenTooliKit.ViewModels;
using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BiliTkHomeUserControl : UserControl
    {
        public BiliTkHomeUserControl(BiliTkHomeUserControlVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

        }

    }
}