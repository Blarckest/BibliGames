using Logger;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Modele
{
    public class SteamSearcher : GameSearcher
    {
        private const string steam = "SOFTWARE\\Wow6432Node\\Valve\\Steam\\";
        protected override void GetGames()
        {
            jeux = new List<Jeu>();
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
                                else if (line.Contains("installdir")) //recuperation du dossier
                                {
                                    folderName = line.Substring(16);
                                    folderName = folderName.Replace("\"", "");
                                    folderName = $"{pathToSteamApps}{@"\common\"}{folderName}";
                                    break;
                                }
                            }
                            if (nom != "Steamworks Common Redistributables") //Ce dossier n'est pas un jeu
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

        protected override void GetGamesDirectory()
        {
            dossiers = new List<string>();

            List<string> paths = new List<string>();

            RegistryKey key;
            if ((key = Registry.LocalMachine.OpenSubKey(steam)) != null)
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
                            dossiers.Add(directory);
                        }
                    }
                }
            }
        }
    }
}
