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
        public IList<Element> Elements { get; set; }
        public IList<string> Dossiers { get; set; }

        public Data(IList<Element> Elements, IList<string> Dossiers)
        {
            this.Elements = new ObservableCollection<Element>(Elements);
            this.Dossiers = Dossiers;
        }


        public void AjoutJeu(LauncherName Launcher, string Exec)
        {
            Jeu Jeu = SearchInfo.ExtractGameInfoFromExec(Exec);
            Jeu.IsManuallyAdded = true;
            InsertGame(Launcher, Jeu);
        }

        public void AjoutJeu(Jeu Jeu)
        {
            InsertGame(Jeu.Launcher, Jeu);
            Jeu.IsManuallyAdded = true;
        }

        public void ModifDetail(string Image, string Description, string Exec, Jeu Elementselected)
        {
            var Element = Elementselected as Jeu;
            if (Element.Image != Image && File.Exists(Image))
            {
                Element.Image = Image;
            }
            if (Element.Description != Description)
            {
                Element.Description = Description;
            }
            if (Element.Exec != Exec && File.Exists(Exec))
            {
                Element.Exec = Exec;
            }
        }

        public void SuppJeu(Jeu Elementselected)
        {
            (Elements[GetLauncherIndex(Elementselected.Launcher)] as Launcher).NbJeux--; //on enleve un jeu au launcher concerné
            Logs.InfoLog($"Suppression du jeu {Elementselected.Nom}");
            Elements.Remove(Elementselected);
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="Dossier"></param>
        public bool AjouterDossier(string Dossier)
        {
            if (!Dossiers.Contains(Dossier))
            {
                Logs.InfoLog($"Ajout du dossier {Dossier}");
                Dossiers.Add(Dossier);
                List<Jeu> Res = new List<Jeu>();
                List<string> Folder = new List<string>(Directory.GetDirectories(Dossier));
                SearchForExecutableAndName.SearchForExecutables(Res, Folder);
                foreach (Jeu Jeu in Res)
                {
                    AjoutJeu(Jeu);
                    Thread thread = new Thread(new ParameterizedThreadStart(SearchInfo.SetInfo));
                    thread.Start(Jeu);
                }
                return true;
            }
            return false;
        }

        public bool SuppDossier(string Dossier)
        {
            if (Dossiers.Contains(Dossier))
            {
                Logs.InfoLog($"Suppression du dossier {Dossier}");
                int indexLauncher = GetLauncherIndex(LauncherName.Autre);
                if (indexLauncher != -1)
                {
                    for (int i = indexLauncher + 1; i < Elements.Count; i++)
                    {
                        Jeu Jeu = Elements[i] as Jeu;
                        if (Directory.GetParent(Jeu.Dossier).FullName == Dossier)
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
                Dossiers.Remove(Dossier);
                return true;
            }
            return false;
        }



        private void InsertGame(LauncherName Launcher, Jeu Jeu)
        {
            if (Elements.Any(e => e.Nom.Equals(Launcher.ToString())))
            {
                int index = GetLauncherIndex(Launcher);
                Launcher LauncherActuel = Elements[index] as Launcher; //garde en memoire l'instance du launcher
                index++;
                while (index != Elements.Count && Elements[index].GetType() == typeof(Jeu) && Elements[index].CompareTo(Jeu as Element) < 0)
                {
                    index++; //tant que l'ordre alphabetique est pas respecté
                }
                if (index >= Elements.Count) //si on est a la fin
                {
                    Elements.Add(Jeu);
                }
                else
                {
                    Elements.Insert(index, Jeu);
                }
                (Elements[index] as Jeu).Launcher = Launcher; //on set le launcher associé au jeu
                (Elements.Where(e => ReferenceEquals(e, LauncherActuel)).First() as Launcher).NbJeux++; //on ajoute un jeu
            }
            else
            {
                InsertLauncher(Launcher); //on insert le launcher
                InsertGame(Launcher, Jeu); //on relance et on va se retrouver dans le code au dessus car cette fois-ci le launcher existe
            }

        }

        private void InsertLauncher(LauncherName Launcher)
        {
            int index = 0;
            if (Elements.Count == 0) //si c'est le premier element inserer
            {
                Elements.Add(new Launcher(Launcher));
                return;
            }
            if (Launcher != LauncherName.Autre)
            {
                while (index != Elements.Count && (Elements[index] as Launcher).Nom.CompareTo(Launcher.ToString()) < 0) //tant que le launcher est pas a sa place dans l'ordre alphabetique
                {
                    index += (Elements[index] as Launcher).NbJeux + 1;
                }
                if (index >= Elements.Count) //si on est a la fin
                {
                    Elements.Add(new Launcher(Launcher));
                }
                else
                {
                    Elements.Insert(index, new Launcher(Launcher));
                }
            }
            else
            {
                Elements.Add(new Launcher(Launcher)); //launcher.autre est obligatoirement en dernier
            }

        }

        public int GetLauncherIndex(LauncherName LauncherName)
        {
            Launcher Temp = new Launcher(LauncherName);
            return Elements.IndexOf(Temp);
        }
    }
}
