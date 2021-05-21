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
    public partial class JeuUC : UserControl
    {
        public JeuUC()
        {
            InitializeComponent();
        }

        private void DemandeSuppression(object sender, MouseButtonEventArgs e)
        {
            bool trouver = false;
            var temp = VisualTreeHelper.GetParent(this);
            while (!trouver) //on set le selecteditem a l'item courant
            {
                if (temp.GetType() == typeof(ListBox))
                {
                    (temp as ListBox).SelectedItem = DataContext;
                    trouver = true;
                }
                temp = VisualTreeHelper.GetParent(temp);
            }

            if (MessageBox.Show("Supprimer ce jeu?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                (App.Current as App).Navigator.Manager.SuppJeu();
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
