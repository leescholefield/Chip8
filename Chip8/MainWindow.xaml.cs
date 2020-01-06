using Chip8.Emulator;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chip8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Chip8Intepreter Chip8 { get; set; }

        private Grid GameScreen { get; set; }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Chip8 = new Chip8Intepreter();
            Chip8.LoadRom("Roms/Cave.ch8");
            CreateScreen();
            screenRoot.Children.Add(GameScreen);
            keypad.Keys = Chip8.KeypadArray;
        }

        private void CreateScreen()
        {
            GameScreen = new Grid()
            {
                Background = Brushes.White
            };

            // 64
            var NUM_COLS = 64;
            for (int i = 0; i <= NUM_COLS; i++)
            {
                GameScreen.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // 32
            var NUM_ROWS =  32;
            for (int i = 0; i <= NUM_ROWS; i++)
            {
                GameScreen.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }


        int frameCounter;
        private void CompositionTarget_Rendering(object sender, EventArgs args)
        {
            int refreshRate = 60 / 20;
            if (frameCounter % refreshRate == 0)
            {
                Chip8.Cycle();

               if (Chip8.Redraw)
                {
                    DrawScreen();
                    Chip8.Redraw = false;
                 
                }

                opcodeLabel.Content = Chip8.Opcode.ToString("X");
                pcLabel.Content = Chip8.ProgramCounter;
                indexLabel.Content = Chip8.Index;

                register0.Content = "Register 0: " + Chip8.Registers[0];
                register1.Content = "Register 1: " + Chip8.Registers[1];
                register2.Content = "Register 2: " + Chip8.Registers[2];
                register3.Content = "Register 3: " + Chip8.Registers[3];
                register4.Content = "Register 4: " + Chip8.Registers[4];
                register5.Content = "Register 5: " + Chip8.Registers[5];
                register6.Content = "Register 6: " + Chip8.Registers[6];
                register7.Content = "Register 7: " + Chip8.Registers[7];
                register8.Content = "Register 8: " + Chip8.Registers[8];
                register9.Content = "Register 9: " + Chip8.Registers[9];
                register10.Content = "Register 10: " + Chip8.Registers[10];
                register11.Content = "Register 11: " + Chip8.Registers[11];
                register12.Content = "Register 12: " + Chip8.Registers[12];
                register13.Content = "Register 13: " + Chip8.Registers[13];
                register14.Content = "Register 14: " + Chip8.Registers[14];
                register15.Content = "Register 15: " + Chip8.Registers[15];
            }
            frameCounter++;
        }

        private void DrawScreen()
        {
            for (int i = 0; i < Chip8.GraphicsArray.Length; i++)
            {
                if (Chip8.GraphicsArray[i])
                {
                    var fill = Chip8.GraphicsArray[i] == true ? Brushes.Black : Brushes.White;

                    int col = i % 64;
                    int row = i / 64;

                    var rect = new Rectangle
                    {
                        Fill = fill,
                        Width = 10,
                        Height = 10
                    };

                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    GameScreen.Children.Add(rect);
                }
            }
        }
    }
}
