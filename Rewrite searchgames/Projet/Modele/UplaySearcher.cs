using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    public class UplaySearcher : GameSearcher
    {
        protected override void GetGames()
        {
            if (Dossiers != null)
            {
                base.SearchForExecutables(Jeux, Dossiers, LauncherName.Uplay);
            }
            else
            {
                GetGamesDirectory(); //si la fonction a jamais ete execute on l'execute
                GetGames(); //on revient a la fonction actuel avec cette fois un dossiers non null
            }
        }
        protected override void GetGamesDirectory()
        {
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
                    Dossiers.Add(path);
                }
            }
        }
    }
}
