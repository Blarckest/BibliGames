using Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DataManager
{
    public class Stub :Loader
    {
        public Data Data { get; set; }
        public Stub(string Path = "") : base(Path)
        {
            List<Element> Elements = new List<Element>();
            List<Jeu> Games = new List<Jeu>();
            string[] PathToTest = { "../../../../../Test" }; //on va au dossier de test
            SearchForExecutableAndName.SearchForExecutables(Games, SearchForGameDirectory.GetGameDirectoryFromPaths(PathToTest)); //on charge
            if (Games.Count > 0)//si l'utilisateur a des jeux
            {
                Launcher Actuel = new Launcher(Games[0].Launcher);
                Elements.Add(Actuel);
                for (int i = 0; i < Games.Count; i++)
                {
                    if (Games[i].Launcher.ToString() == Actuel.ToString()) //on est dans le meme launcher
                    {
                        Elements.Add(Games[i]);
                        Actuel.NbJeux++;//on augmente le nb de jeu
                    }
                    else
                    {
                        Actuel = new Launcher(Games[i].Launcher); //on ajoute le launcher
                        Elements.Add(Actuel);
                        Elements.Add(Games[i]);
                        Actuel.NbJeux++;//on augmente le nb de jeu
                    }
                }
            }
            foreach (Element element in Elements)//on set les infos
            {
                if (element.GetType() == typeof(Jeu))
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(SearchInfo.SetInfo));
                    thread.Start(element);
                }
            }
            Data = new Data(Elements,new List<string>{ });
        }

        public override Data Load()
        {
            return Data;
        }

        public override IList<string> LoadAdditionalPath()
        {
            return new List<string> { "Path1", "Path2" };
        }
    }
}
