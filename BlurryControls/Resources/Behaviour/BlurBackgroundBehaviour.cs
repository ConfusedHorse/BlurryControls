using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace BlurryControls.Resources.Behaviour
{
    /// <summary>
    /// Behaviour which blurs the background of a given <see cref="BlurContainer"/>
    /// </summary>
    //based on: https://stackoverflow.com/a/27447817/6649611 (2017/12)
    internal class BlurBackgroundBehavior : Behavior<Shape>
    {
        #region Dependecy Properties

        public static readonly DependencyProperty BlurContainerProperty = DependencyProperty.Register(
            "BlurContainer", typeof(UIElement), typeof(BlurBackgroundBehavior), new PropertyMetadata(OnContainerChanged));

        /// <summary>
        /// represents the underlying element that will be blured
        /// </summary>
        public UIElement BlurContainer
        {
            get => (UIElement)GetValue(BlurContainerProperty);
            set => SetValue(BlurContainerProperty, value);
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(int), typeof(BlurBackgroundBehavior), new PropertyMetadata(25));

        /// <summary>
        /// impact of the blur
        /// </summary>
        public int BlurRadius
        {
            get => (int)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public static readonly DependencyProperty RenderingBiasProperty = DependencyProperty.Register(
            "RenderingBias", typeof(RenderingBias), typeof(BlurBackgroundBehavior), new PropertyMetadata(RenderingBias.Quality));

        /// <summary>
        /// can be changed to RenderingBias.Performance when facing performance issues
        /// </summary>
        public RenderingBias RenderingBias
        {
            get => (RenderingBias)GetValue(RenderingBiasProperty);
            set => SetValue(RenderingBiasProperty, value);
        }

        private static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof(VisualBrush), typeof(BlurBackgroundBehavior), new PropertyMetadata());

        private VisualBrush Brush
        {
            get => (VisualBrush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        #endregion

        #region Constructor

        protected override void OnAttached()
        {
            AssociatedObject.Effect = new BlurEffect
            {
                Radius = BlurRadius,
                KernelType = KernelType.Gaussian,
                RenderingBias = RenderingBias
            };

            AssociatedObject.SetBinding(Shape.FillProperty,
                new Binding { Source = this, Path = new PropertyPath(BrushProperty) });

            AssociatedObject.LayoutUpdated += (sender, args) => UpdateBounds();
            UpdateBounds();
        }

        protected override void OnDetaching()
        {
            BindingOperations.ClearBinding(AssociatedObject, Border.BackgroundProperty);
        }

        #endregion

        #region Private Methods

        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlurBackgroundBehavior)d).OnContainerChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
        }

        private void OnContainerChanged(UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null)
                oldValue.LayoutUpdated -= OnContainerLayoutUpdated;

            if (newValue != null)
            {
                Brush = new VisualBrush(newValue) { ViewboxUnits = BrushMappingMode.Absolute };

                newValue.LayoutUpdated += OnContainerLayoutUpdated;
                UpdateBounds();
            }
            else
            {
                Brush = null;
            }
        }

        private void OnContainerLayoutUpdated(object sender, EventArgs eventArgs)
        {
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            if (AssociatedObject == null || BlurContainer == null || Brush == null) return;
            var difference = AssociatedObject.TranslatePoint(new Point(), BlurContainer);
            Brush.Viewbox = new Rect(difference, AssociatedObject.RenderSize);
        }

        #endregion
    }
}