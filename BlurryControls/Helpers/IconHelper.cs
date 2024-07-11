﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlurryControls.Internals;

namespace BlurryControls.Helpers
{
    internal static class IconHelper
    {
        #region Dialog

        //icon paths for BlurryDialogWindow presentation
        private const string RelativeQuestionIconPath = @"pack://application:,,,/BlurryControls;component/Resources/Icons/064/question.png";
        private const string RelativeInformationIconPath = @"pack://application:,,,/BlurryControls;component/Resources/Icons/064/information.png";
        private const string RelativeWarningIconPath = @"pack://application:,,,/BlurryControls;component/Resources/Icons/064/warning.png";
        private const string RelativeErrorIconPath = @"pack://application:,,,/BlurryControls;component/Resources/Icons/064/error.png";

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
                    return RelativeQuestionIconPath.GetImageSource();
                case BlurryDialogIcon.Information:
                    return RelativeInformationIconPath.GetImageSource();
                case BlurryDialogIcon.Warning:
                    return RelativeWarningIconPath.GetImageSource();
                case BlurryDialogIcon.Error:
                    return RelativeErrorIconPath.GetImageSource();
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon), icon, null);
            }
        }

        #endregion

        #region Cursor

        private const string GrabCursorPath = @"pack://application:,,,/BlurryControls;component/Resources/Cursors/grab.cur";
        private const string GrabbingCursorPath = @"pack://application:,,,/BlurryControls;component/Resources/Cursors/grabbing.cur";

        public static Cursor GetCursor(this DragCursor cursor)
        {
            switch (cursor)
            {
                case DragCursor.Grab:
                    return new Cursor(
                        Application.GetResourceStream(new Uri(GrabCursorPath))?.Stream ??
                        throw new InvalidOperationException()
                    );
                case DragCursor.Grabbing:
                    return new Cursor(
                        Application.GetResourceStream(new Uri(GrabbingCursorPath))?.Stream ??
                        throw new InvalidOperationException()
                    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null);
            }
        }

        #endregion

        /// <summary>
        /// returns a new instance of a <see cref="BitmapImage"/> with the given icon path as <see cref="string"/>
        /// </summary>
        /// <param name="packageSource">given icon path as <see cref="string"/></param>
        /// <returns>a new instance of a <see cref="BitmapImage"/> with the given icon path as <see cref="string"/></returns>
        private static BitmapImage GetImageSource(this string packageSource)
        {
            return new BitmapImage(new Uri(packageSource));
        }
    }
}