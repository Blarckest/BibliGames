using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Modele
{
    public abstract class GameSearcher
    {
        protected List<string> dossiers = new List<string>();
        protected List<Jeu> jeux = new List<Jeu>();

        public List<string> Dossiers { 
            get
            {
                if (!dossiers.Any())
                {
                    GetGamesDirectory(); //rempli dossiers
                }
                return dossiers;
            } 
            protected set
            {
                dossiers = value;
            }
        }
        public List<Jeu> Jeux
        {
            get
            {
                if (!jeux.Any() || jeux.Any(j=>j.Nom==null || j.Dossier==null || j.Exec==null)) //on verifie si il a des données non mise si il y en a on demande a getGames de corriger les données manquantes
                {
                    GetGames(); //rempli/modifie jeux 
                }
                return jeux;
            }
            protected set
            {
                jeux = value;
            }
        }

        protected virtual void SearchForExecutables(LauncherName launcher = LauncherName.Autre)
        {
            foreach (string dossier in Dossiers) //pour chaque dossier recup l'executable le plus probable d'etre le bon
            {
                var nom = Path.GetFileName(dossier);
                string[] nomExecutables = Directory.GetFiles(dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
                var executable = Filter(nomExecutables, nom, launcher);
                jeux.Add(new Jeu(nom, dossier, executable, launcher));//ajout
                Logs.InfoLog($"Ajout du jeu {nom}");
            }
            jeux.Sort();
        }

        protected virtual Jeu SearchForExecutables(string dossier, LauncherName launcher = LauncherName.Autre)
        {
            var nom = Path.GetFileName(dossier);
            string[] nomExecutables = Directory.GetFiles(dossier, "*.exe", SearchOption.AllDirectories); //recup tout les .exe dans tout les sous-dossier
            string executable = Filter(nomExecutables, nom, launcher);
            return new Jeu(nom, dossier, executable, launcher);
        }

        protected string Filter(string[] executables, string nom = null, LauncherName launcher = LauncherName.Autre)
        {
            int archi = Environment.Is64BitOperatingSystem ? 64 : 32;
            if (launcher == LauncherName.Riot)//Riot est un peu speciale (peu de jeux)(launcher....etc) donc hardcodage de ceux la
            {
                //manque le nom pour runeterra et apparemment valorant se lance en ligne de commande avec le RiotClientServices.exe
                return executables.First(e => e.Contains("LeagueClient.exe") || e.Contains("VALORANT.exe") || e.Contains("LoR.exe"));
            }
            IEnumerable<string> res = executables.Where(e => FilterIndesirables(e)); //apllication du filtre
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

        protected bool FilterIndesirables(string executable)
        {
            return !(executable.Contains("Unins", StringComparison.OrdinalIgnoreCase) || executable.Contains("Crash", StringComparison.OrdinalIgnoreCase) || executable.Contains("Helper", StringComparison.OrdinalIgnoreCase)
                || executable.Contains("AntiCheat", StringComparison.OrdinalIgnoreCase) || executable.Contains("Downloader", StringComparison.OrdinalIgnoreCase) || executable.Contains("Upload") || executable.Contains("Report", StringComparison.OrdinalIgnoreCase)
                || executable.Contains("Unreal", StringComparison.OrdinalIgnoreCase) || executable.Contains("Redist", StringComparison.OrdinalIgnoreCase) || executable.Contains("egodumper", StringComparison.OrdinalIgnoreCase)
                || executable.Contains("mono.exe", StringComparison.OrdinalIgnoreCase)); //traitement des cas communs
        }

        

        protected bool IsDirectoryEmpty(string path)
        {
            return !(Directory.EnumerateFileSystemEntries(path).Any() || (Directory.GetFiles(path + "\\\\", "*.exe", SearchOption.AllDirectories).Any()));//renvoie si le dossier et vide ou contient aucun executable (any renvoie un booleen true si il y a qq chose ds le IEnumerable renvoyer)
        }

        abstract protected void GetGamesDirectory();
        abstract protected void GetGames();
    }
}
