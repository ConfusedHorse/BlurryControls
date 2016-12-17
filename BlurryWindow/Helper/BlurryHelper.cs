using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using BlurryControls.Internals;
//using BlurryControls.Properties;
//using OperatingSystem = BlurryControls.Internals.OperatingSystem;

namespace BlurryControls.Helper
{
    internal static class GlassingExtension
    {
        public static void Blur(this Window win)
        {
            EnableBlur10(win, true);

            //switch (VersionHelper.OperatingSystem)
            //{
            //    case OperatingSystem.Windows78:
            //        EnableBlur78(win, true);
            //        break;
            //    case OperatingSystem.Windows10:
            //        EnableBlur10(win, true);
            //        break;
            //    case OperatingSystem.NotSupported:
            //        throw new NotSupportedException(Resources.VersionNotSupported);
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }
        public static void UnBlur(this Window win)
        {
            EnableBlur10(win, false);

            //switch (VersionHelper.OperatingSystem)
            //{
            //    case OperatingSystem.Windows78:
            //        EnableBlur78(win, false);
            //        break;
            //    case OperatingSystem.Windows10:
            //        EnableBlur10(win, false);
            //        break;
            //    case OperatingSystem.NotSupported:
            //        throw new NotSupportedException(Resources.VersionNotSupported);
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }

        #region Windows10

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [DllImport("DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        /// <summary>
        /// this method uses the SetWindowCompositionAttribute to apply an AeroGlass effect to the window
        /// </summary>
        private static void EnableBlur10(Window win, bool enable)
        {
            //this code is taken from a sample application provided by Rafael Rivera
            //see the full code sample here: (2016/07)
            // https://github.com/riverar/sample-win10-aeroglass (2016/08)
            var windowHelper = new WindowInteropHelper(win);

            var accent = new AccentPolicy
            {
                AccentState = enable ? AccentState.ACCENT_ENABLE_BLURBEHIND : AccentState.ACCENT_DISABLED
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        #endregion

        #region Windows78

        /// <summary>
        /// this method uses DwmEnableBlurBehindWindow to apply an AeroGlass effect to the window
        /// </summary>
        [Obsolete("not working without a window style and as such redundant")]
        private static void EnableBlur78(Window win, bool enable)
        {
            //this code is taken from a sample provided by MSDN
            //see https://msdn.microsoft.com/en-us/library/ms748975.aspx (2016/10)
            var mainWindowPtr = new WindowInteropHelper(win).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            if (mainWindowSrc == null) return;
            if (mainWindowSrc.CompositionTarget != null)
                mainWindowSrc.CompositionTarget.BackgroundColor = ((SolidColorBrush)win.Background).Color;
            var margins = new MARGINS
            {
                // If any of the margins is "-1" the whole window is glass!
                cxLeftWidth = enable ? -1 : 0,
                cxRightWidth = 0,
                cyBottomHeight = 0,
                cyTopHeight = 0
            };

            var hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
            if (hr < 0) { /* DwmExtendFrameIntoClientArea Failed */ }
        }

        #endregion
    }
}