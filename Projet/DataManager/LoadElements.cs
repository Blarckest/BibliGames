using Modele;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager
{
    public class LoadElements : Loader
    {
        public LoadElements(string Path) : base(Path)
        {
        }

        public override IList<Element> Load()
        {
            throw new NotImplementedException();
        }
    }
}
