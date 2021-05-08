using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
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
            FolderExplorerView FolderExplorer = new FolderExplorerView();
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
        private void TextBoxChemin_TouchEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox TextBoxChemin = sender as TextBox;
                if (Directory.Exists(TextBoxChemin.Text) && !ListeFolder.Items.Contains(TextBoxChemin.Text)) //equivaut a si dossier exist/si on est pas deja a cet endroit
                {
                    ListeFolder.Items.Add(TextBoxChemin.Text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                }
            }
        }
    }
}