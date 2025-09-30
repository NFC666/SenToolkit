using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using SenTooliKit.Common.Message;
using SenTooliKit.ViewModels;

namespace SenTooliKit.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BackgroundToggle_OnClick(object sender, RoutedEventArgs e)
        {
            DrawerHost.IsLeftDrawerOpen = !DrawerHost.IsLeftDrawerOpen;
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowRestore(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                
                WindowState = WindowState.Normal;
            }
        }

        private void WindowMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}