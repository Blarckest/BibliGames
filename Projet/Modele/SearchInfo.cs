using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Modele
{
    public class SearchInfo
    {
        public static void SetInfo(/*Jeu Jeu*/)
        {
            HtmlWeb site = new HtmlWeb();
            HtmlDocument page = site.Load(@"https://www.igdb.com/games/league-of-legends");
            var doc = page.DocumentNode.Element("html").Element("head").Element("title");

            if (doc != null)
            {
                string description = doc.InnerText;
                Console.WriteLine("Description: {0}", description);
            }
            else
            {
                Console.WriteLine("Pas de description");
            }
        }
        public static Jeu ExtractGameInfoFromExec(string Exec)
        {
            Jeu Jeu = new Jeu();
            Jeu.Exec = Exec;
            Jeu.Dossier = Directory.GetParent(Exec).FullName;
            Jeu.Dossier = Path.GetFileName(Directory.GetParent(Exec).FullName);
            return Jeu;
        }

        
    }
}
