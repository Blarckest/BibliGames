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
using System.Windows.Shapes;

namespace Vues
{
    /// <summary>
    /// Logique d'interaction pour AjoutJeuWindow.xaml
    /// </summary>
    public partial class AjoutJeuWindow : Window
    {
        public AjoutJeuWindow()
        {
            InitializeComponent();
        }
        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Valider(object sender, RoutedEventArgs e)
        {
            //valider modif
            (App.Current as App).Manager.AjoutJeu((LauncherName)ListeLuncher.SelectedItem, textBoxLienExe.Text);
            this.Close();
        }

        private void ChercherExecutable(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                InitialDirectory = "C:\\Users",
                Multiselect = false,
                FileName = "Executable", // Default file name
                DefaultExt = ".exe", // Default file extension
                Filter = "All Executables files (.exe) | *.exe", // Filter files by extension
            };
            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // update la textbox
                textBoxLienExe.Text = dlg.FileName;
            }
        }
    }
}
