using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlurryControls.Internals;

namespace BlurryControls.Helpers
{
    internal static class IconHelper
    {
        #region Fields

        private const string PackagePrefix = @"pack://application:,,,/BlurryControls;component/Resources/Icons/";

        #endregion

        #region Dialog

        private const string Dialog = @"064/";

        //icon paths for BlurryDialogWindow presentation
        private const string RelativeQuestionIcon = @"question.png";
        private const string RelativeInformationIcon = @"information.png";
        private const string RelativeWarningIcon = @"warning.png";
        private const string RelativeErrorIcon = @"error.png";

        /// <summary>
        /// returns a <see cref="BitmapImage"/> which is provided by a viable input <see cref="BlurryDialogIcon"/> value
        /// </summary>
        /// <param name="icon">the demanded icon identifier of enum type <see cref="BlurryDialogIcon"/></param>
        /// <returns>
        ///     <see cref="BitmapImage"/> corresponding to an input identifier of type <see cref="BlurryDialogIcon"/>
        ///     returns null if no icon is demanded
        /// </returns>
        internal static BitmapImage GetDialogImage(this BlurryDialogIcon icon)
        {
            switch (icon)
            {
                case BlurryDialogIcon.None:
                    return null;
                case BlurryDialogIcon.Question:
                    return RelativeQuestionIcon.GetImageSource(Dialog);
                case BlurryDialogIcon.Information:
                    return RelativeInformationIcon.GetImageSource(Dialog);
                case BlurryDialogIcon.Warning:
                    return RelativeWarningIcon.GetImageSource(Dialog);
                case BlurryDialogIcon.Error:
                    return RelativeErrorIcon.GetImageSource(Dialog);
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon), icon, null);
            }
        }

        #endregion

        #region Cursor

        private const string Cursors = @"Cursors/";

        private const string GrabCursor = @"grab.cur";
        private const string GrabbingCursor = @"grabbing.cur";

        public static Cursor GetCursor(this DragCursor cursor)
        {
            switch (cursor)
            {
                case DragCursor.Grab:
                    return new Cursor(
                        Application.GetResourceStream(GrabCursor.GetImagePath(Cursors).ToUri())?.Stream ??
                        throw new InvalidOperationException()
                    );
                case DragCursor.Grabbing:
                    return new Cursor(
                        Application.GetResourceStream(GrabbingCursor.GetImagePath(Cursors).ToUri())?.Stream ??
                        throw new InvalidOperationException()
                    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null);
            }
        }

        #endregion

        #region Color Matrix

        private const string Color = @"Colors/";

        public static BitmapImage ReferenceColors => "rgbMatrixActualColors.png".GetImageSource(Color);

        #endregion

        #region Private Methods

        /// <summary>
        /// returns a new instance of a <see cref="BitmapImage"/> with the given icon path as <see cref="string"/>
        /// </summary>
        /// <param name="identifier">given image path as <see cref="string"/></param>
        /// <param name="directory">name of the directory in the package"/></param>
        /// <param name="packagePrefix">path to the package the image is found in</param>
        /// <returns></returns>
        private static BitmapImage GetImageSource(this string identifier, string directory, string packagePrefix = PackagePrefix)
        {
            return GetImagePath(identifier, directory, packagePrefix).ToUri().ToBitmap();
        }

        private static string GetImagePath(this string identifier, string directory, string packagePrefix = PackagePrefix)
        {
            return $"{packagePrefix}{directory}{identifier}";
        }

        private static BitmapImage ToBitmap(this Uri uri)
        {
            return new BitmapImage(uri);
        }

        private static Uri ToUri(this string uri)
        {
            return new Uri(uri);
        }

        #endregion
    }
}