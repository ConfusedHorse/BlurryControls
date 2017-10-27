using System.Windows;
using System.Windows.Controls;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;

namespace BlurryControls.Example.Control
{
    /// <summary>
    /// Interaction logic for BlurryDogeDialogControl.xaml
    /// </summary>
    public partial class BlurryDogeDialogControl : UserControl
    {
        public BlurryDogeDialogControl()
        {
            InitializeComponent();
        }

        private void SampleInvokeButton_OnClick(object sender, RoutedEventArgs e)
        {
            //show a custom dialog
            var mainWindow = Application.Current.MainWindow;
            var dogeControl = new BlurryDogeControl();
            var customButtonCollection = new ButtonCollection()
            {
                new Button(){Content = "Custom Ok", Margin = new Thickness(2.5, 0, 2.5, 0)},
                new Button(){Content = "Custom Cancel", Margin = new Thickness(2.5, 0, 2.5, 0)}
            };

            var result = BlurryMessageBox.Show(mainWindow, "Custom Dialog", dogeControl, customButtonCollection, 0d);
        }
    }
}
