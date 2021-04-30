using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Modele
{
    public class SearchInfo
    {
        public static void SetInfo(Jeu Jeu)
        {
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
