using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    public class Jeu : Element
    {
        public string Dossier { get; set; } = null;
        public string Exec { get; set;} = null;
        public string Description { get; set; } = null;
        public string Note { get; set; } = null;
        public string Image { set; get; } = null;


        public Jeu():base(Type.Jeu) { }

        public Jeu(string Nom,string Dossier,string Exec) : base(Nom,Type.Jeu)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
        }
        public Jeu(string Nom, string Icone, string Dossier, string Exec) : base(Nom, Icone, Type.Jeu)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
        }
        public Jeu(string Nom, string Icone, string Dossier, string Exec, string Image,string Note,string Description) : this(Nom, Icone, Dossier,Exec)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Image = Image;
            this.Note = Note;
            this.Description = Description;
        }        
    }
}
