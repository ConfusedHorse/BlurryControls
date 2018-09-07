using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using BlurryControls.Helpers;
using BlurryControls.Internals;

namespace BlurryControls.Controls
{
    /// <summary>
    ///     UserControl which appears blurry over a given <see cref="BlurContainer" />
    /// </summary>
    //based on: https://stackoverflow.com/a/27447817/6649611 (2017/12)
    public class BlurryUserControl : UserControl
    {
        #region Fields

        private Rectangle _blur;

        private bool _inDrag;
        private Point _anchorPoint;
        private Size _containerSize;
        private Point _difference;
        private double _scaleX;
        private double _scaleY;
        
        private const bool ShowUglyCurser = false;

        #endregion

        #region Dependecy Properties

        public static readonly DependencyProperty BlurContainerProperty = DependencyProperty.Register(
            "BlurContainer", typeof(UIElement), typeof(BlurryUserControl),
            new PropertyMetadata(default(UIElement), OnBlurContainerChanged));

        private static void OnBlurContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryUserControl = (BlurryUserControl) d;
            blurryUserControl.UpdateVisual(e.OldValue as UIElement);
        }

        /// <summary>
        ///     represents the underlying element that will be blured
        /// </summary>
        public UIElement BlurContainer
        {
            get => (UIElement) GetValue(BlurContainerProperty);
            set => SetValue(BlurContainerProperty, value);
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(int), typeof(BlurryUserControl), new PropertyMetadata(10, OnBlurRadiusChanged));

        private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryUserControl = (BlurryUserControl) d;
            blurryUserControl.UpdateVisual();
        }

        /// <summary>
        ///     impact of the blur
        /// </summary>
        public int BlurRadius
        {
            get => (int) GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public static readonly DependencyProperty RenderingBiasProperty = DependencyProperty.Register(
            "RenderingBias", typeof(RenderingBias), typeof(BlurryUserControl),
            new PropertyMetadata(RenderingBias.Quality, OnRenderingBiasChanged));

        private static void OnRenderingBiasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryUserControl = (BlurryUserControl) d;
            blurryUserControl.UpdateVisual();
        }

        /// <summary>
        ///     can be changed to RenderingBias.Performance when facing performance issues
        /// </summary>
        public RenderingBias RenderingBias
        {
            get => (RenderingBias) GetValue(RenderingBiasProperty);
            set => SetValue(RenderingBiasProperty, value);
        }

        public static readonly DependencyProperty MagnificationProperty = DependencyProperty.Register(
            "Magnification", typeof(double), typeof(BlurryUserControl), new PropertyMetadata(1.0d, OnMagnificationChanged));

        private static void OnMagnificationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryUserControl = (BlurryUserControl)d;
            blurryUserControl.UpdateVisual();
        }

        /// <summary>
        ///     magnify the area beneath the control to reduce bleed near borders of the <see cref="BlurContainer"/>
        /// </summary>
        public double Magnification
        {
            get => (double)GetValue(MagnificationProperty);
            set => SetValue(MagnificationProperty, value);
        }

        private static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof(VisualBrush), typeof(BlurryUserControl), new PropertyMetadata());
        
        private VisualBrush Brush
        {
            get => (VisualBrush) GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public static readonly DependencyProperty DragMoveEnabledProperty = DependencyProperty.Register(
            "DragMoveEnabled", typeof(bool), typeof(BlurryUserControl),
            new PropertyMetadata(false, OnDragMoveEnabledChanged));

        private static void OnDragMoveEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryUserControl = (BlurryUserControl) d;
            if ((bool) e.NewValue) blurryUserControl.ApplyDragHandles();
            else blurryUserControl.RemoveDragHandles();
        }

        public bool DragMoveEnabled
        {
            get => (bool) GetValue(DragMoveEnabledProperty);
            set => SetValue(DragMoveEnabledProperty, value);
        }

        #endregion

        #region Constructor

        static BlurryUserControl()
        {
            //ensure loading template of BlurryUserControl defined in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurryUserControl),
                new FrameworkPropertyMetadata(typeof(BlurryUserControl)));
        }

        public BlurryUserControl()
        {
            Loaded += OnLoaded;

            Background = Brushes.WhiteSmoke.OfStrength(0.2d);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateVisual();

            if (DragMoveEnabled) ApplyDragHandles();
            else RemoveDragHandles();
        }

        public override void OnApplyTemplate()
        {
            //initialize visual parts
            _blur = (Rectangle) GetTemplateChild("Blur");

            _blur?.SetBinding(Shape.FillProperty,
                new Binding {Source = this, Path = new PropertyPath(BrushProperty)});

            base.OnApplyTemplate();
        }

        #endregion

        #region Drag

        private void ApplyDragHandles()
        {
            // handle drag move
            MouseMove += OnMouseMove;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;

            // grabby hand
            MouseEnter += OnMouseEnter;
        }

        private void RemoveDragHandles()
        {
            MouseMove -= OnMouseMove;
            MouseLeftButtonDown -= OnMouseLeftButtonDown;
            MouseLeftButtonUp -= OnMouseLeftButtonUp;

            MouseEnter -= OnMouseEnter;
        }

        private void OnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162
            if (ShowUglyCurser) Cursor = DragCursor.Grab.GetCursor();
