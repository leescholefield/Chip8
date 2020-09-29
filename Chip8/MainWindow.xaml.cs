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
    /// 
    /// TODO methods for changing keybindings and pixel colors
    /// </summary>
    public partial class MainWindow : Window
    {

        public Chip8Intepreter Chip8 { get; set; }

        private WriteableBitmap GameScreen { get; set; }

        private Chip8Settings UserSettings = new Chip8Settings();

        private bool gamePaused = false;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Chip8 = new Chip8Intepreter();
            LoadGame("Roms/space_invaders.ch8");
        }

        private void LoadGame(string loc)
        {
            Chip8.LoadRom(loc);
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
        
        #region Screen drawing

        /// <summary>
        /// This is only used to demonstrate it works; it was designed for simple annimations, not to render 2D games (no matter 
        /// how graphically simple). Now I've got the rest of the program working as intended I need to think about removing this.
        /// </summary>
        private void CompositionTarget_Rendering(object sender, EventArgs args)
        {
            if (gamePaused)
                return;

            Chip8.Cycle();

            if (Chip8.Redraw)
            {
                DrawScreen();
                Chip8.Redraw = false;
            }


            opcodeLabel.Content = Chip8.Opcode.ToString("X");
            pcLabel.Content = Chip8.ProgramCounter;
        }

        /// <summary>
        /// Writes the values in <see cref="Chip8Intepreter.GraphicsArray"/> into the <see cref="GameScreen"/>.
        /// 
        /// The process for this is somewhat inefficient at the moment, since it will have to loop over the entire GraphicsArray and 
        /// then set the corresponding pixel color in the GameScreen to UserSettings.OnColor or UserSettings.OfColor, depending on 
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
                    var fill = Chip8.GraphicsArray[i] == true ? UserSettings.OnColor : UserSettings.OffColor;

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

        #endregion

        #region Input handling

        private void Key_Down(object sender, KeyEventArgs e)
        {
            // special case for pause button
            if (e.Key == Key.Space)
            {
                if (gamePaused)
                {
                    paused_screen.Visibility = Visibility.Hidden;
                    gamePaused = false;
                }
                else
                {
                    paused_screen.Visibility = Visibility.Visible;
                    gamePaused = true;
                }
            }

            if (UserSettings.KeyBindings.TryGetValue(e.Key, out byte pressed))
            {
                Chip8.KeypadArray.KeyPressed(pressed);
            }

            e.Handled = true;
        }

        private void Key_Up(object sender, KeyEventArgs e)
        {
            if (UserSettings.KeyBindings.TryGetValue(e.Key, out byte pressed))
            {
                Chip8.KeypadArray.KeyReleased(pressed);
            }
        }

        #endregion

        private void optionsButton_Click(object sender, RoutedEventArgs e)
        {
            gamePaused = true;
            paused_screen.Visibility = Visibility.Visible;

            Window w = new Options(UserSettings);
            if (w.ShowDialog() == true)
            {
                var newSettings = ((Options)w).NewSettings;
                if (newSettings != null)
                {
                    UserSettings = newSettings;
                }

               
            } 
            
            screenRoot.Focus();
        }

        private void GameSelectButton_Click(object sender, RoutedEventArgs e)
        {
            gamePaused = true;
            paused_screen.Visibility = Visibility.Visible;

            Window w = new SelectGameDialog();
            if (w.ShowDialog() == true)
            {
                KeyValuePair<string, string> game = ((SelectGameDialog)w).SelectedGame;
                LoadGame(game.Value);

                gamePaused = false;
                paused_screen.Visibility = Visibility.Hidden;
            }
        }
    }
}
