using System.Windows;
using System.Windows.Controls;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;

namespace BlurryControls.Example.Control
{
    /// <summary>
    /// Interaction logic for BlurryContentControl.xaml
    /// Contains some simple logic to demonstrate the BlurryWindow control
    /// </summary>
    public partial class BlurryContentControl : UserControl
    {
        public BlurryContentControl()
        {
            InitializeComponent();
        }

        private void SampleInvokeButton_OnClick(object sender, RoutedEventArgs e)
        {
            //show a sample dialog
            var mainWindow = Application.Current.MainWindow;
            var result = BlurryMessageBox.Show(mainWindow,
                "By pressing OK a sample TrayControl will show up for the duration of 5 seconds, or until you click somewhere else.",
                "This is a MessageBox!", BlurryDialogButton.Ok, BlurryDialogIcon.Information);
            // show a sample tray control when ok is clicked
            if (result == BlurryDialogResult.Ok)
            {
                var tray = new InvokedTrayWindow();
                tray.Show();
            }
        }
    }
}
