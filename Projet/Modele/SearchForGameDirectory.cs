using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Microsoft.Win32;

namespace Modele
{
    public static class SearchForGameDirectory
    {
        /// <summary>
        /// Retourne tout les dossiers dans launcher Steam/Uplay/Epic/Riot/origin
        /// chaque dossier a le format suivant : X:\\path\\to\\folder
        /// la fonction retourne un dictionnaire avec comme clé un launcher et comme value un liste contenant les dossiers associé a ce launcher
        /// </summary>
        /// <returns>Dictionary<Launcher, List<string>></returns>
        public static Dictionary<LauncherName, List<string>> GetAllGameDirectory()
        {
            Dictionary<LauncherName, List<string>> Dossiers = new Dictionary<LauncherName, List<string>>();
            SearchEpicGames(Dossiers);
            SearchOriginGames(Dossiers);
            SearchRiotGames(Dossiers);
            SearchSteamGames(Dossiers);
            SearchUplayGames(Dossiers);
            return Dossiers;
        }

        public static Dictionary<LauncherName, List<string>> GetAllGameDirectory(List<string> Paths)
        {
            Dictionary<LauncherName, List<string>> Dossiers = new Dictionary<LauncherName, List<string>>();
            SearchEpicGames(Dossiers);
            SearchOriginGames(Dossiers);
            SearchRiotGames(Dossiers);
            SearchSteamGames(Dossiers);
            SearchUplayGames(Dossiers);
            if (Paths!=null && Paths.Count!=0)
            {
                Dossiers.Add(LauncherName.Autre, GetGameDirectoryFromPaths(Paths.ToArray()));
            }
            return Dossiers;
        }

