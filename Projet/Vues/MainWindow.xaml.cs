using DataManager;
using Logger;
using Modele;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vues
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Manager manager;
        public MainWindow()
        {
            (App.Current as App).Navigator.Setup(out manager);
            DataContext = manager;
            InitializeComponent();
            (App.Current as App).Navigator.SetupMasterDetail(MasterDetailCC);
            manager.SearchActivated = false;
            BarreDeRecherche.Text = "Rechercher";
            manager.SearchActivated = true;
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
            manager.SearchActivated = false;
            if (((TextBox)sender).Text =="Rechercher")
            {
                ((TextBox)sender).Text = ""; //met a vide le champ 
            }
            manager.SearchActivated = true;
        }
        private void ChampRechQuitter(object sender, RoutedEventArgs e)
        {
            manager.SearchActivated = false;
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = "Rechercher";
            }
            manager.SearchActivated = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (App.Current as App).Navigator.Save();
        }

        private void Recherche(object sender, TextChangedEventArgs e)//appeler quand le texte de la barre de recherche change
        {
            if (manager.SearchActivated)
            {
                manager.UpdateRecherche();
            }
        }
    }
}
