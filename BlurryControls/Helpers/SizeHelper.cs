using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace BlurryControls.Helpers
{
    public static class SizeHelper
    {
        /* the following code is taken from 
            https://codekong.wordpress.com/2010/11/10/custom-window-style-and-accounting-for-the-taskbar/
        */

        public enum AbEdge
        {
            AbeLeft = 0,
            AbeTop = 1,
            AbeRight = 2,
            AbeBottom = 3
        }

        public enum AbMsg
        {
            AbmNew = 0,
            AbmRemove = 1,
            AbmQuerypos = 2,
            AbmSetpos = 3,
            AbmGetstate = 4,
            AbmGettaskbarpos = 5,
            AbmActivate = 6,
            AbmGetautohidebar = 7,
            AbmSetautohidebar = 8,
            AbmWindowposchanged = 9,
            AbmSetstate = 10
        }

        private const int MonitorDefaulttOnNearest = 0x00000002;

        private static Minmaxinfo AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, Minmaxinfo mmi)
        {
            var hwnd = FindWindow("Shell_TrayWnd", null);
            var monitorWithTaskbarOnIt = MonitorFromWindow(hwnd, MonitorDefaulttOnNearest);
            if (!monitorContainingApplication.Equals(monitorWithTaskbarOnIt)) return mmi;
            var abd = new Appbardata();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            SHAppBarMessage((int) AbMsg.AbmGettaskbarpos, ref abd);
            var uEdge = GetEdge(abd.rc);
            var autoHide = Convert.ToBoolean(SHAppBarMessage((int) AbMsg.AbmGetstate, ref abd));

            if (!autoHide) return mmi;

            switch (uEdge)
            {
                case (int) AbEdge.AbeLeft:
                    mmi.ptMaxPosition.x += 2;
                    mmi.ptMaxTrackSize.x -= 2;
                    mmi.ptMaxSize.x -= 2;
                    break;
                case (int) AbEdge.AbeRight:
                    mmi.ptMaxSize.x -= 2;
                    mmi.ptMaxTrackSize.x -= 2;
                    break;
                case (int) AbEdge.AbeTop:
                    mmi.ptMaxPosition.y += 2;
                    mmi.ptMaxTrackSize.y -= 2;
                    mmi.ptMaxSize.y -= 2;
                    break;
                case (int) AbEdge.AbeBottom:
                    mmi.ptMaxSize.y -= 2;
                    mmi.ptMaxTrackSize.y -= 2;
                    break;
                default:
                    return mmi;
            }
            return mmi;
        }

        private static int GetEdge(Rect rc)
        {
            var uEdge = -1;
            if (rc.top == rc.left && rc.bottom > rc.right)
                uEdge = (int) AbEdge.AbeLeft;
            else if (rc.top == rc.left && rc.bottom < rc.right)
                uEdge = (int) AbEdge.AbeTop;
            else if (rc.top > rc.left)
                uEdge = (int) AbEdge.AbeBottom;
            else
                uEdge = (int) AbEdge.AbeRight;
            return uEdge;
        }

        public static void WindowInitialized(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (IntPtr) 0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (Minmaxinfo) Marshal.PtrToStructure(lParam, typeof(Minmaxinfo));
            var monitorContainingApplication = MonitorFromWindow(hwnd, MonitorDefaulttOnNearest);

            if (monitorContainingApplication != IntPtr.Zero)
            {
                var monitorInfo = new Monitorinfo();
                GetMonitorInfo(monitorContainingApplication, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                //mmi.ptMaxTrackSize.x = mmi.ptMaxSize.x;
                //mmi.ptMaxTrackSize.y = mmi.ptMaxSize.y;
                mmi.ptMinTrackSize.x = 20;
                mmi.ptMinTrackSize.y = 20;
                mmi = AdjustWorkingAreaForAutoHide(monitorContainingApplication, mmi);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Appbardata
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public Rect rc;
            public bool lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Minmaxinfo
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class Monitorinfo
        {
            public int cbSize = Marshal.SizeOf(typeof(Monitorinfo));
            public Rect rcMonitor = new Rect();
            public Rect rcWork = new Rect();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #region DLLImports

        [DllImport("shell32", CallingConvention = CallingConvention.StdCall)]
        public static extern int SHAppBarMessage(int dwMessage, ref Appbardata pData);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, Monitorinfo lpmi);

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion
    }
}