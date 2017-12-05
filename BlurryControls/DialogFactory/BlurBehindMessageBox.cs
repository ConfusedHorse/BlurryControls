using System.Windows;
using BlurryControls.Controls;
using BlurryControls.Internals;

namespace BlurryControls.DialogFactory
{
    /// <summary>
    /// Provides a variety of overloads for displaying a <see cref="BlurBehindDialogWindow"/>
    /// to show a prompt or information which blurs the content of its owner while it's active 
    /// </summary>
    public static class BlurBehindMessageBox
    {
        private const double Strength = 0.5;
        private const int BlurDuration = 300;
        private const int UnblurDuration = 250;
        private const int BlurRadius = 15;

        #region Conventional Content Dialogs

        /// <summary>
        /// Displays a message box that has a message; and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string messageBoxText, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = string.Empty,
                DialogIcon = BlurryDialogIcon.None,
                DialogMessage = messageBoxText,
                Button = BlurryDialogButton.None,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string messageBoxText, string caption, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                DialogMessage = messageBoxText,
                Button = BlurryDialogButton.None,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string messageBoxText, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = string.Empty,
                DialogIcon = BlurryDialogIcon.None,
                DialogMessage = messageBoxText,
                Button = BlurryDialogButton.None,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption and button;
        /// and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="button">A <see cref="BlurryDialogButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string messageBoxText, string caption, BlurryDialogButton button, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                DialogMessage = messageBoxText,
                Button = button,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message, title bar caption; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string messageBoxText, string caption, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                DialogMessage = messageBoxText,
                Button = BlurryDialogButton.None,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon;
        /// and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="button">A <see cref="BlurryDialogButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="icon">A <see cref="BlurryDialogIcon"/> value that specifies the icon to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string messageBoxText, string caption, BlurryDialogButton button, BlurryDialogIcon icon, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = icon,
                DialogMessage = messageBoxText,
                Button = button,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message, title bar caption and button; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="button">A <see cref="BlurryDialogButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string messageBoxText, string caption, BlurryDialogButton button, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                DialogMessage = messageBoxText,
                Button = button,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message, title bar caption, button, and icon; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="button">A <see cref="BlurryDialogButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="icon">A <see cref="BlurryDialogIcon"/> value that specifies the icon to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string messageBoxText, string caption, BlurryDialogButton button, BlurryDialogIcon icon, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = icon,
                DialogMessage = messageBoxText,
                Button = button,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        #endregion

        #region Custom Content Dialogs

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="content">A <see cref="FrameworkElement"/> that specifies the content to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string caption, FrameworkElement content, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                CustomContent = content,
                Button = BlurryDialogButton.None,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="content">A <see cref="FrameworkElement"/> that specifies the content to display.</param>
        /// <param name="button">A <see cref="BlurryDialogButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string caption, FrameworkElement content, BlurryDialogButton button, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                CustomContent = content,
                Button = button,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="content">A <see cref="FrameworkElement"/> that specifies the content to display.</param>
        /// <param name="customDialogButtons"> A <see cref="ButtonCollection"/> shown instead of the conventional dialog buttons.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(string caption, FrameworkElement content, ButtonCollection customDialogButtons, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                CustomContent = content,
                CustomDialogButtons = customDialogButtons,
                Owner = Application.Current.MainWindow,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="content">A <see cref="FrameworkElement"/> that specifies the content to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string caption, FrameworkElement content, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                CustomContent = content,
                Button = BlurryDialogButton.None,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="content">A <see cref="FrameworkElement"/> that specifies the content to display.</param>
        /// <param name="button">A <see cref="BlurryDialogButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string caption, FrameworkElement content, BlurryDialogButton button, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                CustomContent = content,
                Button = button,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays
        /// a message; and returns a result.
        /// </summary>
        /// <param name="owner">A <see cref="Window"/> that represents the owner window of the message box.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="content">A <see cref="FrameworkElement"/> that specifies the content to display.</param>
        /// <param name="customDialogButtons"> A <see cref="ButtonCollection"/> shown instead of the conventional dialog buttons.</param>
        /// <param name="strength"> Determines the opacity of the window which is set to 0.75 by default and may not exceed 1</param>
        /// <returns>A <see cref="BlurryDialogResult"/> value that specifies which message box button is clicked by the user.</returns>
        public static BlurryDialogResult Show(Window owner, string caption, FrameworkElement content, ButtonCollection customDialogButtons, double strength = Strength)
        {
            var result = BlurryDialogResult.None;
            var dialog = new BlurBehindDialogWindow
            {
                Title = caption,
                DialogIcon = BlurryDialogIcon.None,
                CustomContent = content,
                CustomDialogButtons = customDialogButtons,
                Owner = owner,
                Strength = strength,
                BlurDuration = BlurDuration,
                UnblurDuration = UnblurDuration,
                BlurRadius = BlurRadius
            };

            dialog.ResultAquired += (sender, args) =>
            {
                dialog.Close();
                result = args.Result;
            };

            dialog.ShowDialog();

            return result;
        }

        #endregion
    }
}