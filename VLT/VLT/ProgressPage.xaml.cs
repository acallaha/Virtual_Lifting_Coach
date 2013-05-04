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

        /// <summary>
        /// Loads the workout data and sets the setNum instance variable
        /// </summary>
        /// <param name="session">session index</param>
        public void loadWorkouts(int session)
        {
            dateWorkoutText.Text = "Select a Workout:";
            sessNum = session;
            // Add each workout label to the sepRepList ListBox
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
            // initially select workout 0, and set 0 in that workout (must be set for the Next Set button to work)
            wrkoutNum = 0;
            curSet = 0;
        }

        /// <summary>
        /// Loads the set and rep data for a particular workout when it is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadSetData(object sender, RoutedEventArgs e)
        {
            setRepList.Items.Clear();
            // Get the identity of the workout
            String workout = ((Label)sender).Name;
            wrkoutNum = Convert.ToInt32(Regex.Match(workout, @"\d+").Value);
            loadScoreData(); // get all the scoring information about that workout
            // modify title
            this.titleText.Text = MainWindow.data.sessions[sessNum].date.ToShortDateString() + " -- " + ((Label)sender).Content;

            // Add each set label to the sepRepList ListBox
            int i = 0;
            foreach (Set s in MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets)
            {
                int quality = cumSetScores[i];
                Color c;
                if (quality < DECENT) c = Colors.Red;
                else if (quality < GOOD) c = Colors.Yellow;
                else c = Colors.Green;
                c.A = 127; 
                Label setLbl = new Label() {
                    Name = "set" + i,
                    Content = "Set " + (i+1) + " --------------------------------------------------",
                    Background = new SolidColorBrush(c)
                };
                setLbl.MouseLeftButtonDown += openSet;
                setRepList.Items.Add(setLbl);
                
                // Add each rep label to the sepRepList ListBox
                int j = 0;
                foreach (Rep r in s.reps) {
                    quality = (repScores[i])[j];
                    if (quality < DECENT) c = Colors.Red;
                    else if (quality < GOOD) c = Colors.Yellow;
                    else c = Colors.Green;
                    c.A = 127; 
                    Label repLbl = new Label() {
                        Name = "set" + i +"rep" + j,
                        Content = "\t\tRep " + (j+1) + "\t\t",
                        Background = new SolidColorBrush(c)
                    };
                    repLbl.MouseLeftButtonDown += openRep;
                    setRepList.Items.Add(repLbl);
                    j++;
                }
                i++;
            }
        }

        /// <summary>
        /// Opens the graphs for a particular set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Opens the graphs for the set that contains this rep. Opens feedback for
        /// that particular rep. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Show the advice upon the click of a feeback label in the feedbackList ListBox. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAdvice(object sender, RoutedEventArgs e)
        {
            adviceText.Text = ((String)((Label)sender).Tag);
        }

        /// <summary>
        /// Load all the score data about a particular workout into the array 
        /// cumSetScores and the arrays that make up the repScores List that can
        /// be manipulated and used for graphing and coloring different labels.
        /// </summary>
        private void loadScoreData() {
            int numOfSets = MainWindow.data.sessions[sessNum].workouts[wrkoutNum].sets.Count;
            cumSetScores = new int[numOfSets];
            repScores = new List<int[]>(numOfSets);
            int[] repScoresArr;
            // cycles through the sets, the reps, and the scores for a particular rep
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
        }
        
        /// <summary>
        /// Upon the page being loaded. Not used at the moment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressPageLoaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Reset the graph scale tick marks to 0 - 100 by 10's
        /// </summary>
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

        /// <summary>
        /// Reset the graph scale tick marks to identify weights. 
        /// Dependent on the weights used for the set 
        /// Needs work!!!
        /// </summary>
        /// <param name="maxWeight"></param>
        /// <param name="minWeight"></param>
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
        
        /// <summary>
        /// Make bars corresponding to the scores which range from 0 - 100 
        /// </summary>
        /// <param name="score"></param>
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

        /// <summary>
        /// Removes all the bars from the graph grid
        /// </summary>
        private void removeBars()
        {
            this.barGrid.Children.Clear();
        }

        /// <summary>
        /// Make bars dependent on the weight used in a set
        /// </summary>
        /// <param name="score"></param>
        /// <param name="numOfBars"></param>
        private void makeWeightBar(int score, int numOfBars)
        {
            removeBars();

        }

        /// <summary>
        /// Advance the graph to the scores (or weights - coming soon!) for the next set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Display the graph of the reps for the current set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repScoresButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a bar for each rep, scaled 0 to 100
            // get the currently selected set
            makeScoreBars(repScores[curSet]);
            graphTitle.Text = "Set " + (curSet+1) + " Rep Score Averages";
        }

        /// <summary>
        /// Coming soon!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setWeightsButton_Click(object sender, RoutedEventArgs e)
        {
            // If no weight data, display a message indicating that there is no data
            
            // Display bars corresponding the scaled weights for each set

            
        }

        /// <summary>
        /// Display a graph of the average scores for each set. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setAvgButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a bar for each set, sized by the score (0-100)
            makeScoreBars(cumSetScores);
            graphTitle.Text = "Set Score Averages";
        }
    }
}
