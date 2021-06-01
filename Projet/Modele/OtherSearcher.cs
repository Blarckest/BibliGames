using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Modele
{
    public class OtherSearcher : GameSearcher
    {
        private IEnumerable<string> Paths { get; set; }
        public OtherSearcher(IEnumerable<string> paths)
        {
            if (paths!=null)
            {
                Paths = paths;
            }
            else
            {
                Paths = new List<string>();
            }
        }

        public OtherSearcher(string path)
        {
            if (path != null)
            {
                Paths = new List<string>(new List<string> { path } );
            }
            else
            {
                Paths = new List<string>();
            }
        }

        private void GetGameDirectoryFromPaths()
        {
            foreach (string path in Paths)
            {
                if (Directory.Exists(path))
                {
                    foreach (string dir in Directory.GetDirectories(path))
                    {
                        try //getfiles peut lancer une exception si il n'a pas les droits suffisants
                        {
                            if (!IsDirectoryEmpty(dir)) //on verifie que il y a un executable dans le dossier et que le dossier n'est pas vide
                            {
                                dossiers.Add(dir);
                            }
                        }
                        catch (Exception)
                        {

                            continue; //si exception lancer on ignore le dossier
                        }

                    }
                }
            }
        }
        
        protected override void GetGames()
        {
            if (dossiers != null)
            {
                jeux = new List<Jeu>();
                base.SearchForExecutables();
            }
            else
            {
                GetGamesDirectory(); //si la fonction a jamais ete execute on l'execute
                GetGames(); //on revient a la fonction actuel avec cette fois un Dossiers non null
            }
        }
        protected override void GetGamesDirectory()
        {
            dossiers = new List<string>();
            GetGameDirectoryFromPaths();
        }
    }
}
