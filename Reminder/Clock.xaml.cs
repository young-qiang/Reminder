using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Reminder
{
    /// <summary>
    /// Clock.xaml 的交互逻辑
    /// </summary>
    public partial class Clock
    {
         
        public Clock()
        {
            InitializeComponent();
            this.Topmost = true;


            // 低频率检查焦点变化
            focusCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            focusCheckTimer.Tick += (s, e) => EnsureAlwaysOnTop();
            focusCheckTimer.Start();

            Task.Factory.StartNew(async () =>
           {
               while (true   )
               {
                   if (!MainWindow.isRunning)
                   {
                       continue;
                   }
                   // do something

                   await Task.Delay(500);
                   UpdateTime();
               }
           }, TaskCreationOptions.LongRunning);




        }

        private void UpdateTime()
        {
            TimeSpan remainingTime = MainWindow.nextReminder - DateTime.Now;
            // 将事件格式化为 00:00 分钟:秒钟，然后赋值到 countdown 的 TextBlock
            countdown.Dispatcher.Invoke(new Action(() =>
            {
                countdown.Text = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            }));

        }

        private void SetLocation()
        {
            // 获取系统托盘信息
            var infos = SystemTrayInfo.GetSystemTrayPosition();

            this.Left = infos.X - this.Width - 20;
            this.Top = infos.Y + 10;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetLocation();


            WindowInteropHelper helper = new WindowInteropHelper(this);
            windowHandle = helper.Handle;

            hwndSource = HwndSource.FromHwnd(windowHandle);
            hwndSource?.AddHook(WndProc);

            // 初始设置
            SetAlwaysOnTop();

            // 注册系统级事件
            RegisterForSystemEvents();
        }

        private IntPtr windowHandle;
        private HwndSource hwndSource;
        private readonly DispatcherTimer focusCheckTimer;

        private void RegisterForSystemEvents()
        {
            // 注册系统事件，如任务栏变化、显示设置变化等
            SystemEvents.DisplaySettingsChanged += (s, e) => SetAlwaysOnTop();
            SystemEvents.SessionSwitch += (s, e) => SetAlwaysOnTop();

            // 窗口事件
            this.Activated += (s, e) => SetAlwaysOnTop();
            this.Deactivated += (s, e) =>
            {
                // 在UI线程延迟执行，确保在失去焦点后仍然置顶
                Dispatcher.BeginInvoke(new Action(SetAlwaysOnTop), DispatcherPriority.ApplicationIdle);
            };
        }

        private void SetAlwaysOnTop()
        {
            if (windowHandle == IntPtr.Zero) return;

            // 修改窗口样式
            int exStyle = GetWindowLong(windowHandle, GWL_EXSTYLE);
            SetWindowLong(windowHandle, GWL_EXSTYLE, exStyle | WS_EX_TOPMOST);

            // 设置窗口位置
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);

            // 确保窗口是可见的
            ShowWindow(windowHandle, SW_SHOW);
        }

        private void EnsureAlwaysOnTop()
        {
            // 检查当前窗口是否真的在最前
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow != windowHandle && !IsWindowVisible(windowHandle))
            {
                SetAlwaysOnTop();
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_WINDOWPOSCHANGING:
                case WM_ACTIVATE:
                case WM_ACTIVATEAPP:
                case WM_NCACTIVATE:
                case WM_SHOWWINDOW:
                    SetAlwaysOnTop();
                    break;

                case WM_SYSCOMMAND:
                    // 防止最小化和隐藏命令
                    uint command = (uint)(wParam.ToInt64() & 0xFFF0);
                    if (command == SC_MINIMIZE)
                    {
                        handled = true;
                        return IntPtr.Zero;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        // Win32 常量
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const int WM_ACTIVATE = 0x0006;
        private const int WM_ACTIVATEAPP = 0x001C;
        private const int WM_NCACTIVATE = 0x0086;
        private const int WM_SHOWWINDOW = 0x0018;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;
        private const int SW_SHOW = 5;

        // Win32 API
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);
    }
}
