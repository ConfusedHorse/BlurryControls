﻿using System;
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
    /// UserControl which appears blurry over a given <see cref="BlurContainer"/>
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

        #endregion

        #region Dependecy Properties

        public static readonly DependencyProperty BlurContainerProperty = DependencyProperty.Register(
            "BlurContainer", typeof(UIElement), typeof(BlurryUserControl), new PropertyMetadata(default(UIElement), OnBlurContainerChanged));

        private static void OnBlurContainerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var flipControl = (BlurryUserControl)dependencyObject;
            flipControl.UpdateVisual();
        }

        /// <summary>
        /// represents the underlying element that will be blured
        /// </summary>
        public UIElement BlurContainer
        {
            get => (UIElement)GetValue(BlurContainerProperty);
            set => SetValue(BlurContainerProperty, value);
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(int), typeof(BlurryUserControl), new PropertyMetadata(10, OnBlurRadiusChanged));

        private static void OnBlurRadiusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var flipControl = (BlurryUserControl)dependencyObject;
            flipControl.UpdateVisual();
        }

        /// <summary>
        /// impact of the blur
        /// </summary>
        public int BlurRadius
        {
            get => (int)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public static readonly DependencyProperty RenderingBiasProperty = DependencyProperty.Register(
            "RenderingBias", typeof(RenderingBias), typeof(BlurryUserControl), new PropertyMetadata(RenderingBias.Quality, OnRenderingBiasChanged));

        private static void OnRenderingBiasChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var flipControl = (BlurryUserControl)dependencyObject;
            flipControl.UpdateVisual();
        }

        /// <summary>
        /// can be changed to RenderingBias.Performance when facing performance issues
        /// </summary>
        public RenderingBias RenderingBias
        {
            get => (RenderingBias)GetValue(RenderingBiasProperty);
            set => SetValue(RenderingBiasProperty, value);
        }

        private static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof(VisualBrush), typeof(BlurryUserControl), new PropertyMetadata());

        
        private VisualBrush Brush
        {
            get => (VisualBrush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public static readonly DependencyProperty DragMoveEnabledProperty = DependencyProperty.Register(
            "DragMoveEnabled", typeof(bool), typeof(BlurryUserControl), new PropertyMetadata(false, OnDragMoveEnabledChanged));

        private static void OnDragMoveEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var flipControl = (BlurryUserControl)dependencyObject;
            if ((bool) args.NewValue) flipControl.ApplyDragHandles();
            else flipControl.RemoveDragHandles();
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

            Background = Brushes.WhiteSmoke.OfStrength(50);
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
            _blur = (Rectangle)GetTemplateChild("Blur");

            _blur?.SetBinding(Shape.FillProperty,
                new Binding { Source = this, Path = new PropertyPath(BrushProperty) });
            
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
            Cursor = DragCursor.Grab.GetCursor();
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (!_inDrag) return;

            // offset change
            var currentPoint = args.GetPosition(null);
            var movement = currentPoint - _anchorPoint;

            // horizontal change
            var newX = Margin.Left + movement.X;
            var maxX = _containerSize.Width - Width + BorderThickness.Right;
            var thresholdX = _difference.X + movement.X;

            // vertical change
            var newY = Margin.Top + movement.Y;
            var maxY = _containerSize.Height - Height + BorderThickness.Bottom;
            var thresholdY = _difference.Y + movement.Y;

            // determine new offset
            var left = thresholdX > BorderThickness.Left ? thresholdX < maxX ? newX : Margin.Left : Margin.Left;
            var top = thresholdY > BorderThickness.Top ? thresholdY < maxY ? newY : Margin.Top : Margin.Top;

            Margin = new Thickness(left, top, -left, -top);
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

            Cursor = DragCursor.Grabbing.GetCursor();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (!_inDrag) return;
            ReleaseMouseCapture();
            _inDrag = false;
            args.Handled = true;

            Cursor = DragCursor.Grab.GetCursor();
        }

        #endregion

        #region Blur

        private void RefreshBounds()
        {
            if (_blur == null || BlurContainer == null || Brush == null) return;
            _difference = _blur.TranslatePoint(new Point(), BlurContainer);
            Brush.Viewbox = new Rect(_difference, _blur.RenderSize);
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
        }

        private void UpdateVisual()
        {
            BlurContainer.LayoutUpdated -= OnContainerLayoutUpdated;

            if (BlurContainer != null && _blur != null)
            {
                Brush = new VisualBrush(BlurContainer) { ViewboxUnits = BrushMappingMode.Absolute };

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