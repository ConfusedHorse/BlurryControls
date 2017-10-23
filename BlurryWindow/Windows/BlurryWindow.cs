﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BlurryControls.Helpers;
using BlurryControls.Internals;

namespace BlurryControls.Windows
{
    /// <summary>
    /// provides an alternative WindowBase which uses the AeroGlass effect.
    /// Additional properties are:
    /// <para><see cref="IsResizable"/></para>
    /// <para><see cref="IsMenuBarVisible"/></para>
    /// <para><see cref="Strength"/></para>
    /// <para><see cref="CloseOnIconDoubleClick"/></para>
    /// <para><see cref="AdditionalMenuBarButtons"/></para>
    /// </summary>
    public class BlurryWindow : Window
    {
        #region Fields

        private bool _customBackground;

        private WindowState _lastState;
        private bool _originalIsResizable;
        private bool _isFullScreen;
        private bool _originalIsMenuBarVisible;

        private Color _menuBarColor;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsResizableProperty = DependencyProperty.Register(
            "IsResizable", typeof(bool), typeof(BlurryWindow), new PropertyMetadata(true));

        /// <summary>
        /// gets or sets the IsResizableProperty
        /// the window will not be resizable if this property is set to false
        /// </summary>
        public bool IsResizable
        {
            get => (bool)GetValue(IsResizableProperty);
            set => SetValue(IsResizableProperty, value);
        }

        public static readonly DependencyProperty IsMenuBarVisibleProperty = DependencyProperty.Register(
            "IsMenuBarVisible", typeof(bool), typeof(BlurryWindow), new PropertyMetadata(true));

        /// <summary>
        /// gets or sets the IsMenuBarVisibleProperty
        /// the MenuBar will not be accessible if this property is set to false
        /// the window will not be resizable if this property is set to true
        /// the window can not be closed manually if this property is set to true
        /// </summary>
        public bool IsMenuBarVisible
        {
            get => (bool)GetValue(IsMenuBarVisibleProperty);
            set
            {
                SetValue(IsMenuBarVisibleProperty, value);
                if (!value) IsResizable = false;
            }
        }

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength", typeof(double), typeof(BlurryWindow), new PropertyMetadata(0.5));

