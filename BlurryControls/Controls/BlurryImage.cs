using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace BlurryControls.Controls
{
    public class BlurryImage : Image
    {
        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(double), typeof(BlurryImage),
            new PropertyMetadata(default(double), OnBlurRadiusChanged));

        public BlurryImage()
        {
            Loaded += delegate { Effect = new BlurEffect {Radius = BlurRadius}; };
        }

        /// <summary>
        ///     impact of the blur
        /// </summary>
        public double BlurRadius
        {
            get => (double) GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blurryImage = (BlurryImage) d;
            blurryImage.Effect = new BlurEffect {Radius = (double) e.NewValue};
        }
    }
}