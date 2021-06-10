using System.Windows.Controls;

namespace Vues.windowParts
{
    /// <summary>
    /// Interaction logic for MasterDetail.xaml
    /// </summary>
    public partial class MasterDetail : UserControl
    {
        public MasterDetail()
        {
            InitializeComponent();
        }

        private void ListeJeu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).Navigator.UpdateDetail(ListeJeu, Detail);
        }
    }
}
