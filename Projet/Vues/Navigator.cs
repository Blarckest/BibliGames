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
                //ta besoin de recup le stream dde la ressource a partir de l'assembly Icones.dll (le nom du manifest c'est Icones.g.ressources
                //ensuite tu recup la donnée qui t'interesse et tu la met dans un byte[]
                //ensuite une fois que ta ce tableau tu le met dans un memorystream avec un offset de 4.
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
            window.ShowDialog(); //on ouvre en bloquant
        }
        public void OpenAjoutJeu()
        {
            AjoutJeuWindow window = new AjoutJeuWindow();
            window.ShowDialog();//on ouvre en bloquant
        }
        public void OpenAjoutDetail()
        {
            AjoutDetailWindow window = new AjoutDetailWindow();
            window.ShowDialog();//on ouvre en bloquant
        }

        public string OpenFolderExplorer()
        {
            FolderExplorerView folderExplorer = new FolderExplorerView();
            folderExplorer.ShowDialog();//on ouvre en bloquant
            return folderExplorer.DossierSelectionner; //on renvoie le dossier selectionner par l'utilisateur a la fonction appelante
        }

        public void UpdateDetail(ListBox listeJeu, ContentControl detail)
        {
            if (listeJeu.SelectedItem != null)
            {
                if (listeJeu.SelectedItem.GetType() == typeof(Launcher))
                {
                    detail.Content = new User_Controls.DetailLauncher() { DataContext = (App.Current as App).Manager }; //si l'item est un launcher on met le Content du CC avec un detailLauncher avec le bon datacontext
                }
                else
                {
                    detail.Content = new User_Controls.DetailsJeu() { DataContext = listeJeu.SelectedItem }; //pareil mais avec Jeu
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
