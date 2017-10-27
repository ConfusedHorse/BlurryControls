using System.Windows;
using System.Windows.Controls;

namespace BlurryControls.Example.Control
{
    /// <summary>
    /// Interaction logic for BlurryDogeControl.xaml
    /// </summary>
    public partial class BlurryDogeControl : UserControl
    {
        public BlurryDogeControl()
        {
            InitializeComponent();
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BlurryDoge.BlurRadius = e.NewValue;
        }
    }
}
