using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using BlurryControls.Internals;

namespace BlurryControls.Helpers
{
    internal static class BlurryHelper
    {
        #region Window

        #region Public Methods

        public static void Blur(this Window win)
        {
            EnableBlur(win, true);
        }

        public static void UnBlur(this Window win)
        {
            EnableBlur(win, false);
        }

        #endregion

        #region Private Methods

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

        #endregion

        #endregion

        #region Image

        #region Public Methods

        /// <summary>
        /// Blurs a picture by a certain amount
        /// </summary>
        /// <param name="image">image to blur</param>
        /// <param name="blurSize">impact of the blur</param>
        /// <returns>a picture blured by a certain amount</returns>
        public static Image Blur(this Image image, int blurSize = 5)
        {
            return image.ToBitmap().Blur(image.ToRectangle(), blurSize);
        }

        /// <summary>
        /// Blurs a picture by a certain amount
        /// </summary>
        /// <param name="image">image to blur</param>
        /// <param name="blurSize">impact of the blur</param>
        /// <returns>a picture blured by a certain amount</returns>
        public static Bitmap Blur(this Bitmap image, int blurSize = 5)
        {
            return image.Blur(image.ToRectangle(), blurSize);
        }

        /// <summary>
        /// Blurs a picture by a certain amount in an area determined by a rectangle
        /// </summary>
        /// <param name="image">image to blur</param>
        /// <param name="rectangle">area in the picture</param>
        /// <param name="blurSize">impact of the blur</param>
        /// <returns>a picture blured by a certain amount in an area determined by a rectangle</returns>
        public static Image Blur(this Image image, Rectangle rectangle, int blurSize = 5)
        {
            return image.ToBitmap().Blur(rectangle, blurSize);
        }

        /// <summary>
        /// Blurs a picture by a certain amount in an area determined by a rectangle
        /// </summary>
        /// <param name="image">image to blur</param>
        /// <param name="rectangle">area in the picture</param>
        /// <param name="blurSize">impact of the blur</param>
        /// <returns>a picture blured by a certain amount in an area determined by a rectangle</returns>
        //based on http://notes.ericwillis.com/2009/10/blur-an-image-with-csharp/ (2016/08)
        public static Bitmap Blur(this Bitmap image, Rectangle rectangle, int blurSize = 5)
        {
            var blurred = new Bitmap(image.Width, image.Height);

            using (var graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            for (var xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (var yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    var blurPixelCount = 0;

                    for (var x = xx; x < xx + blurSize && x < image.Width; x++)
                    {
                        for (var y = yy; y < yy + blurSize && y < image.Height; y++)
                        {
                            var pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    for (var x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    for (var y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// converts an <see cref="Image"/> to a <see cref="Bitmap"/>
        /// </summary>
        /// <param name="image">image to be converted</param>
        /// <returns>bitmap with the image data</returns>
        private static Bitmap ToBitmap(this Image image)
        {
            return new Bitmap(image);
        }

        /// <summary>
        /// returns a <see cref="Rectangle"/> with dimensions of a given <see cref="Image"/>
        /// </summary>
        /// <param name="image">image with given dimensions</param>
        /// <returns>rectangle with same dimensions as a given image</returns>
        private static Rectangle ToRectangle(this Image image)
        {
            return new Rectangle(0, 0, image.Width, image.Height);
        }

        /// <summary>
        /// returns a <see cref="Rectangle"/> with dimensions of a given <see cref="Bitmap"/>
        /// </summary>
        /// <param name="image">image with given dimensions</param>
        /// <returns>rectangle with same dimensions as a given image</returns>
        private static Rectangle ToRectangle(this Bitmap image)
        {
            return new Rectangle(0, 0, image.Width, image.Height);
        }

        #endregion

        #endregion
    }
}