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
            Loader loader = new Stub("");
        }

        public static IDictionary<LauncherName, List<string>> TestGameDirectory()
        {
            TestStub();
            List<string> path = new List<string>();
            path.Add("./Stub");
            IDictionary<LauncherName, List<string>> dossiers = SearchForGameDirectory.GetAllGameDirectory(path);
            foreach (KeyValuePair<LauncherName, List<string>> element in dossiers)
            {
                Console.WriteLine("Dossier dans le launcher :");
                Console.WriteLine(element.Key);
                List<string> chem = element.Value;
                foreach(string chemin in chem)
                {
                    Console.WriteLine(chemin);
                }
            }
            return dossiers;
        }

        private static void TestExecutable()
        {
            IDictionary<LauncherName, List<string>> dossiers = TestGameDirectory();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("DOSSIER  LAUNCHER  EXECUTABLE");
            List<Jeu> jeux = SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(dossiers);
            foreach(Jeu jeu in jeux)
            {
                Console.WriteLine($"{jeu.Nom},  {jeu.Launcher},  {jeu.Exec}");
            }
        }

        private static void TestSave()
        {
            const string path = "./Ressource/sauvegarde";
            Saver save = new SaveElements(path);
            Loader loader = new Stub("");
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
