using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FolderExplorerLogic
{
    public class FolderExplorerLogic
    {
        public bool SearchActivated; //dit a la barre de recherche si la recherche est activé
        private Stack<string> Historique; //contient l'historique
        private Stack<string> ForwardHistorique; //permet de retourner a la valeur d'avant
        public string DossierSelectionner { get; set; } //contient la valeur du dossier actuel
        public ObservableCollection<LigneExplorateur> ListeDossier { get; set; } //la liste afficher
        public ObservableCollection<LigneExplorateur> QuickAccess { get; set; } //la liste des raccourcisafficher
        public string Message;
        public string MessageError;
        public string Chemin;

        public FolderExplorerLogic()
        {
            SearchActivated = false;
            ForwardHistorique = new Stack<string>();
            Historique = new Stack<string>();
            ListeDossier = new ObservableCollection<LigneExplorateur>();
            QuickAccess = new ObservableCollection<LigneExplorateur>();
            InitializeQuickAccess();
            Message = "Veuillez selectionner un dossier";

            Historique.Push(null); //init
            SetDirectories();
            SearchActivated = true;
        }

        private void InitializeQuickAccess() //initialise la list view quick access
        {
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/desktop.png", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), true));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/document.png", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), true));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/picture.png", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), true));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/music.png", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), true));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/movie.png", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), true));
            QuickAccess.Add(new LigneExplorateur("/Icones;Component/computer.png", "Ce PC", null));
            foreach (string Name in GetDrives())
            {
                QuickAccess.Add(new LigneExplorateur("/Icones;Component/disk.png", Name));
            }
        }

        public string GetRepertoireChoisi(LigneExplorateur Item)
        {
            return Historique.Peek() + "\\" + Item.Nom;
        }

        public void GoBackward() //fonction appeller pour revenir en arriere
        {
            if (Historique.Count > 1) //si on peux pop
            {
                ForwardHistorique.Push(Historique.Pop()); //on ajoute a notre historique forward
            }
            SetDirectories();
        }

        public void GoForward() //fonction appeller pour aller la ou on etait avant d'aller on arriere
        {
            if (ForwardHistorique.Count > 0) //si on peux pop
            {
                Historique.Push(ForwardHistorique.Pop()); //on ajoute a notre historique backward
            }
            SetDirectories();
        }

        public void Remonter() //fonction appeller pour aller au dossier parent
        {
            if (DossierSelectionner != null)
            {
                if (DossierSelectionner.Length > 3)
                {
                    Historique.Push(Directory.GetParent(DossierSelectionner).FullName);
                }
                else
                {
                    Historique.Push(null);
                }
                SetDirectories();
            }
        }

        private void UpdatePathTextBox() //met a jour la textbox
        {
            Chemin = Historique.Peek() == null ? "/" : Historique.Peek();
        }

        private void TouchEnterPressed(string Text) //la textbox a ete modifier cette fonction sert a voir si on peux aller a l'endroit demander
        {
            if (Directory.Exists(Text) && Text != DossierSelectionner) //equivaut a si dossier exist/si on est pas deja a cet endroit
            {
                Historique.Push(Text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                SetDirectories(); //update
            }

        }

        private void UpdateVue(LigneExplorateur Item) //appeler lors d'un double clique sur un element
        {
            string Path;
            Path = Historique.Peek() == null ? "" : Historique.Peek().Length == 3 ? Historique.Peek() : Historique.Peek() + "\\"; //un peu complexe mais je trouvais ca marrant equivaut a if(count==1){if length==3 -> peek else -> peek+"\\"}
            Path += Item != null ? Item.Nom : ""; //si pas goback alors on rajoute le nom du dossier selectionner au chemin actuelle
            Historique.Push(Path);
            if (!SetDirectories()) //on a pas pu rentrer dans le dossier choisi 
            {
                Historique.Pop(); //on annule
            }
        }

        private void QuickAccessUsed(LigneExplorateur Item) //appeler lors d'un selection dans le QuickAccess
        {
            Historique.Push(Item.Path);
            ForwardHistorique.Clear();//n'a pas de sens de revenir a un endroit peutetre jamais decouvert
            SetDirectories();
        }

        /// <summary>
        /// update la vue en consequence 
        /// </summary>
        /// <returns>true si pas d'erreur false si une erreur est apparu</returns>
        private bool SetDirectories(string Containing = null)
        {
            bool ErrorOccur = false;
            MessageError = "";
            string Path = Historique.Peek();
            if (Historique.Peek() == null)
            {
                FillVueWithDrives();
                Path = null;
            }
            else
            {
                try
                {
                    string[] Dirs = Directory.GetDirectories(Path); //recupere les dossiers
                    Dirs = Filter(Dirs); //filtre les dossiers indesirables
                    if (Containing != null) //cas ou il s'agit d'une recherche
                    {
                        Containing = Containing.Trim(); //enleve de potentiel espace a la fin et au debut de la recherche
                        Dirs = Dirs.Where(d => d.Contains(Containing, StringComparison.OrdinalIgnoreCase)).ToArray(); //update les dirs a affiché
                    }
                    ListeDossier.Clear(); //vide la vue
                    foreach (string Dir in Dirs) //rempli la vue
                    {
                        ListeDossier.Add(new LigneExplorateur("/Icones;Component/folder.png", System.IO.Path.GetFileName(Dir)));
                    }
                    if (Path.Last() == '\\' && Path.Length > 3) //permet d'enlever les \\ a la fin si jamais il y en a
                    {
                        Path = Directory.GetParent(Path).FullName;
                    }
                }
                catch (Exception) //probleme en general il s'agit soit d'un manque de droit soit d'un bug(normalement yen a plus) d'ou le acces refuse
                {
                    Path = null;
                    ErrorOccur = true;
                    MessageError = "Acces refusé";
                    //VueDesDossiers.SelectedItem = null;
                }
            }
            if (!ErrorOccur && Containing == null) //pas besoin de mettre a jour si erreur ou recherche
            {
                DossierSelectionner = Path; //met a jour le dossier actuel
                UpdatePathTextBox();
            }
            return !ErrorOccur;
        }

        private void FillVueWithDrives() //rempli la vue avec les disques
        {
            ListeDossier.Clear();
            foreach (string Name in GetDrives()) //ajoute chaque disque a la liste
            {
                ListeDossier.Add(new LigneExplorateur("/Icones;Component/disk.png", Name));
            }
        }

        private string[] GetDrives()
        {
            return DriveInfo.GetDrives().Select(d => d.Name).ToArray();
        } //recupere les nom des disque presents sur l'ordi

        private string[] Filter(string[] Dirs) //filtre les dossiers indesirables
        {
            return Dirs.Where(d => !(System.IO.Path.GetFileName(d).StartsWith(".") || System.IO.Path.GetFileName(d).StartsWith("$") || (char.IsDigit(System.IO.Path.GetFileName(d)[0]) && char.IsDigit(d.Last()))
                   || System.IO.Path.GetFileName(d).Contains("MSOCache") || System.IO.Path.GetFileName(d).Contains("System Volume Information") || System.IO.Path.GetFileName(d).Contains("Documents and Settings")
                   || System.IO.Path.GetFileName(d).Contains("Recovery") || System.IO.Path.GetFileName(d).Contains("ProgramData"))).ToArray();
        }

        public void Recherche(string Text)//appeler quand le texte de la barre de recherche change
        {
            if (SearchActivated)
            {
                SetDirectories(Text);
            }
        }
    }

}
