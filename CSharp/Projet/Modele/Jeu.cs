using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Modele
{
    public class Jeu : Element, INotifyPropertyChanged
    {
        private string exec=null, description=null, image=null, note=null, icone=null;
        public bool IsManuallyAdded { get; set; } = false;
        public string Dossier { get; set; } = null;
        public LauncherName Launcher { get; set; }
        public string Icone
        {
            get => icone;
            set
            {
                icone = value;
                NotifyPropertyChanged();
            }
        }
        public string Exec
        {
            get => exec;
            set
            {
                exec = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                NotifyPropertyChanged();
            }
        }
        public string Note
        {
            get => note;
            set
            {
                note = value;
                NotifyPropertyChanged();
            }
        }
        public string Image {
            get => image;
            set
            {
                image  =  value;
                NotifyPropertyChanged();
            }
        }
        

        public Jeu(string nom,string dossier,string exec,LauncherName launcher=LauncherName.Autre) : base(nom)
        {
            Dossier = dossier;
            Exec = exec;
            Launcher = launcher;
        }

        public Jeu(string nom, string dossier, string exec, string image, string icone, string note,string description, LauncherName launcher = LauncherName.Autre, bool isManuallyAdded=false) : this(nom, dossier,exec,launcher)
        {
            Note = note;
            Description = description;
            IsManuallyAdded = isManuallyAdded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
