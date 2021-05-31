using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Modele
{
    public class Launcher:Element
    {
        private int nbjeux = 0;
        public int NbJeux
        {
            get => nbjeux;
            set => nbjeux = value < 0 ? 0 : value;
        }
        public Launcher(LauncherName name=LauncherName.Autre):base(name.ToString())
        {

        }
        public Launcher(int nbJeux, LauncherName name = LauncherName.Autre):this(name)
        {
            NbJeux = nbJeux;
        }
    }
}
