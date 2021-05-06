using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Linq;

namespace Modele
{
    public class SearchInfo
    {
        ///exec
        ///     ressources
        ///         infojeux
        ///             1dossier/jeu 
        ///                 icon.jpg/image.jpg/text.txt
        ///             sauvegarde
        ///                liste des jeux                
        private static HtmlWeb Web = new HtmlWeb();
        private static string[] LinesOfTheWebPage;
        private static Random Rand = new Random();
        private static WebClient WebClient = new WebClient();
        public static void SetInfo(Jeu Jeu)
        {
                      
            bool Image=false, Icone=false, Description=false;
            CreateFolderStructure(Jeu);
            if (!File.Exists(@$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg"))
            {
                Image = true;
            }
            if(!File.Exists(@$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg"))
            {
                Icone = true;
            }
            if(!File.Exists(@$".\Ressources\InfoJeux\{Jeu.Nom}\text.txt"))
            {
                Description = true;
            }

            ExtractGameInfoFromWeb(Jeu, Image, Icone, Description);
        }

        public static Jeu ExtractGameInfoFromExec(string Exec)
        {
            string Dossier = Directory.GetParent(Exec).FullName;
            string Nom = Path.GetFileName(Directory.GetParent(Exec).FullName);
            return new Jeu(Nom,Dossier,Exec);
        }

        private static void ExtractGameInfoFromWeb(Jeu Jeu,bool NeedImage,bool NeedIcon,bool NeedDescription)
        {
            GoToGamePage(Jeu);
            if(NeedImage && ExctractImage(Jeu))
            {
                Jeu.Image = @$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg";
            }
            if(NeedIcon && ExctractIcon(Jeu))
            {
                Jeu.Icone = @$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg";
            }
            if (NeedDescription)
            {
                Jeu.Description = ExtractDescription(Jeu);
            }
            
        }
        private static string ReplaceName(Jeu Jeu)
        {
            //aller directement a la page du jeux en allant https://www.igdb.com/games/+(jeux en minuscule et espace remplacer par '-' tt les caracteres speciaux sont supprimé)
            string Nom=Jeu.Nom;
            Nom = Nom.ToLower();
            Regex Reg = new Regex("[*'\",_&#^@]");
            Nom = Reg.Replace(Nom, string.Empty);

            Reg = new Regex("[ ]");
            Nom = Reg.Replace(Nom, "-");
            return Nom;
        }

        private static void GoToGamePage(Jeu Jeu)
        {
            HtmlDocument doc = Web.Load(@$"https://www.igdb.com/games/{ReplaceName(Jeu)}");
            string texte = doc.Text;
            texte = texte.Replace("><", ">\n<");
            LinesOfTheWebPage = texte.Split("\n");
        }

        private static string ExtractDescription(Jeu Jeu)
        {
            string Texte = LinesOfTheWebPage.Where(l => l.Contains("<div data-react-class=\"GamePageHeader\" data-react-props")).First(); //get la ligne qui nous interesse
            Texte = System.Web.HttpUtility.HtmlDecode(Texte);  //transforme les caractere speciaux au html en ascii
            Texte = Regex.Unescape(Texte); //on met les \003u en < et autre 
            Texte = Texte.Substring(Texte.IndexOf("<p>"));
            Texte = Texte.Substring(0,Texte.IndexOf("</p>\",\"websites\":"));
            Regex Reg = new Regex("<.?p>");
            Texte = Reg.Replace(Texte, string.Empty); //on supp toutes les balise de paragraphes
            StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{Jeu.Nom}\text.txt");
            fichier.WriteLine(Texte);
            fichier.Close();
            return Texte;
        }

        private static bool ExctractImage(Jeu Jeu)
        {
            var Images = LinesOfTheWebPage.Where(l => l.Contains("<a href=\"https://images.igdb.com/igdb/image/upload/t_original/")).ToList();  //get les lignes qui nous interesse          
            string Image = Images[Rand.Next(Images.Count())]; //on en choisis une au hasard
            Image = Image.Substring(Image.IndexOf("http"));
            Image = Image.Substring(0, Image.IndexOf(".jpg") + 4);
            WebClient.DownloadFile(new Uri(Image), @$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg"); //on telecharge
            return true; //a revoir peutetre remettrre le try-catch
        }

        private static bool ExctractIcon(Jeu Jeu)
        {
            string Icon = LinesOfTheWebPage.Where(l => l.Contains("<meta content=\"https://images.igdb.com/igdb/image/upload/t_cover_big/")).First();
            Icon = Icon.Substring(Icon.IndexOf("http"));
            Icon = Icon.Substring(0, Icon.IndexOf(".jpg") + 4);
            WebClient.DownloadFile(new Uri(Icon), @$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg");
            return true; //a revoir peutetre remettrre le try-catch
        }

        private static void CreateFolderStructure(Jeu Jeu)
        {
            Directory.CreateDirectory(@$".\Ressources\Infojeux\{Jeu.Nom}");
        }
    }
}