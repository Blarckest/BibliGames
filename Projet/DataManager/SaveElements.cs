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

            var launchers = elements.Where(e => e.GetType() == typeof(Launcher)) //sauvegarde des launchers
                                    .Select(e => e as Launcher)
                                    .Select(e => new XElement("Launcher",
                                    new XAttribute("Nom", e.Nom),               
                                    new XElement("NbJeux", e.NbJeux)));

            fichier.Add(new XElement("Launchers", launchers));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; //on active l'indatage du fichier
            TextWriter textWriter = File.CreateText($"{Folder}/LauncherInfo.xml");
            XmlWriter writer = XmlWriter.Create(textWriter, settings);
            fichier.Save(writer); //on ecrit
            writer.Close();
            textWriter.Close();

            fichier = new XDocument();
            var jeux = elements.Where(e => e.GetType() == typeof(Jeu)) //sauvegarde des jeux
                             .Select(e => e as Jeu)
                             .Select(e => new XElement("Jeu",
                             new XAttribute("Nom", e.Nom),
                             new XElement("Dossier", e.Dossier),
                             new XElement("Exec", e.Exec),
                             new XElement("Launcher", e.Launcher.ToString()),
                             new XElement("Description", e.Description),
                             new XElement("Note", e.Note),
                             new XElement("Image", e.Image),
                             new XElement("Icone", e.Icone),
                             new XElement("IsManuallyAdded",e.IsManuallyAdded)));

            fichier.Add(new XElement("Jeux", jeux));

            textWriter = File.CreateText($"{Folder}/GamesInfo.xml");
            writer = XmlWriter.Create(textWriter, settings);
            fichier.Save(writer); //on ecrit
            writer.Close();
            textWriter.Close();

            TextWriter fichierAdditionalPaths = new StreamWriter($"{Folder}/AdditionalFolder.txt");
            if (additionalFolder!=null)
            {
                foreach (string path in additionalFolder)
                {
                    fichierAdditionalPaths.WriteLine(path); //on copie chaque dossier supplementaire dans unfichier
                }
            }            
            fichierAdditionalPaths.Close();
            Logs.InfoLog("Sauvegarde de l'application");
        }

        public override void Save(Data data) //constructeur prenant un manager (fait la meme chose que l'autre)
        {
            Save(data.Elements, data.Dossiers);
        }
    }
}
