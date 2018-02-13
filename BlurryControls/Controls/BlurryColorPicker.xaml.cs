using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BlurryControls.Helpers;

namespace BlurryControls.Controls
{
    /// <summary>
    ///     Interaction logic for BlurryColorPicker.xaml
    /// </summary>
    public partial class BlurryColorPicker
    {
        #region Constructor

        public BlurryColorPicker()
        {
            InitializeComponent();

            _delayDispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(Delay)};
            _delayDispatcherTimer.Tick += DelayDispatcherTimerOnTick;
        }

        #endregion

        #region Fields

        private bool _isMouseDown;
        private Color _selectedColor;
        private readonly DispatcherTimer _delayDispatcherTimer;
        private readonly BitmapImage _source = IconHelper.ReferenceColors;

        #endregion

        #region Events

        public delegate void ColorChangedHandler(object sender, Color color);

        public event ColorChangedHandler ColorChanged;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay", typeof(double), typeof(BlurryColorPicker), new PropertyMetadata(50d, OnDelayChanged));

        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryColorPicker = (BlurryColorPicker) d;
            if (blurryColorPicker._delayDispatcherTimer == null) return;
            blurryColorPicker._delayDispatcherTimer.Interval = TimeSpan.FromMilliseconds((double) e.NewValue);
        }

        /// <summary>
        ///     Gets or sets the delay of the color change event in milliseconds
        /// </summary>
        public double Delay
        {
            get => (double) GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(BlurryColorPicker), new PropertyMetadata(default(Color)));

        /// <summary>
        ///     Gets the selected <see cref="Color"/>
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            private set => SetValue(ColorProperty, value);
        }

        #endregion

        #region Private Methods

        private void DelayDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            _delayDispatcherTimer.Stop();

            ColorChanged?.Invoke(this, _selectedColor);
        }

        private void RgbMatrixCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(RgbMatrixCanvas);
            DrawLineCross(mousePos);

            if (!_isMouseDown) return;

            SetColorFromPosition(mousePos);
            Color = _selectedColor;
            _delayDispatcherTimer.Start();
        }

        private void RgbMatrixCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }

        private void RgbMatrixCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;

            var mousePos = e.GetPosition(RgbMatrixCanvas);
            SetColorFromPosition(mousePos);
            _delayDispatcherTimer.Start();
        }

        private void RgbMatrixCanvas_OnMouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseDown = Mouse.LeftButton == MouseButtonState.Pressed;
            Cursor = Cursors.None;

            YLine.Visibility = Visibility.Visible;
            XLine.Visibility = Visibility.Visible;

            var mousePos = e.GetPosition(RgbMatrixCanvas);
            DrawLineCross(mousePos);
        }

        private void RgbMatrixCanvas_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            Cursor = Cursors.Arrow;

            YLine.Visibility = Visibility.Collapsed;
            XLine.Visibility = Visibility.Collapsed;
        }

        private void SetColorFromPosition(Point mousePos)
        {
            mousePos.X *= _source.PixelWidth / RgbMatrixImage.ActualWidth * 0.999;
            mousePos.Y *= _source.PixelHeight / RgbMatrixImage.ActualHeight * 0.999;

            var pixel = new byte[4];
            try
            {
                var cb = new CroppedBitmap(_source,
                    new Int32Rect((int) mousePos.X, (int) mousePos.Y, 1, 1));
                cb.CopyPixels(pixel, 4, 0);
            }
            catch (Exception)
            {
                // ignored
            }
            _selectedColor = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]);
        }

        private void DrawLineCross(Point mousePos)
        {
            YLine.X1 = mousePos.X;
            YLine.X2 = mousePos.X;
            YLine.Y1 = 0;
            YLine.Y2 = RgbMatrixCanvas.ActualHeight;

            XLine.X1 = 0;
            XLine.X2 = RgbMatrixCanvas.ActualWidth;
            XLine.Y1 = mousePos.Y;
            XLine.Y2 = mousePos.Y;
        }

        #endregion
    }
}