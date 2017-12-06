using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using BlurryControls.Resources.Behaviour;

namespace BlurryControls.Controls
{
    public class BlurryUserControl : UserControl
    {
        #region Fields
        
        private Rectangle _blur;
        private BlurBackgroundBehavior _behaviour;

        #endregion

        #region Dependecy Properties

        public static readonly DependencyProperty BlurContainerProperty = DependencyProperty.Register(
            "BlurContainer", typeof(UIElement), typeof(BlurryUserControl), new PropertyMetadata(default(UIElement)));

        /// <summary>
        /// represents the underlying element that will be blured
        /// </summary>
        public UIElement BlurContainer
        {
            get => (UIElement)GetValue(BlurContainerProperty);
            set
            {
                SetValue(BlurContainerProperty, value);
                if (_behaviour != null) _behaviour.BlurContainer = value;
                else InitializeBehaviour();
                RefreshBehaviour();
            }
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof(int), typeof(BlurryUserControl), new PropertyMetadata(25));

        /// <summary>
        /// impact of the blur
        /// </summary>
        public int BlurRadius
        {
            get => (int)GetValue(BlurRadiusProperty);
            set
            {
                SetValue(BlurRadiusProperty, value);
                if (_behaviour != null) _behaviour.BlurRadius = value;
                else InitializeBehaviour();
                RefreshBehaviour();
            }
        }

        public static readonly DependencyProperty RenderingBiasProperty = DependencyProperty.Register(
            "RenderingBias", typeof(RenderingBias), typeof(BlurryUserControl), new PropertyMetadata(RenderingBias.Quality));
        

        /// <summary>
        /// can be changed to RenderingBias.Performance when facing performance issues
        /// </summary>
        public RenderingBias RenderingBias
        {
            get => (RenderingBias)GetValue(RenderingBiasProperty);
            set
            {
                SetValue(RenderingBiasProperty, value);
                if (_behaviour != null) _behaviour.RenderingBias = value;
                else InitializeBehaviour();
                RefreshBehaviour();
            }
        }

        #endregion

        #region Constructor

        static BlurryUserControl()
        {
            //ensure loading template of BlurryUserControl defined in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurryUserControl),
                new FrameworkPropertyMetadata(typeof(BlurryUserControl)));
        }

        public BlurryUserControl()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            InitializeBehaviour();
            RefreshBehaviour();
        }

        public override void OnApplyTemplate()
        {
            //initialize visual parts
            _blur = (Rectangle)GetTemplateChild("Blur");

            base.OnApplyTemplate();
        }

        #endregion

        #region Private Methods

        private void InitializeBehaviour()
        {
            _behaviour = new BlurBackgroundBehavior
            {
                BlurRadius = BlurRadius,
                RenderingBias = RenderingBias,
                BlurContainer = BlurContainer
            };
        }

        private void RefreshBehaviour()
        {
            if (_blur == null) return;

            var behaviours = Interaction.GetBehaviors(_blur);
            behaviours.Clear();
            behaviours.Add(_behaviour);
        }

        #endregion
    }
}