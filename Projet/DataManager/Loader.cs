using Modele;
using System;
using System.Collections.Generic;

namespace DataManager
{
    public abstract class Loader
    {
        protected string Path;
        public Loader(string Path)
        {
            this.Path = Path;
        }
        public abstract IList<Element> Load();
    }
}