        /// <summary>
        /// gets or sets the StrengthProperty
        /// the strength property determines the opacity of the window controls
        /// it is set to 0.5 by default and may not exceed 1
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
                _menuBarColor = Background.OfStrength(0d).Color;
            }
        }

        public static readonly DependencyProperty CloseOnIconDoubleClickProperty = DependencyProperty.Register(
            "CloseOnIconDoubleClick", typeof(bool), typeof(BlurryWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// gets or sets the CloseOnIconDoubleClickProperty
        /// the window can not be closed manually by performing a double click on the window icon if this property is set to false
        /// </summary>
        public bool CloseOnIconDoubleClick
        {
            get => (bool)GetValue(CloseOnIconDoubleClickProperty);
            set => SetValue(CloseOnIconDoubleClickProperty, value);
        }

        public static readonly DependencyProperty AdditionalMenuBarButtonsProperty = DependencyProperty.Register(
            "AdditionalMenuBarButtons", typeof(ButtonCollection), typeof(BlurryWindow), new PropertyMetadata(default(ButtonCollection)));

        /// <summary>
        /// a button collection shown additionaly to lefthand side of the conventional window buttons
        /// </summary>
        public ButtonCollection AdditionalMenuBarButtons
        {
            get => (ButtonCollection)GetValue(AdditionalMenuBarButtonsProperty);
            set => SetValue(AdditionalMenuBarButtonsProperty, value);
        }

        public static readonly DependencyProperty HorizontalTitleAlignmentProperty = DependencyProperty.Register(
            "HorizontalTitleAlignment", typeof(HorizontalAlignment), typeof(BlurryWindow), new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment HorizontalTitleAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalTitleAlignmentProperty);
            set => SetValue(HorizontalTitleAlignmentProperty, value);
        }

        #endregion

        #region Constructor

        static BlurryWindow()
        {
            //ensure loading template of BlurryWindowBase defined in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurryWindow),
                new FrameworkPropertyMetadata(typeof(BlurryWindow)));
        }

        public BlurryWindow()
        {
            Loaded += Window_Loaded;
            SourceInitialized += OnSourceInitialized;
            KeyDown += OnKeyDown;
            StateChanged += OnStateChanged;
            InitializeParameters();
        }

        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("MenuBar") is Grid menuBar)
            {
                //apply visual (calming) behaviour to the MenuBar
                menuBar.MouseEnter += MenuBarOnMouseEnter;
                menuBar.MouseLeave += MenuBarOnMouseLeave;

                //apply events to all ContextMenu children
                menuBar.MouseLeftButtonDown += MenuBar_OnMouseLeftButtonDown;

                if (menuBar.ContextMenu == null) return;
                foreach (UIElement element in menuBar.ContextMenu.Items)
                    if (element is MenuItem menuItem)
                        menuItem.Click += ContextMenuItemOnClick;
            }

            if (GetTemplateChild("RightPanel") is StackPanel rightPanel)
            {
                //apply events to all Buttons which are children of MenuBar
                foreach (UIElement element in rightPanel.Children)
                    if (element is Button button)
                        button.Click += MenuBarButtonOnClick;
            }

            if (GetTemplateChild("BorderControl") is Grid borderControl)
            {
                //apply events to all BorderControl children
                foreach (UIElement element in borderControl.Children)
                    if (element is Rectangle resizeRectangle)
                        resizeRectangle.PreviewMouseLeftButtonDown += WindowResize;
            }

            //apply left click event to the window icon (only accepts double click)
            if (GetTemplateChild("TitleImage") is Image titleImage)
                titleImage.MouseLeftButtonDown += TitleImage_OnMouseLeftButtonDown;

            base.OnApplyTemplate();
        }

        #endregion

        #region Visual Behaviour

        private void InitializeParameters()
        {
            if (!_customBackground) SystemParameters.StaticPropertyChanged += SystemParametersOnStaticPropertyChanged;
        }

        // apply system color when changed
        private void SystemParametersOnStaticPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Background = ColorHelper.SystemWindowGlassBrushOfStrength(Strength);
        }

        private void MenuBarOnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!(sender is Grid menuBar)) return;

            var colorTargetPath = new PropertyPath("(Panel.Background).(SolidColorBrush.Color)");
            var menuBarMouseEnterAnimation = new ColorAnimation
            {
                To = Background.GetColor(),
                FillBehavior = FillBehavior.HoldEnd,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            Storyboard.SetTarget(menuBarMouseEnterAnimation, menuBar);
            Storyboard.SetTargetProperty(menuBarMouseEnterAnimation, colorTargetPath);

            var menuBarMouseEnterStoryboard = new Storyboard();
            menuBarMouseEnterStoryboard.Children.Add(menuBarMouseEnterAnimation);
            menuBarMouseEnterStoryboard.Begin();
        }

        private void MenuBarOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!(sender is Grid menuBar)) return;

            var colorTargetPath = new PropertyPath("(Panel.Background).(SolidColorBrush.Color)");
            var menuBarMouseLeaveAnimation = new ColorAnimation
            {
                To = _menuBarColor,
                FillBehavior = FillBehavior.HoldEnd,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            Storyboard.SetTarget(menuBarMouseLeaveAnimation, menuBar);
            Storyboard.SetTargetProperty(menuBarMouseLeaveAnimation, colorTargetPath);

            var menuBarMouseLeaveStoryboard = new Storyboard();
            menuBarMouseLeaveStoryboard.Children.Add(menuBarMouseLeaveAnimation);
            menuBarMouseLeaveStoryboard.Begin();
        }

        #endregion

        #region Basic Functionality

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //apply blurry filter to the window
            this.Blur();

            //set matching button style for maximize/restore
            var maximizeContextMenuItem = GetTemplateChild("MaximizeContextMenuItem") as MenuItem;
            var restoreContextMenuItem = GetTemplateChild("RestoreContextMenuItem") as MenuItem;
            if (maximizeContextMenuItem == null || restoreContextMenuItem == null) return;
            restoreContextMenuItem.IsEnabled = WindowState == WindowState.Maximized;
            maximizeContextMenuItem.IsEnabled = !restoreContextMenuItem.IsEnabled;

            _customBackground = Background != null;
            //use system accent color for window (is overwritten if Background is set)
            Background = !_customBackground ? ColorHelper.SystemWindowGlassBrushOfStrength(Strength) : Background.OfStrength(Strength);
            _menuBarColor = Background.OfStrength(0d).Color;

            _lastState = WindowState;
            _originalIsResizable = IsResizable;
            _originalIsMenuBarVisible = IsMenuBarVisible;
        }

        private void OnSourceInitialized(object sender, EventArgs eventArgs)
        {
            SizeHelper.WindowInitialized(this);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void OnStateChanged(object sender, EventArgs eventArgs)
        {
            if (WindowState == WindowState.Maximized)
            {
                IsResizable = false;
            }
            else
            {
                IsResizable = _originalIsResizable;
                if (_isFullScreen)
                {
                    _isFullScreen = false;
                    WindowState = _lastState;
                    IsMenuBarVisible = _originalIsMenuBarVisible;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.F11) return;

            if (_isFullScreen)
            {
                _isFullScreen = false;
                WindowState = _lastState;
                IsMenuBarVisible = _originalIsMenuBarVisible;
            }
            else if (_originalIsResizable)
            {
                _isFullScreen = true;
                IsMenuBarVisible = false; //overrides IsResizable
                _lastState = WindowState;
                WindowState = WindowState.Maximized;
            }
        }

        private void MenuBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //double click on MenuBar causes Maximize/Restore event
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                MaximizeRestoreButton_OnClick(this, null);
            }
            //single click on MenuBar enables drag move
            //this also enables native Windows10 gestures
            //such as Aero Snap and Aero Shake
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, 0xA1, (IntPtr)0x2, (IntPtr)0);
        }

        private void TitleImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //double click on window Icon causes Close event
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2 && CloseOnIconDoubleClick)
            {
                Close();
            }
        }

        private static void WindowResize(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement)) return;

            // performing Interop call depending on the Rectangle tag which raised the event
            // see http://stackoverflow.com/a/25095026 (2016/08)
            var type = (int)frameworkElement.Tag.ToString().ToEnum<ResizeDirection>();
            if (PresentationSource.FromVisual((Visual)sender) is HwndSource hwndSource)
                SendMessage(hwndSource.Handle, 0x112, (IntPtr)type, IntPtr.Zero);
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // MenuBar context menu handling depending on the calling element's name
        private void ContextMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) return;
            switch (menuItem.Name)
            {
                case "MinimizeContextMenuItem":
                    MinimizeButton_OnClick(sender, e);
                    break;
                case "RestoreContextMenuItem":
                    MaximizeRestoreButton_OnClick(sender, e);
                    break;
                case "MaximizeContextMenuItem":
                    MaximizeRestoreButton_OnClick(sender, e);
                    break;
                case "CloseContextMenuItem":
                    CloseButton_OnClick(sender, e);
                    break;
            }
        }

        // MenuBar button handling depending on the calling element's name
        private void MenuBarButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button menuItem)) return;
            switch (menuItem.Name)
            {
                case "MinimizeButton":
                    MinimizeButton_OnClick(sender, e);
                    break;
                case "MaximizeRestoreButton":
                    MaximizeRestoreButton_OnClick(sender, e);
                    break;
                case "CloseButton":
                    CloseButton_OnClick(sender, e);
                    break;
            }
        }

        // toggle between maximized and previous state
        private void MaximizeRestoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            //the switch case also handles the visual appearance of Maximize/Restore elements
            //such as the MenuBar buttons and MenuBar's context menu
            var maximizeContextMenuItem = GetTemplateChild("MaximizeContextMenuItem") as MenuItem;
            var restoreContextMenuItem = GetTemplateChild("RestoreContextMenuItem") as MenuItem;
            if (maximizeContextMenuItem == null || restoreContextMenuItem == null) return;
            switch (WindowState)
            {
                case WindowState.Normal:
                    Window_MaximizeRestoreButtonClicked();
                    maximizeContextMenuItem.IsEnabled = false;
                    restoreContextMenuItem.IsEnabled = true;
                    break;
                case WindowState.Maximized:
                    Window_MinimizeButtonClicked();
                    maximizeContextMenuItem.IsEnabled = true;
                    restoreContextMenuItem.IsEnabled = false;
                    break;
                case WindowState.Minimized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // minimizing the application into TaskBar
        private void Window_MinimizeButtonClicked()
        {
            WindowState = WindowState.Normal;
        }

        private void Window_MaximizeRestoreButtonClicked()
        {
            // logic is based on http://stackoverflow.com/a/16035678 (2016/08)
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
