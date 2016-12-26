using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BlurryControls.Helper;

namespace BlurryControls.Resources.Colors
{
    public class InvertColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();

            if (type == typeof(Brush)) return ((Brush)value).Invert();
            if (type == typeof(Color)) return ((Color)value).Invert();
            if (type == typeof(SolidColorBrush)) return ((SolidColorBrush)value).Invert();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}