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
using System.Text.RegularExpressions;



namespace VLT
{
    /// <summary>
    /// Interaction logic for ProgressPage.xaml
    /// </summary>
    public partial class ProgressPage : Page
    {
        private static int MAX_BAR_HEIGHT = 250; // pixels
        private static int MAX_GRAPH_WIDTH = 600; // pixels
        private static int NUM_OF_TICK_MARKS = 11;
        private Queue<TextBlock> scaleText; 
        private static int DECENT = 40;
        private static int GOOD = 80;

        private int[] cumSetScores;
        private List<int[]> repScores;
        private int sessNum;
        private int wrkoutNum;
        private int curSet;
        private int curRep;


        public ProgressPage()
        {
            InitializeComponent();
            scaleText = new Queue<TextBlock>();
            scaleText.Enqueue(scale0);
            scaleText.Enqueue(scale1);
            scaleText.Enqueue(scale2);
            scaleText.Enqueue(scale3);
            scaleText.Enqueue(scale4);
            scaleText.Enqueue(scale5);
            scaleText.Enqueue(scale6);
            scaleText.Enqueue(scale7);
            scaleText.Enqueue(scale8);
            scaleText.Enqueue(scale9);
            scaleText.Enqueue(scale10);
            adviceText.Text = "";
        }

        public void loadWorkouts(int session)
        {
            dateWorkoutText.Text = "Select a Workout:";
            sessNum = session;
            int i = 0;
            foreach (Workout workout in MainWindow.data.sessions[sessNum].workouts) {
                Label workoutLbl = new Label()
                {
                    Name = "workout" + i,
                    Content = workout.exercise,
                    FontSize = 24,
                    Height = 50
                };
                workoutLbl.MouseLeftButtonDown += loadSetData;
                dateWorkoutList.Items.Add(workoutLbl);
                i++;
            }
            wrkoutNum = 0;
            curSet = 0;
        }

        private void loadSetData(object sender, RoutedEventArgs e)
        {
            setRepList.Items.Clear();
            String workout = ((Label)sender).Name;
            wrkoutNum = Convert.ToInt32(Regex.Match(workout, @"\d+").Value);
            loadScoreData();
            this.titleText.Text = MainWindow.data.sessions[sessNum].date.ToShortDateString() + " -- " + ((Label)sender).Content;
            int i = 0;
            foreach (Set s in MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets)
            {
                Label setLbl = new Label() {
                    Name = "set" + i,
                    Content = "Set " + (i+1) + " --------------------------------------"
                };
                setLbl.MouseLeftButtonDown += openSet;
                setRepList.Items.Add(setLbl);
                int j = 0;
                foreach (Rep r in s.reps) {
                    Label repLbl = new Label() {
                        Name = "set" + i +"rep" + j,
                        Content = "\t\tRep " + (j+1) + "\t\t"
                    };
                    repLbl.MouseLeftButtonDown += openRep;
                    setRepList.Items.Add(repLbl);
                    j++;
                }
                i++;
            }

            
        }

        private void openSet(object sender, RoutedEventArgs e)
        {
            // clear advice data
            feedbackList.Items.Clear();
            adviceText.Text = "";
            String set = ((Label)sender).Name;
            curSet = Convert.ToInt32(Regex.Match(set, @"\d+").Value);
            // Display graph of this set's rep scores
            makeScoreBars(repScores[curSet]);
            graphTitle.Text = "Set " + (curSet+1) + " Rep Score Averages";
        }
        private void openRep(object sender, RoutedEventArgs e)
        { 
            // clear advice data
            feedbackList.Items.Clear();
            adviceText.Text = "";
            // get referenced set and rep
            String setRep = ((Label)sender).Name;
            String setStr = Regex.Match(setRep, @"set\d+").Value;
            String repStr = Regex.Match(setRep, @"rep\d+").Value;
            curSet = Convert.ToInt32(Regex.Match(setStr, @"\d+").Value);
            curRep = Convert.ToInt32(Regex.Match(repStr, @"\d+").Value);
            // Display graph of this set's rep scores
            makeScoreBars(repScores[curSet]);
            graphTitle.Text = "Set " + (curSet + 1) + " Rep Score Averages";
            // add issues to feedback list box
            List<Label> issues = MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets[curSet].reps[curRep].getProblems();
            foreach (Label l in issues)
            {
                l.MouseLeftButtonDown += showAdvice;
                feedbackList.Items.Add(l);
            }
        }

