using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Modele;

namespace Vues.User_Controls
{
    /// <summary>
    /// Logique d'interaction pour DetailsJeu.xaml
    /// </summary>
    public partial class DetailsJeu : UserControl
    {
        public DetailsJeu()
        {
            InitializeComponent();
        }

        private void ModifierJeu(object sender, RoutedEventArgs e)
        {
            AjoutDetailWindow window = new AjoutDetailWindow(DataContext as Jeu);
            window.ShowDialog();
        }

        private void LancerJeu(object sender, RoutedEventArgs e)
        {
            //lancer le jeu
            (App.Current as App).Navigator.Manager.LancerJeu();
        }
    }
}
