using System.Windows.Controls;

using SenTooliKit.ViewModels.LoginPages;

namespace SenTooliKit.Views.LoginPages
{
    public partial class LoginUserControl : UserControl
    {
        public LoginUserControl(LoginUserControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}