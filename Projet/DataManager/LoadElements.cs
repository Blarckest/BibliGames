﻿using Logger;
using Modele;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace DataManager
{
    public class LoadElements : Loader
    {
        public LoadElements(string folder) : base(folder)
        {
        }

        public override Data Load()
        {
            bool needRecupGames = true; //determine si on a besoin de recuperer les jeux a nouveaux
            string[] additionalFolder = null;
            List<Launcher> launchers = null;
            List<Jeu> games = null;
            List<Element> elements=new List<Element>();
            IDictionary<LauncherName, List<string>> directoryDetected;
            if (File.Exists($"{Folder}/LauncherInfo.xml") && File.Exists($"{Folder}/GamesInfo.xml") && File.Exists($"{Folder}/AdditionalFolder.txt") && new FileInfo($"{Folder}/LauncherInfo.xml").Length != 0) //si la sauvegarde existe et que les fichiers sont pas vide si AdditionalFolder.txt est vide c pas grave
            {
                XDocument launchersFile = XDocument.Load($"{Folder}/LauncherInfo.xml");

                launchers = launchersFile.Descendants("Launcher") //chargement des launcher
                                      .Select(e => new Launcher()
                                      {
                                          Nom = e.Attribute("Nom").Value,
                                          NbJeux = int.Parse(e.Element("NbJeux").Value)
                                      })
                                      .ToList();

                XDocument gamesFile = XDocument.Load($"{Folder}/GamesInfo.xml");

                games = gamesFile.Descendants("Jeu") //chargement des jeux
                                   .Select(e => new Jeu(
                                        e.Attribute("Nom").Value,
                                        e.Element("Dossier").Value,
                                        e.Element("Exec").Value,
                                        e.Element("Image").Value,
                                        e.Element("Icone").Value,
                                        e.Element("Note").Value,
                                        e.Element("Description").Value,
                                        (LauncherName)Enum.Parse(typeof(LauncherName), e.Element("Launcher").Value),
                                        Convert.ToBoolean(e.Element("IsManuallyAdded").Value)))
                                   .ToList();



                additionalFolder = System.IO.File.ReadAllLines($"{Folder}/AdditionalFolder.txt"); //on recupere les dossier de recherche
                directoryDetected = SearchForGameDirectory.GetAllGameDirectory(additionalFolder.ToList()); //get les directory qu'est censer avoir la sauvegarde
                var jeuxManuallyAdded = games.Where(j => j.IsManuallyAdded); //on recup les jeux ajouter manuellement
                if (jeuxManuallyAdded.Count()!=0)
                {
                    var dossierJeuxManuallyAdded = jeuxManuallyAdded.Select(j => j.Dossier).ToList(); //on recup leurs dossier pour que la detection de mise a jour se passe bien
                    if (directoryDetected.ContainsKey(LauncherName.Autre))
                    {
                        directoryDetected[LauncherName.Autre].AddRange(dossierJeuxManuallyAdded); //on append si la clé était deja defini
                    }
                    else
                    {
                        directoryDetected.Add(LauncherName.Autre, dossierJeuxManuallyAdded); //on defini la clé si elle l'eteit pas avant
                    }
                }

                if (launchers.All(l => directoryDetected.Keys.Contains((LauncherName)Enum.Parse(typeof(LauncherName), l.Nom)))) //si on a bien tt les clé en rapport avec la sauvegarde
                {
                    foreach (LauncherName launcher in directoryDetected.Keys) //on itere sur les clés
                    {
                        List<string> listeDossier;
                        if (directoryDetected.TryGetValue(launcher, out listeDossier))
                        {
                            if (games.Where(e => e.Launcher == launcher).All(e => listeDossier.Contains(e.Dossier))) //on regarde si chaque jeu a son dossier dans les dossiers retourné par GetAllGameDirectory
                            {
                                needRecupGames = false;
                            }
                        }
                    }
                    if (!needRecupGames)
                    {
                        foreach (Launcher launcher in launchers) //on remplit la liste d'élément
                        {
                            elements.Add(launcher);
                            elements.AddRange(games.Take(launcher.NbJeux));
                            games.RemoveRange(0, launcher.NbJeux);
                        }
                    }
                }
                else
                {
                    needRecupGames = true;
                }
            }
            else
            {
                needRecupGames = true;
                directoryDetected = SearchForGameDirectory.GetAllGameDirectory();
            }




            if (needRecupGames) //si on a besoin de recuperer les jeux
            {
                var gamesFound = SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(directoryDetected);
                if (gamesFound.Count>0)//si l'utilisateur a des jeux
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

            foreach (Element element in elements)//on set les infos
            {
                if (element.GetType() == typeof(Jeu))
                {
                    Logs.InfoLog($"Recherche des données pour {element.Nom}");
                    Thread thread = new Thread(SearchInfo.SetInfo);
                    thread.Start(element);
                }
            }

            Logs.InfoLog("Chargement des données");

            if (elements.Count==0)
            {
                Logs.WarningLog("Pas de données présente->utilisation du stub");
                if (games!=null && launchers!=null && games.Count==10 && launchers.Count==1)//si on a charger probablement un stub on renvoie les données de la sauvegarde
                {
                    elements.AddRange(launchers);
                    elements.AddRange(games);
                    return new Data(elements, new List<string>());
                }
                return new Stub().Load(); // sinon on charge un nouveau stub
            }

            Data data;
            if (additionalFolder!=null)
            {
                 data = new Data(elements, additionalFolder.ToList());
            }
            else
            {
                data = new Data(elements, LoadAdditionalPath());
            }
            
            return data;
        }

        public override IList<string> LoadAdditionalPath()
        {
            if (File.Exists($"{Folder}/AdditionalFolder.txt"))
            {
                return new List<string>(File.ReadAllLines($"{Folder}/AdditionalFolder.txt")); //on recupere les dossier de recherche
            }
            return new List<string>() { }; //si le fichier existait pas on retourne une liste vide
            
        }
    }
}
