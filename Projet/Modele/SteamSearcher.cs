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
        private readonly IList<string> listSteamApps = new List<string>();
        protected override void GetGames()
        {
            if (dossiers != null)
            {
                jeux = new List<Jeu>();
                string nom = "";
                string folderName = "";
                foreach (string pathToSteamApps in listSteamApps) //on parcours les steamapps
                {
                    string[] allFiles = Directory.GetFiles(pathToSteamApps, "*.acf"); //ce dossier contient tout les fichiers de config de tout les jeux
                    foreach (string file in allFiles)
                    {
                        if (File.Exists(file))
                        {
                            string[] lines = File.ReadAllLines(file);
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
                                    folderName = $"{pathToSteamApps}{@"common\"}{folderName}";
                                    break;
                                }
                            }
                            if (nom != "Steamworks Common Redistributables") //Ce dossier n'est pas un jeu
                            {
                                Jeu jeu = SearchForExecutables(folderName, LauncherName.Steam);
                                jeu.Nom = nom;
                                jeux.Add(jeu);
                                Logs.InfoLog($"Ajout du jeu {nom}");

                            }
                        }
                    }
                }
                jeux.Sort();
            }
            else
            {
                GetGamesDirectory(); //si la fonction a jamais ete execute on l'execute
                GetGames(); //on revient a la fonction actuel avec cette fois un dossiers non null
            }
        }

        protected override void GetGamesDirectory()
        {
            dossiers = new List<string>();
            IList<string> listeCommon = new List<string>();
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
                                path += "steamapps\\";
                                listSteamApps.Add(path);
                                path += "common\\";
                                listeCommon.Add(path);
                            }
                        }
                    }
                    steamPath += "\\steamapps\\";
                    listSteamApps.Add(steamPath);
                    steamPath += "common\\";
                    listeCommon.Add(steamPath);
                }

                foreach (string path in listeCommon)
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
