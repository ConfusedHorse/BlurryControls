using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using BlurryControls.Helper;
using BlurryControls.Internals;

namespace BlurryControls.Control
{
    //the following definition represents an implementation which is similar to the native windows
    //MessageBox control. it contains a visual definition for a caption, an icon which is arranged 
    //in the content area, a prompt to the user and a dynamic set of buttons which provide options
    //for a user to answer the prompt

    /// <summary>
    /// Interaction logic for BlurryDialogWindow.xaml
    /// </summary>
    internal partial class BlurryDialogWindow : Window
    {
        #region events

        /// <summary>
        /// a delegate that is called when an action is performed by the user
        /// </summary>
        /// <param name="sender">the event raising control of type <see cref="BlurryDialogWindow"/></param>
        /// <param name="args">a preset argument list which is provided when an action is performed of type <see cref="BlurryDialogResultArgs"/></param>
        public delegate void BlurryDialogResultEventHandler(object sender, BlurryDialogResultArgs args);
        /// <summary>
        /// an event that is raised when an action is performed by the user
        /// </summary>
        public event BlurryDialogResultEventHandler ResultAquired;

        #endregion

        #region dependency properties

        /// <summary>
        /// the caption show in the MenuBar 
        /// </summary>
        public new string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
                TitleTextBlock.Text = value;
            }
        }

        public static readonly DependencyProperty DialogMessageProperty = DependencyProperty.Register(
            "DialogMessage", typeof(string), typeof(BlurryDialogWindow), new PropertyMetadata(default(string)));

        /// <summary>
        /// gets or sets the DialogMessageProperty
        /// an information or prompt that is shown to a user
        /// </summary>
        public string DialogMessage
        {
            get { return (string) GetValue(DialogMessageProperty); }
            set
            {
                SetValue(DialogMessageProperty, value);
                DialogMessageTextBlock.Text = value;
            }
        }

        /// <summary>
        /// an icon in the left area of the MenuBar which is currently not in use
        /// </summary>
        public new ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set
            {
                SetValue(IconProperty, value);
                //TitleImage.Source = value;
                ContentImage.Source = value;
            }
        }

        public static readonly DependencyProperty DialogIconProperty = DependencyProperty.Register(
            "DialogIcon", typeof(BlurryDialogIcon), typeof(BlurryDialogWindow), new PropertyMetadata(default(BlurryDialogIcon)));


        /// <summary>
        /// gets or sets the DialogIconProperty
        /// represents a chosen value of enum <see cref="BlurryDialogIcon"/>
        /// </summary>
        public BlurryDialogIcon DialogIcon
        {
            get { return (BlurryDialogIcon) GetValue(DialogIconProperty); }
            set
            {
                SetValue(DialogIconProperty, value);
                SetDialogIconSource(value);
            }
        }

        public static readonly DependencyProperty ButtonProperty = DependencyProperty.Register(
            "Button", typeof(BlurryDialogButton), typeof(BlurryDialogWindow), new PropertyMetadata(default(BlurryDialogButton)));

        /// <summary>
        /// gets or sets the ButtonProperty
        /// a preset composition chosen from enum <see cref="BlurryDialogButton"/>
        /// </summary>
        public BlurryDialogButton Button
        {
            get { return (BlurryDialogButton) GetValue(ButtonProperty); }
            set
            {
                SetValue(ButtonProperty, value);
                SetButtonSet(value);
            }
        }

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength", typeof(double), typeof(BlurryDialogWindow), new PropertyMetadata(0.75));

        /// <summary>
        /// gets or sets the StrengthProperty
        /// the strength property determines the opacity of the window controls
        /// it is set to 0.75 by default and may not exceed 1
        /// </summary>
        public double Strength
        {
            get { return (double)GetValue(StrengthProperty); }
            set
            {
                var correctValue = (value >= 1 ? 1 : value) <= 0 ? 0 : value;
                SetValue(StrengthProperty, correctValue);

                var backgroundColor = ((SolidColorBrush)Background).Color;
                backgroundColor.A = (byte)(Strength * 255);
                Background = new SolidColorBrush(backgroundColor);
            }
        }

        #endregion

        public BlurryDialogWindow()
        {
            InitializeComponent();
        }

        #region blurry internals

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //apply blurry filter to the window
            EnableBlur();
        }

        /// <summary>
        /// this method uses the SetWindowCompositionAttribute to apply an AeroGlass effect to the window
        /// </summary>
        internal void EnableBlur()
        {
            //this code is taken from a sample application provided by Rafael Rivera
            //see the full code sample here: (2015/07)
            // https://github.com/riverar/sample-win10-aeroglass (2016/08)
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
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

        #region basic functionality

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void MenuBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //single click on MenuBar enables drag move
            //this also enables native Windows10 gestures
            //such as Aero Snap and Aero Shake
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle,
                0xA1, (IntPtr)0x2, (IntPtr)0);
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            CancelButton_OnClick(sender, e);
        }

        #endregion

        #region dialog management

        /// <summary>
        /// sets the ImageSource of <see cref="Icon"/> to a value corresponding to a
        /// chosen value <see cref="DialogIcon"/> using <see cref="IconHelper.GetDialogImage"/>
        /// </summary>
        /// <param name="value">a chosen value of enum <see cref="BlurryDialogIcon"/></param>
        private void SetDialogIconSource(BlurryDialogIcon value)
        {
            Icon = value.GetDialogImage();
        }

        /// <summary>
        /// sets the visibility of the buttons in ButtonGrid by a preset combination
        /// provided by the <see cref="Button"/> property
        /// </summary>
        /// <param name="value">a chosen value of enum <see cref="BlurryDialogButton"/></param>
        private void SetButtonSet(BlurryDialogButton value)
        {
            switch (value)
            {
                case BlurryDialogButton.Ok:
                    OkButton.Visibility = Visibility.Visible;

                    YesButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case BlurryDialogButton.OkCancel:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;

                    YesButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    break;
                case BlurryDialogButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;

                    OkButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case BlurryDialogButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;

                    OkButton.Visibility = Visibility.Collapsed;
                    break;
                case BlurryDialogButton.None:
                    OkButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        /// <summary>
        /// raises an event of type <see cref="BlurryDialogResultEventHandler"/> when the YesButton was clicked
        /// </summary>
        /// <param name="sender">the event raising control of type <see cref="BlurryDialogWindow"/></param>
        /// <param name="e">a preset argument list which is provided when an action is performed of type <see cref="BlurryDialogResultArgs"/></param>
        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var resultArguments = new BlurryDialogResultArgs {Result = BlurryDialogResult.Yes};
            ResultAquired?.Invoke(this, resultArguments);
        }

        /// <summary>
        /// raises an event of type <see cref="BlurryDialogResultEventHandler"/> when the NoButton was clicked
        /// </summary>
        /// <param name="sender">the event raising control of type <see cref="BlurryDialogWindow"/></param>
        /// <param name="e">a preset argument list which is provided when an action is performed of type <see cref="BlurryDialogResultArgs"/></param>
        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var resultArguments = new BlurryDialogResultArgs { Result = BlurryDialogResult.No };
            ResultAquired?.Invoke(this, resultArguments);
        }


        /// <summary>
        /// raises an event of type <see cref="BlurryDialogResultEventHandler"/> when the OkButton was clicked
        /// </summary>
        /// <param name="sender">the event raising control of type <see cref="BlurryDialogWindow"/></param>
        /// <param name="e">a preset argument list which is provided when an action is performed of type <see cref="BlurryDialogResultArgs"/></param>
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var resultArguments = new BlurryDialogResultArgs { Result = BlurryDialogResult.Ok };
            ResultAquired?.Invoke(this, resultArguments);
        }


        /// <summary>
        /// raises an event of type <see cref="BlurryDialogResultEventHandler"/> when the CancelButton was clicked
        /// </summary>
        /// <param name="sender">the event raising control of type <see cref="BlurryDialogWindow"/></param>
        /// <param name="e">a preset argument list which is provided when an action is performed of type <see cref="BlurryDialogResultArgs"/></param>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            var resultArguments = new BlurryDialogResultArgs { Result = BlurryDialogResult.Cancel };
            ResultAquired?.Invoke(this, resultArguments);
        }

        #endregion
    }
}
