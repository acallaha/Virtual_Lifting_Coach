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
        private String curUser;

        public SelectDateProgressPage()
        {
            InitializeComponent();
            curUser = "Matt";
            
        }

        private void loadSessions()
        {
            int i = 0;
            foreach (Session sess in MainWindow.data.sessions)
            {
                i++;
                if (curUser.CompareTo(sess.username) == 0) {
                    Label dateLabel = new Label()
                    {
                        Name = i.ToString(),
                        Content = sess.date.ToString(),
                    };
                    dateLabel.MouseLeftButtonDown += goToProgressPage;
                    dateSelection.Items.Add(dateLabel);
                }
            }
        }

        private void goToProgressPage(object sender, RoutedEventArgs e)
        {
            ProgressPage progressPage = new ProgressPage();
            this.NavigationService.Navigate(progressPage);
            int index = Convert.ToInt32(((Label)sender).Name);
            progressPage.titleText.Text = MainWindow.data.sessions[index].date.ToString();
            progressPage.loadWorkouts(index);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }
    }
}
