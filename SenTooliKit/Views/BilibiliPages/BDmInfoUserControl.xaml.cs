using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using QRCoder;

using SenTooliKit.ViewModels.BilibiliPages;

namespace SenTooliKit.Views.BilibiliPages
{
    public partial class BDmInfoUserControl : UserControl
    {
        public BDmInfoUserControl(BDmInfoUserControlVM userControlVm)
        {
            InitializeComponent();
            DataContext = userControlVm;
        }

    }
}