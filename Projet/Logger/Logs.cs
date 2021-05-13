using System;
using System.IO;

namespace Logger
{
    public static class Logs
    {
        private static string Dossier = "./Logs";
        private static DateTime Date = DateTime.Now;

        private static void SaveLog(string Action, string Type)
        {
            Directory.CreateDirectory(Dossier);
            StreamWriter Sw = new StreamWriter($"{Dossier}/FichierDeLogs.txt", true);
            Sw.WriteLine(Date);
            Sw.WriteLine($"{Type} : {Action}");
            Sw.WriteLine();
            Sw.Close();
        }

        public static void InfoLog(string Action)
        {
            SaveLog(Action, "Info");
        }

        public static void ErrorLog(string Action)
        {
            SaveLog(Action, "Error");
        }

        public static void WarningLog(string Action)
        {
            SaveLog(Action, "Warning");
        }

        public static void SuppLog()
        {
            StreamReader Sr = new StreamReader($"{Dossier}/FichierDeLogs.txt");
            string line = Sr.ReadLine();
            Sr.Close();
            line = line.Substring(0, 10);
            DateTime date = DateTime.Today;
            if (line != date.ToString("d"))
            {
                File.Delete($"{Dossier}/FichierDeLogs.txt");
            }
        }
    }
}
