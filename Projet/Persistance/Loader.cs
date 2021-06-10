using Modele;

namespace Persistance
{
    internal abstract class Loader
    {
        protected readonly string Folder;
        protected Loader(string folder)
        {
            this.Folder = folder;
        }
        public abstract Data Load();
    }
}
