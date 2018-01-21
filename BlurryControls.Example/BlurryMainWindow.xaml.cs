using System.Windows;
using BlurryControls.Controls;
using BlurryControls.DialogFactory;
using BlurryControls.Example.Resources;

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
            // TODO auto define width
            // TODO auto overflow
            // TODO Ok button is fallback
            BlurryMessageBox.Show(this, Lorem.Ipsum, "Hello");
            //BlurryMessageBox.Show(this, "You did the thing.", "Hello");
        }
    }
}
