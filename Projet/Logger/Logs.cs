using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Logger
{
    public static class Logs
    {
        private static Thread Thread = new Thread(SaveLog);
        private static string Dossier = "./Logs";
        private static ConcurrentQueue<string> Queue= new ConcurrentQueue<string>();

        private static void SaveLog()
        {
            string log;
            Directory.CreateDirectory(Dossier);
            StreamWriter sw = new StreamWriter($"{Dossier}/FichierDeLogs.txt", true);
            while (Queue.TryDequeue(out log))
            {
                sw.WriteLine(log);
            }
            sw.Close();
        }

        public static void InfoLog(string action)
        {
            Queue.Enqueue($"{DateTime.Now} | Info : {action}");
            if (!Thread.IsAlive)
            {
                Thread = new Thread(SaveLog);
                Thread.Start(); 
            }
        }

        public static void ErrorLog(string action)
        {
            Queue.Enqueue($"{DateTime.Now} | Error : {action}");
            if (!Thread.IsAlive)
            {
                Thread = new Thread(SaveLog);
                Thread.Start();
            }
        }

        public static void WarningLog(string action)
        {
            Queue.Enqueue($"{DateTime.Now} | Warning : {action}");
            if (!Thread.IsAlive)
            {
                Thread = new Thread(SaveLog);
                Thread.Start();
            }
        }

        public static void SuppLog()
        {
            string fichier = Dossier + "/FichierDeLogs.txt";
            if (File.Exists(fichier))
            {
                StreamReader sr = new StreamReader($"{Dossier}/FichierDeLogs.txt");
                string line = sr.ReadLine();
                sr.Close();
                line = line.Substring(0, 10);
                DateTime date = DateTime.Today;
                if (line != date.ToString("d"))
                {
                    File.Delete($"{Dossier}/FichierDeLogs.txt");
                }
            }
        }
    }
}
