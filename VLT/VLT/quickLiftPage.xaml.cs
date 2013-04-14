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
    /// Interaction logic for quickLiftPage.xaml
    /// </summary>
    public partial class quickLiftPage : Page
    {
        private int currentSet = 1;
        private int currentRep = 0;
        public quickLiftPage()
        {
            InitializeComponent();
        }
        public void makeRep() {
            Label setLabel;
            //RowDefinition newSet;
            if (currentRep == 0) {
                // Make set row
                Console.WriteLine("Making new set row");
                setLabel = new Label();
                setLabel.Content = "Set " + currentSet;
                setLabel.Name = "Set" + currentSet;
                setLabel.MouseLeftButtonDown += showData;
                setRepList.Items.Add(setLabel);
            }
            currentRep++;

            setLabel = new Label();
            setLabel.Content = "\tRep " + currentRep;
            setLabel.Name = "Rep" + currentRep;
            setLabel.MouseLeftButtonDown += showData;
            setRepList.Items.Add(setLabel);

            Console.WriteLine("Increase current Rep: " + currentRep);
        }
        public void endSet()
        {
            currentRep = 0;
            currentSet++;
        }
        public void repButtonClick(object sender, RoutedEventArgs e)
        {
            makeRep();
        }

        public void setButtonClick(object sender, RoutedEventArgs e)
        {
            endSet();
        }
        public void showData(object sender, RoutedEventArgs e)
        {
            issueBox.Items.Clear();
            Label issueLabel = new Label();
            issueLabel.Name = ((Label)sender).Name;
            issueLabel.Content = ((Label)sender).Name;
            issueLabel.MouseLeftButtonDown += showAdvice;

            issueBox.Items.Add(issueLabel);

        }
        private int counter = 0;
        public void showAdvice(object sender, RoutedEventArgs e)
        {
            counter++;
            adviceBox.Text = counter.ToString();
        }
    }
}
