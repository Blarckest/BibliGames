﻿using Modele;
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
            //on change l'item selected pour aller au bon endroit
            (App.Current as App).Manager.ElementSelected = DataContext as Element;
        }
    }
}
