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
using Modele;
using System.Collections.ObjectModel;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour Parametre.xaml
    /// </summary>
    public partial class Parametre : Window
    {
        public ObservableCollection<string> DossierAffiche { get; set; }
        private Manager Manager;
        private IList<string> DossierSupp = new List<string>();
        public Parametre(Manager manager)
        {
            Manager = manager;
            DataContext = this;
            DossierAffiche = new ObservableCollection<string>(manager.Data.Dossiers);
            InitializeComponent();
        }
        public void ParcourirDossiers(object sender, MouseButtonEventArgs e)
        {
            FolderExplorerView FolderExplorer = new FolderExplorerView();
            FolderExplorer.ShowDialog();
            string Dossier = FolderExplorer.DossierSelectionner;
            if (Dossier != null)
            {
                DossierAffiche.Add(Dossier);
            }
        }
        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Sauvegarder(object sender, RoutedEventArgs e)
        {
            if (DossierSupp != null)
            {
                foreach(string Dossier in DossierSupp)
                {
                    Manager.SuppDossier(Dossier);
                }
            }
            foreach (string Dossier in DossierAffiche)
            {
                Manager.AjouterDossier(Dossier);
            }          
            this.Close();
        }

        private void SupprimeChemin(object sender, MouseButtonEventArgs e)
        {
            DossierSupp.Add(ListeFolder.SelectedItem as string);
            DossierAffiche.Remove(ListeFolder.SelectedItem as string);
            //suppression du chemin selectionné
        }

        private void TextBoxChemin_TouchEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox TextBoxChemin = sender as TextBox;
                if (Directory.Exists(TextBoxChemin.Text) && !DossierAffiche.Contains(TextBoxChemin.Text,StringComparer.OrdinalIgnoreCase)) //equivaut a si dossier exist/si on est pas deja a cet endroit
                {
                    DossierAffiche.Add(TextBoxChemin.Text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                }
            }
        }
    }
}