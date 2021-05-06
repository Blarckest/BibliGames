using System;
using System.Collections.Generic;
using DataManager;
using Modele;

namespace Test_Fonctionnels
{
    class Test
    {
        public static void TestAfficherElements(Manager Modele)
        {
            foreach (Element element in Modele.Elements)
            {
                Console.WriteLine(element);
            }
        }

        public static void TestStub()
        {
            Loader Loader = new Stub("");
        }

        public static Dictionary<LauncherName, List<string>> TestGameDirectory()
        {
            TestStub();
            List<string> path = new List<string>();
            path.Add("./Stub");
            Dictionary<LauncherName, List<string>> dossiers;
            dossiers = SearchForGameDirectory.GetAllGameDirectory(path);
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
            Dictionary<LauncherName, List<string>> dossiers;
            dossiers =TestGameDirectory();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("DOSSIER  LAUNCHER  EXECUTABLE");
            List<Jeu> Jeux;
            Jeux =SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(dossiers);
            foreach(Jeu Jeu in Jeux)
            {
                Console.WriteLine($"{Jeu.Nom},  {Jeu.Launcher},  {Jeu.Exec}");
            }
        }

        private static void TestSave()
        {
            string path = "./ressource/sauvegarde";
            Saver save = new SaveElements(path);
            Loader Loader = new Stub("");
            IList<Element> Elements = Loader.Load();
            IList<string> Add = new List<string>();
            Add.Add("zefzqf");
            save.Save(Elements, Add);
        }

        private static void TestLoad()
        {
            string path = "./ressource/sauvegarde";
            Loader Loader = new LoadElements(path);
            Manager Manager = new Manager() { Elements = Loader.Load(),
                Dossiers = Loader.LoadAdditionalPath()
            };
            TestAfficherElements(Manager);
        }
        static void Main(string[] args)
        {
            //TestExecutable();
            //TestSave();
            TestLoad();
        }
    }
}
