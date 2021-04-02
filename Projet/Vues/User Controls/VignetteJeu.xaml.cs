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
            DataContext = this;
        }
        public string NomJeu { get; set; }
        public string LinkToImage { get; set; }
    }
}
