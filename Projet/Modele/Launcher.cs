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
            get { return nbjeux; }
            set
            {
                if (value < 0)
                {
                    nbjeux = 0;
                }
                else
                {
                    nbjeux = value;
                }
            }
        }
        public Launcher(LauncherName Name=LauncherName.Autre):base(Name.ToString())
        {

        }
        public Launcher(int NbJeux, LauncherName Name = LauncherName.Autre):this(Name)
        {
            this.NbJeux = NbJeux;
        }
    }
}
