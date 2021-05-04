using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Modele
{
    public class Jeu : Element
    {
        public string Icone { get; set; } = null;
        public string Dossier { get; set; } = null;
        public string Exec { get; set;} = null;
        public string Description { get; set; } = null;
        public string Note { get; set; } = null;
        public string Image { set; get; } = null;
        public LauncherName Launcher { get; set; }

        public Jeu(string Nom,string Dossier,string Exec,LauncherName Launcher=LauncherName.Autre) : base(Nom)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Launcher = Launcher;
        }

        public Jeu(string Nom, string Icone, string Dossier, string Exec, string Image,string Note,string Description, LauncherName Launcher = LauncherName.Autre) : this(Nom, Dossier,Exec,Launcher)
        {
            this.Icone = Icone;
            this.Image = Image;
            this.Note = Note;
            this.Description = Description;
        }
    }
}
