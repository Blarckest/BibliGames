using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    public class Element
    {
        public string Icone { get; set; } = null;
        public string Nom { get; set;} = null;
        public Type Type { get; set; }

        protected Element(Type Type = Type.Launcher)
        {
            this.Type = Type;
        }
        protected Element(string Nom,Type Type = Type.Launcher)
        {
            this.Nom = Nom;
            this.Type = Type;
        }
        protected Element(string Nom, string Icone,Type Type=Type.Launcher)
        {
            this.Nom = Nom;
            this.Icone = Icone;
            this.Type = Type;
        }

    }
}
