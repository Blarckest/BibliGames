using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;

namespace Modele
{
    public class Manager : INotifyPropertyChanged
    {
        private Element elementSelected;
        public Element ElementSelected
        {
            get => elementSelected;
            set
            {
                elementSelected = value;
                NotifyPropertyChanged();
            }
        }
        public bool SearchActivated { get; set; } = true;
        public IList<Element> Affichage
        {
            get
            {
                var temp = GetData().Elements; //on recupere les elements et on applique recherche dessus
                Recherche(temp);
                return temp;
            }
        }
        public IList<string> Dossiers => GetData().Dossiers.ToList(); //le ToList permet d'eviter la modification de dossier depuis l'exterieur
        public List<Element> JeuLauncherSelected
        {
            get
            {
                if (ElementSelected!=null)
                {
                    if (ElementSelected.GetType() == typeof(Launcher))
                    {
                        Launcher launcher = ElementSelected as Launcher;
                        var temp = GetData().Elements.Skip(GetData().GetLauncherIndex((LauncherName)Enum.Parse(typeof(LauncherName), launcher.Nom)) + 1).Take(launcher.NbJeux).ToList();
                        Recherche(temp);
                        return temp;
                    }
                    return new List<Element> { }; 
                }
                return null;
            }
        }
        public string Pattern { get; set; } = "Rechercher";
        private readonly Data data;

        private IPersistance Persistance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Manager(IPersistance persistance)
        {
            Setup();
            Persistance = persistance;
            data = Persistance.Load().CloneAll();
            data.SetInfoForAll(); //on set les infos
            Logs.InfoLog("Chargement des données");            
        }
        
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            Persistance.Save(GetData());
        }

        public void AjoutJeu(LauncherName launcher, string exec)
        {
            if (File.Exists(exec))
            {
                data.AjoutJeu(launcher, exec);
                NotifyPropertyChanged("Affichage");
                NotifyPropertyChanged("JeuLauncherSelected");
            }
        }

        public void AjoutJeu(Jeu jeu)
        {
            //notify se fait dans la fonction appeler
            data.AjoutJeu(jeu);
        }

        public void ModifDetail(string image, string description, string exec, string icone)
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                data.ModifDetail(image, description, exec, icone, ElementSelected as Jeu);
            }
        }

        public void SuppJeu()
        {
            if (ElementSelected.GetType() == typeof(Jeu))
            {
                data.SuppJeu(ElementSelected as Jeu);
                NotifyPropertyChanged("Affichage");
            }
        }

        public void LancerJeu()
        {
            var elem = ElementSelected as Jeu;
            try
            {
                Logs.InfoLog($"Lancement du jeu {elem.Nom}");
                System.Diagnostics.Process.Start(elem.Exec); //normalement ca marche a tester
            }
            catch
            {
                Logs.ErrorLog($"Lancement du jeu {elem.Nom} imposssible");
                return;
            }
        }

        /// <summary>
        /// Appeller lors de l'ajout d'un dossier dans les parametre
        /// </summary>
        /// <param name="dossier"></param>
        public void AjouterDossier(string dossier)
        {
            data.AjouterDossier(dossier);
            NotifyPropertyChanged("Affichage");
            NotifyPropertyChanged("JeuLauncherSelected");
        }

        public void SuppDossier(string dossier)
        {
            data.SuppDossier(dossier);
            NotifyPropertyChanged("Affichage");
            NotifyPropertyChanged("JeuLauncherSelected");
        }

        public void UpdateRecherche()
        {
            if (SearchActivated)
            {
                NotifyPropertyChanged("Affichage");
                NotifyPropertyChanged("JeuLauncherSelected");
            }
        }

        //effectue juste une suppression des elements non désiré (correspondant pas au pattern)
        private void Recherche(IList<Element> elements)
        {
            if (!(string.IsNullOrEmpty(Pattern) || Pattern == "Rechercher") && SearchActivated)
            {
                int i = 0;
                while (i < elements.Count)
                {
                    if (elements[i].GetType() == typeof(Jeu) && !elements[i].Nom.Contains(Pattern, StringComparison.OrdinalIgnoreCase)) //si on est sur on jeu et que il correspond pas au pattern
                    {
                        elements.RemoveAt(i);
                        continue;
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// permet de s'assurer que les images par defaut existe
        /// </summary>
        private void Setup()
        {
            if (!File.Exists("./Ressources/Defaut/icone.png") || !File.Exists("./Ressources/Defaut/image.png"))
            {
                Directory.CreateDirectory("./Ressources/Defaut");
                CopyDllRessourceToFile(Assembly.LoadFrom("Icones.dll"), "image.png", "./Ressources/Defaut/image.png");
                CopyDllRessourceToFile(Assembly.LoadFrom("Icones.dll"), "icone.png", "./Ressources/Defaut/icone.png");
            }
        }
        private void CopyDllRessourceToFile(Assembly assembly, string ressource, string destination)
        {
            var memStream = ExctractImageFromDll(assembly, ressource); //on recupere l'image sous forme de stream
            var fileStream = File.Create(destination); //on creer le fichier
            memStream.CopyTo(fileStream); //on copy les données
            fileStream.Close(); //on ferme
        }
        private MemoryStream ExctractImageFromDll(Assembly assembly, string nomFichier)
        {
            string ressourcePath = assembly.GetName().Name + ".g.resources"; //les ressources sont la dedans
            System.Resources.ResourceReader resourceReader = new System.Resources.ResourceReader(assembly.GetManifestResourceStream(ressourcePath)); //on met un ressourcereader sur le fichier qui nous interesse
            resourceReader.GetResourceData(nomFichier, out _, out byte[] data);//on ne s'interesse pas au type // data contient les données
            return new MemoryStream(data, 4, data.Length - 4);//on revoit un memorystream le 4 correspond a un offset de 4 octet qui est present pour une raison inconnu
        }
        private Data GetData()
        {
            return data.CloneCollections(); //le clonage permet d'eviter une modification des collections non voulu depuis l'exterieur
        }
    }
}
