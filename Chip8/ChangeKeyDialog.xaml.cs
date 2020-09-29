using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Chip8
{
    /// <summary>
    /// Interaction logic for ChangeKeyDialog.xaml
    /// </summary>
    public partial class ChangeKeyDialog : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string KeyChanging { get; private set; }

        // to detect whether the user has inputed a new key when the ok button is clicked
        private bool newKeyRegistered = false;
        private Key newKey;
        public Key NewKey { 
            get{
                return newKey;
            }
            set
            {
                newKey = value;
                OnPropertyChanged();
            }
        }

        public ChangeKeyDialog(string keyChanging)
        {
            KeyChanging = keyChanging;
            this.DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Attatches the <see cref="KeyDownCallback"/> to the Window KeyDown Event.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += new KeyEventHandler(KeyDownCallback);
        }

        private void KeyDownCallback(object sender, KeyEventArgs e)
        {
            // ignore it if it is space
            if (e.Key == Key.Space)
            {
                input_error_text.Content = "Space key is reserved for pausing the game";
                input_error_text.Visibility = Visibility.Visible;
                return;
            }
            else
            {

                NewKey = e.Key;
                newKeyRegistered = true;
                // doesn't matter if its already hidden. Not much cost
                input_error_text.Visibility = Visibility.Hidden;
            }

            // stop child key pressed events being triggered
            // to handle a bug where 'o' key was propogating to the 'OK' button
            e.Handled = true;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (!newKeyRegistered)
            {
                input_error_text.Visibility = Visibility.Visible;
                return;
            }
            this.DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
