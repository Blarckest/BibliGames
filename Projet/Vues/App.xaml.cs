using Modele;
using System.Windows;

namespace Vues
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Navigator Navigator { get; set; } = new Navigator();
        public Manager Manager { get; set; } = new Manager(new Persistance.Persistance()); //on load la sauvegarde
    }
}
