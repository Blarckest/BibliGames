using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Linq;

namespace Modele
{
    public class SearchInfo
    {
        //on pourrait tenter d'aller directement a la page du jeux en allant https://www.igdb.com/games/+(jeux en minuscule et espace remplacer par '-' tt les caracteres speciaux sont supprimé)

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
        private static string ReplaceName(Jeu Jeu) //faudra peutetre changer la regex
        {
            //   return Uri.EscapeDataString(Jeu.Nom).Replace("%20","+");
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
            //Driver.Url = @$"https://www.igdb.com/search?type=1&q={ReplaceName(Jeu)}";
            //Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            //Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[2]/div[1]/div/div[1]/a/div/div/img")).Click();
        }

        private static string ExtractDescription(Jeu Jeu)
        {
            //try
            //{
            //    IWebElement desc = Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[1]/div/div[2]/div[2]/div[2]/div[2]/div[1]"));
            //    desc.Click();
            //    string Res = desc.Text;
            //    StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{Jeu.Nom}\text.txt");
            //    fichier.WriteLine(Res);
            //    fichier.Close();
            //    return Res;
            //}
            //catch (Exception)
            //{

            //    return "";
            //}

            string Texte = LinesOfTheWebPage.Where(l => l.Contains("<div data-react-class=\"GamePageHeader\" data-react-props")).First();
            Texte = Regex.Unescape(Texte);
            Texte = Texte.Substring(Texte.IndexOf("<p>"));
            Texte = Texte.Substring(0,Texte.IndexOf("</p>&"));
            Regex Reg = new Regex("<\?p>");
            Texte = Reg.Replace(Texte, string.Empty);
            StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{Jeu.Nom}\text.txt");
            fichier.WriteLine(Texte);
            fichier.Close();
            return Texte;
        }

        private static bool ExctractImage(Jeu Jeu)
        {
            var Images = LinesOfTheWebPage.Where(l => l.Contains("<a href=\"https://images.igdb.com/igdb/image/upload/t_original/")).ToList();            
            string Image = Images[Rand.Next(Images.Count())];
            Image = Image.Substring(Image.IndexOf("http"));
            Image = Image.Substring(0, Image.IndexOf(".jpg") + 4);
            WebClient WebClient = new WebClient();
            WebClient.DownloadFile(new Uri(Image), @$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg");
            return true; //a revoir peutetre remettrre le try-catch
        }

        private static bool ExctractIcon(Jeu Jeu)
        {
            string Icon = LinesOfTheWebPage.Where(l => l.Contains("<meta content=\"https://images.igdb.com/igdb/image/upload/t_cover_big/")).First();
            Icon = Icon.Substring(Icon.IndexOf("http"));
            Icon = Icon.Substring(0, Icon.IndexOf(".jpg") + 4);
            WebClient WebClient = new WebClient();
            WebClient.DownloadFile(new Uri(Icon), @$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg");
            return true; //a revoir peutetre remettrre le try-catch
        }

        private static void CreateFolderStructure(Jeu Jeu)
        {
            Directory.CreateDirectory(@$".\Ressources\Infojeux\{Jeu.Nom}");
        }
    }
}


/*
 * faire les remplacement en consequences 
 *          HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://www.igdb.com/games/buildings-have-feelings-too");
            string texte = doc.Text;
            texte=texte.Replace("><", ">\n<");
            string[] lines = texte.Split("\n");
            var icon = lines.Where(l => l.Contains("<meta content=\"https://images.igdb.com/igdb/image/upload/t_cover_big/")).First();
            icon = icon.Substring(icon.IndexOf("http"));
            icon = icon.Substring(0, icon.IndexOf(".jpg")+4);
            var Images = lines.Where(l => l.Contains("<a href=\"https://images.igdb.com/igdb/image/upload/t_original/")).ToList();
            var rand = new Random();
            var Image = Images[rand.Next(Images.Count())];
            Image = Image.Substring(Image.IndexOf("http"));
            Image = Image.Substring(0, Image.IndexOf(".jpg") + 4);
            var Texte = lines.Where(l => l.Contains("<div data-react-class=\"GamePageHeader\" data-react-props")).First();
            Texte = Texte.Substring(Texte.IndexOf("\\u003cp\\u003e") + "\\u003cp\\u003e".Length);
            Texte = Texte.Substring(0, Texte.IndexOf("\\u003c/p\\u003e"));
*/