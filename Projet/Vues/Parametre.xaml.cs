using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour Parametre.xaml
    /// </summary>
    public partial class Parametre : Window
    {
        public Parametre()
        {
            InitializeComponent();
        }
        public void ParcourirDossiers(object sender, MouseButtonEventArgs e)
        {
            //Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //dlg.InitialDirectory = "C:"; // chemin de depart
            //dlg.Title = "Selectionner un dossier"; // nom fenetre
            //dlg.Filter = "Directory|*.a.directory"; // ne montre pas les fichiers seulement les dossier (en donnant comme filtre l'extension:".a.directory"
            //dlg.FileName = "select"; // nom par defaut: "select.a.directory" avec l'extension .a.directory -> chemin\\select.a.directory.a.directory

            //if (dlg.ShowDialog() == true)
            //{
            //    string path = dlg.FileName;
            //    path = path.Replace("\\select.a.directory", ""); //enleve le nom du fichier du chemin -> chemin.a.directory
            //    path = path.Replace(".a.directory", ""); //enleve l'extension -> chemin
            //    if (!System.IO.Directory.Exists(path)) //si l'utilisateur a changer le nom de base on créé le dossier
            //    {
            //        System.IO.Directory.CreateDirectory(path);
            //    }
            //    ListeFolder.Items.Add(path);
            //}
            FolderExplorer FolderExplorer = new FolderExplorer();
            FolderExplorer.ShowDialog();
            string Dossier = FolderExplorer.DossierSelectionner;
            if (Dossier!=null)
            {
                ListeFolder.Items.Add(Dossier);
            }
        }
        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SupprimeChemin(object sender, MouseButtonEventArgs e)
        {
            ListeFolder.Items.Remove(ListeFolder.SelectedItem);
            //suppression du chemin selectionné
        }
    }
}