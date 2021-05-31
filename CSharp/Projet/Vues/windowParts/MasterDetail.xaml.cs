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

namespace Vues.windowParts
{
    /// <summary>
    /// Interaction logic for MasterDetail.xaml
    /// </summary>
    public partial class MasterDetail : UserControl
    {
        public MasterDetail()
        {
            InitializeComponent();
        }

        private void ListeJeu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).Navigator.UpdateDetail(ListeJeu,Detail);            
        }
    }
}
