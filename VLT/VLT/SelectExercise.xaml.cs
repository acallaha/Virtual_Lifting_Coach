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
    /// Interaction logic for SelectExercise.xaml
    /// </summary>
    public partial class SelectExercise : Page
    {
        public SelectExercise()
        {
            InitializeComponent();

        }

        private void exerciseButtonClick(object sender, RoutedEventArgs e)
        {
            // open the selectmode page
            SelectMode selectMode = new SelectMode();
            this.NavigationService.Navigate(selectMode);
            selectMode.exerciseName.Text = (String)((Button)sender).Content;
        }

        private void systemInfo_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new systemInformationPage());
        }

        private void progressPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SortByProgressPage());
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }
    }
}
