using System.Windows;
using System.Windows.Controls;

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
            //modif les details du jeu
            (App.Current as App).Navigator.OpenAjoutDetail();
        }

        private void LancerJeu(object sender, RoutedEventArgs e)
        {
            //lancer le jeu
            (App.Current as App).Manager.LancerJeu();
        }
    }
}
