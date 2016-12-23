using System.Windows;
using System.Windows.Media;

namespace BlurryControls.Helper
{
    /// <summary>
    /// provides functionality relating to types <see cref="Brush"/> and <see cref="Color"/>
    /// </summary>
    internal static class ColorHelper
    {
        /// <summary>
        /// returns the currently selected <see cref="SystemParameters.WindowGlassColor"/> with chosen opacity
        /// </summary>
        /// <param name="strength">opacity weight from 0.0 to 1.0 and is set to 0.75 if no value is provided</param>
        /// <returns>the currently selected <see cref="SystemParameters.WindowGlassColor"/> with chosen opacity</returns>
        internal static SolidColorBrush TransparentSystemWindowGlassBrush(double strength = 0.75)
        {
            var defaultSystemMenuBarColor = SystemParameters.WindowGlassColor;
            defaultSystemMenuBarColor.A = (byte)(strength * 255);
            return new SolidColorBrush(defaultSystemMenuBarColor);
        }

        /// <summary>
        /// returns the a given <see cref="Color"/> with chosen opacity
        /// </summary>
        /// <param name="color"></param>
        /// <param name="strength">opacity weight from 0.0 to 1.0 and is set to 0.75 if no value is provided</param>
        /// <returns>a given <see cref="Color"/> with chosen opacity</returns>
        internal static Color ApplyStrength(this Color color, double strength = 0.75)
        {
            color.A = (byte)(strength * 255);
            return color;
        }

        /// <summary>
        /// returns a given <see cref="SolidColorBrush"/> with chosen opacity
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="strength">opacity weight from 0.0 to 1.0 and is set to 0.75 if no value is provided</param>
        /// <returns>a given <see cref="SolidColorBrush"/> with chosen opacity</returns>
        internal static SolidColorBrush ApplyStrength(this SolidColorBrush brush, double strength = 0.75)
        {
            var color = brush.Color;
            color.A = (byte)(strength * 255);
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// returns a given or <see cref="Brush"/> as <see cref="SolidColorBrush"/> with chosen opacity
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="strength">opacity weight from 0.0 to 1.0 and is set to 0.75 if no value is provided</param>
        /// <returns>a given <see cref="SolidColorBrush"/> with chosen opacity</returns>
        internal static SolidColorBrush ApplyStrength(this Brush brush, double strength = 0.75)
        {
            var color = ((SolidColorBrush)brush).Color;
            color.A = (byte)(strength * 255);
            return new SolidColorBrush(color);
        }
    }
}