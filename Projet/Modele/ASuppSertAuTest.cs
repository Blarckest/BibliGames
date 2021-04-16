using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    class ASuppSertAuTest
    {
        public static void Main(string[] args)
        {
            var res = AutoSearchForGameDirectory.GetAllGameDirectory();
            AutoSearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(res);
        }
    }
}
