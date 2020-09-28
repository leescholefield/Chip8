using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Chip8
{
    /// <summary>
    /// Converts a <see cref="Color"/> to a <see cref="Brush"/>
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }
    }
}
