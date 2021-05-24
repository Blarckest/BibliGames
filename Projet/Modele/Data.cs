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


        public void AjoutJeu(LauncherName launcher, string exec)
        {
            Jeu jeu = SearchInfo.ExtractGameInfoFromExec(exec);
            jeu.IsManuallyAdded = true;
            InsertGame(launcher, jeu);
        }

        public void AjoutJeu(Jeu jeu)
        {
            InsertGame(jeu.Launcher, jeu);
            jeu.IsManuallyAdded = true;
        }

        public void ModifDetail(string image, string description, string exec, Jeu elementselected)
        {
            if (elementselected.Image != image && File.Exists(image))
            {
                elementselected.Image = image;
            }
            if (elementselected.Description != description)
            {
                elementselected.Description = description;
            }
            if (elementselected.Exec != exec && File.Exists(exec))
            {
                elementselected.Exec = exec;
            }
        }

        public void SuppJeu(Jeu elementselected)
        {
            (Elements[GetLauncherIndex(elementselected.Launcher)] as Launcher).NbJeux--; //on enleve un jeu au launcher concerné
            Logs.InfoLog($"Suppression du jeu {elementselected.Nom}");
            Elements.Remove(elementselected);
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="dossier"></param>
        public bool AjouterDossier(string dossier)
        {
            if (!Dossiers.Contains(dossier))
            {
                Logs.InfoLog($"Ajout du dossier {dossier}");
                Dossiers.Add(dossier);
                List<Jeu> res = new List<Jeu>();
                List<string> folder = new List<string>(Directory.GetDirectories(dossier));
                SearchForExecutableAndName.SearchForExecutables(res, folder);
                foreach (Jeu jeu in res)
                {
                    AjoutJeu(jeu);
                    Thread thread = new Thread(new ParameterizedThreadStart(SearchInfo.SetInfo));
                    thread.Start(jeu);
                }
                return true;
            }
            return false;
        }

        public bool SuppDossier(string dossier)
        {
            if (Dossiers.Contains(dossier))
            {
                Logs.InfoLog($"Suppression du dossier {dossier}");
                int indexLauncher = GetLauncherIndex(LauncherName.Autre);
                if (indexLauncher != -1)
                {
                    for (int i = indexLauncher + 1; i < Elements.Count; i++)
                    {
                        Jeu jeu = Elements[i] as Jeu;
                        if (Directory.GetParent(jeu.Dossier).FullName == dossier)
                        {
                            Elements.RemoveAt(i);
                            (Elements[indexLauncher] as Launcher).NbJeux--;
                            if ((Elements[indexLauncher] as Launcher).NbJeux == 0)
                            {
                                Elements.RemoveAt(indexLauncher);
                            }
                        }
                    }
                }
                Dossiers.Remove(dossier);
                return true;
            }
            return false;
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

        public int GetLauncherIndex(LauncherName launcherName)
        {
            Launcher temp = new Launcher(launcherName);
            return Elements.IndexOf(temp);
        }
    }
}
