using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Modele;
using Persistance;

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
