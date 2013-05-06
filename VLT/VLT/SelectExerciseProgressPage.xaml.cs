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
    /// Interaction logic for SelectExerciseProgressPage.xaml
    /// </summary>
    public partial class SelectExerciseProgressPage : Page
    {
        public SelectExerciseProgressPage()
        {
            InitializeComponent();
        }

        private void backSquatProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }

        private void frontSquatProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }

        private void deadLiftProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }

        private void bentOverRowProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }

        private void overheadPressProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }

        private void hangCleanProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }
    }
}
