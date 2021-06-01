using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Modele
{
    public class OriginSearcher : GameSearcher
    {
        private IDictionary<string, string> DossierToNomOrigin = new Dictionary<string, string>();
        protected override void GetGames()
        {
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
                        nom = new Regex("[®™]").Replace(nom, "");
                        var jeu = SearchForExecutables(pathToFolder, LauncherName.Origin);
                        jeu.Nom = nom;
                        temp.Add(jeu);
                        Logs.InfoLog($"Ajout du jeu {nom}");
                    }
                }
            }
            temp.Sort();
           Jeux.AddRange(temp);
        }

        protected override void GetGamesDirectory()
        {
            List<string> pathsToGameDirectory = new List<string>();
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
                        Dossiers.Add(pathToFolder);
                        DossierToNomOrigin.Add(pathToFolder, Path.GetFileNameWithoutExtension(fichier));
                    }
                }
            }
        }
    }
}
