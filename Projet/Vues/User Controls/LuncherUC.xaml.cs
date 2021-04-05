using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vues.User_Controls
{
    /// <summary>
    /// Logique d'interaction pour LuncherUC.xaml
    /// </summary>
    public partial class LuncherUC : UserControl
    {
        public LuncherUC()
        {
            InitializeComponent();
        }
        private void UCMouseWheelEvent(object sender, MouseWheelEventArgs e) //permet au user control d'interagir avec la list view quand l'utilisateur scroll
        {
            ListViewItem Item;
            StackPanel send = sender as StackPanel;
            DependencyObject Temp = send;
            while (!(Temp is ListViewItem)) //tant que l'on trouve pas notre listViewItem dans la hierarchie on remonte
            {
                Temp = VisualTreeHelper.GetParent(Temp);
            }
            Item = Temp as ListViewItem;
            if (Item != null)
            {
                //creation de l'event a envoyer
                MouseWheelEventArgs args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                args.RoutedEvent = ListViewItem.MouseWheelEvent;
                args.Source = sender;
                Item.RaiseEvent(args); //envoi!
            }
        }
    }
}
