﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Linq;
using FolderExplorerLogic;

namespace Vues
{
    /// <summary>
    /// Interaction logic for FolderExplorer.xaml
    /// </summary>
    public partial class FolderExplorerView : Window
    {
        public string DossierSelectionner { get; set; }
        FolderExplorer FolderExplorer = new FolderExplorer(); 
        public FolderExplorerView()
        {

            DataContext = FolderExplorer;
            InitializeComponent();

        }

        private void ChampRechEntre(object sender, RoutedEventArgs e)
        {
            FolderExplorer.SearchActivated = false;
            if (((TextBox)sender).Text == "Rechercher")
            {
                ((TextBox)sender).Text = ""; //met a vide le champ 
            }
            FolderExplorer.SearchActivated = true;
        }

        private void ChampRechQuitter(object sender, RoutedEventArgs e)
        {
            FolderExplorer.SearchActivated = false;
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = "Rechercher";
            }
            FolderExplorer.SearchActivated = true;
        }
       
        private void Annuler(object sender, RoutedEventArgs e) //appeller lors du clic sur le bouton annuler
        {
            DossierSelectionner = null; //met retour a null
            this.Close();
        }

        private void Selectionner(object sender, RoutedEventArgs e) //appeller lors du clic sur le bouton selectionner
        {
            if (VueDesDossiers.SelectedItem!=null) //on ferme uniquement si un dossier a été selectionner (peut-etre qu'il faudrait changer ce comportement)
            {
                DossierSelectionner = FolderExplorer.GetRepertoireChoisi((LigneExplorateur)VueDesDossiers.SelectedItem);
                this.Close();
            }
        }

        private void GoBackward(object sender, MouseButtonEventArgs e) //fonction appeller pour revenir en arriere
        {
            FolderExplorer.GoBackward();
        }

        private void GoForward(object sender, MouseButtonEventArgs e) //fonction appeller pour aller la ou on etait avant d'aller on arriere
        {
            FolderExplorer.GoForward();
        }

        private void Remonter(object sender, MouseButtonEventArgs e) //fonction appeller pour aller au dossier parent
        {
            FolderExplorer.Remonter();
        }
        
        private void TextBoxChemin_TouchEnterPressed(object sender, KeyEventArgs e) //la textbox a ete modifier cette fonction sert a voir si on peux aller a l'endroit demander
        {
            if (e.Key == Key.Return)
            {
                FolderExplorer.TouchEnterPressed(TextBoxChemin.Text);
            }
        }

        private void UpdateVue(object sender, MouseButtonEventArgs e) //appeler lors d'un double clique sur un element
        {
            FolderExplorer.UpdateVue((LigneExplorateur)VueDesDossiers.SelectedItem);
        }

        private void QuickAccessUsed(object sender, MouseButtonEventArgs e) //appeler lors d'un selection dans le QuickAccess
        {
            FolderExplorer.QuickAccessUsed((LigneExplorateur)QuickAccess.SelectedItem);
        }

        private void Recherche(object sender, TextChangedEventArgs e)//appeler quand le texte de la barre de recherche change
        {
            TextBox TextBox = sender as TextBox;
            FolderExplorer.Recherche(TextBox.Text);
        }
    }
}