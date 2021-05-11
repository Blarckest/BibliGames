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
        ///         sauvegarde
        ///             liste des jeux                
        private static HtmlWeb Web = new HtmlWeb();
        private static string[] LinesOfTheWebPage;
        private static Random Rand = new Random();
        private static WebClient WebClient = new WebClient();
        public static void SetInfo(Jeu Jeu)
        {
            bool NeedImage=false, NeedIcone=false, NeedDescription=false;
            CreateFolderStructure(Jeu);
            if (!File.Exists(@$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\image.jpg") || new FileInfo(@$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\image.jpg").Length==0 || Jeu.Image==null) //si fichier existe pas ou qu'il est vide
            {
                NeedImage = true;
            }
            if(!File.Exists(@$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\icon.jpg") || new FileInfo(@$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\icon.jpg").Length == 0 || Jeu.Icone==null) //si fichier existe pas ou qu'il est vide
            {
                NeedIcone = true;
            }
            if(!File.Exists(@$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\text.txt") || new FileInfo(@$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\text.txt").Length == 0 || Jeu.Description==null) //si fichier existe pas ou qu'il est vide
            {
                NeedDescription = true;
            }

            if (NeedDescription||NeedIcone||NeedDescription)
            {
                ExtractGameInfoFromWeb(Jeu, NeedImage, NeedIcone, NeedDescription);
            }
        }

        public static Jeu ExtractGameInfoFromExec(string Exec)
        {
            string Dossier = Directory.GetParent(Exec).FullName;
            string Nom = Path.GetFileName(Directory.GetParent(Exec).FullName);
            return new Jeu(Nom,Dossier,Exec);
        }

        private static string Translate(string Original)
        {
            Original = Original.Replace("\n", "\\n");
            Regex charToApostrophe = new Regex("[\"”’“]");
            Regex charToEmpty = new Regex("[®™]"); //le serveur ne supporte pas le caracteres echapé/speciaux
            Original = charToApostrophe.Replace(Original, "'");
            Original = charToEmpty.Replace(Original, "");
            WebRequest request = WebRequest.Create("https://api.pons.com/text-translation-web/v4/translate?locale=fr");
            string postsourcedata = $"{{\"impressionId\":\"e69edd59-88af-47de-aba6-e40d065b838d\",\"sourceLanguage\":\"en\",\"targetLanguage\":\"fr\",\"text\":\"{Original}\"}}"; //requete post a envoyer
            request.Method = "POST"; //parametre de la requete
            request.ContentType = "application/json; charset=UTF-8"; //parametre de la requete
            request.ContentLength = postsourcedata.Length;  //parametre de la requete


            Stream writeStream = request.GetRequestStream(); //recuperation du flux
            Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postsourcedata);
            writeStream.Write(bytes, 0, bytes.Length); //envoie
            writeStream.Close();
            WebResponse response = request.GetResponse(); //recup de la reponse
            Stream responseStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8); //extraction de la reponse
            string Page = readStream.ReadToEnd();
            Page = Page.Substring(Page.IndexOf("text\":\"")+ "text\":\"".Length); //exctraction de la partie qui nous interesse
            Page = Page.Substring(0, Page.IndexOf("\",\"links\":"));
            Page = Page.Replace("\\n ", "\n"); //on remet les saut de lignes
            Page = Page.Replace("\\n", "\n");
            return Page;
        }

        private static void ExtractGameInfoFromWeb(Jeu Jeu,bool NeedImage,bool NeedIcon,bool NeedDescription)
        {
            if (GoToGamePage(Jeu))
            {
                if (NeedImage && ExctractImage(Jeu))
                {
                    Jeu.Image = @$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\image.jpg";
                }
                if (NeedIcon && ExctractIcon(Jeu))
                {
                    Jeu.Icone = @$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\icon.jpg";
                }
                if (NeedDescription)
                {
                    Jeu.Description = ExtractDescription(Jeu);
                }
            }
            
        }
        private static string ReplaceName(Jeu Jeu)
        {
            //aller directement a la page du jeux en allant https://www.igdb.com/games/+(jeux en minuscule et espace remplacer par '-' tt les caracteres speciaux sont supprimé)
            string Nom=Jeu.Nom;
            Nom = Nom.ToLower();
            Regex Reg = new Regex("[*':\",_&#^@]");
            Nom = Reg.Replace(Nom, string.Empty);

            Reg = new Regex("[ ]");
            Nom = Reg.Replace(Nom, "-");

            Reg = new Regex("[.]");
            Nom = Reg.Replace(Nom, "-dot-"); //"." devient "-dot-" 
            return Nom;
        }

        private static bool GoToGamePage(Jeu Jeu)
        {
            try
            {
                HtmlDocument doc = Web.Load(@$"https://www.igdb.com/games/{ReplaceName(Jeu)}");
                string texte = doc.Text;
                texte = texte.Replace("><", ">\n<");
                LinesOfTheWebPage = texte.Split("\n");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private static string ExtractDescription(Jeu Jeu)
        {
            try
            {
                string Texte = LinesOfTheWebPage.Where(l => l.Contains("<div data-react-class=\"GamePageHeader\" data-react-props")).First(); //get la ligne qui nous interesse
                Texte = System.Web.HttpUtility.HtmlDecode(Texte);  //transforme les caractere speciaux au html en ascii
                Texte = Regex.Unescape(Texte); //on met les \003u en < et autre 
                Texte = Texte.Substring(Texte.IndexOf("<p>"));
                Texte = Texte.Substring(0, Texte.IndexOf("</p>\",\"websites\":"));
                Regex Reg = new Regex("<.?.?..?>");
                Texte = Reg.Replace(Texte, string.Empty); //on supp toutes les balise de paragraphes et break
                Texte = Translate(Texte);
                StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{GetFolderName(Jeu)}\text.txt");
                fichier.WriteLine(Texte);
                fichier.Close();
                return Texte;
            }
            catch (Exception)
            {

                return "No description found";
            }
        }

        private static bool ExctractImage(Jeu Jeu)
        {
            try
            {
                var Images = LinesOfTheWebPage.Where(l => l.Contains("<a href=\"https://images.igdb.com/igdb/image/upload/t_original/")).ToList();  //get les lignes qui nous interesse          
                string Image = Images[Rand.Next(Images.Count())]; //on en choisis une au hasard
                Image = Image.Substring(Image.IndexOf("http"));
                Image = Image.Substring(0, Image.IndexOf(".jpg") + 4);
                WebClient.DownloadFile(new Uri(Image), @$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\image.jpg"); //on telecharge
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private static bool ExctractIcon(Jeu Jeu)
        {
            try
            {
                string Icon = LinesOfTheWebPage.Where(l => l.Contains("<meta content=\"https://images.igdb.com/igdb/image/upload/t_cover_big/")).First();
                Icon = Icon.Substring(Icon.IndexOf("http"));
                Icon = Icon.Substring(0, Icon.IndexOf(".jpg") + 4);
                WebClient.DownloadFile(new Uri(Icon), @$".\Ressources\InfoJeux\{GetFolderName(Jeu)}\icon.jpg");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private static string GetFolderName(Jeu Jeu)
        {
            Regex Reg = new Regex("[<>:\"“/\\|?*]");
            string Nom = Reg.Replace(Jeu.Nom, "");
            return Path.GetFileName(Nom);
        }

        private static void CreateFolderStructure(Jeu Jeu)
        {

            Directory.CreateDirectory(@$".\Ressources\Infojeux\{GetFolderName(Jeu)}");
        }
    }
}