using Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Stub
{
    public class Stub : IPersistance
    {
        public Data Load()
        {
            List<Element> elements = new List<Element>();
            List<Jeu> games = new List<Jeu>();
            string[] pathToTest = { "./Test" }; //on va au dossier de test

            games = new OtherSearcher(pathToTest).Jeux;//on charge

            if (games.Count > 0)
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
            return new Data(elements, new List<string> { });
        }

        public void Save(Data data)
        {
            throw new NotImplementedException();
        }
    }
}
