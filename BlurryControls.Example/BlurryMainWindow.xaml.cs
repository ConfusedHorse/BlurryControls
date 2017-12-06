using System.Windows;
using BlurryControls.Controls;
using BlurryControls.DialogFactory;

namespace BlurryControls.Example
{
    /// <summary>
    /// Interaction logic for BlurryMainWindow.xaml
    /// Contains some simple logic to demonstrate the BlurryWindow control
    /// </summary>
    public partial class BlurryMainWindow : BlurryWindow
    {
        public BlurryMainWindow()
        {
            InitializeComponent();
        }

        private void HelloMenuBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            BlurryMessageBox.Show(this, "You did the thing.", "Hello");
        }
    }
}
