using System.Windows;
using System.Windows.Controls;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;

namespace BlurryControls.Example.Control
{
    /// <summary>
    /// Interaction logic for BlurBehindDialogControl.xaml
    /// </summary>
    public partial class BlurBehindDialogControl : UserControl
    {
        public BlurBehindDialogControl()
        {
            InitializeComponent();
        }

        private void SampleInvokeButton_OnClick(object sender, RoutedEventArgs e)
        {
            //show a blur behind dialog
            BlurBehindMessageBox.Show("Everything behind me should appear blured now.", "I am important!", BlurryDialogButton.Ok);
        }
    }
}
