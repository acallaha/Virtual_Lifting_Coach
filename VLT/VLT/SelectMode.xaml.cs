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
    /// Interaction logic for SelectMode.xaml
    /// </summary>
    public partial class SelectMode : Page
    {
        public SelectMode()
        {
            InitializeComponent();

        }

        private void quickLift_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.session.username.Equals("Guest"))
            {
                TipsPage tp = new TipsPage();
                this.NavigationService.Navigate(tp);
                tp.exerciseName = (String)((Button)sender).Content;
            }
            else
            {
                quickLiftPage quickLiftPage = new quickLiftPage();
                this.NavigationService.Navigate(quickLiftPage);
                quickLiftPage.exerciseName.Content = this.exerciseName.Text;
            }



        }

        private void teachMe_Click(object sender, RoutedEventArgs e)
        {
            teachMePage teachMePage = new teachMePage();
            this.NavigationService.Navigate(teachMePage);
            teachMePage.exerciseName.Content = this.exerciseName.Text;
        }
    }
}
