using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chip8
{
    /// <summary>
    /// Interaction logic for Keypad.xaml
    /// </summary>
    public partial class Keypad : UserControl
    {

        public Emulator.Keypad Keys { get; set; }

        private readonly Dictionary<Button, byte> ButtonDictionary;

        public Keypad()
        {
            InitializeComponent();

            ButtonDictionary = new Dictionary<Button, byte>()
            {
                {button0, 0x0},
                {button1, 0x1},
                {button2, 0x2},
                {button3, 0x3},
                {button4, 0x4},
                {button5, 0x5},
                {button6, 0x6},
                {button7, 0x7},
                {button8, 0x8},
                {button9, 0x9},
                {buttonC, 0xC},
                {buttonD, 0xD},
                {buttonE, 0xE},
                {buttonF, 0xF},
                {buttonA, 0xA},
                {buttonB, 0xB},
            };
        }

        private void Key_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var key = ButtonDictionary[(Button)sender];
            Keys.KeyPressed(key);
        }

        private void Key_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var key = ButtonDictionary[(Button)sender];
            Keys.KeyReleased(key);
        }
    }
}
