using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Messaging;

using SenTooliKit.Common.Message;
using SenTooliKit.Manager;
using SenTooliKit.Views.LoginPages;

namespace SenTooliKit.Views.Components
{
    public partial class TitleBar : UserControl
    {
        private readonly WindowManager _windowManager = new WindowManager();

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(TitleBar),
                new PropertyMetadata(default(string)));

        public TitleBar()
        {
            InitializeComponent();
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }
        private void WindowRestore(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Normal)
                {
                    parentWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    parentWindow.WindowState = WindowState.Normal;
                }
            }
        }

        private void WindowMinimize(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized; // 最小化窗口
            }
        }
        
    }
}