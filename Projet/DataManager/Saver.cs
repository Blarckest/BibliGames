using Modele;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager
{
    public abstract class Saver
    {
        protected string Folder;
        public Saver(string Folder)
        {
            this.Folder = Folder;
        }
        public abstract void Save(Manager Manager);
        public abstract void Save(IList<Element> Elements, IList<string> AdditionalFolder);
    }
}
