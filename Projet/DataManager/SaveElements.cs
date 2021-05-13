using Logger;
using Modele;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace DataManager
{
    public class SaveElements : Saver
    {
        public SaveElements(string Folder) : base(Folder)
        {
           
        }

        public override void Save(IList<Element> Elements, IList<string> AdditionalFolder)
        {
            XDocument Fichier = new XDocument();

            var Launchers = Elements.Where(e => e.GetType() == typeof(Launcher)) //sauvegarde des launchers
                                    .Select(e => e as Launcher)
                                    .Select(e => new XElement("Launcher",
                                    new XAttribute("Nom", e.Nom),               
                                    new XElement("NbJeux", e.NbJeux)));

            Fichier.Add(new XElement("Launchers", Launchers));
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.Indent = true; //on active l'indatage du fichier
            TextWriter TextWriter = File.CreateText($"{Folder}/LauncherInfo.xml");
            XmlWriter Writer = XmlWriter.Create(TextWriter, Settings);
            Fichier.Save(Writer); //on ecrit
            Writer.Close();
            TextWriter.Close();

            Fichier = new XDocument();
            var Jeux = Elements.Where(e => e.GetType() == typeof(Jeu)) //sauvegarde des jeux
                             .Select(e => e as Jeu)
                             .Select(e => new XElement("Jeu",
                             new XAttribute("Nom", e.Nom),
                             new XElement("Dossier", e.Dossier),
                             new XElement("Exec", e.Exec),
                             new XElement("Launcher", e.Launcher.ToString()),
                             new XElement("Description", e.Description),
                             new XElement("Note", e.Note),
                             new XElement("Image", e.Image),
                             new XElement("Icone", e.Icone)));

            Fichier.Add(new XElement("Jeux", Jeux));

            TextWriter = File.CreateText($"{Folder}/GamesInfo.xml");
            Writer = XmlWriter.Create(TextWriter, Settings);
            Fichier.Save(Writer); //on ecrit
            Writer.Close();
            TextWriter.Close();

            TextWriter FichierAdditionalPaths = new StreamWriter($"{Folder}/AdditionalFolder.txt");
            if (AdditionalFolder!=null)
            {
                foreach (string Path in AdditionalFolder)
                {
                    FichierAdditionalPaths.WriteLine(Path); //on copie chaque dossier supplementaire dans unfichier
                }
            }            
            FichierAdditionalPaths.Close();
            Logs.InfoLog("Sauvegarde de l'application");
            Logs.SuppLog();
        }

        public override void Save(Manager Manager) //constructeur prenant un manager (fait la meme chose que l'autre)
        {
            Save(Manager.Elements, Manager.Dossiers);
        }
    }
}
