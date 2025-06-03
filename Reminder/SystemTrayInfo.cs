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

        /// <summary>
        /// 获取系统托盘的位置和大小
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 获取任务栏的坐标和大小
        /// </summary>
        /// <returns></returns>
        public static Rect GetTaskbarRect()
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle != IntPtr.Zero)
            {
                RECT rect;
                GetWindowRect(taskbarHandle, out rect);
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 获取时间区域的坐标
        /// </summary>
        /// <returns></returns>
        public static Point GetTimeAreaCoordinates()
        {
            Rect taskbarRect = GetTaskbarRect();
            if (taskbarRect != Rect.Empty)
            {
                // 假设时间区域位于右下角
                double timeAreaWidth = 100; // 时间区域的示例宽度
                double timeAreaHeight = taskbarRect.Height; // 任务栏的完整高度
                double x = taskbarRect.Right - timeAreaWidth;
                double y = taskbarRect.Bottom - timeAreaHeight;
                return new Point(x, y);
            }
            return new Point(0, 0);
        }
    }
}
