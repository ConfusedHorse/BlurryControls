using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BlurryControls.Controls
{
    public class BlurrySlider : Slider
    {
        private Thumb _thumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_thumb != null) _thumb.MouseEnter -= Thumb_MouseEnter;
            _thumb = (GetTemplateChild("PART_Track") as Track)?.Thumb;
            if (_thumb != null) _thumb.MouseEnter += Thumb_MouseEnter;
        }
        
        private static void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
            {
                RoutedEvent = MouseLeftButtonDownEvent
            };
            (sender as Thumb)?.RaiseEvent(args);
        }
    }
}