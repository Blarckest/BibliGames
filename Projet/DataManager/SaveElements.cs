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
            var Launchers = Elements.Where(e => e.GetType() == typeof(Launcher))
                                    .Select(e => e as Launcher)
                                    .Select(e => new XElement("Launcher",
                                    new XAttribute("Nom", e.Nom),               
                                    new XElement("NbJeux", e.NbJeux)));

            Fichier.Add(new XElement("Launchers", Launchers));
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.Indent = true;
            TextWriter TextWriter = File.CreateText($"{Folder}/LauncherInfo.xml");
            XmlWriter Writer = XmlWriter.Create(TextWriter, Settings);
            Fichier.Save(Writer);

            Fichier = new XDocument();
            var Jeux = Elements.Where(e => e.GetType() == typeof(Jeu))
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

            Fichier.Add(new XElement("Launchers", Launchers));

            TextWriter = File.CreateText($"{Folder}/GamesInfo.xml");
            Writer = XmlWriter.Create(TextWriter, Settings);
            Fichier.Save(Writer);

            TextWriter FichierAdditionalPaths = new StreamWriter($"{Folder}/AdditionalFolder.txt");
            foreach (string Path in AdditionalFolder)
            {
                FichierAdditionalPaths.WriteLine(Path);
            }
            FichierAdditionalPaths.Close();
        }

        public override void Save(Manager Manager)
        {
            Save(Manager.Elements, Manager.Dossiers);
        }
    }
}
