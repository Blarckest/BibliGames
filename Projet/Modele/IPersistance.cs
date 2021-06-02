using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    public interface IPersistance
    {
        Data Load();
        void Save(Data data);
    }
}
