using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Modele
{
    public class Jeu : Element, INotifyPropertyChanged
    {
        private string exec=null, description=null, image=null, note=null; 
        public string Icone { get; set; } = null;
        public string Dossier { get; set; } = null;
        public LauncherName Launcher { get; set; }
        public string Exec
        {
            get
            {
                return exec;
            }
            set
            {
                exec = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                NotifyPropertyChanged();
            }
        }
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
                NotifyPropertyChanged();
            }
        }
        public string Image {
            get
            {
                return image;
            }
            set
            {
                image  =  value;
                NotifyPropertyChanged();
            }
        }
        

        public Jeu(string Nom,string Dossier,string Exec,LauncherName Launcher=LauncherName.Autre) : base(Nom)
        {
            this.Dossier = Dossier;
            this.Exec = Exec;
            this.Launcher = Launcher;
        }

        public Jeu(string Nom, string Dossier, string Exec, string Image, string Icone, string Note,string Description, LauncherName Launcher = LauncherName.Autre) : this(Nom, Dossier,Exec,Launcher)
        {
            this.Icone = Icone;
            this.Image = Image;
            this.Note = Note;
            this.Description = Description;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
