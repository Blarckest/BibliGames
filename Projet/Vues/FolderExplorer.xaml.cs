using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Linq;

namespace Vues
{
    /// <summary>
    /// Interaction logic for FolderExplorer.xaml
    /// </summary>
    public partial class FolderExplorer : Window
    {
        private bool SearchActivated; //dit a la barre de recherche si la recherche est activé
        private Stack<string> Historique; //contient l'historique
        private Stack<string> ForwardHistorique; //permet de retourner a la valeur d'avant
        public string DossierSelectionner { get; set; } //contient la valeur du dossier actuel
        public ObservableCollection<LigneExplorateur> ListeDossier { get; set; } //la liste afficher
        public FolderExplorer()
        {

            SearchActivated = false;
            ForwardHistorique = new Stack<string>();
            Historique = new Stack<string>();
            ListeDossier = new ObservableCollection<LigneExplorateur>();
            InitializeComponent();
            InitializeQuickAccess();
            MessageBlock.Text = "Veuillez selectionner un dossier";

            Binding bindingObject = new Binding("ListeDossier"); //binding de ListeDossier vers la listview
            bindingObject.Source = this;
            VueDesDossiers.SetBinding(ListBox.ItemsSourceProperty, bindingObject);

            Historique.Push(null); //init
            SetDirectories();
            SearchActivated = true;
        }

