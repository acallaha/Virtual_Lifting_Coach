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
using System.Text.RegularExpressions;

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
            curUser = "MattAdam";
            loadSessions();
            
        }

        private void loadSessions()
        {
            int i = 0;
            foreach (Session sess in MainWindow.data.sessions)
            {
                
                if (this.curUser.CompareTo(sess.username) == 0) {
                    Label dateLabel = new Label()
                    {
                        Name = "session" + i.ToString(),
                        Content = sess.date.ToString(),
                    };
                    dateLabel.MouseLeftButtonDown += goToProgressPage;
                    dateSelection.Items.Add(dateLabel);
                }
                i++;
            }
        }

        private void goToProgressPage(object sender, RoutedEventArgs e)
        {
            ProgressPage progressPage = new ProgressPage();
            this.NavigationService.Navigate(progressPage);
            String session = ((Label)sender).Name;
            int sessIndex = Convert.ToInt32(Regex.Match(session, @"\d+").Value);
            progressPage.titleText.Text = MainWindow.data.sessions[sessIndex].date.ToShortDateString();
            progressPage.loadWorkouts(sessIndex);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProgressPage());
        }
    }
}
