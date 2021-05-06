﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;



namespace Modele
{
    public static class SearchForExecutableAndName
    {
        /// <summary>
        /// Retourne la liste des executables en fonction du launcher
        /// </summary>
        /// <returns>Dictionary<Launcher,List<Tuple<string,string>>>(launcher,executable,nom)</returns>
        public static List<Jeu> GetExecutableAndNameFromGameDirectory(Dictionary<LauncherName, List<string>> Dossiers)
        {
            List<Jeu> Jeux = new List<Jeu>();
            List<string> DossiersLauncher;

            if (Dossiers.Keys.Contains(LauncherName.EpicGames)) //verifie si il y a des jeux epic
                SearchForEpicExecutables(Jeux); //pas besoin des dossiers tout est situé dans des fichiers de config

            if (Dossiers.TryGetValue(LauncherName.Origin, out DossiersLauncher)) //recup dossiers du launcher Origin et recherche d'executable si il y en a
                SearchForExecutables(Jeux, DossiersLauncher, LauncherName.Origin);

            if (Dossiers.TryGetValue(LauncherName.Riot, out DossiersLauncher)) //recup dossiers du launcher riot et recherche d'executable si il y en a
                SearchForExecutables(Jeux, DossiersLauncher, LauncherName.Riot);

            if (Dossiers.TryGetValue(LauncherName.Steam, out DossiersLauncher)) //recup dossiers du launcher steam et recherche d'executable si il y en a
                SearchForSteamExecutables(Jeux, DossiersLauncher);

            if (Dossiers.TryGetValue(LauncherName.Uplay, out DossiersLauncher)) //recup dossiers du launcher uplay et recherche d'executable si il y en a
                SearchForExecutables(Jeux, DossiersLauncher, LauncherName.Uplay);

            if (Dossiers.TryGetValue(LauncherName.Autre, out DossiersLauncher)) //recup dossiers de launcer inconnu et recherche d'executable si il y en a
                SearchForExecutables(Jeux, DossiersLauncher, LauncherName.Autre);

            return Jeux;
        }

        public static string Filter(string[] Executables, string Nom = null, LauncherName Launcher = LauncherName.Autre)
        {
            int archi;
            archi = Environment.Is64BitOperatingSystem ? 64 : 32;
            if (Launcher == LauncherName.Riot)//Riot est un peu speciale (peu de jeux)(launcher....etc) donc hardcodage de ceux la
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
                temp = Res.Where(e => Path.GetFileName(e).Contains(Nom.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
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

        public static void SearchForExecutables(List<Jeu> Jeux, IList<string> Dossiers, LauncherName Launcher = LauncherName.Autre)
        {
            List<Jeu> Temp = new List<Jeu>();
            string Nom;
            string Executable;
            foreach (string Dossier in Dossiers) //pour chaque dossier recup l'executable le plus probable d'etre le bon
            {
                Nom = Path.GetFileName(Dossier);
                string[] NomExecutables = Directory.GetFiles(Dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                Executable = Filter(NomExecutables, Nom, Launcher); //filtrage
                Temp.Add(new Jeu(Nom, Dossier, Executable, Launcher));//ajout
            }
            Temp.Sort();
            Jeux.AddRange(Temp);
        }

        private static Jeu SearchForExecutables(string Dossier, LauncherName Launcher = LauncherName.Autre)
        {
            string Nom;
            string Executable;
            Nom = Path.GetFileName(Dossier);
            string[] NomExecutables = Directory.GetFiles(Dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
            Executable = Filter(NomExecutables, Nom, Launcher); //filtrage
            return new Jeu(Nom, Dossier, Executable, Launcher);
        }

        private static void SearchForSteamExecutables(List<Jeu> Jeux, IList<string> Dossiers)
        {
            List<Jeu> Temp = new List<Jeu>();
            string Nom = "";
            string FolderName = "";
            List<string> SteamAppsVisited = new List<string>();
            foreach (string Dossier in Dossiers) //sert juste a parcourir les differents steamapps
            {
                string PathToSteamApps = Directory.GetParent(Directory.GetParent(Dossier).FullName).FullName; 
                if (!SteamAppsVisited.Contains(PathToSteamApps)) //si on pas deja traiter ce steamapps
                {
                    string[] AllFiles = Directory.GetFiles(PathToSteamApps, "*.acf"); //ce dossier contient tout les fichiers de config de tout les jeux
                    foreach (string File in AllFiles)
                    {
                        if (System.IO.File.Exists(File))
                        {
                            string[] Lines = System.IO.File.ReadAllLines(File);
                            foreach (string Line in Lines) //parcour du fichier
                            {
                                if (Line.Contains("name")) //recuperation du nom
                                {
                                    Nom = Line.Substring(10);
                                    Nom = Nom.Replace("\"", "");
                                }
                                else if(Line.Contains("installdir")) //recuperation du dossier
                                {
                                    FolderName= Line.Substring(16);
                                    FolderName=FolderName.Replace("\"", "");
                                    FolderName = $"{PathToSteamApps}{@"\common\"}{FolderName}";
                                    break;
                                }
                            }
                            if (Nom!= "Steamworks Common Redistributables") //Ce dossier n'est pas un jeu
                            {
                                Jeu Jeu = SearchForExecutables(FolderName, LauncherName.Steam);
                                Jeu.Nom = Nom;
                                Temp.Add(Jeu);

                            }
                        }
                    }
                    SteamAppsVisited.Add(PathToSteamApps);
                }
            }
            Temp.Sort();
            Jeux.AddRange(Temp);
        }

        private static void SearchForEpicExecutables(List<Jeu> Jeux)
        {
            List<Jeu> Temp = new List<Jeu>();
            string Nom = "";
            string Executable = "";
            string Dossier = "";
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
                            Nom = Line.Substring(Line.IndexOf(": \"") + 3); //recuperation du nom jusqua la fin de la ligne
                            Nom = Nom.Substring(0, Nom.Length - 2);  //suppression de l'apostrophe et de la virgule de fin de ligne
                        }
                        else if (Line.Contains("InstallLocation")) //recuperation du chemin de dossier
                        {
                            Dossier = Line.Substring(Line.IndexOf(":\\") - 1); //recuperation du debut du chemin jusqua la fin de la ligne
                            Dossier = Dossier.Substring(0, Dossier.Length - 1);  //suppression de la virgule de fin de ligne
                            Dossier = Dossier.Replace("\\\\", "\\");  //tout les  \ sont echapé on a donc besoin d'en enlever 
                            Dossier = Dossier.Replace("\"", "");
                        }
                    }
                    if (File.Exists(Dossier + Executable) && !Executable.Contains("UplayLaunch.exe")) //filter les jeux associe a uplay
                    {
                        Executable = Dossier + Executable;
                    }
                    else //cas ou le fichier existait pas ou jeu uplay->solution general(filtrage de tout les exe)
                    {
                        string[] NomExecutables = Directory.GetFiles(Dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                        Executable = Filter(NomExecutables, Nom); //filtrage
                    }
                    Temp.Add(new Jeu(Nom, Dossier, Executable, LauncherName.EpicGames));//ajout
                }
            }
            Temp.Sort();
            Jeux.AddRange(Temp);
        }
    }
}
