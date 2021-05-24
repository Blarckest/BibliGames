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
        private IList<string> DossierSupp { get; } = new List<string>();
        public Parametre()
        {
            DataContext = this;
            DossierAffiche = new ObservableCollection<string>((App.Current as App).Manager.Data.Dossiers);
            InitializeComponent();
        }

        private void ParcourirDossiers(object sender, MouseButtonEventArgs e)
        {
            var dossier = (App.Current as App).Navigator.OpenFolderExplorer();
            if (dossier != null)
            {
                DossierAffiche.Add(dossier);
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
                foreach(string dossier in DossierSupp)
                {
                    (App.Current as App).Manager.SuppDossier(dossier);
                }
            }
            foreach (string dossier in DossierAffiche)
            {
                (App.Current as App).Manager.AjouterDossier(dossier);
            }          
            this.Close();
        }

        private void SupprimeChemin(object sender, MouseButtonEventArgs e)
        {
            //suppression du chemin selectionné
            DossierSupp.Add(ListeFolder.SelectedItem as string);
            DossierAffiche.Remove(ListeFolder.SelectedItem as string);            
        }

        private void TextBoxChemin_TouchEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox textBoxChemin = sender as TextBox;
                if (Directory.Exists(textBoxChemin.Text) && !DossierAffiche.Contains(textBoxChemin.Text,StringComparer.OrdinalIgnoreCase)) //equivaut a si dossier exist/si on est pas deja a cet endroit
                {
                    DossierAffiche.Add(textBoxChemin.Text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                }
            }
        }
    }
}