using Logger;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace Modele
{
    public class RiotSearcher : GameSearcher
    {
        private const string regKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";
        IDictionary<string, string> dossierToNom = new Dictionary<string, string>();
        protected override void GetGames()
        {
            if (dossiers != null)
            {
                jeux = new List<Jeu>();
                foreach (var dossier in dossierToNom)
                {
                    string[] nomExecutables = Directory.GetFiles(dossier.Value, "*.exe", SearchOption.AllDirectories);
                    var executable = Filter(nomExecutables, dossier.Key, LauncherName.Riot);
                    jeux.Add(new Jeu(dossier.Key, dossier.Value, executable, LauncherName.Riot));
                    Logs.InfoLog($"Ajout du jeu {dossier.Key}");
                }
                jeux.Sort();
            }
            else
            {
                GetGamesDirectory();
                GetGames();
            }
        }

        protected override void GetGamesDirectory()
        {
            dossiers = new List<string>();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regKey);
            foreach (string subKey in key.GetSubKeyNames()) //parcour des sous-clé
            {
                if (subKey.Contains("Riot Game")) //cas ou la sous-clé nous interesse
                {
                    RegistryKey keyJeu = Registry.CurrentUser.OpenSubKey(regKey + subKey);
                    string path = keyJeu.GetValue("InstallLocation").ToString();
                    path = path.Replace("/", "\\"); //certains jeux sont marque avec des / et d'autres avec des \\ donc on transforme ceux en / en \\
                    string nom = keyJeu.GetValue("DisplayName").ToString();
                    dossiers.Add(path);
                    dossierToNom.Add(nom, path);
                }
            }
        }
    }
}