        public static List<string> GetGameDirectoryFromPaths(string[] Paths)
        {
            List<string> Dossiers = new List<string>();
            foreach (string Path in Paths)
            {
                if (Directory.Exists(Path))
                {
                    foreach (string Dir in Directory.GetDirectories(Path))
                    {
                        try //getfiles peut lancer une exception si il n'a pas les droits suffisants
                        {
                            if (!IsDirectoryEmpty(Dir)) //on verifie que il y a un executable dans le dossier et que le dossier n'est pas vide
                            {
                                Dossiers.Add(Dir);
                            }
                        }
                        catch (Exception)
                        {

                            continue; //si exception lancer on ignore le dossier
                        }
                       
                    }
                }
            }
            return Dossiers;
        }
        private static bool IsDirectoryEmpty(string Path)
        {
            return !(Directory.EnumerateFileSystemEntries(Path).Any() && (Directory.GetFiles(Path+"\\\\","*.exe",SearchOption.AllDirectories).Any()));//renvoie si le dossier et vide ou contient aucun executable (any renvoie un booleen true si il y a qq chose ds le IEnumerable renvoyer)
        }
        private static void SearchSteamGames(Dictionary<LauncherName, List<string>> Dossiers)
        {
            List<string> Paths = new List<string>();
            List<string> PathsToGameDirectory = new List<string>();
            string Steam = "SOFTWARE\\Wow6432Node\\Valve\\Steam\\";
            string SteamPath;
            string ConfigPath;
            RegistryKey Key;
            if ((Key = Registry.LocalMachine.OpenSubKey(Steam))!=null)
            {
                SteamPath = Key.GetValue("InstallPath").ToString(); //cle contenant le chemin jusqu'au dossier steam
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
                            string Path = Line.Substring(Line.IndexOf(matched)); //prend a partir de D:\\ jusqua la fin de la ligne
                            Path = Path.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                            Path = Path.Replace("\"", "\\");  //met les dernier \ à \\
                            if (Directory.Exists(Path + "steamapps\\common"))
                            {
                                Path += "steamapps\\common\\";
                                Paths.Add(Path);
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
                if (PathsToGameDirectory.Count > 0)
                {
                    Dossiers.Add(LauncherName.Steam, PathsToGameDirectory);
                }
            }
        }

        private static void SearchUplayGames(Dictionary<LauncherName, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string RegKey = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher\\Installs";
            RegistryKey Key;
            if ((Key = Registry.LocalMachine.OpenSubKey(RegKey))!=null)
            {
                foreach (string Jeu in Key.GetSubKeyNames()) //parcours les cle de tout les jeux
                {
                    RegistryKey Valeurs = Key.OpenSubKey(Jeu);
                    string Path = Valeurs.GetValue("InstallDir").ToString(); //get le dossier
                    Path = Path.Substring(0, Path.Length - 1);
                    Path = Path.Replace("/", "\\"); //pour avoir une sortie pareil pour tout les launcher ex d:\\path\\to\\directory
                    PathsToGameDirectory.Add(Path);
                }
                if (PathsToGameDirectory.Count > 0)
                {
                    Dossiers.Add(LauncherName.Uplay, PathsToGameDirectory);
                }
            }
        }

        private static void SearchEpicGames(Dictionary<LauncherName, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string Temp;
            string RegKey = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";
            RegistryKey Key;
            if ((Key = Registry.LocalMachine.OpenSubKey(RegKey))!=null) //si la cle existe on continue
            {
                string Path = Key.GetValue("AppDataPath").ToString(); //get location du dossier ou epic stock les infos utiles
                Path += "Manifests\\";
                if (Directory.Exists(Path))
                {
                    string[] AllFiles = Directory.GetFiles(Path, "*.item"); //ce dossier contient tout les fichiers de config de tout les jeux
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
                    if (PathsToGameDirectory.Count > 0)
                    {
                        Dossiers.Add(LauncherName.EpicGames, PathsToGameDirectory);
                    }  
                }
            }
        }

        private static void SearchRiotGames(Dictionary<LauncherName, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string RegKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\"; //tout les chemin des jeux riot sont dispo ici
            RegistryKey Key = Registry.CurrentUser.OpenSubKey(RegKey);
            foreach (string SubKey in Key.GetSubKeyNames()) //parcour des sous-clé
            {
                if (SubKey.Contains("Riot Game")) //cas ou la sous-clé nous interesse
                {
                    RegistryKey KeyJeu = Registry.CurrentUser.OpenSubKey(RegKey+SubKey);
                    string Path = KeyJeu.GetValue("InstallLocation").ToString();
                    Path = Path.Replace("/", "\\"); //certains jeux sont marque avec des / et d'autres avec des \\ donc on transforme ceux en / en \\
                    PathsToGameDirectory.Add(Path);
                }
            }
            if (PathsToGameDirectory.Count>0)
            {
                Dossiers.Add(LauncherName.Riot, PathsToGameDirectory);
            }
        }

        private static void SearchOriginGames(Dictionary<LauncherName, List<string>> Dossiers)
        {
            List<string> PathsToGameDirectory = new List<string>();
            string PathToProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string Path = PathToProgramData+"\\Origin\\LocalContent\\"; //dossier qui nous interesse
            if (Directory.Exists(Path))
            {
                string[] Dirs = Directory.GetDirectories(Path);
                foreach (string Dir in Dirs)
                {
                    string Fichier = Directory.GetFiles(Dir, "*.mfst").Count() == 1 ? Directory.GetFiles(Dir, "*.mfst").First() : null; //fichier .mfst contient les infos utile
                    if (File.Exists(Fichier))
                    {
                        string Line = File.ReadAllLines(Fichier).First(); //le fichier contient qu'une ligne
                        Line = System.Uri.UnescapeDataString(Line); //la ligne est au format web " "==%20 par ex
                        string[] Lines = Line.Split('&');
                        string PathToFolder = Lines.Where(e => e.Contains("installpath=", System.StringComparison.OrdinalIgnoreCase) && e.Contains(":\\")).First(); //recuperation de la valeur qui nous interesse
                        PathToFolder = PathToFolder.Substring(PathToFolder.IndexOf(":\\") - 1); //suppression du "installpath="
                        PathsToGameDirectory.Add(PathToFolder);
                    }
                }
                if (PathsToGameDirectory.Count > 0)
                {
                    Dossiers.Add(LauncherName.Origin, PathsToGameDirectory);
                }
            }
        }
    }
}

