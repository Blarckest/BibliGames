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
        private Stack<string> Historique; //contient l'historique
        public string DossierSelectionner { get; set; } //contient la valeur du dossier actuel
        public ObservableCollection<LigneExplorateur> ListeDossier { get; set; } //la liste afficher
        public FolderExplorer()
        {
            Historique = new Stack<string>();
            ListeDossier = new ObservableCollection<LigneExplorateur>();
            InitializeComponent();

            Binding bindingObject = new Binding("ListeDossier"); //binding de ListeDossier vers la listview
            bindingObject.Source = this;
            VueDesDossiers.SetBinding(ListBox.ItemsSourceProperty, bindingObject);

            Historique.Push(null); //init
            FillVueWithDrives();
            UpdatePathTextBox();
        }

        private void Annuler(object sender, RoutedEventArgs e)
        {
            DossierSelectionner = null; //met retour a null
            this.Close();
        }

        private void Selectionner(object sender, RoutedEventArgs e)
        {
            if (VueDesDossiers.SelectedItem!=null) //on ferme uniquement si un dossier a été selectionner (peut-etre qu'il faudrait changer ce comportement)
            {
                DossierSelectionner = Historique.Peek() + ((LigneExplorateur)VueDesDossiers.SelectedItem).Nom;
                this.Close();
            }
        }

        private void Remonter(object sender, MouseButtonEventArgs e) //fonction appeller pour remonter dans l'historique
        {
            if (Historique.Count > 1) //si on peux pop
            {
                Historique.Pop();
            }
            if (Historique.Peek() == null) //si on est a la racine
            {
                FillVueWithDrives();
                UpdatePathTextBox();
            }
            else //cas normal
            {
                UpdateVueToPrecedent();
                UpdatePathTextBox();
            }
        }

        private void UpdateVue(object sender, MouseButtonEventArgs e) //appeler lors d'un double clique sur un element
        {
            string Path = SetDirectories();
            if (Path!=null) //ajout a l'historique
            {
                Historique.Push(Path);
            }
            UpdatePathTextBox();
        }

        private void UpdateVueToPrecedent()
        {
            SetDirectories(true);
        }

        /// <summary>
        /// update la vue en consequence 
        /// </summary>
        /// <param name="IsAGoBack">donne l'indication de si il s'agit d'un retour en arriere ou pas</param>
        /// <returns></returns>
        private string SetDirectories(bool IsAGoBack=false)
        {
            ErrorBlock.Text = "";
            string Path = "";
            try
            {
                Path = Historique.Count == 1 ? "" : Historique.Peek().Length == 3 ? Historique.Peek() : Historique.Peek() + "\\"; //un peu complexe mais je trouvais ca marrant equivaut a if(count==1){if length==3 -> peek else -> peek+"\\"}
                if (!IsAGoBack)
                {
                    Path += VueDesDossiers.SelectedItem != null ? ((LigneExplorateur)VueDesDossiers.SelectedItem).Nom : ""; //si pas goback alors on rajoute le nom du dossier selectionner au chemin actuelle
                }
                string[] Dirs = Directory.GetDirectories(Path); //recupere les dossiers
                Dirs = Filter(Dirs); //filtre les dossiers
                ListeDossier.Clear(); //vide la vue
                foreach (string Dir in Dirs) //rempli la vue
                {
                    ListeDossier.Add(new LigneExplorateur("/Icones/folder.png",System.IO.Path.GetFileName(Dir)));
                }
                DossierSelectionner=Path; //met a jour le dossier actuel
            }
            catch (Exception) //probleme en general il s'agit soit d'un manque de droit soit d'un bug(normalement yen a plus) d'ou le acces refuse
            {
                Path = null;
                ErrorBlock.Text="Acces refusé";
                VueDesDossiers.SelectedItem = null;
            }
            return Path; //renvoi le chemin pour mettre a jour l'historique
        }

        private void FillVueWithDrives() //recupere les disques presents sur l'ordi
        {
            ListeDossier.Clear();
            foreach (DriveInfo Drive in DriveInfo.GetDrives()) //ajoute chaque disque a la liste
            {
                ListeDossier.Add(new LigneExplorateur("Icones/disk.png",Drive.Name));
            }
        }

        private string[] Filter(string[] Dirs) //filtre les dossiers indesirables
        {
            return Dirs.Where(d => !(System.IO.Path.GetFileName(d).StartsWith(".") || System.IO.Path.GetFileName(d).StartsWith("$") || char.IsDigit(System.IO.Path.GetFileName(d)[0])
                   || System.IO.Path.GetFileName(d).Contains("MSOCache") || System.IO.Path.GetFileName(d).Contains("System Volume Information") || System.IO.Path.GetFileName(d).Contains("Documents and Settings")
                   || System.IO.Path.GetFileName(d).Contains("Temp", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetFileName(d).Contains("Recovery") || System.IO.Path.GetFileName(d).Contains("ProgramData"))).ToArray();
        }

        private void UpdatePathTextBox() //met a jour la textbox
        {
            string Temp = Historique.Peek() == null ? "/" : Historique.Peek();
            TextBoxChemin.Text = Temp;
        }

        private void TextBoxChemin_TouchEnterPressed(object sender, KeyEventArgs e) //la textbox a ete modifier cette fonction sert a voir si on peux aller a l'endroit demander
        {
            if (e.Key==Key.Return)
            {
                if (Directory.Exists(TextBoxChemin.Text) && TextBoxChemin.Text!=DossierSelectionner.Substring(0,DossierSelectionner.Length-1)) //equivaut a si dossier exist/si on est pas deja a cet endroit
                {
                    Historique.Push(TextBoxChemin.Text.Trim()); //trim au cas ou l'utilisateur aurait decider de mettre des espaces a la fin du chemin
                    SetDirectories(); //update
                } 
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

        public LigneExplorateur(string Path,string Text)
        {
            Image = Path;
            Nom = Text;
        }
    }
}


