using Modele;
using System;
using System.Collections.Generic;

namespace Test_Fonctionnels
{
    class Test
    {
        public static void TestAfficherElements(Manager modele)
        {
            foreach (Element element in modele.Affichage)
            {
                Console.WriteLine(element);
            }
        }

        public static IPersistance TestStub()
        {
            return new Stub.Stub();
        }

        public static void TestGameDirectory()
        {
            TestStub();
            List<string> path = new List<string>();
            path.Add("./Test");
            List<string> dossiers = new List<string>();
            List<GameSearcher> searchers = new List<GameSearcher>
             {
                new EpicSearcher(),
                new OriginSearcher(),
                new RiotSearcher(),
                new SteamSearcher(),
                new UplaySearcher(),
            };
            List<Jeu> jeux = new List<Jeu>();
            foreach (var searcher in searchers)
            {
                jeux.AddRange(searcher.Jeux);
            }
            foreach (string dossier in dossiers)
            {
                Console.WriteLine(dossier);
            }
        }

        private static void TestExecutable()
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("DOSSIER  LAUNCHER  EXECUTABLE");
            List<GameSearcher> searchers = new List<GameSearcher>
            {
                new EpicSearcher(),
                new OriginSearcher(),
                new RiotSearcher(),
                new SteamSearcher(),
                new UplaySearcher(),
            };
            List<Jeu> jeux = new List<Jeu>();
            foreach (var searcher in searchers)
            {
                jeux.AddRange(searcher.Jeux);
            }
            foreach (Jeu jeu in jeux)
            {
                Console.WriteLine($"{jeu.Nom},  {jeu.Launcher},  {jeu.Exec}");
            }
        }

        private static void TestLoadAndSave()
        {
            Modele.Manager manager = new Manager(new Persistance.Persistance());//load
            TestAfficherElements(manager);
            manager.AjoutJeu(new Jeu("Valorant", "dossier", "Valorant.exe", "image", "icone", "note", "description", LauncherName.Riot));
            TestAfficherElements(manager);
            manager.Save();
            manager = new Manager(new Persistance.Persistance());//load
            TestAfficherElements(manager);
        }

        static void Main(string[] args)
        {
            //TestExecutable();
            //TestGameDirectory();
            //TestLoadAndSave();
        }
    }
}
