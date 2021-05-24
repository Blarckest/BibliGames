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
        public bool SearchActivated = true;
        private Element elementSelected;

        public IList<Element> Affichage
        {
            get
            {
                var temp = Data.Elements.ToList(); //on recupere les elements et on applique recherche dessus
                Recherche(temp);
                return temp;
            }
        }
        public IList<string> Dossiers => Data.Dossiers;

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
                        return temp;
                    }
                    return new List<Element> { }; 
                }
                return null;
            }
        }
        public Element ElementSelected
        {
            get => elementSelected;
            set
            {
                elementSelected = value;
                NotifyPropertyChanged();
            }
        }
        public string Pattern { get; set; } = null;
        public Data Data { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Manager(Data data)
        {
            Data = data;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AjoutJeu(LauncherName launcher, string exec)
        {
            if (File.Exists(exec))
            {
                Data.AjoutJeu(launcher, exec);
                NotifyPropertyChanged("Affichage");
            }
        }

        public void AjoutJeu(Jeu jeu)
        {
            //notify se fait dans la fonction appeler
            Data.AjoutJeu(jeu);
        }

        public void ModifDetail(string image, string description, string exec)
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                Data.ModifDetail(image, description, exec, ElementSelected as Jeu);
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
        }

        public void SuppDossier(string dossier)
        {
            Data.SuppDossier(dossier);
            NotifyPropertyChanged("Affichage");
        }

        public void UpdateRecherche()
        {
            if (SearchActivated)
            {
                NotifyPropertyChanged("Affichage");
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