#pragma warning restore 162
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (!_inDrag) return;

            // offset change
            var currentPoint = args.GetPosition(null);
            var currentTranslation = RenderTransform as TranslateTransform ?? new TranslateTransform(0, 0);
            var movement = currentPoint - _anchorPoint;

            // horizontal change
            var newX = currentTranslation.X + movement.X;
            var maxX = _containerSize.Width - Width + BorderThickness.Right;
            var thresholdX = _difference.X + movement.X + BlurRadius * Magnification;

            // vertical change
            var newY = currentTranslation.Y + movement.Y;
            var maxY = _containerSize.Height - Height + BorderThickness.Bottom;
            var thresholdY = _difference.Y + movement.Y + BlurRadius * Magnification;

            // determine new offset
            var left = thresholdX > BorderThickness.Left ? thresholdX < maxX ? newX : currentTranslation.X : currentTranslation.X;
            var top = thresholdY > BorderThickness.Top ? thresholdY < maxY ? newY : currentTranslation.Y : currentTranslation.Y;

            RenderTransform = new TranslateTransform(left, top);
            _anchorPoint = currentPoint;
            args.Handled = true;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (_inDrag) return;
            _anchorPoint = args.GetPosition(null);
            CaptureMouse();
            _inDrag = true;
            args.Handled = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162
            if (ShowUglyCurser) Cursor = DragCursor.Grabbing.GetCursor();
#pragma warning restore 162
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (!_inDrag) return;
            ReleaseMouseCapture();
            _inDrag = false;
            args.Handled = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162
            if (ShowUglyCurser) Cursor = DragCursor.Grab.GetCursor();
#pragma warning restore 162
        }

        #endregion

        #region Blur

        private void RefreshBounds()
        {
            if (_blur == null || BlurContainer == null || Brush == null) return;
            _difference = _blur.TranslatePoint(new Point(), BlurContainer);
            _scaleX = 1 + 2 * BlurRadius * Magnification / RenderSize.Width;
            _scaleY = 1 + 2 * BlurRadius * Magnification / RenderSize.Height;

            var renderSize = new Size(RenderSize.Width * _scaleX, RenderSize.Height * _scaleY);
            Brush.Viewbox = new Rect(_difference, renderSize);

            _containerSize = BlurContainer.RenderSize;
        }

        private void RefreshEffect()
        {
            if (_blur == null) return;
            _blur.Effect = new BlurEffect
            {
                Radius = BlurRadius,
                KernelType = KernelType.Gaussian,
                RenderingBias = RenderingBias
            };

            _blur.RenderTransform = new MatrixTransform(_scaleX, 0, 0, _scaleY, -BlurRadius * Magnification, -BlurRadius * Magnification);
        }

        private void UpdateVisual(UIElement oldBlurContainer = null)
        {
            if (oldBlurContainer != null)
                oldBlurContainer.LayoutUpdated -= OnContainerLayoutUpdated;

            if (BlurContainer != null && _blur != null)
            {
                Brush = new VisualBrush(BlurContainer)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Stretch = Stretch.None
                };

                BlurContainer.LayoutUpdated += OnContainerLayoutUpdated;
                RefreshBounds();
                RefreshEffect();
            }
            else
            {
                Brush = null;
            }
        }

        private void OnContainerLayoutUpdated(object sender, EventArgs eventArgs)
        {
            RefreshBounds();
        }

        #endregion
    }
}