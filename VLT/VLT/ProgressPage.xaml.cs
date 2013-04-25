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
    }
}
