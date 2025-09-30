using System.Windows.Controls;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BDmAnalysisUserControl : UserControl
    {
        public BDmAnalysisUserControl(BDmAnalysisUserControlVM userControlVm)
        {
            InitializeComponent();
            DataContext = userControlVm;
        }
    }
}