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

        public LigneExplorateur(string pathImage, string text, string path)
        {
            Image = pathImage;
            Nom = text;
            Path = path;
        }
    }
}
