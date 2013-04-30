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
using System.Data.SqlClient;
using System.Data;


namespace VLT
{
    /// <summary>
    /// Interaction logic for ProgressPage.xaml
    /// </summary>
    public partial class ProgressPage : Page
    {
        private int MAX_BAR_HEIGHT = 250; // pixels
        private int MAX_GRAPH_WIDTH = 280; // pixels

        public ProgressPage()
        {
            InitializeComponent();
        }

        private void ProgressPageLoaded(object sender, RoutedEventArgs e)
        {
            this.showData("Adam");
        }

        private void addData()
        {
            /* SqlConnection con = new SqlConnection();

            con.ConnectionString = "Data Source= (local); Initial Catalog=VLTUserDatabase;";
            con.Open();

            SqlCommand com = new SqlCommand();

            string query = "";
            con.Close();
            */

        }

        private void showData(String username)
        {
            LiftData ld = new LiftData();
            Stack<String> output = ld.getUserData(username);
            while (output.Count != 0)
            {
                Label lift = new Label();
                lift.Content = output.Pop();
                listBox1.Items.Add(lift);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LiftData ld = new LiftData();
            ld.writeLine("Adam", "back squat", 3, 30, 82);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            LiftData ld = new LiftData();
            Stack<String> output = ld.getUserData("Adam");
            while (output.Count != 0)
            {
                Label lift = new Label();
                lift.Content=output.Pop();
                listBox1.Items.Add(lift);
            }
        }

        private void makeScoreScale()
        {

        }

        private void makeWeightScale(int maxWeight, int minWeight) {

            }

        private void makeScoreBar(int score, int numOfBars)
        {
            Rectangle bar = new Rectangle();
            //bar.
        }

        private void makeWeightBar(int score, int numOfBars)
        {

        }

        private void nextSetButton_Click(object sender, RoutedEventArgs e)
        {
            // Increment the set and refresh the graph page
            // if it is the last set, go back to the first
        }

        private void repScoresButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a bar for each rep, scaled 0 to 100
        }

        private void setWeightsButton_Click(object sender, RoutedEventArgs e)
        {
            // Display bars corresponding the scaled weights for each set

            // If no weight data, display a message indicating that there is no data
        }

        private void setAvgButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a bar for each set, sized by the score (0-100)
        }
    }
}
