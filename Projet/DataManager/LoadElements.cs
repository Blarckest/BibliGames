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
        public LoadElements(string Folder) : base(Folder)
        {
        }

        public override Data Load()
        {
            bool NeedRecupGames = true; //determine si on a besoin de recuperer les jeux a nouveaux
            string[] AdditionalFolder=null;
            List<Launcher> Launchers;
            List<Jeu> Games;
            List<Element> Elements=new List<Element>();
            Dictionary<LauncherName, List<string>> DirectoryDetected;
            if (File.Exists($"{Folder}/LauncherInfo.xml") && File.Exists($"{Folder}/GamesInfo.xml") && File.Exists($"{Folder}/AdditionalFolder.txt") && new FileInfo($"{Folder}/LauncherInfo.xml").Length != 0) //si la sauvegarde existe et que les fichiers sont pas vide si AdditionalFolder.txt est vide c pas grave
            {
                XDocument LaunchersFile = XDocument.Load($"{Folder}/LauncherInfo.xml");

                Launchers = LaunchersFile.Descendants("Launcher") //chargement des launcher
                                      .Select(e => new Launcher()
                                      {
                                          Nom = e.Attribute("Nom").Value,
                                          NbJeux = int.Parse(e.Element("NbJeux").Value)
                                      })
                                      .ToList();

                XDocument GamesFile = XDocument.Load($"{Folder}/GamesInfo.xml");

                Games = GamesFile.Descendants("Jeu") //chargement des jeux
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



                AdditionalFolder = System.IO.File.ReadAllLines($"{Folder}/AdditionalFolder.txt"); //on recupere les dossier de recherche
                DirectoryDetected = SearchForGameDirectory.GetAllGameDirectory(AdditionalFolder.ToList()); //get les directory qu'est censer avoir la sauvegarde
                var JeuxManuallyAdded = Games.Where(j => j.IsManuallyAdded); //on recup les jeux ajouter manuellement
                if (JeuxManuallyAdded.Count()!=0)
                {
                    var DossierJeuxManuallyAdded = JeuxManuallyAdded.Select(j => j.Dossier).ToList(); //on recup leurs dossier pour que la detection de mise a jour se passe bien
                    if (DirectoryDetected.ContainsKey(LauncherName.Autre))
                    {
                        DirectoryDetected[LauncherName.Autre].AddRange(DossierJeuxManuallyAdded); //on append si la clé était deja defini
                    }
                    else
                    {
                        DirectoryDetected.Add(LauncherName.Autre, DossierJeuxManuallyAdded); //on defini la clé si elle l'eteit pas avant
                    }
                }

                if (Launchers.All(l => DirectoryDetected.Keys.Contains((LauncherName)Enum.Parse(typeof(LauncherName), l.Nom)))) //si on a bien tt les clé en rapport avec la sauvegarde
                {
                    foreach (LauncherName Launcher in DirectoryDetected.Keys) //on itere sur les clés
                    {
                        List<string> ListeDossier;
                        if (DirectoryDetected.TryGetValue(Launcher, out ListeDossier))
                        {
                            if (Games.Where(e => e.Launcher == Launcher).All(e => ListeDossier.Contains(e.Dossier))) //on regarde si chaque jeu a son dossier dans les dossiers retourné par GetAllGameDirectory
                            {
                                NeedRecupGames = false;
                            }
                        }
                    }
                    if (!NeedRecupGames)
                    {
                        foreach (Launcher launcher in Launchers) //on remplit la liste d'élément
                        {
                            Elements.Add(launcher);
                            Elements.AddRange(Games.Take(launcher.NbJeux));
                            Games.RemoveRange(0, launcher.NbJeux);
                        }
                    }
                }
                else
                {
                    NeedRecupGames = true;
                }
            }
            else
            {
                NeedRecupGames = true;
                DirectoryDetected = SearchForGameDirectory.GetAllGameDirectory();
            }




            if (NeedRecupGames) //si on a besoin de recuperer les jeux
            {
                Games = SearchForExecutableAndName.GetExecutableAndNameFromGameDirectory(DirectoryDetected);
                if (Games.Count>0)//si l'utilisateur a des jeux
                {
                    Launcher Actuel = new Launcher(Games[0].Launcher);
                    Elements.Add(Actuel);
                    for (int i = 0; i < Games.Count; i++)
                    {
                        if (Games[i].Launcher.ToString() == Actuel.ToString()) //on est dans le meme launcher
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
            }

            foreach (Element element in Elements)//on set les infos
            {
                if (element.GetType() == typeof(Jeu))
                {
                    Logs.InfoLog($"Recherche des données pour {element.Nom}");
                    Thread thread = new Thread(new ParameterizedThreadStart(SearchInfo.SetInfo));
                    thread.Start(element);
                }
            }

            Logs.InfoLog("Chargement des données");

            Data data;
            if (AdditionalFolder!=null)
            {
                 data = new Data(Elements, AdditionalFolder.ToList());
            }
            else
            {
                data = new Data(Elements, LoadAdditionalPath());
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
