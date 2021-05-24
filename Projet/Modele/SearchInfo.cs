using Logger;
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
    public static class SearchInfo
    {
        ///exec
        ///     ressources
        ///         infojeux
        ///             1dossier/jeu 
        ///                 icon.jpg/image.jpg/text.txt
        ///         sauvegarde
        ///             liste des jeux                
        [ThreadStatic] private static string[] LinesOfTheWebPage;
        [ThreadStatic] private static Random Rand;
        [ThreadStatic] private static WebClient WebClient;

        public static void SetInfo(object jeu) //on recoit un objet pour etre en accord avec le deleguate de ParameterizedThreadStart
        {
            if (jeu.GetType()==typeof(Jeu))
            {
                Jeu jeuRecu = jeu as Jeu;
                Rand = new Random();//on est obligé d'instancier les variables threadStatic
                WebClient = new WebClient();
                bool needImage = false, needIcone = false, needDescription = false;
                CreateFolderStructure(jeuRecu);
                string pathToFolderExec = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//si jamais on en a besoin
                if (string.IsNullOrEmpty(jeuRecu.Image))
                {
                    if (!File.Exists(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\image.jpg") || new FileInfo(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\image.jpg").Length == 0) //si fichier existe pas ou qu'il est vide
                    {
                        needImage = true;
                    }
                    else//si la description est vide on va la chercher dans le fichier
                    {
                        jeuRecu.Image = Path.Combine(pathToFolderExec, @$"Ressources\InfoJeux\{GetFolderName(jeuRecu)}\image.jpg");
                    } 
                }
                if (string.IsNullOrEmpty(jeuRecu.Icone))
                {
                    if (!File.Exists(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\icon.jpg") || new FileInfo(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\icon.jpg").Length == 0) //si fichier existe pas ou qu'il est vide
                    {
                        needIcone = true;
                    }
                    else//si la description est vide on va la chercher dans le fichier
                    {
                        jeuRecu.Icone = Path.Combine(pathToFolderExec, @$"Ressources\InfoJeux\{GetFolderName(jeuRecu)}\icon.jpg");
                    } 
                }
                if (string.IsNullOrEmpty(jeuRecu.Description))
                {
                    if (!File.Exists(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\text.txt") || new FileInfo(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\text.txt").Length == 0) //si fichier existe pas ou qu'il est vide
                    {
                        needDescription = true;
                    }
                    else //si la description est vide on va la chercher dans le fichier
                    {
                        jeuRecu.Description = File.ReadAllText(@$".\Ressources\InfoJeux\{GetFolderName(jeuRecu)}\text.txt");
                    } 
                }

                if (needImage || needIcone || needDescription)
                {
                    ExtractGameInfoFromWeb(jeuRecu, needImage, needIcone, needDescription);
                }
            } 
        }

        public static Jeu ExtractGameInfoFromExec(string exec)
        {
            string dossier = Directory.GetParent(exec).FullName;
            string nom = Path.GetFileName(Directory.GetParent(exec).FullName);
            return new Jeu(nom,dossier,exec);
        }

        private static string Translate(string original)
        {
            original = original.Replace("\n", "\\n");
            Regex charToApostrophe = new Regex("[\"”’“]");
            Regex charToEmpty = new Regex("[®™]"); //le serveur ne supporte pas le caracteres echapé/speciaux
            original = charToApostrophe.Replace(original, "'");
            original = charToEmpty.Replace(original, "");
            WebRequest request = WebRequest.Create("https://api.pons.com/text-translation-web/v4/translate?locale=fr");
            string postsourcedata = $"{{\"impressionId\":\"e69edd59-88af-47de-aba6-e40d065b838d\",\"sourceLanguage\":\"en\",\"targetLanguage\":\"fr\",\"text\":\"{original}\"}}"; //requete post a envoyer
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
            string page = readStream.ReadToEnd();
            page = page.Substring(page.IndexOf("text\":\"")+ "text\":\"".Length); //exctraction de la partie qui nous interesse
            page = page.Substring(0, page.IndexOf("\",\"links\":"));
            page = page.Replace("\\n ", "\n"); //on remet les saut de lignes
            page = page.Replace("\\n", "\n");
            return page;
        }

        private static void ExtractGameInfoFromWeb(Jeu jeu,bool needImage,bool needIcon,bool needDescription)
        {
            if (GoToGamePage(jeu))
            {
                string pathToFolderExec = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (needImage && ExctractImage(jeu))
                {
                    jeu.Image = Path.Combine(pathToFolderExec, @$"Ressources\InfoJeux\{GetFolderName(jeu)}\image.jpg");
                }
                if (needIcon && ExctractIcon(jeu))
                {
                    jeu.Icone = Path.Combine(pathToFolderExec, @$"Ressources\InfoJeux\{GetFolderName(jeu)}\icon.jpg");
                }
                if (needDescription)
                {
                    jeu.Description = ExtractDescription(jeu);
                }
            }
            
        }
        private static string ReplaceName(Jeu jeu)
        {
            //aller directement a la page du jeux en allant https://www.igdb.com/games/+(jeux en minuscule et espace remplacer par '-' tt les caracteres speciaux sont supprimé)
            string nom=jeu.Nom;
            nom = nom.ToLower();
            Regex reg = new Regex("[*':\",_&#^@]");
            nom = reg.Replace(nom, string.Empty);

            reg = new Regex("[ ]");
            nom = reg.Replace(nom, "-");

            reg = new Regex("[.]");
            nom = reg.Replace(nom, "-dot-"); //"." devient "-dot-" 
            return nom;
        }

        private static bool GoToGamePage(Jeu jeu)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(@$"https://www.igdb.com/games/{ReplaceName(jeu)}");
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

        private static string ExtractDescription(Jeu jeu)
        {
            try
            {
                string texte = LinesOfTheWebPage.First(l => l.Contains("<div data-react-class=\"GamePageHeader\" data-react-props")); //get la ligne qui nous interesse
                texte = System.Web.HttpUtility.HtmlDecode(texte);  //transforme les caractere speciaux au html en ascii
                texte = Regex.Unescape(texte); //on met les \003u en < et autre 
                texte = texte.Substring(texte.IndexOf("<p>"));
                texte = texte.Substring(0, texte.IndexOf("</p>\",\"websites\":"));
                Regex reg = new Regex("<.?.?..?>");
                texte = reg.Replace(texte, string.Empty); //on supp toutes les balise de paragraphes et break
                texte = Translate(texte);
                StreamWriter fichier = new StreamWriter(@$".\Ressources\Infojeux\{GetFolderName(jeu)}\text.txt");
                fichier.WriteLine(texte);
                fichier.Close();
                return texte;
            }
            catch (Exception)
            {
                Logs.WarningLog($"Aucune description trouvée pour {jeu.Nom}");
                return "No description found";
            }
        }

        private static bool ExctractImage(Jeu jeu)
        {
            try
            {
                var images = LinesOfTheWebPage.Where(l => l.Contains("<a href=\"https://images.igdb.com/igdb/image/upload/t_original/")).ToList();  //get les lignes qui nous interesse          
                string image = images[Rand.Next(images.Count())]; //on en choisis une au hasard
                image = image.Substring(image.IndexOf("http"));
                image = image.Substring(0, image.IndexOf(".jpg") + 4);
                WebClient.DownloadFile(new Uri(image), @$".\Ressources\InfoJeux\{GetFolderName(jeu)}\image.jpg"); //on telecharge
                return true;
            }
            catch (Exception)
            {
                Logs.WarningLog($"Aucune image trouvée pour {jeu.Nom}");
                return false;
            }
        }

        private static bool ExctractIcon(Jeu jeu)
        {
            try
            {
                string icon = LinesOfTheWebPage.First(l => l.Contains("<meta content=\"https://images.igdb.com/igdb/image/upload/t_cover_big/"));
                icon = icon.Substring(icon.IndexOf("http"));
                icon = icon.Substring(0, icon.IndexOf(".jpg") + 4);
                WebClient.DownloadFile(new Uri(icon), @$".\Ressources\InfoJeux\{GetFolderName(jeu)}\icon.jpg");
                return true;
            }
            catch (Exception)
            {
                Logs.WarningLog($"Aucune icone trouvée pour {jeu.Nom}");
                return false;
            }
        }

        private static string GetFolderName(Jeu jeu)
        {
            Regex reg = new Regex("[<>:\"“/\\|?*]");
            string nom = reg.Replace(jeu.Nom, "");
            return Path.GetFileName(nom);
        }

        private static void CreateFolderStructure(Jeu jeu)
        {

            Directory.CreateDirectory(@$".\Ressources\Infojeux\{GetFolderName(jeu)}");
        }
    }
}