using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Chip8
{
    /// <summary>
    /// Contains various alterable settings for the Chip8 interpreter, such as key bindings and pixel colors. These will be initialised 
    /// with default values but they can be overriten.
    /// </summary>
    public class Chip8Settings
    {
        
        private Color _onColor = Colors.DarkBlue;
        /// <summary>
        /// Color of a pixel when it is on (meaning has a value of 1 or true).
        /// </summary>
        public Color OnColor { get
            {
                return _onColor;
            }
            set
            {
                _onColor = value;
                OnRGBValue = ComputeRgbValue(_onColor);
            }
        }
        

        /// <summary>
        /// An int representing the RGB value of <see cref="OnColor"/>
        /// </summary>
        public int OnRGBValue { get; private set; }

        private Color _offColor = Colors.LightBlue;
        /// <summary>
        /// Color of a pixel when it is off (meaning has a value of 0 or false).
        /// </summary>
        public Color OffColor { 
            get 
            {
                return _offColor;
            } 
            set 
            {
                _offColor = value;
                OffRGBValue = ComputeRgbValue(_offColor);
            }
        }

        /// <summary>
        /// An Int representing the RGB value of <see cref="OffColor"/>.
        /// </summary>
        public int OffRGBValue { get; private set; }

        private int ComputeRgbValue(Color color)
        {
            int rgbColor = color.R << 16;
            rgbColor |= color.G << 8;
            rgbColor |= color.B << 0;

            return rgbColor;
        }

        public Dictionary<Key, byte> KeyBindings { get; set; } = new Dictionary<Key, byte> {
                {Key.NumPad0, 0x0 },
                {Key.NumPad1, 0x1},
                {Key.NumPad2, 0x2},
                {Key.NumPad3, 0x3},
                {Key.NumPad4, 0x4},
                {Key.NumPad5, 0x5},
                {Key.NumPad6, 0x6},
                {Key.NumPad7, 0x7},
                {Key.NumPad8, 0x8},
                {Key.NumPad9, 0x9},
                {Key.A, 0xA},
                {Key.B, 0xB},
                {Key.C, 0xC},
                {Key.D, 0xD},
                {Key.E, 0xE},
                {Key.F, 0xF}
        };

    }
}
