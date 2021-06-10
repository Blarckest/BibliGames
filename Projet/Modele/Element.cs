using System;
using System.Diagnostics.CodeAnalysis;

namespace Modele
{
    public abstract class Element : IEquatable<Element>, IComparable<Element>
    {
        public string Nom { get; set; } = null;
        protected Element(string nom)
        {
            Nom = nom;
        }

        public virtual bool Equals([AllowNull] Element other)
        {
            if (other != null)
            {
                return Nom.Equals(other.Nom);
            }
            return false;
        }

        public virtual int CompareTo([AllowNull] Element other)
        {
            return Nom.CompareTo(other.Nom);
        }
        public override string ToString()
        {
            return Nom;
        }
    }
}
