using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;



namespace Modele
{
    public static class AutoSearchForExecutableAndName
    {
        /// <summary>
        /// Retourne la liste des executables en fonction du launcher
        /// </summary>
        /// <returns>Dictionary<Launcher,List<Tuple<string,string>>>(launcher,executable,nom)</returns>
        public static Dictionary<Launcher, List<Tuple<string, string>>> GetExecutableAndNameFromGameDirectory(Dictionary<Launcher, List<string>> Dossiers)
        {
            Dictionary<Launcher, List<Tuple<string, string>>> Executables = new Dictionary<Launcher, List<Tuple<string, string>>>();
            List<string> DossiersLauncher;

            if(Dossiers.TryGetValue(Launcher.Steam, out DossiersLauncher)) //recup dossiers du launcher steam et recherche d'executable si il y en a
                SearchForExecutables(Executables, DossiersLauncher,Launcher.Steam);

            if(Dossiers.TryGetValue(Launcher.Uplay, out DossiersLauncher)) //recup dossiers du launcher uplay et recherche d'executable si il y en a
                SearchForExecutables(Executables, DossiersLauncher,Launcher.Uplay);

            if(Dossiers.Keys.Contains(Launcher.EpicGames)) //verifie si il y a des jeux epic
                SearchForEpicExecutables(Executables); //pas besoin des dossiers tout est situé dans des fichiers de config

            if(Dossiers.TryGetValue(Launcher.Riot, out DossiersLauncher)) //recup dossiers du launcher riot et recherche d'executable si il y en a
                SearchForExecutables(Executables, DossiersLauncher,Launcher.Riot);

            if(Dossiers.TryGetValue(Launcher.Origin, out DossiersLauncher)) //recup dossiers du launcher Origin et recherche d'executable si il y en a
                SearchForExecutables(Executables, DossiersLauncher, Launcher.Origin);

            if (Dossiers.TryGetValue(Launcher.Autre, out DossiersLauncher)) //recup dossiers de launcer inconnu et recherche d'executable si il y en a
                SearchForExecutables(Executables, DossiersLauncher, Launcher.Autre);

            return Executables;
        }

        public static string Filter(string[] Executables, string Nom = null,Launcher Launcher=Launcher.Autre)
        {
            int archi;
            archi = Environment.Is64BitOperatingSystem ? 64 : 32;
            if (Launcher==Launcher.Riot)//Riot est un peu speciale (peu de jeux)(launcher....etc) donc hardcodage de ceux la
            {
                //manque le nom pour runeterra et apparemment valorant se lance en ligne de commande avec le RiotClientServices.exe
                return Executables.First(e => e.Contains("LeagueClient.exe") || e.Contains("VALORANT.exe") || e.Contains("LoR.exe"));
            }
            IEnumerable<string> Res = Executables.Where(e => Filter(e)); //apllication du filtre
            if (Res.Count() == 0) //si tout a disparu dans le filtre 
            {
                return Executables[0];
            }
            else if (Nom != null && Res.Count() > 1) //si un nom est defini on prend les executables contenant le nom (si il y en a)
            {
                var temp = Res.Where(e => Path.GetFileName(e).Contains(Nom, StringComparison.OrdinalIgnoreCase));
                Res = temp.Count() == 0 ? Res : temp;
                temp = Res.Where(e => Path.GetFileName(e).Contains(Nom.Replace(" ",""), StringComparison.OrdinalIgnoreCase));
                Res = temp.Count() == 0 ? Res : temp;
            }
            if (Res.Count() > 1 && Res.Any(e => e.Contains("bin", StringComparison.OrdinalIgnoreCase))) //preferer les exe contenu dans un dossier bin ou binaries (si il y en a)
            {
                Res = Res.Where(e => e.Contains("bin", StringComparison.OrdinalIgnoreCase));
            }
            if (Res.Count() > 1 && Res.Any(e => e.Contains(archi.ToString()))) //preferer les exe contenu dans un dossier 64 ou 32 bit en fonction de l'architecture supporté (si il y en a) (on suppose que ARM n'existe pas bien entendu)
            {
                Res = Res.Where(e => e.Contains(archi.ToString(), StringComparison.OrdinalIgnoreCase));
            }
            return Res.First();
        }

