using System;
using System.Collections.Generic;
using System.Text;

namespace FolderExplorerLogic
{
    /// <summary>
    /// contient les informations presentes sur une ligne de l'explorateur a savoir une image et du text
    /// </summary>
    public class LigneExplorateur
    {
        public string Image { get; set; }
        public string Nom { get; set; }
        public string Path { get; set; }

        public LigneExplorateur(string PathImage, string Text, bool IsTextPath = false)
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
                Path = Text; //pour garantir qu'un clique dans acces rapide sur un lecteur ouvre le lecteur
            }
        }
        public LigneExplorateur(string PathImage, string Text, string Path)
        {
            Image = PathImage;
            Nom = Text;
            this.Path = Path; //pour garantir qu'un clique dans acces rapide sur un lecteur ouvre le lecteur

        }
    }
}
