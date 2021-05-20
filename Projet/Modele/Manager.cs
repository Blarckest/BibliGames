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
        public IList<Element> Affichage
        {
            get
            {
                var temp = Data.Elements;
                Recherche(temp);
                return temp;
            }
        }
        public IList<string> Dossiers
        {
            get
            {
                return Data.Dossiers;
            }
        }
        public List<Element> JeuLauncherSelected
        {
            get
            {
                if (ElementSelected.GetType()==typeof(Launcher))
                {
                    Launcher launcher = ElementSelected as Launcher;
                    var temp = Data.Elements.Skip(Data.GetLauncherIndex((LauncherName)Enum.Parse(typeof(LauncherName), launcher.Nom))+1).Take(launcher.NbJeux).ToList();
                    return temp;
                }
                return new List<Element>{ };
            }
        }
        public Element ElementSelected { get; set; }
        public string Pattern { get; set; } = null;
        public Data Data { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Manager(Data data)
        {
            Data = data;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AjoutJeu(LauncherName Launcher, string Exec)
        {
            if (File.Exists(Exec))
            {
                Data.AjoutJeu(Launcher, Exec);
            }
        }

        public void AjoutJeu(Jeu Jeu)
        {
            Data.AjoutJeu(Jeu);
        }

        public void ModifDetail(string Image, string Description, string Exec)
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                Data.ModifDetail(Image, Description, Exec, ElementSelected as Jeu);
            }
        }

        public void SuppJeu()
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                Data.SuppJeu(ElementSelected as Jeu);
            }
        }

        public void LancerJeu()
        {
            var Elem = ElementSelected as Jeu;
            try
            {
                Logs.InfoLog($"Lancement du jeu {Elem.Nom}");
                System.Diagnostics.Process.Start(Elem.Exec); //normalement ca marche a tester
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="Dossier"></param>
        public void AjouterDossier(string Dossier)
        {
            Data.AjouterDossier(Dossier);            
        }

        public void SuppDossier(string Dossier)
        {
            Data.SuppDossier(Dossier);
        }

        public void UpdateRecherche()
        {
            if (SearchActivated)
            {
                NotifyPropertyChanged("Affichage");
            }
        }

        //effectue juste une suppression des elements non désiré (correspondant pas au pattern)
        public void Recherche(IList<Element> elements)
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
