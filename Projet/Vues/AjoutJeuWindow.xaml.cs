using Modele;
using System.Windows;
using System.Windows.Input;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour AjoutJeuWindow.xaml
    /// </summary>
    public partial class AjoutJeuWindow : Window
    {
        public AjoutJeuWindow()
        {
            InitializeComponent();
        }
        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Valider(object sender, RoutedEventArgs e)
        {
            //valider modif
            (App.Current as App).Manager.AjoutJeu((LauncherName)ListeLuncher.SelectedItem, textBoxLienExe.Text);
            this.Close();
        }

        private void ChercherExecutable(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                InitialDirectory = "C:\\Users",
                Multiselect = false,
                FileName = "Executable", // Default file name
                DefaultExt = ".exe", // Default file extension
                Filter = "All Executables files (.exe) | *.exe", // Filter files by extension
            };
            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // update la textbox
                textBoxLienExe.Text = dlg.FileName;
            }
        }
    }
}
