using Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Modele
{
    public class Data
    {
        public IList<Element> Elements { get; }
        public IList<string> Dossiers { get; }

        public Data(IList<Element> elements, IList<string> dossiers)
        {
            Elements = new ObservableCollection<Element>(elements);
            Dossiers = dossiers;
        }


        internal void AjoutJeu(LauncherName launcher, string exec)
        {
            Jeu jeu = SearchInfo.ExtractGameInfoFromExec(exec);
            jeu.IsManuallyAdded = true;
            InsertGame(launcher, jeu);
            SetInfo(jeu);
        }

        internal void AjoutJeu(Jeu jeu)
        {
            InsertGame(jeu.Launcher, jeu);
            jeu.IsManuallyAdded = true;
            SetInfo(jeu);
        }

        internal void ModifDetail(string image, string description, string exec, string icone, Jeu elementselected)
        {
            if (elementselected.Image != image && File.Exists(image))
            {
                Logs.InfoLog($"Modification de l'image de {elementselected.Nom} par {image}");
                elementselected.Image = image;
            }
            if (elementselected.Icone != icone && File.Exists(icone))
            {
                Logs.InfoLog($"Modification de l'icone de {elementselected.Nom} par {icone}");
                elementselected.Icone = icone;
            }
            if (elementselected.Description != description)
            {
                Logs.InfoLog($"Modification de la description de {elementselected.Nom} par {description}");
                elementselected.Description = description;
            }
            if (elementselected.Exec != exec && File.Exists(exec))
            {
                Logs.WarningLog($"Modification de l'executable de {elementselected.Nom} par {exec}");
                elementselected.Exec = exec;
            }
        }

        internal void SuppJeu(Jeu elementselected)
        {
            (Elements[GetLauncherIndex(elementselected.Launcher)] as Launcher).NbJeux--; //on enleve un jeu au launcher concerné
            Logs.InfoLog($"Suppression du jeu {elementselected.Nom}");
            Elements.Remove(elementselected);
            int index = GetLauncherIndex(elementselected.Launcher);
            if ((Elements[index] as Launcher).NbJeux == 0)
            {
                Elements.RemoveAt(index);
            }
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="dossier"></param>
        internal bool AjouterDossier(string dossier)
        {
            if (!Dossiers.Contains(dossier))
            {
                Logs.InfoLog($"Ajout du dossier {dossier}");
                Dossiers.Add(dossier);
                List<Jeu> res = new OtherSearcher(dossier).Jeux;
                foreach (Jeu jeu in res)
                {
                    InsertGame(jeu.Launcher, jeu);
                    SetInfo(jeu);
                }
                return true;
            }
            return false;
        }

        internal bool SuppDossier(string dossier)
        {
            if (Dossiers.Contains(dossier))
            {
                Logs.InfoLog($"Suppression du dossier {dossier}");
                int indexLauncher = GetLauncherIndex(LauncherName.Autre);
                if (indexLauncher != -1)
                {
                    int index = indexLauncher + 1;
                    while (index < Elements.Count)
                    {
                        Jeu jeu = Elements[index] as Jeu;
                        if (Directory.GetParent(jeu.Dossier).FullName == dossier)
                        {
                            Elements.RemoveAt(index);
                            (Elements[indexLauncher] as Launcher).NbJeux--;
                            if ((Elements[indexLauncher] as Launcher).NbJeux == 0)
                            {
                                Elements.RemoveAt(indexLauncher);
                            }
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
                Dossiers.Remove(dossier);
                return true;
            }
            return false;
        }

        internal int GetLauncherIndex(LauncherName launcherName)
        {
            Launcher temp = new Launcher(launcherName);
            return Elements.IndexOf(temp);
        }

        private void InsertGame(LauncherName launcher, Jeu jeu)
        {
            if (Elements.Any(e => e.Nom.Equals(launcher.ToString())))
            {
                int index = GetLauncherIndex(launcher);
                Launcher launcherActuel = Elements[index] as Launcher; //garde en memoire l'instance du launcher
                index++;
                while (index != Elements.Count && Elements[index].GetType() == typeof(Jeu) && Elements[index].CompareTo(jeu as Element) < 0)
                {
                    index++; //tant que l'ordre alphabetique est pas respecté
                }
                if (index >= Elements.Count) //si on est a la fin
                {
                    Elements.Add(jeu);
                }
                else
                {
                    Elements.Insert(index, jeu);
                }
                (Elements[index] as Jeu).Launcher = launcher; //on set le launcher associé au jeu
                (Elements.First(e => ReferenceEquals(e, launcherActuel)) as Launcher).NbJeux++; //on ajoute un jeu
            }
            else
            {
                InsertLauncher(launcher); //on insert le launcher
                InsertGame(launcher, jeu); //on relance et on va se retrouver dans le code au dessus car cette fois-ci le launcher existe
            }

        }

        private void InsertLauncher(LauncherName launcher)
        {
            int index = 0;
            if (Elements.Count == 0) //si c'est le premier element inserer
            {
                Elements.Add(new Launcher(launcher));
                return;
            }
            if (launcher != LauncherName.Autre)
            {
                while (index != Elements.Count && (Elements[index] as Launcher).Nom.CompareTo(launcher.ToString()) < 0) //tant que le launcher est pas a sa place dans l'ordre alphabetique
                {
                    index += (Elements[index] as Launcher).NbJeux + 1;
                }
                if (index >= Elements.Count) //si on est a la fin
                {
                    Elements.Add(new Launcher(launcher));
                }
                else
                {
                    Elements.Insert(index, new Launcher(launcher));
                }
            }
            else
            {
                Elements.Add(new Launcher(launcher)); //launcher.autre est obligatoirement en dernier
            }

        }

        private void SetInfo(Jeu jeu)
        {
            Thread thread = new Thread(SearchInfo.SetInfo);
            thread.Start(jeu);
        }
    }
}
