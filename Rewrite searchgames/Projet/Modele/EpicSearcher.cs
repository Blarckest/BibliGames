using Logger;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Modele
{
    public class EpicSearcher : GameSearcher
    {
        protected override void GetGames()
        {
            if (Dossiers!=null)
            {
                foreach (var jeu in Jeux)
                {
                    if (jeu.Exec==null)
                    {
                        string[] nomExecutables = Directory.GetFiles(jeu.Dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                        jeu.Exec = Filter(nomExecutables, jeu.Nom); //filtrage
                    }
                }
            }
            else
            {
                GetGamesDirectory(); //si la fonction a jamais ete execute on l'execute
                GetGames(); //on revient a la fonction actuel avec cette fois un dossiers non null
            }
        }

        protected override void GetGamesDirectory()
        {
            string nom = "";
            string executable = "";
            string dossier = "";
            const string regKey = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";
            RegistryKey key;
            if ((key = Registry.LocalMachine.OpenSubKey(regKey)) != null) //si la cle existe on continue
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
                            else
                            {
                                executable = null; //si c'etait un jeu uplay alors on s'en ocupera dans GetGames()
                            }
                            Jeux.Add(new Jeu(nom, dossier, executable, LauncherName.EpicGames));//ajout a ce niveau car on a eu toutes les infos qu'on voulais
                        }
                    }
                }
            }
            Jeux.Sort();
            Dossiers.AddRange(Jeux.Select(j => j.Dossier)); //on rempli avec les dossiers des jeux trouver
        }
    }
}
