using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BlurryControls.Helper;

namespace BlurryControls.Resources.Colors
{
    public class ColorStrengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();
            double strength;
            var success = double.TryParse(parameter.ToString(), out strength);
            if (!success) strength = 1.0d;

            if (type == typeof(Brush)) return ((Brush)value).OfStrength(strength);
            if (type == typeof(Color)) return ((Color)value).OfStrength(strength).Color;
            if (type == typeof(SolidColorBrush)) return ((SolidColorBrush)value).Invert();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}