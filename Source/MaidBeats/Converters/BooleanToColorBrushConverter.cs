using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using MetroRadiance.Platform;

namespace MaidBeats.Converters
{
    internal class BooleanToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return new SolidColorBrush(ImmersiveColor.GetColorByTypeName(b ? "ImmersiveLightWUNormal" : "ImmersiveLightWUError"));
            return Application.Current.Resources["ForegroundBrushKey"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}