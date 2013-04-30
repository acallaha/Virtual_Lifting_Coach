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
    /// Interaction logic for SelectDateProgressPage.xaml
    /// </summary>
    public partial class SelectDateProgressPage : Page
    {
        public SelectDateProgressPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }
    }
}
