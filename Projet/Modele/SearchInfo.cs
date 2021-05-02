using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;

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

            ExtractDescription(Driver, Jeu);


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
            Driver.Url = "https://www.igdb.com/discover";
            GoToGamePage(Driver, Jeu);
            if(NeedImage)
            {
                Jeu.Image= ExctractImage(Driver,Jeu)
            }
            if(NeedIcon)
            {
                Jeu.Icon
            }
        }

        private static void GoToGamePage(IWebDriver Driver,Jeu Jeu)
        {
            IWebElement SearchBar = Driver.FindElement(By.XPath("/html/body/header/nav/div/div[2]/div/form/div/div/div[1]/div/input"));//go to search bar
            SearchBar.Clear();
            SearchBar.SendKeys(Jeu.Nom);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            IWebElement LinkToPage=Driver.FindElement(By.XPath("/html/body/div[2]/main/div[2]/div[2]/div[1]/div/div[1]/a/div/div/img"));
            LinkToPage.Click();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        }

        private static void ExtractDescription(IWebDriver Driver, Jeu Jeu)
        {
            IWebElement desc = Driver.FindElement(By.XPath("/html/body/div[3]/main/div[2]/div[1]/div/div[2]/div[2]/div[2]/div[2]/div[1]"));
            desc.Click();
            StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{Jeu.Nom}\text.txt");
            fichier.WriteLine(desc.Text);
            fichier.Close();
        }

        private static void ExctractImage(IWebDriver Driver,Jeu Jeu)
        {
            IWebElement Image = Driver.FindElement(By.XPath("/html/body/div[2]/main/div[2]/div[1]/div/div[1]/div"));
            string Style = Image.GetAttribute("style");
            Style = Style.Substring(Style.IndexOf("http"), Style.Length - 2);
            Style.Replace("t_screenshot_big",  "t_original");
            WebClient WebClient = new WebClient();
            WebClient.DownloadFileAsync(new Uri(Style),  @$".\Ressources\InfoJeux\{Jeu.Nom}\image.jpg");
        }

        private static void ExctractIcon(IWebDriver Driver, Jeu Jeu)
        {
            IWebElement Icon = Driver.FindElement(By.XPath("/html/body/div[2]/main/div[2]/div[1]/div/div[2]/div[1]/img"));
            string Chemin = Image.GetAttribute("src");
            WebClient WebClient = new WebClient();
            WebClient.DownloadFileAsync(new Uri(Chemin), @$".\Ressources\InfoJeux\{Jeu.Nom}\icon.jpg");
        }
        
        private static void CreateFolderStructure()
        {
            bool exist;
            exist= Directory.Exists(@$".\Ressources");
            if(exist==)
        }
    }
}
