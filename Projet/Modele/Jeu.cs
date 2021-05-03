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
        public LauncherName Launcher { get; private set; }


        public Jeu():base(Type.Jeu) { }

        public Jeu(string Nom,string Dossier,string Exec,LauncherName Launcher=LauncherName.Autre) : base(Nom,Type.Jeu)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Launcher = Launcher;
        }
        public Jeu(string Nom, string Icone, string Dossier, string Exec,LauncherName Launcher= LauncherName.Autre) : base(Nom, Icone, Type.Jeu)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Launcher = Launcher;
        }
        public Jeu(string Nom, string Icone, string Dossier, string Exec, string Image,string Note,string Description, LauncherName Launcher = LauncherName.Autre) : this(Nom, Icone, Dossier,Exec,Launcher)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Image = Image;
            this.Note = Note;
            this.Description = Description;
            this.Launcher = Launcher;
        }        
    }
}
