using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FolderExplorer
{
    public class FolderExplorer : INotifyPropertyChanged
    {
        public bool SearchActivated; //dit a la barre de recherche si la recherche est activé
        private readonly Stack<string> historique; //contient l'historique
        private readonly Stack<string> forwardHistorique; //permet de retourner a la valeur d'avant
        public string DossierSelectionner { get; set; } //contient la valeur du dossier actuel
        public ObservableCollection<LigneExplorateur> ListeDossier { get; } //la liste afficher
        public ObservableCollection<LigneExplorateur> QuickAccess { get; } //la liste des raccourcisafficher
        private string message, messageError, chemin;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                NotifyPropertyChanged();
            }
        }
        public string MessageError
        {
            get => messageError;
            set
            {
                messageError = value;
                NotifyPropertyChanged();
            }
        }
        public string Chemin
        {
            get => chemin;
            set
            {
                chemin = value;
                NotifyPropertyChanged();
            }
        }

        public FolderExplorer()
        {
            SearchActivated = false;
            forwardHistorique = new Stack<string>();
            historique = new Stack<string>();
            ListeDossier = new ObservableCollection<LigneExplorateur>();
            QuickAccess = new ObservableCollection<LigneExplorateur>();
            InitializeQuickAccess();
            Message = "Veuillez selectionner un dossier";

            historique.Push(null); //init
            SetDirectories();
            SearchActivated = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeQuickAccess() //initialise la list view quick access
        {
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/desktop.png", Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/document.png", Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/picture.png", Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)), Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/music.png", Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)), Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/movie.png", Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)), Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/computer.png", "Ce PC", null));
            foreach (string name in GetDrives())
            {
                QuickAccess.Add(new LigneExplorateur("/Icones;Component/disk.png", name, name));
            }
        }

        public string GetRepertoireChoisi(LigneExplorateur item)
        {
            return item.Path;  //on renvoie le path de l'item choisi
        }

        public void GoBackward() //fonction appeller pour revenir en arriere
        {
            if (historique.Count > 1) //si on peux pop
            {
                forwardHistorique.Push(historique.Pop()); //on ajoute a notre historique forward
            }
            SetDirectories();
        }

        public void GoForward() //fonction appeller pour aller la ou on etait avant d'aller on arriere
        {
            if (forwardHistorique.Count > 0) //si on peux pop
            {
                historique.Push(forwardHistorique.Pop()); //on ajoute a notre historique backward
            }
            SetDirectories();
        }

        public void Remonter() //fonction appeller pour aller au dossier parent
        {
            if (DossierSelectionner != null)
            {
                if (DossierSelectionner.Length > 3)
                {
                    historique.Push(Directory.GetParent(DossierSelectionner).FullName);
                }
                else
                {
                    historique.Push(null);
                }
                SetDirectories();
            }
        }

        private void UpdatePathTextBox() //met a jour la textbox
        {
            Chemin = historique.Peek() == null ? "/" : historique.Peek();
        }

        public void TouchEnterPressed(string text) //la textbox a ete modifier cette fonction sert a voir si on peux aller a l'endroit demander
        {
            if (Directory.Exists(text) && text != DossierSelectionner) //equivaut a si dossier exist/si on est pas deja a cet endroit
            {
                historique.Push(text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                SetDirectories(); //update
            }

        }

        public bool UpdateVue(LigneExplorateur item) //appeler lors d'un double clique sur un element, retourne si l'action a été possible ou pas
        {
            string path;
            path = historique.Peek() == null ? "" : historique.Peek().Length == 3 ? historique.Peek() : historique.Peek() + "\\"; //un peu complexe mais je trouvais ca marrant equivaut a if(count==1){if length==3 -> peek else -> peek+"\\"}
            path += item != null ? item.Nom : ""; //si pas goback alors on rajoute le nom du dossier selectionner au chemin actuelle
            historique.Push(path);
            bool res = SetDirectories();
            if (!res) //on a pas pu rentrer dans le dossier choisi 
            {
                historique.Pop(); //on annule
            }
            return res;
        }

        public void QuickAccessUsed(LigneExplorateur item) //appeler lors d'un selection dans le QuickAccess
        {
            historique.Push(item.Path);
            forwardHistorique.Clear();//n'a pas de sens de revenir a un endroit peutetre jamais decouvert
            SetDirectories();
        }

        /// <summary>
        /// update la vue en consequence 
        /// </summary>
        /// <returns>true si pas d'erreur false si une erreur est apparu</returns>
        private bool SetDirectories(string containing = null)
        {
            bool errorOccur = false;
            MessageError = "";
            string path = historique.Peek();
            if (historique.Peek() == null)
            {
                FillVueWithDrives();
                path = null;
            }
            else
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(path); //recupere les dossiers
                    dirs = Filter(dirs); //filtre les dossiers indesirables
                    if (containing != null) //cas ou il s'agit d'une recherche
                    {
                        containing = containing.Trim(); //enleve de potentiel espace a la fin et au debut de la recherche
                        dirs = dirs.Where(d => d.Contains(containing, StringComparison.OrdinalIgnoreCase)).ToArray(); //update les dirs a affiché
                    }
                    ListeDossier.Clear(); //vide la vue
                    foreach (string dir in dirs) //rempli la vue
                    {
                        ListeDossier.Add(new LigneExplorateur("/Icones;Component/folder.png", System.IO.Path.GetFileName(dir), dir));
                    }
                    if (path.Last() == '\\' && path.Length > 3) //permet d'enlever les \\ a la fin si jamais il y en a
                    {
                        path = Directory.GetParent(path).FullName;
                    }
                }
                catch (Exception) //probleme en general il s'agit soit d'un manque de droit soit d'un bug(normalement yen a plus) d'ou le acces refuse
                {
                    path = null;
                    errorOccur = true;
                    MessageError = "Acces refusé";
                    //VueDesDossiers.SelectedItem = null;
                }
            }
            if (!errorOccur && containing == null) //pas besoin de mettre a jour si erreur ou recherche
            {
                DossierSelectionner = path; //met a jour le dossier actuel
                UpdatePathTextBox();
            }
            return !errorOccur;
        }

        private void FillVueWithDrives() //rempli la vue avec les disques
        {
            ListeDossier.Clear();
            foreach (string name in GetDrives()) //ajoute chaque disque a la liste
            {
                ListeDossier.Add(new LigneExplorateur("/Icones;Component/disk.png", name, name));
            }
        }

        private string[] GetDrives()
        {
            return DriveInfo.GetDrives().Select(d => d.Name).ToArray();
        } //recupere les nom des disque presents sur l'ordi

        private string[] Filter(string[] dirs) //filtre les dossiers indesirables
        {
            return dirs.Where(d => !(System.IO.Path.GetFileName(d).StartsWith(".") || System.IO.Path.GetFileName(d).StartsWith("$") || (char.IsDigit(System.IO.Path.GetFileName(d)[0]) && char.IsDigit(d.Last()))
                   || System.IO.Path.GetFileName(d).Contains("MSOCache") || System.IO.Path.GetFileName(d).Contains("System Volume Information") || System.IO.Path.GetFileName(d).Contains("Documents and Settings")
                   || System.IO.Path.GetFileName(d).Contains("Recovery") || System.IO.Path.GetFileName(d).Contains("ProgramData"))).ToArray();
        }

        public void Recherche(string text)//appeler quand le texte de la barre de recherche change
        {
            if (SearchActivated)
            {
                SetDirectories(text);
            }
        }
    }

}
