using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
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
    /// Logique d'interaction pour Parametre.xaml
    /// </summary>
    public partial class Parametre : Window
    {
        public Parametre()
        {
            InitializeComponent();
        }
        public void ParcourirDossiers(object sender, MouseButtonEventArgs e)
        {
            ///marche pas du tout faut faire en sorte que ca ouvre un file explorer qui selectionne que les dossiers
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            dlg.ValidateNames = false;
            dlg.ShowDialog();            
        }
        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SupprimeChemin(object sender, MouseButtonEventArgs e)
        {
            //suppression du chemin selectionné
        }
    }
}