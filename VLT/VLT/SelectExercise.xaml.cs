﻿using System;
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
            loginSetUp();
        }

        private void loginSetUp()
        {
            MainWindow.session.username = "Guest";
            instructionText.Text = enterName;
            welcomeText.Text = "Welcome, " + MainWindow.session.username + "!";
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
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            logoutUser();
            MainWindow.session.username = "Guest";
            instructionText.Text = enterName;
            welcomeText.Text = "Welcome, " + MainWindow.session.username + "!";
        }

        private void exerciseButtonClick(object sender, RoutedEventArgs e)
        {
            // open the selectmode page
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }

        private void logoutUser()
        {
            // Save the workout
            MainWindow.data.sessions.Add(MainWindow.session);
            MainWindow.SerializeToXML(MainWindow.data);
        }

        private void makeFakeData_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.writeFakeData();
            //this.logoutUser();
        }




    }
}
