using Modele;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vues.User_Controls
{
    /// <summary>
    /// Logique d'interaction pour VignetteJeu.xaml
    /// </summary>
    public partial class VignetteJeu : UserControl
    {
        public VignetteJeu()
        {
            InitializeComponent();
        }

        private void VignetteClicked(object sender, MouseButtonEventArgs e)
        {
            //on change l'item selected pour aller au bon endroit
            (App.Current as App).Manager.ElementSelected = DataContext as Element;
        }
    }
}
