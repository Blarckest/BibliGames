using Modele;

namespace Persistance
{
    public class Persistance : IPersistance
    {

        private Saver Saver { get; set; } = new SaveElements("Ressources/Sauvegarde");
        private Loader Loader { get; set; } = new LoadElements("Ressources/Sauvegarde");
        public Data Load()
        {
            return Loader.Load();
        }

        public void Save(Data data)
        {
            Saver.Save(data);
        }
    }
}
