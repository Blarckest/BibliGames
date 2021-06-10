namespace Modele
{
    public interface IPersistance
    {
        Data Load();
        void Save(Data data);
    }
}
