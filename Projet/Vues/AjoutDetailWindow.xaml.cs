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
using System.Windows.Shapes;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour AjoutDetailWindow.xaml
    /// </summary>
    public partial class AjoutDetailWindow : Window
    {
        public AjoutDetailWindow()
        {
            InitializeComponent();
        }

        private void Annuler(object sender, RoutedEventArgs e)
        {
            //annuler modif
            this.Close();
        }
        private void Valider(object sender, RoutedEventArgs e)
        {
            //valider modif
            this.Close();
        }
    }
}
