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
            if (ListeJeu.SelectedItem!=null)
            {
                if (ListeJeu.SelectedItem.GetType() == typeof(Launcher))
                {
                    Detail.Content = new User_Controls.DetailLauncher() { DataContext = this.DataContext };
                }
                else
                {
                    Detail.Content = new User_Controls.DetailsJeu() { DataContext = ListeJeu.SelectedItem };
                }
            }
            else
            {
                Detail.Content = null;
            }
        }
    }
}
