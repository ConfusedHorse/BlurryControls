using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using BlurryControls.Internals;

namespace BlurryControls.Helper
{
    internal static class GlassingExtension
    {
        public static void Blur(this Window win)
        {
            EnableBlur(win, true);
        }

        public static void UnBlur(this Window win)
        {
            EnableBlur(win, false);
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [DllImport("DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        /// <summary>
        /// this method uses the SetWindowCompositionAttribute to apply an AeroGlass effect to the window
        /// </summary>
        private static void EnableBlur(Window win, bool enable)
        {
            //this code is taken from a sample application provided by Rafael Rivera
            //see the full code sample here: (2016/08)
            // https://github.com/riverar/sample-win10-aeroglass
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
    }
}