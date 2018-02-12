using System.Windows;
using System.Windows.Media;
using BlurryControls.Controls;

namespace BlurryControls.Example.Control
{
    /// <summary>
    /// Interaction logic for BlurryContentControl.xaml
    /// Contains some simple logic to demonstrate the BlurryWindow control
    /// </summary>
    public partial class BlurryContentControl
    {
        public BlurryContentControl()
        {
            InitializeComponent();
        }

        private void SampleInvokeButton_OnClick(object sender, RoutedEventArgs e)
        {
            new InvokedTrayWindow().Show();
        }

        private void BlurryColorPicker_OnColorChanged(object sender, Color color)
        {
            if (Application.Current.MainWindow is BlurryWindow mainWindow)
                mainWindow.Background = new SolidColorBrush(color);
        }

        private void ResetBackground_OnClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is BlurryWindow mainWindow)
                mainWindow.Background = null;
        }
    }
}