        private void showAdvice(object sender, RoutedEventArgs e)
        {
            adviceText.Text = ((String)((Label)sender).Tag);
        }
        private void loadScoreData() {
            int numOfSets = MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets.Count;
            cumSetScores = new int[numOfSets];
            repScores = new List<int[]>(numOfSets);
            int[] repScoresArr;
            for (int i = 0; i < numOfSets; i++) {
                repScoresArr = new int[MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets[i].reps.Count];
                for (int j = 0; j < repScoresArr.Length; j++) {
                    for (int k = 0; k < MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets[i].reps[j].scores.Length; k++) {
                        repScoresArr[j] += MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets[i].reps[j].scores[k];
                        
                    }
                    repScoresArr[j] = repScoresArr[j] / MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets[i].reps[j].scores.Length;
                    cumSetScores[i] += repScoresArr[j];
                }
                repScores.Add(repScoresArr);
                cumSetScores[i] = cumSetScores[i] / repScoresArr.Length;
            }
            for (int i = 0; i < numOfSets; i++) {
                Console.Write(cumSetScores[i] + " ");
            }
            Console.WriteLine();
            for (int i = 0; i < numOfSets; i++)
            {
                repScoresArr = repScores[i];
                for (int j = 0; j < repScoresArr.Length; j++) {
                    
                    Console.Write(repScoresArr[j] + " ");
                }
                Console.WriteLine();
            }
        }

        private void ProgressPageLoaded(object sender, RoutedEventArgs e)
        {

        }


        private void makeScoreScale()
        {
            int minValue = 0;
            int interval = 10;
            for (int i = 0; i < NUM_OF_TICK_MARKS; i++)
            {
                TextBlock scale = scaleText.Dequeue();
                int val = minValue + i * interval;
                scale.Text = val.ToString();
                scaleText.Enqueue(scale);
            }
        }

        private void makeWeightScale(int maxWeight, int minWeight)
        {
            int difference = maxWeight - minWeight;
            int scaleHeight = difference;
            if (difference < 50) scaleHeight += 50;
            //else 
            int interval = 10;
            for (int i = 0; i < NUM_OF_TICK_MARKS; i++)
            {
                TextBlock scale = scaleText.Dequeue();
                int val = minWeight + i * interval;
                scale.Text = val.ToString();
                scaleText.Enqueue(scale);
            }
        }

        private void makeScoreBars(int[] score)
        {
            makeScoreScale();
            removeBars();

            int numOfSpaces = (score.Length * 2) + 1;
            int width = MAX_GRAPH_WIDTH / numOfSpaces; // double width + width will provide spacing for each bar
            int offset = (MAX_GRAPH_WIDTH % numOfSpaces) / 2;
            for (int i = 0; i < score.Length; i++)
            {
                double fract = ((double)score[i]) / 100;
                double height = fract * MAX_BAR_HEIGHT;
                double margin = (double)((2 * i * width) + width + offset);
                Color c; // set the color of the bar
                if (score[i] < DECENT) c = Colors.Red;
                else if (score[i] < GOOD) c = Colors.Yellow;
                else c = Colors.Green;
                Rectangle bar = new Rectangle() {
                    Name = "bar" + i,  
                    Width = width,
                    Height = height,
                    VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Margin = new Thickness(margin, 0.0, 0.0, 0.0),
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(c),
                    Visibility = System.Windows.Visibility.Visible,
                };
                this.barGrid.Children.Add(bar);
            }
        }

        private void removeBars()
        {
            this.barGrid.Children.Clear();
        }

        private void makeWeightBar(int score, int numOfBars)
        {

        }

        private void nextSetButton_Click(object sender, RoutedEventArgs e)
        {
            // clear advice data
            feedbackList.Items.Clear();
            adviceText.Text = "";
            // Increment the set and refresh the graph page
            // if it is the last set, go back to the first
            if (curSet == cumSetScores.Length - 1) curSet = 0;
            else curSet++;
            makeScoreBars(repScores[curSet]);
            graphTitle.Text = "Set " + (curSet+1) + " Rep Score Averages";
        }

        private void repScoresButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a bar for each rep, scaled 0 to 100
            // get the currently selected set
            makeScoreBars(repScores[curSet]);
            graphTitle.Text = "Set " + (curSet+1) + " Rep Score Averages";
        }

        private void setWeightsButton_Click(object sender, RoutedEventArgs e)
        {
            // If no weight data, display a message indicating that there is no data
            
            // Display bars corresponding the scaled weights for each set

            
        }

        private void setAvgButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a bar for each set, sized by the score (0-100)
            

            makeScoreBars(cumSetScores);
            graphTitle.Text = "Set Score Averages";
        }
    }
}
