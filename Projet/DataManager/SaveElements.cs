using Modele;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager
{
    public class SaveElements : Saver
    {
        public SaveElements(string Path) : base(Path)
        {
           
        }
        public override void Save(IList<Element> Elements)
        {
            throw new NotImplementedException();
        }
    }
}
