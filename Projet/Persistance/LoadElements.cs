using Logger;
using Modele;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace Persistance
{
    internal class LoadElements : Loader
    {
        public LoadElements(string folder) : base(folder)
        {
        }

        public override Data Load()
        {
            bool needRecupGames = !LoadSave(out List<Jeu> games, out List<Launcher> launchers, out List<string> additionalFolder);
            IList<GameSearcher> searchers = new List<GameSearcher>
            {
                new EpicSearcher(),
                new OriginSearcher(),
                new RiotSearcher(),
                new SteamSearcher(),
                new UplaySearcher(),
                new OtherSearcher(additionalFolder)
            };

            List<Element> elements = new List<Element>();


            if (needRecupGames || !IsSaveOk(searchers,games))//si on a besoin de recuperer les jeux
            {
                List<Jeu> gamesFound = new List<Jeu>();
                foreach (var searcher in searchers)//get les directory
                {
                    gamesFound.AddRange(searcher.Jeux);
                }
                if (gamesFound.Count > 0)//si l'utilisateur a des jeux
                {
                    Launcher actuel = new Launcher(gamesFound[0].Launcher);
                    elements.Add(actuel);
                    for (int i = 0; i < gamesFound.Count; i++)
                    {
                        if (gamesFound[i].Launcher.ToString() == actuel.ToString()) //on est dans le meme launcher
                        {
                            elements.Add(gamesFound[i]);
                            actuel.NbJeux++;//on augmente le nb de jeu
                        }
                        else
                        {
                            actuel = new Launcher(gamesFound[i].Launcher); //on ajoute le launcher
                            elements.Add(actuel);
                            elements.Add(gamesFound[i]);
                            actuel.NbJeux++;//on augmente le nb de jeu
                        }
                    }
                }
            }
            else 
            {
                foreach (Launcher launcher in launchers) //on remplit la liste d'élément
                {
                    elements.Add(launcher);
                    elements.AddRange(games.Take(launcher.NbJeux));
                    games.RemoveRange(0, launcher.NbJeux);
                }
            }

            if (Directory.Exists("./Test"))//si on doit retourner un stub (fait pour les profs a supp sur version officiel)
            {
                Logs.WarningLog("Dossier test trouvé->utilisation du stub");
                if (games != null && launchers != null && games.Count == 11 && launchers.Count == 1)//si on a charger probablement un stub on renvoie les données de la sauvegarde
                {
                    elements.AddRange(launchers);
                    elements.AddRange(games);
                    return new Data(elements, new List<string>());
                }
                return new Stub.Stub().Load(); // sinon on charge un nouveau stub
            }
            return new Data(elements, additionalFolder);
        }

        private bool LoadSave(out List<Jeu> games, out List<Launcher> launchers, out List<string> dossiers)
        {
            if (File.Exists($"{Folder}/BibliGames.xml") && new FileInfo($"{Folder}/BibliGames.xml").Length != 0) //si la sauvegarde existe et que le fichiers sont pas vide
            {
                XDocument saveFile = XDocument.Load($"{Folder}/BibliGames.xml");

                var save = saveFile.Descendants();

                launchers = save.Descendants("Launchers").Descendants() //chargement des launcher
                                      .Select(e => new Launcher()
                                      {
                                          Nom = e.Attribute("Nom").Value,
                                          NbJeux = int.Parse(e.Attribute("NbJeux").Value)
                                      })
                                      .ToList();

                games = save.Descendants("Jeux").Descendants() //chargement des jeux
                                   .Select(e => new Jeu(
                                        e.Attribute("Nom").Value,
                                        e.Attribute("Dossier").Value,
                                        e.Attribute("Exec").Value,
                                        e.Attribute("Image").Value,
                                        e.Attribute("Icone").Value,
                                        e.Attribute("Note").Value,
                                        e.Attribute("Description").Value,
                                        (LauncherName)Enum.Parse(typeof(LauncherName), e.Attribute("Launcher").Value),
                                        Convert.ToBoolean(e.Attribute("IsManuallyAdded").Value)))
                                   .ToList();

                dossiers = save.Descendants("DossiersSupp").Descendants() //chargement des dossiers supplementaires
                                        .Select(d => d.Attribute("Nom").Value)
                                        .ToList();
                return true;
            }
            games = null;
            launchers = null;
            dossiers = null;
            return false;
        }

        private bool IsSaveOk(IEnumerable<GameSearcher> searchers, IList<Jeu> games)
        {
            List<string> directoryDetected = new List<string>();

            foreach (var searcher in searchers)//get les directory qu'est censer avoir la sauvegarde
            {
                directoryDetected.AddRange(searcher.Dossiers);
            }

            var jeuxManuallyAdded = games.Where(j => j.IsManuallyAdded); //on recup les jeux ajouter manuellement
            if (jeuxManuallyAdded.Count() != 0)
            {
                var dossierJeuxManuallyAdded = jeuxManuallyAdded.Select(j => j.Dossier).ToList(); //on recup leurs dossier pour que la detection de mise a jour se passe bien
                directoryDetected.AddRange(dossierJeuxManuallyAdded); //on ajoute les dossiers des jeux ajoutés a la main
            }

            if (!games.All(j => directoryDetected.Contains(j.Dossier))) //on regarde si chaque jeu a son dossier dans les dossiers trouvés
            {
                return true;
            }
            return false;
        }
    }
}
