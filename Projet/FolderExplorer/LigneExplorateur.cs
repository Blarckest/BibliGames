using System;
using System.Collections.Generic;
using System.Text;

namespace FolderExplorer
{
    /// <summary>
    /// contient les informations presentes sur une ligne de l'explorateur a savoir une image et du text
    /// </summary>
    public class LigneExplorateur
    {
        public string Image { get; }
        public string Nom { get; }
        public string Path { get; }

        public LigneExplorateur(string pathImage, string text, bool isTextPath = false)
        {
            Image = pathImage;
            if (isTextPath)
            {
                Path = text;
                Nom = System.IO.Path.GetFileName(text);
            }
            else
            {
                Nom = text;
                Path = text; //pour garantir qu'un clique dans acces rapide sur un lecteur ouvre le lecteur
            }
        }
        public LigneExplorateur(string pathImage, string text, string path)
        {
            Image = pathImage;
            Nom = text;
            Path = path; //pour garantir qu'un clique dans acces rapide sur un lecteur ouvre le lecteur

        }
    }
}
