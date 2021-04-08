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
    /// Logique d'interaction pour Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {
        public Details()
        {
            InitializeComponent();
            textBlockDescription.Text = "guilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhgguilshqgdhgiudfhgoisduhgiofuhgiosuhgouhdfoughdfughsduoghfdhg";
        }

        private void ModifierJeu(object sender, RoutedEventArgs e)
        {
            AjoutDetailWindow window = new AjoutDetailWindow();
            window.ShowDialog();
        }

        private void LancerJeu(object sender, RoutedEventArgs e)
        {
            //lancer le jeu
        }
    }
}
