using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BlurryControls.Helpers;

namespace BlurryControls.Resources.Converter
{
    public class InvertColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value?.GetType();
            double strength;
            var success = double.TryParse(parameter.ToString(), out strength);
            if (!success) strength = 1.0d;

            if (type == typeof(Brush)) return ((Brush)value).Invert().OfStrength(strength);
            if (type == typeof(Color)) return ((Color)value).Invert().OfStrength(strength);
            if (type == typeof(SolidColorBrush)) return ((SolidColorBrush)value).Invert().OfStrength(strength);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}