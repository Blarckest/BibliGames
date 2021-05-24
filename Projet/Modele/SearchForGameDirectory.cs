using Logger;
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
            Dictionary<LauncherName, List<string>> dossiers = new Dictionary<LauncherName, List<string>>();
            SearchEpicGames(dossiers);
            SearchOriginGames(dossiers);
            SearchRiotGames(dossiers);
            SearchSteamGames(dossiers);
            SearchUplayGames(dossiers);
            return dossiers;
        }

        public static IDictionary<LauncherName, List<string>> GetAllGameDirectory(List<string> paths)
        {
            IDictionary<LauncherName, List<string>> dossiers = new Dictionary<LauncherName, List<string>>();
            SearchEpicGames(dossiers);
            SearchOriginGames(dossiers);
            SearchRiotGames(dossiers);
            SearchSteamGames(dossiers);
            SearchUplayGames(dossiers);
            if (paths!=null && paths.Count!=0)
            {
                dossiers.Add(LauncherName.Autre, GetGameDirectoryFromPaths(paths.ToArray()));
            }
            return dossiers;
        }

        public static List<string> GetGameDirectoryFromPaths(string[] paths)
        {
            List<string> dossiers = new List<string>();
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    foreach (string dir in Directory.GetDirectories(path))
                    {
                        try //getfiles peut lancer une exception si il n'a pas les droits suffisants
                        {
                            if (!IsDirectoryEmpty(dir)) //on verifie que il y a un executable dans le dossier et que le dossier n'est pas vide
                            {
                                dossiers.Add(dir);
                            }
                        }
                        catch (Exception)
                        {

                            continue; //si exception lancer on ignore le dossier
                        }
                       
                    }
                }
            }
            return dossiers;
        }
        private static bool IsDirectoryEmpty(string path)
        {
            return !(Directory.EnumerateFileSystemEntries(path).Any() || (Directory.GetFiles(path+"\\\\","*.exe",SearchOption.AllDirectories).Any()));//renvoie si le dossier et vide ou contient aucun executable (any renvoie un booleen true si il y a qq chose ds le IEnumerable renvoyer)
        }
        private static void SearchSteamGames(IDictionary<LauncherName, List<string>> dossiers)
        {
            List<string> paths = new List<string>();
            List<string> pathsToGameDirectory = new List<string>();
            const string steam = "SOFTWARE\\Wow6432Node\\Valve\\Steam\\";
            RegistryKey key;
            if ((key = Registry.LocalMachine.OpenSubKey(steam))!=null)
            {
                string steamPath = key.GetValue("InstallPath").ToString();
                string configPath = steamPath + "/steamapps/libraryfolders.vdf";
                string regexChemin = @"[A-Z]:\\"; //cherche pour un debut de chemin ex: D:\\
                if (File.Exists(configPath))
                {
                    string[] configLines = File.ReadAllLines(configPath);
                    foreach (string line in configLines)
                    {
                        Match res = Regex.Match(line, regexChemin);
                        if (line != string.Empty && res.Success)
                        {
                            string matched = res.ToString();  //recupere la partie qui a trigger la regex ex D:\\
                            string path = line.Substring(line.IndexOf(matched)); //prend a partir de D:\\ jusqua la fin de la ligne
                            path = path.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                            path = path.Replace("\"", "\\");  //met les dernier \ à \\
                            if (Directory.Exists(path + "steamapps\\common"))
                            {
                                path += "steamapps\\common\\";
                                paths.Add(path);
                            }
                        }
                    }
                    paths.Add(steamPath + "\\steamapps\\common\\");
                }

                foreach (string path in paths)
                {
                    string[] allDir = Directory.GetDirectories(path);
                    foreach (string directory in allDir)
                    {
                        if (!(IsDirectoryEmpty(directory) || directory.Contains("Steamworks Shared"))) //on veux pas de certains dossier
                        {
                            pathsToGameDirectory.Add(directory);
                        }
                    }
                }
                if (pathsToGameDirectory.Count > 0)
                {
                    dossiers.Add(LauncherName.Steam, pathsToGameDirectory);
                    Logs.InfoLog("Ajout du launcher Steam");
                }
            }
        }

        private static void SearchUplayGames(IDictionary<LauncherName, List<string>> dossiers)
        {
            List<string> pathsToGameDirectory = new List<string>();
            const string regKey = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher\\Installs";
            RegistryKey key;
            if ((key = Registry.LocalMachine.OpenSubKey(regKey))!=null)
            {
                foreach (string jeu in key.GetSubKeyNames()) //parcours les cle de tout les jeux
                {
                    RegistryKey valeurs = key.OpenSubKey(jeu);
                    string path = valeurs.GetValue("InstallDir").ToString(); //get le dossier
                    path = path.Substring(0, path.Length - 1);
                    path = path.Replace("/", "\\"); //pour avoir une sortie pareil pour tout les launcher ex d:\\path\\to\\directory
                    pathsToGameDirectory.Add(path);
                }
                if (pathsToGameDirectory.Count > 0)
                {
                    dossiers.Add(LauncherName.Uplay, pathsToGameDirectory);
                    Logs.InfoLog("Ajout du launcher Uplay");
                }
            }
        }

        private static void SearchEpicGames(IDictionary<LauncherName, List<string>> dossiers)
        {
            List<string> pathsToGameDirectory = new List<string>();
            const string regKey = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";
            RegistryKey key;
            if ((key = Registry.LocalMachine.OpenSubKey(regKey))!=null) //si la cle existe on continue
            {
                string path = key.GetValue("AppDataPath").ToString(); //get location du dossier ou epic stock les infos utiles
                path += "Manifests\\";
                if (Directory.Exists(path))
                {
                    string[] allFiles = Directory.GetFiles(path, "*.item"); //ce dossier contient tout les fichiers de config de tout les jeux
                    foreach (string item in allFiles)
                    {
                        if (File.Exists(item))
                        {
                            string[] lines = File.ReadAllLines(item);
                            foreach (string line in lines) //parcour du fichier
                            {
                                if (line.Contains("InstallLocation")) //traitement sur la ligne qui nous interesse
                                {
                                    string temp = line.Substring(line.IndexOf(":\\") - 1);
                                    temp = temp.Substring(0, temp.Length - 1);  //suppression de la virgule de fin de ligne
                                    temp = temp.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                                    temp = temp.Replace("\"", "");  //enleve les caracteres de fin qu'on ne veut pas
                                    if (Directory.Exists(temp))
                                    {
                                        pathsToGameDirectory.Add(temp);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (pathsToGameDirectory.Count > 0)
                    {
                        dossiers.Add(LauncherName.EpicGames, pathsToGameDirectory);
                        Logs.InfoLog("Ajout du launcher Epic Games");
                    }  
                }
            }
        }

        private static void SearchRiotGames(IDictionary<LauncherName, List<string>> dossiers)
        {
            List<string> pathsToGameDirectory = new List<string>();
            const string regKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\"; //tout les chemin des jeux riot sont dispo ici
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regKey);
            foreach (string subKey in key.GetSubKeyNames()) //parcour des sous-clé
            {
                if (subKey.Contains("Riot Game")) //cas ou la sous-clé nous interesse
                {
                    RegistryKey keyJeu = Registry.CurrentUser.OpenSubKey(regKey+subKey);
                    string path = keyJeu.GetValue("InstallLocation").ToString();
                    path = path.Replace("/", "\\"); //certains jeux sont marque avec des / et d'autres avec des \\ donc on transforme ceux en / en \\
                    pathsToGameDirectory.Add(path);
                }
            }
            if (pathsToGameDirectory.Count>0)
            {
                dossiers.Add(LauncherName.Riot, pathsToGameDirectory);
                Logs.InfoLog("Ajout du launcher Riot");
            }
        }

        private static void SearchOriginGames(IDictionary<LauncherName, List<string>> dossiers)
        {
            List<string> pathsToGameDirectory = new List<string>();
            string pathToProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string path = pathToProgramData+"\\Origin\\LocalContent\\"; //dossier qui nous interesse
            if (Directory.Exists(path))
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    string fichier = Directory.GetFiles(dir, "*.mfst").Count() == 1 ? Directory.GetFiles(dir, "*.mfst").First() : null; //fichier .mfst contient les infos utile
                    if (File.Exists(fichier))
                    {
                        string line = File.ReadAllLines(fichier).First(); //le fichier contient qu'une ligne
                        line = System.Uri.UnescapeDataString(line); //la ligne est au format web " "==%20 par ex
                        string[] lines = line.Split('&');
                        string pathToFolder = lines.First(e => e.Contains("installpath=", StringComparison.OrdinalIgnoreCase) && e.Contains(":\\")); //recuperation de la valeur qui nous interesse
                        pathToFolder = pathToFolder.Substring(pathToFolder.IndexOf(":\\") - 1); //suppression du "installpath="
                        if (pathToFolder.Last() == '\\')
                        {
                            pathToFolder= pathToFolder.Remove(pathToFolder.Length-1);
                        }
                        pathsToGameDirectory.Add(pathToFolder);
                    }
                }
                if (pathsToGameDirectory.Count > 0)
                {
                    dossiers.Add(LauncherName.Origin, pathsToGameDirectory);
                    Logs.InfoLog("Ajout du launcher Origin");
                }
            }
        }
    }
}

