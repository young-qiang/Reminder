using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reminder
{
    public class TaskbarInfo
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        private const int ABM_GETTASKBARPOS = 5;

        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        public static Rect GetTaskbarPosition()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);

            SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);

            return new Rect(abd.rc.Left, abd.rc.Top, abd.rc.Right - abd.rc.Left, abd.rc.Bottom - abd.rc.Top);
        }
    }
}
