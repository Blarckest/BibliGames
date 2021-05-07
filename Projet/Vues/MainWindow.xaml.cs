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
        public MainWindow()
        {
            Loader Loader = new Stub();
            Manager manager = new Manager();
            manager.Elements = Loader.Load();
            manager.Dossiers = Loader.LoadAdditionalPath();
            DataContext = manager;
            InitializeComponent();
        }

        private void AjoutJeu(object sender, RoutedEventArgs e)
        {
            AjoutJeuWindow Window = new AjoutJeuWindow();
            Window.ShowDialog();
        }
        private void OuvrirParametre(object sender, MouseButtonEventArgs e)
        {
            Parametre window = new Parametre();
            window.ShowDialog();
        }
        private void ChampRechEntre(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text =="Rechercher")
            {
                ((TextBox)sender).Text = ""; //met a vide le champ 
            }
        }
        private void ChampRechQuitter(object sender, RoutedEventArgs e)
        {
            //marche pas
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = "Rechercher";
            }
        }
    }
}
