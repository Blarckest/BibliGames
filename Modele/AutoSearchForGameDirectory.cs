using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Modele
{
    public static class AutoSearchForGameDirectory
    {
        public static Dictionary<Launcher, List<string>> GetAllGameDirectory()
        {
            Dictionary<Launcher, List<string>> Dossiers = new Dictionary<Launcher, List<string>>();
            SearchSteamGames(Dossiers);
            SearchUplayGames(Dossiers);
            SearchEpicGames(Dossiers);
            SearchRiotGames(Dossiers);
            return Dossiers;
        }
        private static bool IsDirectoryEmpty(string Path)
        {
            return !Directory.EnumerateFileSystemEntries(Path).Any();//renvoie si le dossier et vide (any renvoie un booleen true si il y a qq chose ds le IEnumerable renvoyer)
        }
        private static void SearchSteamGames(Dictionary<Launcher, List<string>> Dossiers)
        {
            List<string> Paths = new List<string>();
            List<string> PathsToGameDirectory = new List<string>();
            string Steam = "SOFTWARE\\Wow6432Node\\Valve\\Steam\\";
            string SteamPath;
            string ConfigPath;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Steam);
            SteamPath = key.GetValue("InstallPath").ToString(); //cle contenant le chemin jusqu'au dossier steam
            ConfigPath = SteamPath + "/steamapps/libraryfolders.vdf"; //fichier de config
            string RegexChemin = @"[A-Z]:\\"; //cherche pour un debut de chemin ex: D:\\
            if (File.Exists(ConfigPath))
            {
                string[] configLines = File.ReadAllLines(ConfigPath);
                foreach (string Line in configLines)
                {
                    Match Res = Regex.Match(Line, RegexChemin);
                    if (Line != string.Empty && Res.Success)
                    {
                        string matched = Res.ToString();  //recupere la partie qui a trigger la regex ex D:\\
                        string item2 = Line.Substring(Line.IndexOf(matched)); //prend a partir de D:\\ jusqua la fin de la ligne
                        item2 = item2.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                        item2 = item2.Replace("\"", "\\");  //met les dernier \ à \\
                        if (Directory.Exists(item2 + "steamapps\\common"))
                        {
                            item2 = item2 + "steamapps\\common\\";
                            Paths.Add(item2);
                        }
                    }
                }
                Paths.Add(SteamPath + "\\steamapps\\common\\");
            }

            foreach (string Path in Paths)
            {
                string[] AllDir = Directory.GetDirectories(Path);
                foreach (string Directory in AllDir)
                {
                    if (!(IsDirectoryEmpty(Directory) || Directory.Contains("Steamworks Shared"))) //on veux pas de certains dossier
                    {
                        PathsToGameDirectory.Add(Directory);
                    }
                }
            }
            Dossiers.Add(Launcher.Steam, PathsToGameDirectory);
        }

        private static void SearchUplayGames(Dictionary<Launcher, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string RegKey = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher\\Installs";
            RegistryKey Key = Registry.LocalMachine.OpenSubKey(RegKey);
            foreach (string Jeu in Key.GetSubKeyNames()) //parcours les cle de tout les jeux
            {
                RegistryKey Valeurs = Key.OpenSubKey(Jeu);
                string Path = Valeurs.GetValue("InstallDir").ToString(); //get le dossier
                Path = Path.Substring(0, Path.Length - 1);
                Path = Path.Replace("/", "\\"); //pour avoir une sortie pareil pour tout les launcher ex d:\\path\\to\\directory
                PathsToGameDirectory.Add(Path);
            }
            Dossiers.Add(Launcher.Uplay, PathsToGameDirectory);
        }

        private static void SearchEpicGames(Dictionary<Launcher, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string Temp;
            string RegKey = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";
            RegistryKey Key = Registry.LocalMachine.OpenSubKey(RegKey);
            string Path = Key.GetValue("AppDataPath").ToString(); //get location du dossier ou epic stock les infos utiles
            Path += "Manifests\\";
            string[] AllFiles = Directory.GetFiles(Path,"*.item"); //ce dossier contient tout les fichiers de config de tout les jeux
            foreach (string Item in AllFiles)
            {
                if (File.Exists(Item))
                {
                    string[] Lines = File.ReadAllLines(Item);
                    foreach (string Line in Lines) //parcour du fichier
                    {
                        if (Line.Contains("InstallLocation")) //traitement sur la ligne qui nous interesse
                        {
                            Temp = Line.Substring(Line.IndexOf(":\\") - 1); //recuperation du debut du chemin jusqua la fin de la ligne
                            Temp = Temp.Substring(0, Temp.Length - 1);  //suppression de la virgule de fin de ligne
                            Temp = Temp.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                            Temp = Temp.Replace("\"", "");  //enleve les caracteres de fin qu'on ne veut pas
                            if (Directory.Exists(Temp))
                            {
                                PathsToGameDirectory.Add(Temp);
                            }
                            break;
                        }
                    } 
                }
            }
            Dossiers.Add(Launcher.EpicGames, PathsToGameDirectory);
        }

        private static void SearchRiotGames(Dictionary<Launcher, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string RegKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\"; //tout les chemin des jeux riot sont dispo ici
            RegistryKey Key = Registry.CurrentUser.OpenSubKey(RegKey);
            foreach (string SubKey in Key.GetSubKeyNames()) //parcour des sous-clé
            {
                if (SubKey.Contains("Riot Game")) //cas ou la sous-clé nous interesse
                {
                    RegistryKey KeyJeu = Registry.CurrentUser.OpenSubKey(RegKey+SubKey);
                    PathsToGameDirectory.Add(KeyJeu.GetValue("InstallLocation").ToString());
                }
            }
            Dossiers.Add(Launcher.Riot, PathsToGameDirectory);
        }
    }
}

