using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace BlurryControls.Controls
{
    public class BlurryImage : Image
    {
        public BlurryImage()
        {
            Loaded += delegate { 
                Effect = new BlurEffect { Radius = BlurRadius };
            };
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(double), typeof(BlurryImage), new PropertyMetadata(default(double)));

        /// <summary>
        /// impact of the blur
        /// </summary>
        public double BlurRadius
        {
            get => (double) GetValue(BlurRadiusProperty);
            set
            {
                SetValue(BlurRadiusProperty, value);
                Effect = new BlurEffect {Radius = BlurRadius};
            }
        }
    }
}