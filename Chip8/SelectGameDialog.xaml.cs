using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chip8
{
    /// <summary>
    /// Interaction logic for SelectGameDialog.xaml
    /// </summary>
    public partial class SelectGameDialog : Window
    {

        public Dictionary<string, string> Games { get; private set; }

        /// <summary>
        /// The game the user has selected. Used by the hosting window to retrieve the selection.
        /// </summary>
        public KeyValuePair<string, string> SelectedGame { get; private set; }

        public SelectGameDialog()
        {
            InitializeGamesDictionary();
            DataContext = this;
            InitializeComponent();
        }

        private void InitializeGamesDictionary()
        {
            Games = new Dictionary<string, string>
            {
                {"Animal Race", "Roms/Animal_Race.ch8"},
                {"Cave", "Roms/Cave.ch8"},
                {"Ghost Game", "Roms/gg.ch8"},
                {"Space Invaders", "Roms/space_invaders.ch8"}
            };
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (SelectedGame.Equals(default(KeyValuePair<string, string>))) {
                // no selection show error text
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ((ListBox)sender).SelectedItem;
            if (selected != null)
            {
                SelectedGame = (KeyValuePair<string, string>)selected;
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            if (d.ShowDialog() == true)
            {
                string fileName = d.FileName;
                SelectedGame = new KeyValuePair<string, string>("", fileName);
                // auto close this window
                DialogResult = true;
                Close();
            }
        }
    }
}
