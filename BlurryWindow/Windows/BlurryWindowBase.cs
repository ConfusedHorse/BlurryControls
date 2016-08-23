using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using BlurryControls.Helper;
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
    /// </summary>
    public class BlurryWindowBase : Window
    {
        #region fields

        private bool _windowIsMaximized = false;
        private WindowState _lastNonMinimizedState = WindowState.Normal;

        #endregion

        #region dependency properties

        public static readonly DependencyProperty IsResizableProperty = DependencyProperty.Register(
            "IsResizable", typeof(bool), typeof(BlurryWindowBase), new PropertyMetadata(true));

        /// <summary>
        /// gets or sets the IsResizableProperty
        /// the window will not be resizable if this property is set to false
        /// </summary>
        public bool IsResizable
        {
            get { return (bool)GetValue(IsResizableProperty); }
            set{ SetValue(IsResizableProperty, value); }
        }

        public static readonly DependencyProperty IsMenuBarVisibleProperty = DependencyProperty.Register(
            "IsMenuBarVisible", typeof(bool), typeof(BlurryWindowBase), new PropertyMetadata(true));

        /// <summary>
        /// gets or sets the IsMenuBarVisibleProperty
        /// the MenuBar will not be accessible if this property is set to true
        /// the window will not be resizable if this property is set to true
        /// the window can not be closed manually if this property is set to true
        /// </summary>
        public bool IsMenuBarVisible
        {
            get { return (bool)GetValue(IsMenuBarVisibleProperty); }
            set
            {
                SetValue(IsMenuBarVisibleProperty, value);
                if (!value) IsResizable = false;
            }
        }

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength", typeof(double), typeof(BlurryWindowBase), new PropertyMetadata(0.5));
        
        /// <summary>
        /// gets or sets the StrengthProperty
        /// the strength property determines the opacity of the window controls
        /// it is set to 0.5 by default and may not exceed 1
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

        public static readonly DependencyProperty CloseOnIconDoubleClickProperty = DependencyProperty.Register(
            "CloseOnIconDoubleClick", typeof(bool), typeof(BlurryWindowBase), new PropertyMetadata(default(bool)));

        /// <summary>
        /// gets or sets the CloseOnIconDoubleClickProperty
        /// the window can not be closed manually by performing a double click on the window icon if this property is set to false
        /// </summary>
        public bool CloseOnIconDoubleClick
        {
            get { return (bool) GetValue(CloseOnIconDoubleClickProperty); }
            set { SetValue(CloseOnIconDoubleClickProperty, value); }
        }

        #endregion

        #region constructor
        
        static BlurryWindowBase()
        {
            //ensure loading template of BlurryWindowBase defined in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurryWindowBase),
                new FrameworkPropertyMetadata(typeof(BlurryWindowBase)));
        }

        public BlurryWindowBase()
        {
            Loaded += Window_Loaded;
            StateChanged += Window_StateChanged;
            InitializeParameters();
        }

        public override void OnApplyTemplate()
        {
            //apply events to all ContextMenu children
            var menuBar = GetTemplateChild("MenuBar") as Grid;
            if (menuBar != null)
            {
                menuBar.MouseLeftButtonDown += MenuBar_OnMouseLeftButtonDown;
                foreach (UIElement element in menuBar.ContextMenu.Items)
                {
                    var menuItem = element as MenuItem;
                    if (menuItem != null)
                    {
                        menuItem.Click += ContextMenuItemOnClick;
                    }
                }
            }

            //apply events to all Buttons which are children of MenuBar
            var rightPanel = GetTemplateChild("RightPanel") as StackPanel;
            if (rightPanel != null)
            {
                foreach (UIElement element in rightPanel.Children)
                {
                    var button = element as Button;
                    if (button != null)
                    {
                        button.Click += MenuBarButtonOnClick;
                    }
                }
            }

            //apply events to all BorderControl children
            var borderControl = GetTemplateChild("BorderControl") as Grid;
            if (borderControl != null)
            {
                foreach (UIElement element in borderControl.Children)
                {
                    var resizeRectangle = element as Rectangle;
                    if (resizeRectangle != null)
                    {
                        resizeRectangle.PreviewMouseLeftButtonDown += WindowResize;
                    }
                }
            }

            //apply left click event to the window icon (only accepts double click)
            var titleImage = GetTemplateChild("TitleImage") as Image;
            if (titleImage != null)
                titleImage.MouseLeftButtonDown += TitleImage_OnMouseLeftButtonDown;

            base.OnApplyTemplate();
        }

        private void InitializeParameters()
        {
            //exclude TaskBar from maximized state by setting Max* properties to work space sizes
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            Window_StateChanged(this, null);

            //use system accent color for window (is overwritten if Background is set)
            Background = ColorHelper.TransparentSystemWindowGlassBrush(Strength);
        }

        #endregion

        #region blurry internals

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //apply blurry filter to the window
            EnableBlur();

            //set matching button style for maximize/restore
            var maximizeContextMenuItem = GetTemplateChild("MaximizeContextMenuItem") as MenuItem;
            var restoreContextMenuItem = GetTemplateChild("RestoreContextMenuItem") as MenuItem;
            if (maximizeContextMenuItem == null || restoreContextMenuItem == null) return;
            restoreContextMenuItem.IsEnabled = WindowState == WindowState.Maximized;
            maximizeContextMenuItem.IsEnabled = !restoreContextMenuItem.IsEnabled;
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

        #endregion

        #region basic functionality

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

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
            SendMessage(new WindowInteropHelper(this).Handle,
                0xA1, (IntPtr)0x2, (IntPtr)0);
        }

        private void TitleImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //double click on window Icon causes Close event
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2 && CloseOnIconDoubleClick)
            {
                Close();
            }
        }

        private void WindowResize(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null) return;

            // performing Interop call depending on the Rectangle tag which raised the event
            // see http://stackoverflow.com/a/25095026 (2016/08)
            var type = (int)frameworkElement.Tag.ToString().ToEnum<ResizeDirection>();
            var hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            if (hwndSource != null) SendMessage(hwndSource.Handle, 0x112, (IntPtr)type, IntPtr.Zero);
        }
        
        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            _windowIsMaximized = false;
            _lastNonMinimizedState = WindowState;
            WindowState = WindowState.Minimized;
        }
        
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (!_windowIsMaximized && WindowState != WindowState.Minimized)
            {
                _windowIsMaximized = true;
                WindowState = WindowState.Normal;
                WindowState = _lastNonMinimizedState;
            }
            else if (WindowState == WindowState.Minimized)
            {
                _windowIsMaximized = false;
            }
        }

        // MenuBar context menu handling depending on the calling element's name
        private void ContextMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
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
            var menuItem = sender as Button;
            if (menuItem == null) return;
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
            _lastNonMinimizedState = WindowState;
            WindowState = WindowState.Normal;
            _windowIsMaximized = false;
        }

        private void Window_MaximizeRestoreButtonClicked()
        {
            // logic is based on http://stackoverflow.com/a/16035678 (2016/08)
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            _lastNonMinimizedState = WindowState;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
