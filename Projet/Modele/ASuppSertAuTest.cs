using System;
using System.Collections.Generic;
using System.Text;

namespace Modele
{
    class ASuppSertAuTest
    {
        public static void Main(string[] args)
        {
           /* Jeu Test = new Jeu("Among Us", @"D:\Jeux\Steam\steamapps\common", @"D:\Jeux\Steam\steamapps\common\Among Us\Among US.exe");
            SearchInfo.SetInfo(Test);*/
            var res = SearchForGameDirectory.GetAllGameDirectory();
            string[] dossiers = { "G:\\", "D:\\Jeux\\battle.net" };
            res.Add(LauncherName.Autre, SearchForGameDirectory.GetGameDirectoryFromPaths(dossiers));
            SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(res); 
            //SearchInfo.SetInfo();
        }
    }
}
