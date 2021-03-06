using Microsoft.Win32;
using System.Collections.Generic;

namespace Modele
{
    public class UplaySearcher : GameSearcher
    {
        protected override void GetGames()
        {
            if (dossiers != null)
            {
                jeux = new List<Jeu>();
                base.SearchForExecutables(LauncherName.Uplay);
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
            const string regKey = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher\\Installs";
            RegistryKey key;
            if ((key = Registry.LocalMachine.OpenSubKey(regKey)) != null)
            {
                foreach (string jeu in key.GetSubKeyNames()) //parcours les cle de tout les jeux
                {
                    RegistryKey valeurs = key.OpenSubKey(jeu);
                    string path = valeurs.GetValue("InstallDir").ToString(); //get le dossier
                    path = path.Substring(0, path.Length - 1);
                    path = path.Replace("/", "\\"); //pour avoir une sortie pareil pour tout les launcher ex d:\\path\\to\\directory
                    dossiers.Add(path);
                }
            }
        }
    }
}
