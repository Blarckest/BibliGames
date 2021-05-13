using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;

namespace Modele
{
    public class Manager
    {
        private IList<Element> elements;
        public IList<Element> Elements {
            get { return elements; }
            set { elements = value; Affichage = value; }
        }
        public IList<string> Dossiers { get; set; }
        public Element ElementSelected { get; set; }
        public string Pattern { get; set; } = null;
        public IList<Element> Affichage { get; private set; }
        public void AjoutJeu(LauncherName Launcher, string Exec)
        {
            if (File.Exists(Exec))
            {
                Jeu Jeu = SearchInfo.ExtractGameInfoFromExec(Exec);
                InsertGame(Launcher, Jeu);
            }
        }

        public void AjoutJeu(Jeu Jeu)
        {
            InsertGame(Jeu.Launcher, Jeu);
        }

        public void ModifDetail(string Image, string Description, string Exec)
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                var Element = ElementSelected as Jeu;
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
        }
        public void SuppJeu()
        {
            Elements.Remove(ElementSelected);
        }
        public void LancerJeu()
        {
            var Elem = ElementSelected as Jeu;
            System.Diagnostics.Process.Start(Elem.Exec); //normalement ca marche a tester
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="Dossier"></param>
        public void AjouterDossier(string Dossier)
        {
            Dossiers.Add(Dossier);
            List<Jeu> Res = new List<Jeu>();
            List<string> Folder = new List<string>();
            Folder.Add(Dossier);
            SearchForExecutableAndName.SearchForExecutables(Res, Folder);
            foreach (Jeu Jeu in Res)
            {
                AjoutJeu(Jeu);
            }
        }

        public void SuppDossier(string Dossier)
        {
            int index = GetLauncherIndex(LauncherName.Autre);
            if (index!=-1)
            {
                for (int i = index; i < Elements.Count; i++)
                {
                    Jeu Jeu = Elements[i] as Jeu; //à revoir Jeu.dossier != dossier
                    if (Directory.GetParent(Jeu.Dossier).FullName == Dossier)
                    {
                        Elements.Remove(Jeu);
                    }
                }
            }
           
        }
        //servira si jamais on a plus de filtre a appliquer sur les elements affichés
        public void UpdateAffichage()
        {
            Affichage = Elements;
            Recherche();
        }
        //effectue juste une suppression des elements non désiré (correspondant pas au pattern)
        public void Recherche()
        {
            if (Pattern != null)
            {
                foreach (Element Elem in Affichage)
                {
                    if (Elem.GetType() == typeof(Jeu) && !Elem.Nom.Contains(Pattern, StringComparison.OrdinalIgnoreCase)) //si on est sur on jeu et que il correspond pas au pattern
                    {
                        Affichage.Remove(Elem);
                    }
                }
            }
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
            if (Elements.Count==0) //si c'est le premier element inserer
            {
                Elements.Add(new Launcher(Launcher));
                return;
            }
            if (Launcher!=LauncherName.Autre)
            {
                while (index != Elements.Count && (Elements[index] as Launcher).Nom.CompareTo(Launcher.ToString()) < 0) //tant que le launcher est pas a sa place dans l'ordre alphabetique
                {
                    index += (Elements[index] as Launcher).NbJeux+1;
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

        private int GetLauncherIndex(LauncherName LauncherName)
        {
            Launcher Temp = new Launcher(LauncherName);
            return Elements.IndexOf(Temp);
        }
    }
}
