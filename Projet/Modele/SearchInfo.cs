using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using OpenQA.Selenium.Support.UI;
using System.Threading;

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
            Jeu Jeu = new Jeu();
            Jeu.Exec = Exec;
            Jeu.Dossier = Directory.GetParent(Exec).FullName;
            Jeu.Nom = Path.GetFileName(Directory.GetParent(Exec).FullName);
            return Jeu;
        }

        private static void ExtractGameInfoFromWeb(Jeu Jeu,bool NeedImage,bool NeedIcon,bool NeedDescription)
        {
            IWebDriver Driver = new ChromeDriver();
            GoToGamePage(Driver, Jeu);
            if(NeedImage && ExctractImage(Driver, Jeu))
            {
                Jeu.Image = @$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg";
            }
            if(NeedIcon && ExctractIcon(Driver, Jeu,!NeedImage))
            {
                Jeu.Icone = @$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg";
            }
            if (NeedDescription)
            {
                Jeu.Description = ExtractDescription(Driver, Jeu);
            }
            
        }
        private static string ReplaceName(Jeu Jeu)
        {
            return Uri.EscapeDataString(Jeu.Nom).Replace("%20","+");
        }

        private static void GoToGamePage(IWebDriver Driver,Jeu Jeu)
        {
            Driver.Url = @$"https://www.igdb.com/search?type=1&q={ReplaceName(Jeu)}";
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[2]/div[1]/div/div[1]/a/div/div/img")).Click();
        }

        private static string ExtractDescription(IWebDriver Driver, Jeu Jeu)
        {
            try
            {
                IWebElement desc = Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[1]/div/div[2]/div[2]/div[2]/div[2]/div[1]"));
                desc.Click();
                string Res = desc.Text;
                StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{Jeu.Nom}\text.txt");
                fichier.WriteLine(Res);
                fichier.Close();
                return Res;
            }
            catch (Exception)
            {

                return "";
            }
        }

        private static bool ExctractImage(IWebDriver Driver,Jeu Jeu)
        {
            try
            {
                Thread.Sleep(10000);
                IWebElement Image = Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[1]/div/div[1]/div"));
                string Style = Image.GetAttribute("style");
                Style = Style.Substring(Style.IndexOf("http"), (Style.Length-3)- Style.IndexOf("http"));
                Style = Style.Replace("t_screenshot_big", "t_original");
                WebClient WebClient = new WebClient();
                WebClient.DownloadFile(new Uri(Style), @$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private static bool ExctractIcon(IWebDriver Driver, Jeu Jeu, bool NeedAttente)
        {
            try
            {
                if (NeedAttente)
                {
                    Thread.Sleep(10000);
                }
                IWebElement Icon = Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[1]/div/div[2]/div[1]/img"));
                string Chemin = Icon.GetAttribute("src");
                WebClient WebClient = new WebClient();
                WebClient.DownloadFile(new Uri(Chemin), @$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        
        private static void CreateFolderStructure(Jeu Jeu)
        {
            Directory.CreateDirectory(@$".\Ressources\Infojeux\{Jeu.Nom}");
        }
    }
}
