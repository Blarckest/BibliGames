using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Modele
{
    class Manager
    {
        public IList<Element> Elements { get; set; }
        private IList<string> Dossiers { get; set; }
        public Element ElementSelected { get; set; }
        public string Pattern { get; set; } = null;
        public IList<Element> Affichage { get; private set; }
        
        public void AjoutJeu(LauncherName Launcher,string Exec)
        {
            if (File.Exists(Exec))
            {
                Element Jeu = SearchInfo.ExtractGameInfoFromExec(Exec);
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i].Type == Type.Launcher && Elements[i].Nom == Launcher.ToString())
                    {
                        while (Elements[i].Nom.CompareTo(Jeu.Nom)<0)
                        {
                            i++;
                        }
                        Elements.Insert(i, Jeu);
                        break;
                    }
                } 
            }
        }
        public void ModifDetail(string Image,string Description,string Exec)
        {
            if (ElementSelected.Type==Type.Jeu)
            {
                var Element = ElementSelected as Jeu;
                if (Element.Image!=Image && File.Exists(Image))
                {
                    Element.Image = Image;
                }
                if (Element.Description!=Description)
                {
                    Element.Description = Description;
                }
                if (Element.Exec!=Exec && File.Exists(Exec))
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
        public void AjouterDossier(string Dossier)
        { 
            Dossiers.Add(Dossier);
            Dictionary<LauncherName, List<Tuple<string, string>>> res=new Dictionary<LauncherName, List<Tuple<string, string>>>();
            List<string> Folder = new List<string>();
            Folder.Add(Dossier);
            SearchForExecutableAndName.SearchForExecutables(res, Folder);
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].Type == Type.Launcher && Elements[i].Nom == LauncherName.Autre.ToString())
                {
                    List<Tuple<string, string>> Jeux;
                    if (res.TryGetValue(LauncherName.Autre, out Jeux))
                    {
                        foreach (Tuple<string, string> Jeu in Jeux)
                        {
                            while (Elements[i].Nom.CompareTo(Jeu.Item1) < 0)
                            {
                                i++;
                            }
                            Elements.Insert(i, new Jeu(Jeu.Item1, Dossier, Jeu.Item2));
                        }
                    }
                    break;
                }
            }
        }
        public void SuppDossier(string Dossier)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].Type == Type.Launcher && Elements[i].Nom == LauncherName.Autre.ToString())
                {
                    i++;
                    while (i!=Elements.Count && Elements[i].Type != Type.Launcher)
                    {
                        Jeu Jeu = Elements[i] as Jeu;
                        if (Jeu.Dossier == Dossier)
                        {
                            Elements.Remove(Jeu);
                        }
                        i++;
                    }
                    break;
                }
            }
        }
        //servira si jamais on a plus de filtre a appliquer sur les elements affichés
        public void UpdateAffichage()
        {
            Affichage = Elements;
            Recherche();
        }
        //effectue juste un suppression des elements non désiré (correspondant pas au pattern)
        public void Recherche()
        {
            if (Pattern != null)
            {
                foreach (Element Elem in Affichage)
                {
                    if (Elem.Type == Type.Jeu && !Elem.Nom.Contains(Pattern, StringComparison.OrdinalIgnoreCase)) //si on est sur on jeu et que il correspond pas au pattern
                    {
                        Affichage.Remove(Elem);
                    }
                }
            }
        }
        private void GetSave()
        {

        }
        private void GetGame()
        {
            var res = SearchForGameDirectory.GetAllGameDirectory();
            if (Dossiers.Count!=0)
            {
                //res.Add(Launcher.Autre, AutoSearchForGameDirectory.GetGameDirectoryFromPaths(Dossiers));
            }
            SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(res);
        }
        private void GetInfo(Jeu Jeu)
        {

        }
    }
}
