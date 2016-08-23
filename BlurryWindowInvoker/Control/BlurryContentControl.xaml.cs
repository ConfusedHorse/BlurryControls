using System.Windows;
using System.Windows.Controls;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;

namespace BlurryWindowInvoker.Control
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

        private void Todesknopf_OnClick(object sender, RoutedEventArgs e)
        {
            //show a sample dialog
            var mainWindow = Application.Current.MainWindow;
            var result = BlurryMessageBox.Show(mainWindow,
                "By pressing OK a ColorAnimation will alter the control's background and a sample TrayControl will show up for the default duration of 10 seconds or until you click somewhere else.",
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