        private void InitializeQuickAccess() //initialise la list view quick access
        {
            QuickAccess.Items.Add(new LigneExplorateur("/Icones/desktop.png", Environment.GetFolderPath(Environment.SpecialFolder.Desktop),true));
            QuickAccess.Items.Add(new LigneExplorateur("/Icones/document.png", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),true));
            QuickAccess.Items.Add(new LigneExplorateur("/Icones/picture.png", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),true));
            QuickAccess.Items.Add(new LigneExplorateur("/Icones/music.png", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),true));
            QuickAccess.Items.Add(new LigneExplorateur("/Icones/movie.png", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),true));
            QuickAccess.Items.Add(new LigneExplorateur("/Icones/computer.png", "Ce PC",null));
            foreach (string Name in GetDrives())
            {
                QuickAccess.Items.Add(new LigneExplorateur("/Icones/disk.png", Name));
            }
        }

        private void ChampRechEntre(object sender, RoutedEventArgs e)
        {
            SearchActivated = false;
            if (((TextBox)sender).Text == "Rechercher")
            {
                ((TextBox)sender).Text = ""; //met a vide le champ 
            }
            SearchActivated = true;
        }

        private void ChampRechQuitter(object sender, RoutedEventArgs e)
        {
            SearchActivated = false;
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = "Rechercher";
            }
            SearchActivated = true;
        }
       
        private void Annuler(object sender, RoutedEventArgs e) //appeller lors du clic sur le bouton annuler
        {
            DossierSelectionner = null; //met retour a null
            this.Close();
        }

        private void Selectionner(object sender, RoutedEventArgs e) //appeller lors du clic sur le bouton selectionner
        {
            if (VueDesDossiers.SelectedItem!=null) //on ferme uniquement si un dossier a été selectionner (peut-etre qu'il faudrait changer ce comportement)
            {
                DossierSelectionner = Historique.Peek() + ((LigneExplorateur)VueDesDossiers.SelectedItem).Nom;
                this.Close();
            }
        }

        private void GoBackward(object sender, MouseButtonEventArgs e) //fonction appeller pour revenir en arriere
        {
            if (Historique.Count > 1) //si on peux pop
            {
                ForwardHistorique.Push(Historique.Pop()); //on ajoute a notre historique forward
            }
            SetDirectories();
        }

        private void GoForward(object sender, MouseButtonEventArgs e) //fonction appeller pour aller la ou on etait avant d'aller on arriere
        {
            if (ForwardHistorique.Count > 0) //si on peux pop
            {
                Historique.Push(ForwardHistorique.Pop()); //on ajoute a notre historique backward
            }
            SetDirectories(); //pas reflechi encore
        }

        private void Remonter(object sender, MouseButtonEventArgs e) //fonction appeller pour aller au dossier parent
        {
            if (DossierSelectionner!=null)
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
            string Temp = Historique.Peek() == null ? "/" : Historique.Peek();
            TextBoxChemin.Text = Temp;
        }

        private void TextBoxChemin_TouchEnterPressed(object sender, KeyEventArgs e) //la textbox a ete modifier cette fonction sert a voir si on peux aller a l'endroit demander
        {
            if (e.Key == Key.Return)
            {
                if (Directory.Exists(TextBoxChemin.Text) && TextBoxChemin.Text != DossierSelectionner.Substring(0, DossierSelectionner.Length - 1)) //equivaut a si dossier exist/si on est pas deja a cet endroit
                {
                    Historique.Push(TextBoxChemin.Text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                    SetDirectories(); //update
                }
            }
        }

        private void UpdateVue(object sender, MouseButtonEventArgs e) //appeler lors d'un double clique sur un element
        {
            string Path;
            Path = Historique.Peek()==null ? "" : Historique.Peek().Length == 3 ? Historique.Peek() : Historique.Peek() + "\\"; //un peu complexe mais je trouvais ca marrant equivaut a if(count==1){if length==3 -> peek else -> peek+"\\"}
            Path += VueDesDossiers.SelectedItem != null ? ((LigneExplorateur)VueDesDossiers.SelectedItem).Nom : ""; //si pas goback alors on rajoute le nom du dossier selectionner au chemin actuelle
            Historique.Push(Path);
            if (!SetDirectories()) //on a pas pu rentrer dans le dossier choisi 
            {
                Historique.Pop(); //on annule
            }
        }

        private void QuickAccessUsed(object sender, MouseButtonEventArgs e) //appeler lors d'un selection dans le QuickAccess
        {
            LigneExplorateur Item=QuickAccess.SelectedItem as LigneExplorateur;
            Historique.Push(Item.Path);
            ForwardHistorique.Clear();//n'a pas de sens de revenir a un endroit peutetre jamais decouvert
            SetDirectories();
        }

        /// <summary>
        /// update la vue en consequence 
        /// </summary>
        /// <returns>true si pas d'erreur false si une erreur est apparu</returns>
        private bool SetDirectories(string Containing=null)
        {
            bool ErrorOccur=false;
            ErrorBlock.Text = "";
            string Path = Historique.Peek();
            if (Historique.Peek()==null)
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
                    if (Containing!=null) //cas ou il s'agit d'une recherche
                    {
                        Containing = Containing.Trim(); //enleve de potentiel espace a la fin et au debut de la recherche
                        Dirs = Dirs.Where(d => d.Contains(Containing,StringComparison.OrdinalIgnoreCase)).ToArray(); //update les dirs a affiché
                    }
                    ListeDossier.Clear(); //vide la vue
                    foreach (string Dir in Dirs) //rempli la vue
                    {
                        ListeDossier.Add(new LigneExplorateur("/Icones/folder.png", System.IO.Path.GetFileName(Dir)));
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
                    ErrorBlock.Text = "Acces refusé";
                    VueDesDossiers.SelectedItem = null;
                } 
            }
            if (!ErrorOccur && Containing==null) //pas besoin de mettre a jour si erreur ou recherche
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
                ListeDossier.Add(new LigneExplorateur("Icones/disk.png",Name));
            }
        }

        private string[] GetDrives()
        {
            return DriveInfo.GetDrives().Select(d => d.Name).ToArray();
        } //recupere les nom des disque presents sur l'ordi

        private string[] Filter(string[] Dirs) //filtre les dossiers indesirables
        {
            return Dirs.Where(d => !(System.IO.Path.GetFileName(d).StartsWith(".") || System.IO.Path.GetFileName(d).StartsWith("$") || char.IsDigit(System.IO.Path.GetFileName(d)[0])
                   || System.IO.Path.GetFileName(d).Contains("MSOCache") || System.IO.Path.GetFileName(d).Contains("System Volume Information") || System.IO.Path.GetFileName(d).Contains("Documents and Settings")
                   || System.IO.Path.GetFileName(d).Contains("Temp", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetFileName(d).Contains("Recovery") || System.IO.Path.GetFileName(d).Contains("ProgramData"))).ToArray();
        }

        private void Recherche(object sender, TextChangedEventArgs e)//appeler quand le texte de la barre de recherche change
        {
            TextBox TextBox = sender as TextBox;
            if (SearchActivated)
            {
                SetDirectories(TextBox.Text);
            }
        }
    }
    /// <summary>
    /// contient les informations presentes sur une ligne de l'explorateur a savoir une image et du text
    /// </summary>
    public class LigneExplorateur
    {
        public string Image { get; set; }
        public string Nom { get; set; }
        public string Path { get; set; }

        public LigneExplorateur(string PathImage,string Text,bool IsTextPath=false)
        {
            Image = PathImage;
            if (IsTextPath)
            {
                Path = Text;
                Nom = System.IO.Path.GetFileName(Text);
            }
            else
            {
                Nom = Text;
                Path = Text; //pour garantir qu'un clique dans acces rapide sur un leucteur ouvre le lecteur
            }
        }
        public LigneExplorateur(string PathImage, string Text, string Path)
        {
            Image = PathImage;
            Nom = Text;
            this.Path = Path; //pour garantir qu'un clique dans acces rapide sur un leucteur ouvre le lecteur

        }
    }
}


