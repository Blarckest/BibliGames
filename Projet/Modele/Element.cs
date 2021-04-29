using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    public class Element
    {
        public string Icone { get; set; }

        public string Nom { get; set;}

        private Element(string Nom, string Icone)
        {
            this.Nom = Nom;
            this.Icone = Icone;
        }

    }
}
