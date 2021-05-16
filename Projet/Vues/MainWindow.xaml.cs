using DataManager;
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
        public Manager manager = new Manager();
        public MainWindow()
        {
            Loader Loader = new LoadElements("Ressources/Sauvegarde");
            manager.Elements = Loader.Load();
            manager.Dossiers = Loader.LoadAdditionalPath();
            DataContext = manager;
            InitializeComponent();
            manager.SearchActivated = false;
            BarreDeRecherche.Text = "Rechercher";
            manager.SearchActivated = true;
        }

        private void AjoutJeu(object sender, RoutedEventArgs e)
        {
            AjoutJeuWindow Window = new AjoutJeuWindow();
            Window.ShowDialog();
        }
        private void OuvrirParametre(object sender, MouseButtonEventArgs e)
        {
            Parametre window = new Parametre(manager);
            window.ShowDialog();
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
            Saver Saver = new SaveElements("Ressources/Sauvegarde");
            Saver.Save(manager);
        }

        private void Recherche(object sender, TextChangedEventArgs e)//appeler quand le texte de la barre de recherche change
        {
            if (manager.SearchActivated)
            {
                manager.UpdateAffichage();
            }
        }
    }
}
