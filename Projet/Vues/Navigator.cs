using Persistance;
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
                detail.Content = null; //on met rien sinon
            }
        }

        public void SetupMasterDetail(ContentControl masterDetailCc)
        {
            masterDetailCc.Content = new windowParts.MasterDetail();
        }
    }
}

