using System;
using DataManager;
using Modele;

namespace Test_Fonctionnels
{
    class Test
    {
        static void Main(string[] args)
        {
            Manager Modele = new Manager();
            Loader Loader = new Stub("");
            Modele.Elements = Loader.Load();
            foreach (Element element in Modele.Elements)
            {
                Console.WriteLine(element);
            }

        }
    }
}
