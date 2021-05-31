using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Net;

namespace Modele
{
    public static class SearchForExecutableAndName
    {
        /// <summary>
        /// Retourne la liste des executables en fonction du launcher
        /// </summary>
        /// <returns>Dictionary<Launcher,List<Tuple<string,string>>>(launcher,executable,nom)</returns>
        public static List<Jeu> GetExecutableAndNameFromGameDirectory(IDictionary<LauncherName, List<string>> dossiers)
        {
            List<Jeu> jeux = new List<Jeu>();
            List<string> dossiersLauncher;

            if (dossiers.Keys.Contains(LauncherName.EpicGames)) //verifie si il y a des jeux epic
                SearchForEpicExecutables(jeux); //pas besoin des dossiers tout est situé dans des fichiers de config

            if (dossiers.TryGetValue(LauncherName.Origin, out dossiersLauncher)) //recup dossiers du launcher Origin et recherche d'executable si il y en a
                SearchForOriginExecutables(jeux);

            if (dossiers.TryGetValue(LauncherName.Riot, out dossiersLauncher)) //recup dossiers du launcher riot et recherche d'executable si il y en a
                SearchForRiotExecutables(jeux); // tout est situé dans les fichiers de config

            if (dossiers.TryGetValue(LauncherName.Steam, out dossiersLauncher)) //recup dossiers du launcher steam et recherche d'executable si il y en a
                SearchForSteamExecutables(jeux, dossiersLauncher);

            if (dossiers.TryGetValue(LauncherName.Uplay, out dossiersLauncher)) //recup dossiers du launcher uplay et recherche d'executable si il y en a
                SearchForExecutables(jeux, dossiersLauncher, LauncherName.Uplay);

            if (dossiers.TryGetValue(LauncherName.Autre, out dossiersLauncher)) //recup dossiers de launcer inconnu et recherche d'executable si il y en a
                SearchForExecutables(jeux, dossiersLauncher, LauncherName.Autre);

            return jeux;
        }

