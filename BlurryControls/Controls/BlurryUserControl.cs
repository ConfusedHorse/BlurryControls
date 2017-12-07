using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using BlurryControls.Helpers;

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

        #endregion

        #region Dependecy Properties

        public static readonly DependencyProperty BlurContainerProperty = DependencyProperty.Register(
            "BlurContainer", typeof(UIElement), typeof(BlurryUserControl), new PropertyMetadata(default(UIElement)));

        /// <summary>
        /// represents the underlying element that will be blured
        /// </summary>
        public UIElement BlurContainer
        {
            get => (UIElement)GetValue(BlurContainerProperty);
            set
            {
                SetValue(BlurContainerProperty, value);
                UpdateVisual();
            }
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(int), typeof(BlurryUserControl), new PropertyMetadata(10));

        /// <summary>
        /// impact of the blur
        /// </summary>
        public int BlurRadius
        {
            get => (int)GetValue(BlurRadiusProperty);
            set
            {
                SetValue(BlurRadiusProperty, value);
                UpdateVisual();
            }
        }

        public static readonly DependencyProperty RenderingBiasProperty = DependencyProperty.Register(
            "RenderingBias", typeof(RenderingBias), typeof(BlurryUserControl), new PropertyMetadata(RenderingBias.Quality));
        
        /// <summary>
        /// can be changed to RenderingBias.Performance when facing performance issues
        /// </summary>
        public RenderingBias RenderingBias
        {
            get => (RenderingBias)GetValue(RenderingBiasProperty);
            set
            {
                SetValue(RenderingBiasProperty, value);
                UpdateVisual();
            }
        }

        private static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof(VisualBrush), typeof(BlurryUserControl), new PropertyMetadata());

        private VisualBrush Brush
        {
            get => (VisualBrush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
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

        #region Private Methods
        
        private void RefreshBounds()
        {
            if (_blur == null || BlurContainer == null || Brush == null) return;
            var difference = _blur.TranslatePoint(new Point(), BlurContainer);
            Brush.Viewbox = new Rect(difference, _blur.RenderSize);
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