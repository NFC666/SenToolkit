using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;

namespace SenTooliKit.Manager
{
    public class WindowManager
    {
        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        private const int AW_BLEND = 0x00080000; // 淡入
        private const int AW_CENTER = 0x00000010; // 从中心展开
        
        private readonly Dictionary<Type, Window> _windows = new();



        // 打开带动画的无标题栏窗口
        public T ShowChromeWindow<T>() where T : Window
        {
            var type = typeof(T);

            if (_windows.TryGetValue(type, out var existing) && existing.IsVisible)
            {
                existing.Activate();
                return (T)existing;
            }

            var window = App.Service.GetRequiredService<T>();

            // 应用无标题栏设置
            ApplyWindowChromeSettings(window);
            

            window.Closed += (_, _) => _windows.Remove(type);
            _windows[type] = window;

            window.Show();
            return window;
        }

        private void ApplyWindowChromeSettings(Window window)
        {
            window.AllowsTransparency = true;
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.CanResizeWithGrip;
            window.Style = (Style)Application.Current.FindResource("MaterialDesignWindow");
        }

        /// <summary>
        /// 播放打开动画（这里用 WPF Storyboard）
        /// </summary>
        private void PlayOpenAnimation(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            AnimateWindow(handle, 300, AW_BLEND | AW_CENTER);
        }

        public void CloseWindow<T>() where T : Window
        {
            var type = typeof(T);
            if (_windows.TryGetValue(type, out var window))
            {
                window.Close();
                _windows.Remove(type);
            }
        }

        public bool IsWindowOpen<T>() where T : Window
        {
            return _windows.ContainsKey(typeof(T));
        }
    }
}
