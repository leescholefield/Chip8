using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chip8
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window, INotifyPropertyChanged
    {

        /// <summary>
        /// Updated settings. The host window will access this after it is closed.
        /// </summary>
        public Chip8Settings NewSettings { get; private set; }

        /// <summary>
        /// Just NewSettings.KeyBindings stored in a wrapper class. This makes it easier for the DataGrid in our view to edit it.
        /// </summary>
        public List<OptionsInput> OptionsInputs { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;

        #region Color

        private KeyValuePair<string, Color> _selectedOnColor;
        public KeyValuePair<string, Color> SelectedOnColor
        {
            get
            {
                return _selectedOnColor;
            }
            set
            {
                _selectedOnColor = value;
                OnPropertyChanged();
            }
        }

        private KeyValuePair<string, Color> _selectedOffColor;
        public KeyValuePair<string, Color> SelectedOffColor 
        { 

            get
            {
                return _selectedOffColor;
            }
            set
            {
                _selectedOffColor = value;
                OnPropertyChanged();
            }
        }


        public Dictionary<string, Color> ColorDictionary { get; set; }

        #endregion

        public Options(Chip8Settings currentSettings)
        {
            LoadColors();
            CopySettings(currentSettings);
            InitializeComponent();
            DataContext = this;
        }

        private void LoadColors()
        {
            ColorDictionary = new Dictionary<string, Color>()
            {
                {"Red", Colors.Red },
                {"Dark Red", Colors.DarkRed },
                {"Blue", Colors.Blue },
                {"Light Blue", Colors.LightBlue },
                {"Dark Blue", Colors.DarkBlue },
                {"Green", Colors.Green },
                {"Light Green", Colors.LightGreen },
                {"Dark Green", Colors.DarkGreen },
                {"Black", Colors.Black },
                {"White", Colors.White}
            };
        }

        private void CopySettings(Chip8Settings currentSettings)
        {
            NewSettings = new Chip8Settings
            {
                OnColor = currentSettings.OnColor,
                OffColor = currentSettings.OffColor,
                KeyBindings = currentSettings.KeyBindings.ToDictionary(a => a.Key, a => a.Value)
            };

            // copy keybindings to wrapper class. This is to make it easier for view to edit it
            OptionsInputs = new List<OptionsInput>();
            foreach (KeyValuePair<Key, byte> pair in NewSettings.KeyBindings)
            {
                OptionsInputs.Add(new OptionsInput
                {
                    Key = pair.Key,
                    Chip8Input = pair.Value
                });
            }

            // only way this wont find a value is if we set it to a color not currently in ColorDictionary
            SelectedOnColor = ColorDictionary.FirstOrDefault(x => x.Value == NewSettings.OnColor);
            SelectedOffColor = ColorDictionary.FirstOrDefault(x => x.Value == NewSettings.OffColor);
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            // set on and off pixel
            NewSettings.OnColor = SelectedOnColor.Value;
            NewSettings.OffColor = SelectedOffColor.Value;

            // copy key bindings
            Dictionary<Key, byte> bindings = new Dictionary<Key, byte>();
            foreach (var i in OptionsInputs)
            {
                bindings[i.Key] = i.Chip8Input;
            }

            NewSettings.KeyBindings = bindings;

            DialogResult = true;
            Close();
        }

        private void bindings_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            var selectedCell = grid.SelectedItem;
            if (selectedCell != null)
            {
                OptionsInput i = (OptionsInput)selectedCell;
                HexToStringConverter converter = new HexToStringConverter();
                string chip8Key = (string)converter.Convert(i.Chip8Input, null, null, null);

                // launch dialog and wait for result
                Key? newKey = LaunchKeyChangeDialog(chip8Key);
                if (newKey.HasValue)
                {
                    i.Key = newKey.Value;
                }
            }
        }

        private Key? LaunchKeyChangeDialog(string keyChanging)
        {
            ChangeKeyDialog dialog = new ChangeKeyDialog(keyChanging);
            if (dialog.ShowDialog() == true)
            {
                return dialog.NewKey;
            }

            return null;
        }
    }

    public class OptionsInput : INotifyPropertyChanged
    {
        private Key key;
        public Key Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The byte we use to represent the chip8 input.
        /// </summary>
        public byte Chip8Input { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
