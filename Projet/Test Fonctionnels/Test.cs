using System;
using System.Collections.Generic;
using DataManager;
using Modele;

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

        public static void TestStub()
        {
            Loader loader = new LoadElements("");
        }

        public static void TestGameDirectory()
        {
            TestStub();
            List<string> path = new List<string>();
            path.Add("../../../../../Test");
            List<string> dossiers = new List<string>();
             List <GameSearcher> searchers = new List<GameSearcher> 
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

        private static void TestSave()
        {
            const string path = "./Ressource/sauvegarde";
            Saver save = new SaveElements(path);
            Loader loader = new LoadElements(path);
            var manager = new Manager(loader.Load());
            IList<string> add = new List<string>();
            add.Add("zefzqf");
            save.Save(manager.Data.Elements, add);
        }

        private static void TestLoad()
        {
            const string path = "./Ressource/sauvegarde";
            Loader loader = new LoadElements(path);
            Manager manager = new Manager(loader.Load());
            TestAfficherElements(manager);
        }

        private static void TestLoadAfficheSave(Loader loader=null)
        {
            if (loader==null)
            {
                loader = new LoadElements("./Ressources/sauvegarde");
            }
            Manager manager = new Manager(loader.Load());
            if (manager.Affichage.Count==0)
            {
                manager.AjoutJeu(new Jeu("Riot1", "", "Valorant.exe", LauncherName.Riot));
                manager.AjoutJeu(new Jeu("Riot2", "", "Valorant.exe", LauncherName.Riot));
                manager.AjoutJeu(new Jeu("Steam1", "", "Valorant.exe", LauncherName.Steam));
                manager.AjoutJeu(new Jeu("EG1", "", "Valorant.exe", LauncherName.EpicGames));
            }
            TestAfficherElements(manager);
            Saver saver = new SaveElements("./Ressources/sauvegarde");
            saver.Save(manager.Data);
        }

        static void Main(string[] args)
        {
            //TestExecutable();
            //TestSave();
            //TestLoad();
            TestLoadAfficheSave();
        }
    }
}
