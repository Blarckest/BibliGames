using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour AjoutDetailWindow.xaml
    /// </summary>
    public partial class AjoutDetailWindow : Window
    {
        public AjoutDetailWindow()
        {
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
            this.Close();
        }
        private void ChercherImage(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = "C:\\Users\\Public\\Pictures\\Sample Pictures";
            dlg.FileName = "Images"; // Default file name
            dlg.DefaultExt = ".jpg | .png"; // Default file extension
            dlg.Filter = "All images files (.jpg, .png)|*.jpg;*.png|JPG files (.jpg)|*.jpg|PNG files (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                textBoxLienImage.Text = filename;
                //mettre filename 
            }
        }
    }
}
