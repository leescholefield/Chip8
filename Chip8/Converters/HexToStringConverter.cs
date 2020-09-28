using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Chip8
{
    public class HexToStringConverter : IValueConverter
    {

        private Dictionary<byte, string> ValueDictionary = new Dictionary<byte, string>
        {
            {0x0, "0" },
            {0x1, "1"},
            {0x2, "2"},
            {0x3, "3"},
            {0x4, "4"},
            {0x5, "5"},
            {0x6, "6"},
            {0x7, "7"},
            {0x8, "8"},
            {0x9, "9"},
            {0xA, "A"},
            {0xB, "B"},
            {0xC, "C"},
            {0xD, "D"},
            {0xE, "E"},
            {0xF, "F"}
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte b = (byte)value;
            return ValueDictionary[b];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ValueDictionary.FirstOrDefault(x => x.Value == (string)value).Key;
        }
    }
}
