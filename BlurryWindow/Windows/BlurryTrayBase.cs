using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using BlurryControls.Helper;
using BlurryControls.Internals;

namespace BlurryControls.Windows
{
    // the BlurryTraybase can be used for both representing system global information
    // in the form of an overlay in the bottom right corner of the work space or
    // as a temporary control which implements functionality and is called by a tray icon
    // 
    // for information purposes a Duration may be set
    // or as a tray control Duration be set to 0 which disables automatic closing

    /// <summary>
    /// contains logic for a custom tray base control
    /// Additional properties are:
    /// <para><see cref="Strength"/></para>
    /// <para><see cref="DeactivatesOnLostFocus"/></para>
    /// <para><see cref="Duration"/></para>
    /// </summary>
    public class BlurryTrayBase : Window
    {
        #region fields

        private bool _canClose = false;
        private readonly DispatcherTimer _durationDispatcherTimer = new DispatcherTimer();

        #endregion

        #region dependency properties

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength", typeof(double), typeof(BlurryTrayBase), new PropertyMetadata(0.75));

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

        public static readonly DependencyProperty DeactivatesOnLostFocusProperty = DependencyProperty.Register(
            "DeactivatesOnLostFocus", typeof(bool), typeof(BlurryTrayBase), new PropertyMetadata(true));

        /// <summary>
        /// gets or sets the DeactivatesOnLostFocusProperty
        /// the the window will be closed when a click outside the window is performed
        /// </summary>
        public bool DeactivatesOnLostFocus
        {
            get { return (bool)GetValue(DeactivatesOnLostFocusProperty); }
            set { SetValue(DeactivatesOnLostFocusProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration", typeof(uint), typeof(BlurryTrayBase), new PropertyMetadata(default(uint)));

        /// <summary>
        /// gets or sets the DurationProperty
        /// the the window will be closed after a certain amount of time
        /// </summary>
        public uint Duration
        {
            get { return (uint) GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty ActivationDurationProperty = DependencyProperty.Register(
            "ActivationDuration", typeof(int), typeof(BlurryTrayBase), new PropertyMetadata(2000));

        /// <summary>
        /// Milliseconds the activation animation takes
        /// </summary>
        public int ActivationDuration
        {
            get { return (int) GetValue(ActivationDurationProperty); }
            set { SetValue(ActivationDurationProperty, value); }
        }

        public static readonly DependencyProperty DeactivationDurationProperty = DependencyProperty.Register(
            "DeactivationDuration", typeof(int), typeof(BlurryTrayBase), new PropertyMetadata(2000));

        /// <summary>
        /// Milliseconds the deactivation animation takes
        /// </summary>
        public int DeactivationDuration
        {
            get { return (int) GetValue(DeactivationDurationProperty); }
            set { SetValue(DeactivationDurationProperty, value); }
        }

        #endregion

        #region constructor

        static BlurryTrayBase()
        {
            //ensure loading template of BlurryWindowBase defined in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurryTrayBase),
                new FrameworkPropertyMetadata(typeof(BlurryTrayBase)));
        }

        public BlurryTrayBase()
        {
            WindowStartupLocation = WindowStartupLocation.Manual;

            Loaded += Window_Loaded;
            Loaded += InitializeDuration;
            Loaded += InitializeTraySpecificVisualBehaviour;
            Closing += TerminateTraySpecificVisualBehaviour;
            // close window when a click is performed outside the window
            Deactivated += delegate { if (!_canClose && DeactivatesOnLostFocus) Close(); };
        }

        // the Close event is raised after a certain amount of time
        private void InitializeDuration(object sender, RoutedEventArgs e)
        {
            if (Duration == 0) return;
            _durationDispatcherTimer.Interval = new TimeSpan(0, 0, 0, (int)Duration);
            _durationDispatcherTimer.Tick += delegate { Close(); };
            _durationDispatcherTimer.Start();
        }

        #endregion

        #region visual behaviour

        // initial animation
        // performs a calming animation when loading the window
        // the window flows from right to left to its terminal position and gains opacity
        private void InitializeTraySpecificVisualBehaviour(object sender, RoutedEventArgs args)
        {
            SystemParameters.StaticPropertyChanged += SystemParametersOnStaticPropertyChanged;
            Background = ColorHelper.TransparentSystemWindowGlassBrush(Strength);

            MaxWidth = SystemParameters.WorkArea.Width;
            MaxHeight = SystemParameters.WorkArea.Height;

            var top = MaxHeight - ActualHeight;
            var left = MaxWidth - ActualWidth;

            Top = top;

            var easeAnimation = new DoubleAnimation
            {
                From = MaxWidth,
                To = left,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, ActivationDuration)),
                EasingFunction = new ExponentialEase { Exponent = 15 }
            };

            var opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, ActivationDuration)),
                EasingFunction = new ExponentialEase { Exponent = 15 }
            };

            BeginAnimation(LeftProperty, easeAnimation);
            BeginAnimation(OpacityProperty, opacityAnimation);
        }

        // apply system color when changed
        private void SystemParametersOnStaticPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Background = ColorHelper.TransparentSystemWindowGlassBrush(Strength);
        }

        // termination animation
        // performs a calming animation when termination the window
        // the window flows from left to right from its starting position and loses opacity
        // the blur filter is deactivate while this animation is performed
        private void TerminateTraySpecificVisualBehaviour(object sender, CancelEventArgs args)
        {
            // force performing a vanishing animation before the window gets terminated
            if (_canClose) return;
            args.Cancel = true;

            var left = MaxWidth - ActualWidth;

            var easeAnimation = new DoubleAnimation
            {
                From = left,
                To = MaxWidth,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, DeactivationDuration)),
                EasingFunction = new ExponentialEase { Exponent = 15 }
            };

            var opacityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, DeactivationDuration)),
                EasingFunction = new ExponentialEase { Exponent = 15 }
            };

            DisableBlur();
            easeAnimation.Completed += delegate { Close(); };
            BeginAnimation(LeftProperty, easeAnimation);
            BeginAnimation(OpacityProperty, opacityAnimation);

            _canClose = true;
        }

        #endregion

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
        private void EnableBlur()
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

        /// <summary>
        /// this method uses the SetWindowCompositionAttribute to remove the AeroGlass effect from the window
        /// </summary>
        private void DisableBlur()
        {
            //this code is an altered version of a sample application provided by Rafael Rivera
            //see the full code sample here: (2015/07)
            // https://github.com/riverar/sample-win10-aeroglass (2016/08)
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_DISABLED
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
    }
}