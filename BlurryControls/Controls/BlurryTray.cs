using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using BlurryControls.Helpers;

namespace BlurryControls.Controls
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
    /// <para><see cref="ActivationDuration"/></para>
    /// <para><see cref="DeactivationDuration"/></para>
    /// </summary>
    public class BlurryTray : Window
    {
        #region Fields

        private bool _canCloseActualWindow = false;
        private bool _customBackground;
        private readonly DispatcherTimer _durationDispatcherTimer = new DispatcherTimer();

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength", typeof(double), typeof(BlurryTray), new PropertyMetadata(0.75));

        /// <summary>
        /// gets or sets the StrengthProperty
        /// the strength property determines the opacity of the window controls
        /// it is set to 0.75 by default and may not exceed 1
        /// </summary>
        public double Strength
        {
            get => (double)GetValue(StrengthProperty);
            set
            {
                var correctValue = (value >= 1 ? 1 : value) <= 0 ? 0 : value;
                SetValue(StrengthProperty, correctValue);

                var backgroundColor = ((SolidColorBrush)Background).Color.OfStrength(Strength).Color;
                Background = new SolidColorBrush(backgroundColor);
            }
        }

        public static readonly DependencyProperty DeactivatesOnLostFocusProperty = DependencyProperty.Register(
            "DeactivatesOnLostFocus", typeof(bool), typeof(BlurryTray), new PropertyMetadata(true));

        /// <summary>
        /// gets or sets the DeactivatesOnLostFocusProperty
        /// the the window will be closed when a click outside the window is performed
        /// </summary>
        public bool DeactivatesOnLostFocus
        {
            get => (bool)GetValue(DeactivatesOnLostFocusProperty);
            set => SetValue(DeactivatesOnLostFocusProperty, value);
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration", typeof(uint), typeof(BlurryTray), new PropertyMetadata(default(uint)));

        /// <summary>
        /// gets or sets the DurationProperty
        /// the the window will be closed after a certain amount of time
        /// </summary>
        public uint Duration
        {
            get => (uint) GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public static readonly DependencyProperty ActivationDurationProperty = DependencyProperty.Register(
            "ActivationDuration", typeof(int), typeof(BlurryTray), new PropertyMetadata(2000));

        /// <summary>
        /// Milliseconds the activation animation takes
        /// </summary>
        public int ActivationDuration
        {
            get => (int) GetValue(ActivationDurationProperty);
            set => SetValue(ActivationDurationProperty, value);
        }

        public static readonly DependencyProperty DeactivationDurationProperty = DependencyProperty.Register(
            "DeactivationDuration", typeof(int), typeof(BlurryTray), new PropertyMetadata(2000));

        /// <summary>
        /// Milliseconds the deactivation animation takes
        /// </summary>
        public int DeactivationDuration
        {
            get => (int) GetValue(DeactivationDurationProperty);
            set => SetValue(DeactivationDurationProperty, value);
        }

        #endregion

        #region Constructor

        static BlurryTray()
        {
            //ensure loading template of BlurryWindowBase defined in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurryTray),
                new FrameworkPropertyMetadata(typeof(BlurryTray)));
        }

        public BlurryTray()
        {
            WindowStartupLocation = WindowStartupLocation.Manual;

            Loaded += Window_Loaded;
            Loaded += InitializeDuration;
            Loaded += InitializeTraySpecificVisualBehaviour;
            Closing += TerminateTraySpecificVisualBehaviour;
            MouseLeave += OnMouseLeave;
            // close window when a click is performed outside the window
            Deactivated += delegate
            {
                if (!_canCloseActualWindow && DeactivatesOnLostFocus) Close();
            };
        }

        #endregion

        #region Visual Behaviour

        // the Close event is raised after a certain amount of time
        private void InitializeDuration(object sender, RoutedEventArgs e)
        {
            if (Duration == 0) return;
            _durationDispatcherTimer.Interval = new TimeSpan(0, 0, 0, (int)Duration);
            _durationDispatcherTimer.Tick += delegate
            {
                if (!IsMouseOver) Close();
                else _durationDispatcherTimer.Stop();
            };
            _durationDispatcherTimer.Start();
        }

        // initial animation
        // performs a calming animation when loading the window
        // the window flows from right to left to its terminal position and gains opacity
        private void InitializeTraySpecificVisualBehaviour(object sender, RoutedEventArgs args)
        {
            if (!_customBackground)
                SystemParameters.StaticPropertyChanged += SystemParametersOnStaticPropertyChanged;

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
            Background = ColorHelper.SystemWindowGlassBrushOfStrength(Strength);
        }

        // termination animation
        // performs a calming animation when termination the window
        // the window flows from left to right from its starting position and loses opacity
        // the blur filter is deactivate while this animation is performed
        private void TerminateTraySpecificVisualBehaviour(object sender, CancelEventArgs args)
        {
            // force performing a vanishing animation before the window gets terminated
            if (_canCloseActualWindow) return;
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

            this.UnBlur();
            easeAnimation.Completed += delegate { Close(); };
            BeginAnimation(LeftProperty, easeAnimation);
            BeginAnimation(OpacityProperty, opacityAnimation);

            _canCloseActualWindow = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            _durationDispatcherTimer.Start();
        }

        #endregion

        #region Basic Functionality

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //apply blurry filter to the window
            this.Blur();

            _customBackground = Background != null;
            //use system accent color for window (is overwritten if Background is set)
            Background = !_customBackground
                ? ColorHelper.SystemWindowGlassBrushOfStrength(Strength)
                : Background.OfStrength(Strength);
        }

        #endregion
    }
}