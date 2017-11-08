using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace AcrylicControls.Example
{
    /// <summary>
    /// Example page utilizing the acrylic API of UWP with a transparent ToolBar
    /// code taken from: https://stackoverflow.com/a/43711413/6649611
    /// learn more at: https://docs.microsoft.com/de-de/windows/uwp/design/style/acrylic
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Compositor _compositor;
        private SpriteVisual _hostSprite;

        public MainPage()
        {
            InitializeComponent();
            ApplyAcrylicAccent(AcrylicBackground);
            Loaded += PageOnLoaded;
        }

        private static void PageOnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyTransparencyToTitlebar();
        }

        private static void ApplyTransparencyToTitlebar()
        {
            var formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        private void ApplyAcrylicAccent(Panel panel)
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _hostSprite = _compositor.CreateSpriteVisual();
            _hostSprite.Size = new Vector2((float)panel.ActualWidth, (float)panel.ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(panel, _hostSprite);
            _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hostSprite != null)
                _hostSprite.Size = e.NewSize.ToVector2();
        }
    }
}
