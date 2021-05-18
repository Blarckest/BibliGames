﻿using System;
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
            return;
            Directory.CreateDirectory(Dossier);
            StreamWriter Sw = new StreamWriter($"{Dossier}/FichierDeLogs.txt", true);
            Sw.WriteLine($"{Date} | {Type} : {Action}\n");
            Sw.Close();
        }

        public static void InfoLog(string Action)
        {
            Queue.Enqueue($"{DateTime.Now} | Info : {Action}");
            if (!Thread.IsAlive)
            {

            }
            SaveLog(Queue.TryDequeue());
        }

        public static void ErrorLog(string Action)
        {
            Queue.Enqueue($"{DateTime.Now} | Error : {Action}");
        }

        public static void WarningLog(string Action)
        {
            Queue.Enqueue($"{DateTime.Now} | Warning : {Action}");
        }

        public static void SuppLog()
        {
            string Fichier = Dossier + "/FichierDeLogs.txt";
            if (File.Exists(Fichier))
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
}
