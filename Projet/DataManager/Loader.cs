using Modele;
using System;
using System.Collections.Generic;

namespace DataManager
{
    public abstract class Loader
    {
        protected string Folder;
        public Loader(string Folder)
        {
            this.Folder = Folder;
        }
        public abstract IList<Element> Load();
        public abstract IList<string> LoadAdditionalPath();
    }
}
