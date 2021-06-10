using Logger;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vues
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Logs.SuppLog();
            Logs.InfoLog("Demarrage de l'appli");
            InitializeComponent();
            DataContext = (App.Current as App).Manager;
            (App.Current as App).Navigator.SetupMasterDetail(MasterDetailCC);
            (App.Current as App).Manager.SearchActivated = false;
            BarreDeRecherche.Text = "Rechercher";
            (App.Current as App).Manager.SearchActivated = true;
        }

        private void AjoutJeu(object sender, RoutedEventArgs e)
        {
            (App.Current as App).Navigator.OpenAjoutJeu();
        }
        private void OuvrirParametre(object sender, MouseButtonEventArgs e)
        {
            (App.Current as App).Navigator.OpenParametre();
        }
        private void ChampRechEntre(object sender, RoutedEventArgs e)
        {
            (App.Current as App).Manager.SearchActivated = false;
            if (((TextBox)sender).Text == "Rechercher")
            {
                ((TextBox)sender).Text = ""; //met a vide le champ 
            }
            (App.Current as App).Manager.SearchActivated = true;
        }
        private void ChampRechQuitter(object sender, RoutedEventArgs e)
        {
            (App.Current as App).Manager.SearchActivated = false;
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = "Rechercher";
            }
            (App.Current as App).Manager.SearchActivated = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (App.Current as App).Manager.Save();
            Logs.InfoLog("Fermeture de l'application");
        }

        private void Recherche(object sender, TextChangedEventArgs e)//appeler quand le texte de la barre de recherche change
        {
            if ((App.Current as App).Manager.SearchActivated)
            {
                (App.Current as App).Manager.UpdateRecherche();
            }
        }

        private void Raccourcis(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.P)
                {
                    (App.Current as App).Navigator.OpenParametre();
                }
            }
        }
    }
}
