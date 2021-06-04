using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Logger
{
    public static class Logs
    {
        private static Thread thread = new Thread(SaveLog);
        private static string dossier = "./Logs";
        private static readonly ConcurrentQueue<string> queue= new ConcurrentQueue<string>();

        private static void SaveLog()
        {
            string log;
            Directory.CreateDirectory(dossier);
            StreamWriter sw = new StreamWriter($"{dossier}/FichierDeLogs.txt", true);
            while (queue.TryDequeue(out log))
            {
                sw.WriteLine(log);
            }
            sw.Close();
        }

        public static void InfoLog(string action)
        {
            queue.Enqueue($"{DateTime.Now} | Info : {action}");
            if (!thread.IsAlive)
            {
                try
                {
                    thread = new Thread(SaveLog);
                    thread.Start();
                }
                catch { }
            }  
        }

        public static void ErrorLog(string action)
        {
            queue.Enqueue($"{DateTime.Now} | Error : {action}");
            if (!thread.IsAlive)
            {
                try
                {
                    thread = new Thread(SaveLog);
                    thread.Start();
                }
                catch { }
            }
        }

        public static void WarningLog(string action)
        {
            queue.Enqueue($"{DateTime.Now} | Warning : {action}");
            if (!thread.IsAlive)
            {
                try
                {
                    thread = new Thread(SaveLog);
                    thread.Start();
                }
                catch { }
            }
        }

        public static void SuppLog()
        {
            string fichier = dossier + "/FichierDeLogs.txt";
            if (File.Exists(fichier))
            {
                StreamReader sr = new StreamReader($"{dossier}/FichierDeLogs.txt");
                string line = sr.ReadLine();
                sr.Close();
                line = line.Substring(0, 10);
                DateTime date = DateTime.Today;
                if (line != date.ToString("d"))
                {
                    File.Delete($"{dossier}/FichierDeLogs.txt");
                }
            }
        }
    }
}
