using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace BlurryControls.Windows
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

        public double BlurRadius
        {
            get { return (double) GetValue(BlurRadiusProperty); }
            set
            {
                SetValue(BlurRadiusProperty, value);
                Effect = new BlurEffect {Radius = BlurRadius};
            }
        }
    }
}