        private static bool Filter(string Executable)
        {
            return !(Executable.Contains("Unins", StringComparison.OrdinalIgnoreCase) || Executable.Contains("Crash", StringComparison.OrdinalIgnoreCase) || Executable.Contains("Helper", StringComparison.OrdinalIgnoreCase)
                || Executable.Contains("AntiCheat", StringComparison.OrdinalIgnoreCase) || Executable.Contains("Downloader", StringComparison.OrdinalIgnoreCase) || Executable.Contains("Upload") || Executable.Contains("Report", StringComparison.OrdinalIgnoreCase)
                || Executable.Contains("Unreal", StringComparison.OrdinalIgnoreCase) || Executable.Contains("Redist", StringComparison.OrdinalIgnoreCase) || Executable.Contains("egodumper", StringComparison.OrdinalIgnoreCase)
                || Executable.Contains("mono.exe", StringComparison.OrdinalIgnoreCase)); //traitement des cas communs
        }

        public static void SearchForExecutables(Dictionary<Launcher, List<Tuple<string, string>>> Executables, List<string> Dossiers,Launcher Launcher=Launcher.Autre)
        {
            List<Tuple<string, string>> TabExecName = new List<Tuple<string, string>>();
            string Nom;
            string Executable;
            foreach (string Dossier in Dossiers) //pour chaque dossier recup l'executable le plus probable d'etre le bon
            {
                Nom = Path.GetFileName(Dossier);
                string[] NomExecutables = Directory.GetFiles(Dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                Executable = Filter(NomExecutables, Nom, Launcher); //filtrage
                Executable = Executable.Replace("\\", "\\\\");
                TabExecName.Add(new Tuple<string, string>(Executable, Nom)); //ajout
            }
            Executables.Add(Launcher, TabExecName); //met a jour le dictionnaire
        }

        private static void SearchForEpicExecutables(Dictionary<Launcher, List<Tuple<string, string>>> Executables)
        {
            List<Tuple<string, string>> TabExecName = new List<Tuple<string, string>>();
            string Nom="";
            string Executable="";
            string Dossier="";
            string RegKey = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";     
            RegistryKey Key = Registry.LocalMachine.OpenSubKey(RegKey);
            string Path = Key.GetValue("AppDataPath").ToString(); //get location du dossier ou epic stock les infos utiles
            Path += "Manifests\\";
            string[] AllFiles = Directory.GetFiles(Path, "*.item"); //ce dossier contient tout les fichiers de config de tout les jeux
            foreach (string Item in AllFiles)
            {
                if (File.Exists(Item))
                {
                    string[] Lines = File.ReadAllLines(Item);
                    foreach (string Line in Lines) //parcour du fichier
                    {
                        if (Line.Contains("LaunchExecutable")) //recuperation du nom de l'executable
                        {
                            Executable = Line.Substring(Line.IndexOf(": \"") + 3); //recuperation du nom jusqua la fin de la ligne
                            Executable = Executable.Substring(0, Executable.Length - 2);  //suppression de l'apostrophe et de la virgule de fin de ligne
                            Executable = Executable.Replace("/", "\\\\"); //transforme path/to/file en path\\to\\file
                        }
                        else if (Line.Contains("DisplayName")) //recuperation du nom du jeu
                        {
                            Nom = Line.Substring(Line.IndexOf(": \"") +3); //recuperation du nom jusqua la fin de la ligne
                            Nom = Nom.Substring(0, Nom.Length - 2);  //suppression de l'apostrophe et de la virgule de fin de ligne
                        }
                        else if(Line.Contains("InstallLocation")) //recuperation du chemin de dossier
                        {
                            Dossier = Line.Substring(Line.IndexOf(":\\") - 1); //recuperation du debut du chemin jusqua la fin de la ligne
                            Dossier = Dossier.Substring(0, Dossier.Length - 1);  //suppression de la virgule de fin de ligne
                            //Dossier = Dossier.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                            Dossier = Dossier.Replace("\"", "");
                            Dossier += "\\\\";
                        }
                    }
                    if (File.Exists(Dossier+Executable) && !Executable.Contains("UplayLaunch.exe")) //filter les jeux associe a uplay
                    {
                        Executable = Dossier + Executable;
                    }
                    else //cas ou le fichier existait pas ou jeu uplay->solution general(filtrage de tout les exe)
                    {
                        string[] NomExecutables = Directory.GetFiles(Dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                        Executable = Filter(NomExecutables, Nom); //filtrage
                    }
                    TabExecName.Add(new Tuple<string, string>(Executable, Nom));
                }
            }
            Executables.Add(Launcher.EpicGames, TabExecName); //met a jour le dictionnaire
        }
    }
}
