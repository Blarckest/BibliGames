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
    public class LoadElements : Loader
    {
        public LoadElements(string Folder) : base(Folder)
        {
        }

        public override IList<Element> Load()
        {
            bool NeedRecupGames = true;
            XDocument LaunchersFile = XDocument.Load($"{Folder}/LauncherInfo.xml");
            List<Element> Elements = new List<Element>();
            var Launchers = LaunchersFile.Descendants("Launcher")
                                  .Select(e => new Launcher()
                                  {
                                      Nom = e.Attribute("Nom").Value,
                                      NbJeux = int.Parse(e.Element("NbJeux").Value)
                                  })
                                  .ToList();
            XDocument GamesFile = XDocument.Load($"{Folder}/GamesInfo.xml");
            var Games=GamesFile.Descendants("Jeu")
                               .Select(e=>new Jeu(                               
                                    e.Attribute("Nom").Value,
                                    e.Attribute("Dossier").Value,
                                    e.Attribute("Exec").Value,
                                    e.Attribute("Image").Value,
                                    e.Attribute("Icone").Value,
                                    e.Attribute("Note").Value,
                                    e.Attribute("Description").Value,                                     
                                    (LauncherName)Enum.Parse(typeof(LauncherName), e.Attribute("Launcher").Value)))
                               .ToList();



            string[] AdditionalFolder= System.IO.File.ReadAllLines($"{Folder}/AdditionalFolder.txt"); //on recupere les dossier de recherche
            var DirectoryDetected = SearchForGameDirectory.GetAllGameDirectory(AdditionalFolder.ToList()); //get les directory qu'est censer avoir la sauvegarde
           

            if (Launchers.All(l => DirectoryDetected.Keys.Contains((LauncherName)Enum.Parse(typeof(LauncherName),l.Nom)))) //si on a bien tt les clé en rapport avec la sauvegarde
            {
                foreach (LauncherName Launcher in DirectoryDetected.Keys)
                {
                    List<string> ListeDossier;
                    if (DirectoryDetected.TryGetValue(Launcher, out ListeDossier))
                    {
                        if (Games.Where(e=>e.Launcher==Launcher).All(e => ListeDossier.Contains(e.Dossier))) //on regarde si chaque jeu a son dossier dans les dossiers retourné par GetAllGameDirectory
                        {
                            NeedRecupGames = false;
                        }
                    }
                }
            }


            if (NeedRecupGames) //si on a besoin de recuperer les jeux
            {
                Games = SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(DirectoryDetected);
                Launcher Actuel = new Launcher(Games[0].Launcher);
                Elements.Add(Actuel);
                for (int i = 0; i < Games.Count; i++)
                {
                    if (Games[i].Launcher.ToString()==Actuel.ToString()) //on est dans le meme launcher
                    {
                        Elements.Add(Games[i]);
                        Actuel.NbJeux++;//on augmente le nb de jeu
                    }
                    else
                    {
                        Actuel = new Launcher(Games[i].Launcher); //on ajoute le launcher
                        Elements.Add(Actuel);
                        Elements.Add(Games[i]);
                        Actuel.NbJeux++;//on augmente le nb de jeu
                    }
                }
            }
            else
            {
                foreach (Launcher launcher in Launchers)
                {
                    Elements.Add(launcher);
                    Elements.AddRange(Games.Take(launcher.NbJeux));
                    Games.RemoveRange(0, launcher.NbJeux);
                }
            }

            foreach (Element element in Elements)
            {
                if (element.GetType()==typeof(Jeu))
                {
                    SearchInfo.SetInfo(element as Jeu);
                }                
            }

            return Elements;
        }
    }
}
