using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Chip8
{
    /// <summary>
    /// Converts a <see cref="Key"/> to its string representation.
    /// </summary>
    public class KeyToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            KeyConverter conv = new KeyConverter();
            return conv.ConvertToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            KeyConverter conv = new KeyConverter();
            return conv.ConvertFrom(value);
        }
    }
}
