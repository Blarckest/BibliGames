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
            //on trouve le manager est on change l'item selected pour aller au bon endroit
            bool trouver = false;
            var temp = VisualTreeHelper.GetParent(this);
            while (!trouver) //tant qu'on trouve pas un manager on remonte
            {
                if (temp.GetType() == typeof(windowParts.MasterDetail))
                {
                    (temp.GetValue(DataContextProperty) as Manager).ElementSelected = DataContext as Element;
                    trouver = true;
                }
                temp = VisualTreeHelper.GetParent(temp);
            }
        }
    }
}
