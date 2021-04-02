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
            InitializeComponent();
        }

        private void AjoutJeu(object sender, RoutedEventArgs e)
        {
            //ajouter un nouveau Jeu
        }
        private void OuvrirParametre(object sender, MouseButtonEventArgs e)
        {
            //ouvrir fenetre paramatre
        }
        private void ChampRechEntre(object sender, MouseButtonEventArgs e)
        {
            ((TextBlock)sender).Text = ""; //met a vide le champ
        }
        private void ChampRechQuitter(object sender, MouseButtonEventArgs e)
        {
            //marche pas
            if (((TextBlock)sender).Text == "")
            {
                ((TextBlock)sender).Text = "Rechercher";
            }
        }
    }
}
