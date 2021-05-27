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
            Logs.InfoLog("Demarrage de l'appli");
            Loader loader = new LoadElements("Ressources/Sauvegarde"); //new Stub();   
            (App.Current as App).Manager = new Manager(loader.Load()); //on load la sauvegarde

            if(!File.Exists("./Ressources/Defaut/icone.png") || !File.Exists("./Ressources/Defaut/image.png"))
            {
                Directory.CreateDirectory("./Ressources/Defaut");
                CopyDllRessourceToFile( Assembly.LoadFrom("Icones.dll"), "plus.png", "./Ressources/Defaut/image.png");
                CopyDllRessourceToFile( Assembly.LoadFrom("Icones.dll"), "loupe.png", "./Ressources/Defaut/icone.png");
            }
        }

        public void Save()
        {
            Saver saver = new SaveElements("Ressources/Sauvegarde"); 
            saver.Save((App.Current as App).Manager.Data);//on save
            Logs.InfoLog("Fermeture de l'appli");
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
                detail.Content = null; //on met rien sinon
            }
        }

        public void SetupMasterDetail(ContentControl masterDetailCc)
        {
            masterDetailCc.Content = new windowParts.MasterDetail();
        }

        private MemoryStream ExctractImageFromDll(Assembly assembly,string nomFichier)
        {
            string ressourcePath= assembly.GetName().Name + ".g.resources"; //les ressources sont la dedans
            System.Resources.ResourceReader resourceReader = new System.Resources.ResourceReader(assembly.GetManifestResourceStream(ressourcePath)); //on met un ressourcereader sur le fichier qui nous interesse
            resourceReader.GetResourceData(nomFichier, out _, out byte[] data);//on ne s'interesse pas au type // data contient les données
            return new MemoryStream(data,4,data.Length-4);//on revoit un memorystream le 4 correspond a un offset de 4 octet qui est present pour une raison inconnu
        }

        private void CopyDllRessourceToFile(Assembly assembly, string ressource, string destination)
        {
            var memStream = ExctractImageFromDll(assembly, ressource); //on recupere l'image sous forme de stream
            var fileStream = File.Create(destination); //on creer le fichier
            memStream.CopyTo(fileStream); //on copy les données
            fileStream.Close(); //on ferme
        }
    }
}

