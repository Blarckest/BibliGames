using DataManager;
using Logger;
using Modele;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Vues
{
    public class Navigator
    {
        public void Setup()
        {
            Logs.SuppLog();
            Loader loader = new LoadElements("Ressources/Sauvegarde");
            (App.Current as App).Manager = new Manager(loader.Load()); //on load la ssauvegarde
        }
        public void Save()
        {
            Saver saver = new SaveElements("Ressources/Sauvegarde"); 
            saver.Save((App.Current as App).Manager.Data);//on save
        }
        public void OpenParametre()
        {
            Parametre window = new Parametre();
            window.ShowDialog();
        }
        public void OpenAjoutJeu()
        {
            AjoutJeuWindow window = new AjoutJeuWindow();
            window.ShowDialog();
        }
        public void OpenAjoutDetail(Jeu jeu)
        {
            AjoutDetailWindow window = new AjoutDetailWindow(jeu);
            window.ShowDialog();
        }

        public string OpenFolderExplorer()
        {
            FolderExplorerView folderExplorer = new FolderExplorerView();
            folderExplorer.ShowDialog();
            return folderExplorer.DossierSelectionner;
        }

        public void UpdateDetail(ListBox listeJeu, ContentControl detail)
        {
            if (listeJeu.SelectedItem != null)
            {
                if (listeJeu.SelectedItem.GetType() == typeof(Launcher))
                {
                    detail.Content = new User_Controls.DetailLauncher() { DataContext = (App.Current as App).Manager };
                }
                else
                {
                    detail.Content = new User_Controls.DetailsJeu() { DataContext = listeJeu.SelectedItem };
                }
            }
            else
            {
                detail.Content = null;
            }
        }

        public void SetupMasterDetail(ContentControl masterDetailCc)
        {
            masterDetailCc.Content = new windowParts.MasterDetail();
        }
    }
}
