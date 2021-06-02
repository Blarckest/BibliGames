using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows;

namespace Modele
{
    public class Manager : INotifyPropertyChanged
    {
        private Element elementSelected;
        public Element ElementSelected
        {
            get => elementSelected;
            set
            {
                elementSelected = value;
                NotifyPropertyChanged();
            }
        }
        public bool SearchActivated { get; set; } = true;
        public IList<Element> Affichage
        {
            get
            {
                var temp = Data.Elements; //on recupere les elements et on applique recherche dessus
                Recherche(temp);
                return temp;
            }
        }
        public IList<string> Dossiers => Data.Dossiers.ToList(); //le ToList permet d'eviter la modification de dossier depuis l'exterieur
        public List<Element> JeuLauncherSelected
        {
            get
            {
                if (ElementSelected!=null)
                {
                    if (ElementSelected.GetType() == typeof(Launcher))
                    {
                        Launcher launcher = ElementSelected as Launcher;
                        var temp = Data.Elements.Skip(Data.GetLauncherIndex((LauncherName)Enum.Parse(typeof(LauncherName), launcher.Nom)) + 1).Take(launcher.NbJeux).ToList();
                        Recherche(temp);
                        return temp;
                    }
                    return new List<Element> { }; 
                }
                return null;
            }
        }
        public string Pattern { get; set; } = null;
        private Data data;
        public Data Data => data.Clone() as Data; //le clonage permet d'eviter une modification des données non voulu depuis l'exterieur
        private IPersistance Persistance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Manager(IPersistance persistance)
        {
            Persistance = persistance;
            data = Persistance.Load();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            Persistance.Save(Data);
        }

        public void AjoutJeu(LauncherName launcher, string exec)
        {
            if (File.Exists(exec))
            {
                Data.AjoutJeu(launcher, exec);
                NotifyPropertyChanged("Affichage");
                NotifyPropertyChanged("JeuLauncherSelected");
            }
        }

        public void AjoutJeu(Jeu jeu)
        {
            //notify se fait dans la fonction appeler
            Data.AjoutJeu(jeu);
        }

        public void ModifDetail(string image, string description, string exec, string icone)
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                Data.ModifDetail(image, description, exec, icone, ElementSelected as Jeu);
            }
        }

        public void SuppJeu()
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                Data.SuppJeu(ElementSelected as Jeu);
                NotifyPropertyChanged("Affichage");
            }
        }

        public void LancerJeu()
        {
            var elem = ElementSelected as Jeu;
            try
            {
                Logs.InfoLog($"Lancement du jeu {elem.Nom}");
                System.Diagnostics.Process.Start(elem.Exec); //normalement ca marche a tester
            }
            catch
            {
                Logs.ErrorLog($"Lancement du jeu {elem.Nom} imposssible");
                return;
            }
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="dossier"></param>
        public void AjouterDossier(string dossier)
        {
            Data.AjouterDossier(dossier);
            NotifyPropertyChanged("Affichage");
            NotifyPropertyChanged("JeuLauncherSelected");
        }

        public void SuppDossier(string dossier)
        {
            Data.SuppDossier(dossier);
            NotifyPropertyChanged("Affichage");
            NotifyPropertyChanged("JeuLauncherSelected");
        }

        public void UpdateRecherche()
        {
            if (SearchActivated)
            {
                NotifyPropertyChanged("Affichage");
                NotifyPropertyChanged("JeuLauncherSelected");
            }
        }

        //effectue juste une suppression des elements non désiré (correspondant pas au pattern)
        private void Recherche(IList<Element> elements)
        {
            if (!(string.IsNullOrEmpty(Pattern) || Pattern == "Rechercher") && SearchActivated)
            {
                int i = 0;
                while (i < elements.Count)
                {
                    if (elements[i].GetType() == typeof(Jeu) && !elements[i].Nom.Contains(Pattern, StringComparison.OrdinalIgnoreCase)) //si on est sur on jeu et que il correspond pas au pattern
                    {
                        elements.RemoveAt(i);
                        continue;
                    }
                    i++;
                }
            }
        }
    }
}
