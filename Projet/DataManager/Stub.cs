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
        private Data Data { get; }
        public Stub(string path = "") : base(path)
        {
            List<Element> elements = new List<Element>();
            List<Jeu> games = new List<Jeu>();
            string[] pathToTest = { "../../../../../Test" }; //on va au dossier de test
            SearchForExecutableAndName.SearchForExecutables(games, SearchForGameDirectory.GetGameDirectoryFromPaths(pathToTest)); //on charge
            if (games.Count > 0)//si l'utilisateur a des jeux
            {
                Launcher actuel = new Launcher(games[0].Launcher);
                elements.Add(actuel);
                for (int i = 0; i < games.Count; i++)
                {
                    if (games[i].Launcher.ToString() == actuel.ToString()) //on est dans le meme launcher
                    {
                        elements.Add(games[i]);
                        actuel.NbJeux++;//on augmente le nb de jeu
                    }
                    else
                    {
                        actuel = new Launcher(games[i].Launcher); //on ajoute le launcher
                        elements.Add(actuel);
                        elements.Add(games[i]);
                        actuel.NbJeux++;//on augmente le nb de jeu
                    }
                }
            }
            foreach (Element element in elements)//on set les infos
            {
                if (element.GetType() == typeof(Jeu))
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(SearchInfo.SetInfo));
                    thread.Start(element);
                }
            }
            Data = new Data(elements,new List<string>{ });
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
