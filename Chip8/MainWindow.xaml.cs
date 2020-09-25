using Chip8.Emulator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chip8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Chip8Intepreter Chip8 { get; set; }

        private WriteableBitmap GameScreen { get; set; }

        private static Color PIXEL_ON = Colors.Green;
        private static Color PIXEL_OFF = Colors.LightGreen;

        /// <summary>
        /// Maps a Input.Key to a byte value
        /// </summary>
        private readonly Dictionary<Key, byte> KeyDictionary = new Dictionary<Key, byte> 
        {
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
                {Key.C, 0xC},
                {Key.D, 0xD},
                {Key.E, 0xE},
                {Key.F, 0xF},
                {Key.A, 0xA},
                {Key.B, 0xB}
        };

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Chip8 = new Chip8Intepreter();
            Chip8.LoadRom("Roms/space_invaders.ch8");

            CreateScreen();
            Image i = new Image { Width = 600, Height = 300 };
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
            i.Source = GameScreen;
            screenRoot.Children.Add(i);
        }

        private void CreateScreen()
        {
            GameScreen = new WriteableBitmap(64, 34, 50, 50, PixelFormats.Bgr32, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Key_Down(object sender, KeyEventArgs e)
        {
            if (KeyDictionary.TryGetValue(e.Key, out byte pressed))
            {
                Chip8.KeypadArray.KeyPressed(pressed);
            }
        }

        private void Key_Up(object sender, KeyEventArgs e)
        {
            if (KeyDictionary.TryGetValue(e.Key, out byte pressed))
            {
                Chip8.KeypadArray.KeyReleased(pressed);
            }
        }


        int frameCounter;
        private void CompositionTarget_Rendering(object sender, EventArgs args)
        {
            int refreshRate = 1; // refreshing every hit
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
            }
            frameCounter++;
        }

        /// <summary>
        /// Writes the values in <see cref="Chip8Intepreter.GraphicsArray"/> into the <see cref="GameScreen"/>.
        /// 
        /// The process for this is somewhat inefficient at the moment, since it will have to loop over the entire GraphicsArray and 
        /// then set the corresponding pixel color in the GameScreen to <see cref="PIXEL_ON"/> or <see cref="PIXEL_OFF"/>, depending on 
        /// if the value in GraphicsArray is true or false.
        /// 
        /// In the future I would like to have a system where the GraphicsArray keeps track of the pixel positions whose value has changed
        /// so this function does not have to loop over the entire array.
        /// </summary>
        private void DrawScreen()
        {
            try
            {
                // stop back buffer updating
                GameScreen.Lock();

                for (int i = 0; i < Chip8.GraphicsArray.Length; i++)
                {
                    var fill = Chip8.GraphicsArray[i] == true ? PIXEL_ON : PIXEL_OFF;

                    int rgbColor = fill.R << 16;
                    rgbColor |= fill.G << 8;
                    rgbColor |= fill.B << 0;

                    // get the column and row in the GameScreen
                    int col = i % 64;
                    int row = i / 64;

                    unsafe
                    {
                        IntPtr backBuffer = GameScreen.BackBuffer;
                        backBuffer += row * GameScreen.BackBufferStride;
                        backBuffer += col * 4;

                        *((int*)backBuffer) = rgbColor;
                    }
                }
                // since we are updating the entire backbuffer we can just set the dirt region to cover the entire screen
                GameScreen.AddDirtyRect(new Int32Rect(0, 0, GameScreen.PixelWidth, GameScreen.PixelHeight));
            }
            finally
            {
                // release the back buffer so the screen can be updated
                GameScreen.Unlock();
            }
        }
    }
}
