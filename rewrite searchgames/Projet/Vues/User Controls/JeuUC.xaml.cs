using Modele;
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

namespace Vues.User_Controls
{
    /// <summary>
    /// Logique d'interaction pour JeuUC.xaml
    /// </summary>
    public partial class JeuUc : UserControl
    {
        public JeuUc()
        {
            InitializeComponent();
        }

        private void DemandeSuppression(object sender, MouseButtonEventArgs e)
        {
            //on met l'itemselected au jeux clique
            (App.Current as App).Manager.ElementSelected = DataContext as Element;
            if (MessageBox.Show("Supprimer ce jeu?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                (App.Current as App).Manager.SuppJeu(); //on supp si oui
            }
        }

        private void MontrerCroix(object sender, MouseEventArgs e)
        {
            CroixSupp.Visibility = Visibility.Visible;
        }

        private void CacherCroix(object sender, MouseEventArgs e)
        {
            CroixSupp.Visibility = Visibility.Hidden;
        }
    }
}
