using Modele;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager
{
    public abstract class Saver
    {
        protected string Path;
        public Saver(string Path)
        {
            this.Path = Path;
        }
        public abstract void Save(IList<Element> Elements, IList<string> AdditionalFolder);
    }
}
