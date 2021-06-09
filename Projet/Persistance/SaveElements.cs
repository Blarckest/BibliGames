using Logger;
using Modele;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Persistance
{
    internal class SaveElements : Saver
    {
        public SaveElements(string folder) : base(folder)
        {
           
        }

        public override void Save(IList<Element> elements, IList<string> additionalFolder)
        {
            XDocument fichier = new XDocument();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; //on active l'indentage du fichier
            using TextWriter textWriter = File.CreateText($"{Folder}/BibliGames.xml");
            using XmlWriter writer = XmlWriter.Create(textWriter, settings);

            var launchers = new XElement("Launchers",elements.Where(e => e.GetType() == typeof(Launcher)) //sauvegarde des launchers
                                    .Select(e => e as Launcher)
                                    .Select(e => new XElement("Launcher",
                                    new XAttribute("Nom", e.Nom ?? ""),               
                                    new XAttribute("NbJeux", e.NbJeux))));

            var jeux = new XElement("Jeux",elements.Where(e => e.GetType() == typeof(Jeu)) //sauvegarde des jeux
                             .Select(e => e as Jeu)
                             .Select(e => new XElement("Jeu",
                             new XAttribute("Nom", e.Nom ?? ""),
                             new XAttribute("Dossier", e.Dossier ?? ""),
                             new XAttribute("Exec", e.Exec ?? ""),
                             new XAttribute("Launcher", e.Launcher.ToString() ?? ""),
                             new XAttribute("Description", e.Description ?? ""),
                             new XAttribute("Note", e.Note ?? ""),
                             new XAttribute("Image", e.Image ?? ""),
                             new XAttribute("Icone", e.Icone ?? ""),
                             new XAttribute("IsManuallyAdded",e.IsManuallyAdded))));

            var dossiersSupp = new XElement("DossiersSupp", additionalFolder.Select(d=>new XElement("Dossier",new XAttribute("Nom", d ?? "")))); //sauvegarde des dossiers supplementaires

            fichier.Add(new XElement("BibliGames", launchers, jeux, dossiersSupp));           
            fichier.Save(writer); //on ecrit

            Logs.InfoLog("Sauvegarde des données");
        }

        public override void Save(Data data) //constructeur prenant un Data (fait la meme chose que l'autre)
        {
            Save(data.Elements, data.Dossiers);
        }
    }
}
