using System;
using System.Collections.Generic;
using System.Linq;
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

namespace VLT
{
    /// <summary>
    /// Interaction logic for TipsPage.xaml
    /// </summary>
    public partial class TipsPage : Page
    {
        public TipsPage()
        {
            InitializeComponent();

        }

        public string exerciseName;

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            quickLiftPage qp = new quickLiftPage();
            this.NavigationService.Navigate(qp);
            qp.exerciseName.Content = this.exerciseName;

        }
    }
}
