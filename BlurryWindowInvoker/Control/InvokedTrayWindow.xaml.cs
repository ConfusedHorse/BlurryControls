using System.Windows;
using BlurryControls.Windows;
using BlurryWindowInvoker.Resources;

namespace BlurryWindowInvoker.Control
{
    /// <summary>
    /// Interaction logic for InvokedTrayIcon.xaml
    /// Contains some simple logic to demonstrate the BlurryWindow control
    /// </summary>
    public partial class InvokedTrayWindow : BlurryTray
    {
        public InvokedTrayWindow()
        {
            InitializeComponent();
        }

        //show sample text
        private void InvokedTrayIcon_OnLoaded(object sender, RoutedEventArgs e)
        {
            TrayTextBlock.Text = Lorem.Ipsum;
        }
    }
}
