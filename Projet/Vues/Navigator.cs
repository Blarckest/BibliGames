using DataManager;
using Logger;
using Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Controls;

namespace Vues
{
    public class Navigator
    {
        public void Setup()
        {
            Logs.SuppLog();
            Loader loader = new Stub();   // new LoadElements("Ressources/Sauvegarde");
            (App.Current as App).Manager = new Manager(loader.Load()); //on load la sauvegarde

            if(!File.Exists("./Ressources/Defaut/icone.png") || !File.Exists("./Ressources/Defaut/image.png"))
            {
                Directory.CreateDirectory("./Ressources/Defaut");
            }
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
        public void OpenAjoutDetail()
        {
            AjoutDetailWindow window = new AjoutDetailWindow();
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
