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
        private Manager manager;
        public void Setup(out Manager manager)
        {
            Logs.SuppLog();
            Loader Loader = new Stub("Ressources/Sauvegarde");
            manager = new Manager(Loader.Load()); //on load la ssauvegarde
            this.manager = manager; //on recupere le manager
            (App.Current as App).Navigator = new Navigator();//pour que les enfants de la vue puisse acceder au navigator
        }
        public void Save()
        {
            Saver Saver = new SaveElements("Ressources/Sauvegarde"); 
            Saver.Save(manager.Data);//on save
        }
        public void OpenParametre()
        {
            Parametre window = new Parametre(manager);
            window.ShowDialog();
        }
        public void OpenAjoutJeu()
        {
            AjoutJeuWindow Window = new AjoutJeuWindow();
            Window.ShowDialog();
        }

        public void UpdateDetail(ListBox listeJeu, ContentControl detail)
        {
            if (listeJeu.SelectedItem != null)
            {
                if (listeJeu.SelectedItem.GetType() == typeof(Launcher))
                {
                    detail.Content = new User_Controls.DetailLauncher() { DataContext = manager };
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

        public void SetupMasterDetail(ContentControl masterDetailCC)
        {
            masterDetailCC.Content = new windowParts.MasterDetail();
        }
    }
}
