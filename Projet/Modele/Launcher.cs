using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Modele
{
    public class Launcher:Element
    {
        public int NbJeux { get; set; } = 0;
        public Launcher(LauncherName Name=LauncherName.Autre):base(Name.ToString())
        {

        }
        public Launcher(int NbJeux, LauncherName Name = LauncherName.Autre):this(Name)
        {
            this.NbJeux = NbJeux;
        }
    }
}
