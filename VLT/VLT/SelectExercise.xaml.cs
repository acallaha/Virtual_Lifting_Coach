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
        private String enterName = "Enter your username to login.\t";
        private String invalidName = "Invalid username, please try again.\t";
        private String logoutFirst = "Please logout before attempting to login again.\t";
        private String guestName = "Guest";

        public SelectExercise()
        {
            InitializeComponent();
            // TODO: Login
            //MainWindow.session.username = "AdamMatt";
            //if (MainWindow.session.workouts.Count > 0) 
            loginSetUp();
            
        }

        private void loginSetUp()
        {
            if (!MainWindow.loggedIn)
            {
                MainWindow.session.username = "Guest";
                instructionText.Text = enterName;
                welcomeText.Text = "Welcome, " + MainWindow.session.username + "!";
            }
            else
            {
                welcomeText.Text = "Welcome, " + MainWindow.session.username + "!";
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            String usernameAttempt = usernameBox.Text;
            if (MainWindow.session.username.CompareTo(guestName) != 0)
            {
                instructionText.Text = logoutFirst;
                usernameBox.Text = "";
            }
            else if (usernameAttempt.CompareTo("") == 0)
            {
                usernameBox.Text = "";
                instructionText.Text = invalidName;
            }
            else
            {
                MainWindow.session.username = usernameAttempt;
                usernameBox.Text = "";
                welcomeText.Text = "Welcome, " + MainWindow.session.username + "!";
                instructionText.Text = "";
                MainWindow.loggedIn = true;
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            writeData();
            MainWindow.session.username = "Guest";
            instructionText.Text = enterName;
            welcomeText.Text = "Welcome, " + MainWindow.session.username + "!";
            MainWindow.loggedIn = false;
        }

        private void exerciseButtonClick(object sender, RoutedEventArgs e)
        {
            // open the selectmode page
            MainWindow.curExercise = (String)((Button)sender).Content;
            if (((String)((Button)sender).Content).Equals("Back Squat"))
            {
                SelectMode selectMode = new SelectMode();
                this.NavigationService.Navigate(selectMode);
                selectMode.exerciseName.Text = (String)((Button)sender).Content;
            }
        }

        private void systemInfo_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new systemInformationPage());
        }

        private void progressPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SortByProgressPage());
        }

        private void writeData()
        {
            // Save the workout
            MainWindow.data.sessions.Add(MainWindow.session);
            MainWindow.SerializeToXML(MainWindow.data);
            MainWindow.DeserializeFromXML();
            MainWindow.session = new Session();
        }
    }
}
