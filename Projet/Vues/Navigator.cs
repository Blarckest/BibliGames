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
        public Manager Manager { get; set; }
        public void Setup(out Manager manager)
        {
            Logs.SuppLog();
            Loader Loader = new LoadElements("Ressources/Sauvegarde");
            manager = new Manager(Loader.Load()); //on load la ssauvegarde
            Manager = manager; //on recupere le manager
        }
        public void Save()
        {
            Saver Saver = new SaveElements("Ressources/Sauvegarde"); 
            Saver.Save(Manager.Data);//on save
        }
        public void OpenParametre()
        {
            Parametre window = new Parametre();
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
                    detail.Content = new User_Controls.DetailLauncher() { DataContext = Manager };
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
