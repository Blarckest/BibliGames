using Modele;
using System;
using System.Collections.Generic;

namespace DataManager
{
    public abstract class Loader
    {
        protected readonly string Folder;
        protected Loader(string folder)
        {
            this.Folder = folder;
        }
        public abstract Data Load();
        protected abstract IList<string> LoadAdditionalPath();
    }
}
