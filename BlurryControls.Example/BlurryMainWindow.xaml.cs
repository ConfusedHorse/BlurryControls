using System.Windows;
using BlurryControls.DialogFactory;
using BlurryControls.Example.Resources;

namespace BlurryControls.Example
{
    /// <summary>
    /// Interaction logic for BlurryMainWindow.xaml
    /// Contains some simple logic to demonstrate the BlurryWindow control
    /// </summary>
    public partial class BlurryMainWindow
    {
        public BlurryMainWindow()
        {
            InitializeComponent();
        }

        private void HelloMenuBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            BlurryMessageBox.Show(this, Lorem.Ipsum, "Hello");
        }
    }
}
