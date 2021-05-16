using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Modele;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour AjoutDetailWindow.xaml
    /// </summary>
    public partial class AjoutDetailWindow : Window, INotifyPropertyChanged
    {
        Jeu Jeu;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string executable;
        private string image;

        public string Executable { get => executable; set { executable = value; NotifyPropertyChanged(); } }
        public string Image { get => image; set { image = value; NotifyPropertyChanged(); } }
        public string Description { get; set; }
        public AjoutDetailWindow(Jeu Jeu)
        {
            Executable = Jeu.Exec;
            Image = Jeu.Image;
            Description = Jeu.Description;
            DataContext = this;
            this.Jeu = Jeu;
            InitializeComponent();

        }

        private void Annuler(object sender, RoutedEventArgs e)
        {
            //annuler modif
            this.Close();
        }
        private void Valider(object sender, RoutedEventArgs e)
        {
            //valider modif
            Jeu.Exec = Executable;
            Jeu.Image = Image;
            Jeu.Description = Description;
            this.Close();
        }
        private void ChercherImage(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = "C:\\Users\\Public\\Pictures\\Sample Pictures";
            dlg.Multiselect = false;
            dlg.FileName = "Images"; // Default file name
            dlg.DefaultExt = ".jpg | .png"; // Default file extension
            dlg.Filter = "All images files (.jpg, .png)|*.jpg;*.png|JPG files (.jpg)|*.jpg|PNG files (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                Image = dlg.FileName;

            }
        }

        private void ChercherExec(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = "C:\\";
            dlg.Multiselect = false;
            dlg.FileName = "Executable"; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "All Executables files(.exe) | *.exe"; // Filter files by extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                Executable = dlg.FileName;
            }
        }
    }
}