        public static void SearchForExecutables(List<Jeu> jeux, IList<string> dossiers, LauncherName launcher = LauncherName.Autre)
        {
            List<Jeu> temp = new List<Jeu>();
            foreach (string dossier in dossiers) //pour chaque dossier recup l'executable le plus probable d'etre le bon
            {
                var nom = Path.GetFileName(dossier);
                string[] nomExecutables = Directory.GetFiles(dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                var executable = Filter(nomExecutables, nom, launcher);
                temp.Add(new Jeu(nom, dossier, executable, launcher));//ajout
                Logs.InfoLog($"Ajout du jeu {nom}");
            }
            temp.Sort();
            jeux.AddRange(temp);
        }

        private static string Filter(string[] executables, string nom = null, LauncherName launcher = LauncherName.Autre)
        {
            int archi = Environment.Is64BitOperatingSystem ? 64 : 32;
            if (launcher == LauncherName.Riot)//Riot est un peu speciale (peu de jeux)(launcher....etc) donc hardcodage de ceux la
            {
                //manque le nom pour runeterra et apparemment valorant se lance en ligne de commande avec le RiotClientServices.exe
                return executables.First(e => e.Contains("LeagueClient.exe") || e.Contains("VALORANT.exe") || e.Contains("LoR.exe"));
            }
            IEnumerable<string> res = executables.Where(e => Filter(e)); //apllication du filtre
            if (res.Any()) //si tout a disparu dans le filtre 
            {
                return executables[0];
            }
            else if (nom != null && res.Count() > 1) //si un nom est defini on prend les executables contenant le nom (si il y en a)
            {
                var temp = res.Where(e => Path.GetFileName(e).Contains(nom, StringComparison.OrdinalIgnoreCase));
                res = temp.Any() ? res : temp;
                temp = res.Where(e => Path.GetFileName(e).Contains(nom.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
                res = temp.Any() ? res : temp;
            }
            if (res.Count() > 1 && res.Any(e => e.Contains("bin", StringComparison.OrdinalIgnoreCase))) //preferer les exe contenu dans un dossier bin ou binaries (si il y en a)
            {
                res = res.Where(e => e.Contains("bin", StringComparison.OrdinalIgnoreCase));
            }
            if (res.Count() > 1 && res.Any(e => e.Contains(archi.ToString()))) //preferer les exe contenu dans un dossier 64 ou 32 bit en fonction de l'architecture supporté (si il y en a) (on suppose que ARM n'existe pas bien entendu)
            {
                res = res.Where(e => e.Contains(archi.ToString(), StringComparison.OrdinalIgnoreCase));
            }
            return res.First();
        }

        private static bool Filter(string executable)
        {
            return !(executable.Contains("Unins", StringComparison.OrdinalIgnoreCase) || executable.Contains("Crash", StringComparison.OrdinalIgnoreCase) || executable.Contains("Helper", StringComparison.OrdinalIgnoreCase)
                || executable.Contains("AntiCheat", StringComparison.OrdinalIgnoreCase) || executable.Contains("Downloader", StringComparison.OrdinalIgnoreCase) || executable.Contains("Upload") || executable.Contains("Report", StringComparison.OrdinalIgnoreCase)
                || executable.Contains("Unreal", StringComparison.OrdinalIgnoreCase) || executable.Contains("Redist", StringComparison.OrdinalIgnoreCase) || executable.Contains("egodumper", StringComparison.OrdinalIgnoreCase)
                || executable.Contains("mono.exe", StringComparison.OrdinalIgnoreCase)); //traitement des cas communs
        }

        private static Jeu SearchForExecutables(string dossier, LauncherName launcher = LauncherName.Autre)
        {
            var nom = Path.GetFileName(dossier);
            string[] nomExecutables = Directory.GetFiles(dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
            string executable = Filter(nomExecutables, nom, launcher);
            return new Jeu(nom, dossier, executable, launcher);
        }

        private static void SearchForSteamExecutables(List<Jeu> jeux, IList<string> dossiers)
        {
            List<Jeu> temp = new List<Jeu>();
            string nom = "";
            string folderName = "";
            List<string> steamAppsVisited = new List<string>();
            foreach (string dossier in dossiers) //sert juste a parcourir les differents steamapps
            {
                string pathToSteamApps = Directory.GetParent(Directory.GetParent(dossier).FullName).FullName; 
                if (!steamAppsVisited.Contains(pathToSteamApps)) //si on pas deja traiter ce steamapps
                {
                    string[] allFiles = Directory.GetFiles(pathToSteamApps, "*.acf"); //ce dossier contient tout les fichiers de config de tout les jeux
                    foreach (string file in allFiles)
                    {
                        if (System.IO.File.Exists(file))
                        {
                            string[] lines = System.IO.File.ReadAllLines(file);
                            foreach (string line in lines) //parcour du fichier
                            {
                                if (line.Contains("name")) //recuperation du nom
                                {
                                    nom = line.Substring(10);
                                    nom = nom.Replace("\"", "");
                                }
                                else if(line.Contains("installdir")) //recuperation du dossier
                                {
                                    folderName= line.Substring(16);
                                    folderName=folderName.Replace("\"", "");
                                    folderName = $"{pathToSteamApps}{@"\common\"}{folderName}";
                                    break;
                                }
                            }
                            if (nom!= "Steamworks Common Redistributables") //Ce dossier n'est pas un jeu
                            {
                                Jeu jeu = SearchForExecutables(folderName, LauncherName.Steam);
                                jeu.Nom = nom;
                                temp.Add(jeu);
                                Logs.InfoLog($"Ajout du jeu {nom}");

                            }
                        }
                    }
                    steamAppsVisited.Add(pathToSteamApps);
                }
            }
            temp.Sort();
            jeux.AddRange(temp);
        }

        private static void SearchForEpicExecutables(List<Jeu> jeux)
        {
            List<Jeu> temp = new List<Jeu>();
            string nom = "";
            string executable = "";
            string dossier = "";
            const string regKey = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(regKey);
            string path = key.GetValue("AppDataPath").ToString(); //get location du dossier ou epic stock les infos utiles
            path += "Manifests\\";
            string[] allFiles = Directory.GetFiles(path, "*.item"); //ce dossier contient tout les fichiers de config de tout les jeux
            foreach (string item in allFiles)
            {
                if (File.Exists(item))
                {
                    string[] lines = File.ReadAllLines(item);
                    foreach (string line in lines) //parcour du fichier
                    {
                        if (line.Contains("LaunchExecutable")) //recuperation du nom de l'executable
                        {
                            executable = line.Substring(line.IndexOf(": \"") + 3); //recuperation du nom jusqua la fin de la ligne
                            executable = executable.Substring(0, executable.Length - 2);  //suppression de l'apostrophe et de la virgule de fin de ligne
                            executable = executable.Replace("/", "\\\\"); //transforme path/to/file en path\\to\\file
                        }
                        else if (line.Contains("DisplayName")) //recuperation du nom du jeu
                        {
                            nom = line.Substring(line.IndexOf(": \"") + 3); //recuperation du nom jusqua la fin de la ligne
                            nom = nom.Substring(0, nom.Length - 2);  //suppression de l'apostrophe et de la virgule de fin de ligne
                            Regex charToEmpty = new Regex("[®™]"); //le serveur ne supporte pas le caracteres echapé/speciaux
                            nom = charToEmpty.Replace(nom, "");
                        }
                        else if (line.Contains("InstallLocation")) //recuperation du chemin de dossier
                        {
                            dossier = line.Substring(line.IndexOf(":\\") - 1); //recuperation du debut du chemin jusqua la fin de la ligne
                            dossier = dossier.Substring(0, dossier.Length - 1);  //suppression de la virgule de fin de ligne
                            dossier = dossier.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                            dossier = dossier.Replace("\"", "");
                        }
                    }
                    if (File.Exists(dossier + executable) && !executable.Contains("UplayLaunch.exe")) //filter les jeux associe a uplay
                    {
                        executable = dossier + executable;
                    }
                    else //cas ou le fichier existait pas ou jeu uplay->solution general(filtrage de tout les exe)
                    {
                        string[] nomExecutables = Directory.GetFiles(dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                        executable = Filter(nomExecutables, nom); //filtrage
                    }
                    temp.Add(new Jeu(nom, dossier, executable, LauncherName.EpicGames));//ajout
                    Logs.InfoLog($"Ajout du jeu {nom}");
                }
            }
            temp.Sort();
            jeux.AddRange(temp);
        }

        private static void SearchForRiotExecutables(List<Jeu> jeux) // a vérifier avec les jeux
        {
            List<Jeu> temp = new List<Jeu>();
            const string regKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regKey);
            foreach (string subKey in key.GetSubKeyNames()) //parcour des sous-clé
            {
                if (subKey.Contains("Riot Game")) //cas ou la sous-clé nous interesse
                {
                    RegistryKey keyJeu = Registry.CurrentUser.OpenSubKey($"{regKey}\\{subKey}");
                    string dossier = keyJeu.GetValue("InstallLocation").ToString();
                    dossier = dossier.Replace("/", "\\"); //certains jeux sont marque avec des / et d'autres avec des \\ donc on transforme ceux en / en \\
                    string nom = keyJeu.GetValue("DisplayName").ToString();
                    string[] nomExecutables = Directory.GetFiles(dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                    var executable = Filter(nomExecutables, nom, LauncherName.Riot);
                    temp.Add(new Jeu(nom, dossier, executable, LauncherName.Riot));
                    Logs.InfoLog($"Ajout du jeu {nom}");
                }
            }
            temp.Sort();
            jeux.AddRange(temp);
        }

        private static void SearchForOriginExecutables(List<Jeu> jeux)
        {
            List<Jeu> temp = new List<Jeu>();
            string pathToProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string path = pathToProgramData + "\\Origin\\LocalContent\\"; //dossier qui nous interesse
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
                            pathToFolder = pathToFolder.Remove(pathToFolder.Length - 1);
                        }
                        string nom = new WebClient().DownloadString(@$"https://api1.origin.com/ecommerce2/public/{Path.GetFileNameWithoutExtension(fichier)}/en_US"); //on recupere le contenu de la page
                        nom = nom.Substring(nom.IndexOf("displayName") + 14);
                        nom = nom.Substring(0, nom.IndexOf("\",\"short"));
                        nom=new Regex("[®™]").Replace(nom,"");
                        var jeu=SearchForExecutables(pathToFolder, LauncherName.Origin);
                        jeu.Nom = nom;
                        temp.Add(jeu);
                        Logs.InfoLog($"Ajout du jeu {nom}");
                    }
                }
            }
            temp.Sort();
            jeux.AddRange(temp);
        }
    }
}
