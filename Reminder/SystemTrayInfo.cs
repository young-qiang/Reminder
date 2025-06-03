using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reminder
{
    public class SystemTrayInfo
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Rect GetSystemTrayPosition()
        {
            // 找到任务栏窗口
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle == IntPtr.Zero) return Rect.Empty;

            // 找到通知区域窗口
            IntPtr trayHandle = FindWindowEx(taskbarHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            if (trayHandle == IntPtr.Zero) return Rect.Empty;

            // 获取通知区域窗口的位置
            RECT rect;
            if (GetWindowRect(trayHandle, out rect))
            {
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            return Rect.Empty;
        }
    }
}
