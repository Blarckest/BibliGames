using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    public class Jeu : Element
    {
        public string Dossier { get; set; }
        public string Exec { get; set;}
        public string Description { get; set; }
        public string Note { get; set; }




        public Jeu(string Nom, string Icone, string Dossier, string Exec, string Note) : base(Nom, Icone)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Note = Note;

        }        
    }
}
