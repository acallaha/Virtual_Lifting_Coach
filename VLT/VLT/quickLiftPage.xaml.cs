﻿// Do you even Lift
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
using System.IO;
using System.Speech.Synthesis;
using Microsoft.Kinect;
using System.Text.RegularExpressions;

namespace VLT
{
    /// <summary>
    /// Interaction logic for quickLiftPage.xaml
    /// </summary>
    public partial class quickLiftPage : Page
    {
        private int currentSet = 1;
        private int currentRep = 0;
        private int currentSetPos = 0;
        private static int DECENT = 50;
        private static int GOOD = 90;
        private int cumlativeScore = 0;
        private int position;
        private Set my_set;
        private Workout my_workout;
        private SpeechSynthesizer speak;
        private int setWeight = 0; 
        private bool audioCues;
        private String[] advice = { "You're the squat master!", "Good Job boss!", "Fantastic Squat!", "Lord Almighty! Impressive Squat!"  };
        private String[] comments = { "Do you even lift?", "Don't you want to get big?", "My motherboard squats lower than that!", "Ain't nobody got time for squats like that", "Beep beep bop. I am a computer." };
        //private Random rand;
        private int joke_index = 0;
        private int advice_index = 0;


        public quickLiftPage()
        {
            InitializeComponent();
            this.audioCues = false;
            this.speak = new SpeechSynthesizer();
            this.exerciseName.Content = MainWindow.curExercise;
            //rand = new Random();
        }

        public void createNewWorkout()
        {
            this.my_workout = new Workout();
            this.my_set = new Set();
        }

        /// <summary>
        /// Adds a rep and set count to the user interface
        /// Adds a weight to a set upon the start of a new set
        /// </summary>
        /// <param name="quality">A score from 1 to 100 of the current rep</param>
        public void makeRep(int quality, Rep new_rep) {
            Label setLabel;
            Color c;
            this.my_set.reps.Add(new_rep);
            cumlativeScore += quality;
            // adjust color based on the quality of the rep. 
            if (quality < DECENT) c = Colors.Red;
            else if (quality < GOOD) c = Colors.Yellow;
            else c = Colors.Green;
            c.A = 127; 
            // If the first rep of a set, add a set label before making a rep
            // Also get the weight of the set if available
            if (currentRep == 0) {
                currentSetPos = position; // save location of set label
                 // Get and save set weight 
                String weightStr = lbsText.Text;
                Console.WriteLine("Weight: " + weightStr);
                weightStr = Regex.Match(weightStr, @"\d+").Value;
                Console.WriteLine("Weight: " + weightStr);
                if (weightStr.Equals(""))
                    setWeight = 0;
                else
                    setWeight = Convert.ToInt32(weightStr);
                Console.WriteLine("Weight: " + setWeight);
                // Make set row
                Console.WriteLine("Making new set row");
                setLabel = new Label() {
                    Background = new SolidColorBrush(c), // paint the set
                    Content = "Set " + currentSet + " -------------- " + setWeight + " lbs. -----------",
                    Name = "Set" + currentSet
                };
                setLabel.MouseLeftButtonDown += showData;
                setRepList.Items.Add(setLabel);
                position++;

            }
            currentRep++;
            position++;
            Label repLabel = new Label() {
                Tag = this.curRep,
                Content = "\tRep " + currentRep + "\t\t-->",
                Background = new SolidColorBrush(c), // paint the set
                Name = "Rep" + currentRep
            };
            repLabel.MouseLeftButtonDown += showData;
            setRepList.Items.Add(repLabel);
            // adjust color based on average score for the set
            if (cumlativeScore/currentRep < DECENT) c = Colors.Red;
            else if (cumlativeScore/currentRep < GOOD) c = Colors.Yellow;
            else c = Colors.Green;
            c.A = 127; 
            // Update the color for the set
            setLabel = (Label)setRepList.Items.GetItemAt(currentSetPos);
            setLabel.Background = new SolidColorBrush(c);
            Console.WriteLine("Increase current Rep: " + currentRep);
            bool knee_wider = (new_rep.scores[0] < DECENT);
            bool go_lower = (new_rep.scores[1] < DECENT);
            //int randNum = rand.Next(0, comments.Length);
            String joke = comments[joke_index];
            if (this.audioCues && knee_wider && go_lower) {
                this.speak.SpeakAsync("Get your knees wider and get down lower! " + joke);
                joke_index = (joke_index + 1) % comments.Length;
            }
            else if (this.audioCues && knee_wider) {
                this.speak.SpeakAsync("Get your knees wider! " + joke);
                joke_index = (joke_index + 1) % comments.Length;
            }
            else if (this.audioCues && go_lower) {
                this.speak.SpeakAsync("Get down lower! " + joke);
                joke_index = (joke_index + 1) % comments.Length;
            }
            else if (this.audioCues) {
                //int randAdvice = rand.Next(0, advice.Length);
                this.speak.SpeakAsync(advice[advice_index]);
                advice_index = (advice_index + 1) % advice.Length;
            }
 
        }

        public void endSet()
        {
            cumlativeScore = 0;
            if (currentRep != 0)
            {
                currentRep = 0;
                currentSet++;
                Console.WriteLine(setWeight);
                my_set.weight = setWeight;
                this.setWeight = 0;

                // Add set to workout
                this.my_workout.sets.Add(my_set);
                // New Set
                this.my_set = new Set();

            }
        }

        public void showData(object sender, RoutedEventArgs e)
        {
            issueBox.Items.Clear();
            adviceBox.Text = "";
            if (((Label)sender).Name.Contains("Rep"))
            {
                List<Label> issues = ((Rep)((Label)sender).Tag).getProblems();
                foreach (Label l in issues)
                {
                    l.MouseLeftButtonDown += showAdvice;
                    issueBox.Items.Add(l);
                }

            }

            //Label issueLabel = new Label();
            //issueLabel.Name = ((Label)sender).Name;
            //if (issueLabel.Name.Contains("Rep"))
            //    adviceBox.Text = ((Label)sender).Name + ": " + ((Rep)((Label)sender).Tag).getFeedback();
            //issueLabel.MouseLeftButtonDown += showAdvice;

            //issueBox.Items.Add(issueLabel);

        }
        public void showAdvice(object sender, RoutedEventArgs e)
        {
            adviceBox.Text = ((String) ((Label) sender).Tag);
        }



        /////////////////////////////////////////////////////////////////////////
        //////////////////// Copy Paste Kinect Code ////////////////////////////

        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Width of output drawing
        /// </summary>
        private int repCount = 0;

        /// <summary>
        /// Possible squat states
        /// </summary>
        enum SquatState { SQUAT_TOP, SQUAT_IN_PROGRESS, SQUAT_START };

        /// <summary>
        /// Current squat state
        /// </summary>
        SquatState curSquatState = SquatState.SQUAT_START;

        /// <summary>
        /// Number of good reps
        /// </summary>
        int goodReps = 0;

        double curMilliseconds = 0;

        double squatStartTime = 0;

        /// <summary>
        /// Number of bad reps
        /// </summary>
        int badReps = 0;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        private Rep curRep = new Rep();

        //private List<List<Rep>> sets = new List<List<Rep>>();

        private bool hasSkeleton = false;



        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }



        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            this.audioCues = false;
            this.endSet();
            this.my_workout.exercise = (String)this.exerciseName.Content;
            // Add workout to session
            Console.WriteLine(this.my_workout.sets.Count);
            if (!(this.currentSet == 1 && this.currentRep == 0))
            {
                MainWindow.session.workouts.Add(my_workout);
                this.my_workout = new Workout();
            }
            //Console.WriteLine(this.my_workout.sets.Count);
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            //this.sets.Add(new List<Rep>());

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                
                Console.WriteLine("yay it worked");
                
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                Console.WriteLine("boo it failed");
                //this.squatDepthLabel.Text = "No Kinect Found";
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void PageClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("goodbye my dear quicklift page ;(");
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }

        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];


            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
  
                }

            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (!this.hasSkeleton)
                            {
                                this.curSquatState = SquatState.SQUAT_START;
                                Console.WriteLine("has skel grr");
                                this.endSet();
                                //this.sets.Add(new List<Rep>());
                                this.hasSkeleton = true;
                            }
                            this.DrawBonesAndJoints(skel, dc);


                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            Console.WriteLine("no skeleton here");
                            this.hasSkeleton = false;
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }


        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
            //Console.WriteLine("Calling scoreThings");
            scoreThings(skeleton);
            //Console.WriteLine("Calling setState");
            SquatState prevSquatState = this.curSquatState;
            setState(skeleton);
            //Console.WriteLine(this.curSquatState);

        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        private void setState(Skeleton skeleton)
        {
            // this.hipDistText.Text = skeleton.Joints[JointType.HipCenter].Position.Z.ToString();

            double kneeRightX = skeleton.Joints[JointType.KneeRight].Position.X;
            double kneeLeftX = skeleton.Joints[JointType.KneeLeft].Position.X;
            double kneeWidth = kneeRightX - kneeLeftX;


            double hipHeight = skeleton.Joints[JointType.HipCenter].Position.Y;
            double kneeHeight = (skeleton.Joints[JointType.KneeLeft].Position.Y + skeleton.Joints[JointType.KneeRight].Position.Y) / 2.0;
            double squatDepth = hipHeight - kneeHeight;
            double hipZCoord = skeleton.Joints[JointType.HipCenter].Position.Z;
            //this.squatDepthText.Text = squatDepth.ToString("0.000");
            double maxSquatDepth = double.PositiveInfinity;

            curMilliseconds = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
            if (hipZCoord > 1.5)
            {

                if (squatDepth < 0.45 && curSquatState == SquatState.SQUAT_START)
                {
                    // squat has begun
                    curSquatState = SquatState.SQUAT_IN_PROGRESS;
                    //this.currentStateText.Text = "Squat in Progress";
                    squatStartTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
                }

                if (squatDepth < 0.45 && curSquatState == SquatState.SQUAT_IN_PROGRESS)
                {
                    // keep track of max depth
                    if (squatDepth < maxSquatDepth)
                    {
                        maxSquatDepth = squatDepth;
                        //this.maxDepthText.Text = maxSquatDepth.ToString("0.000");
                    }

                    // set colors for live display
                    if (squatDepth < .25)
                    {
                        //this.squatDepthText.Foreground = Brushes.Green;
                        if (kneeWidth > .5) { }
                        //this.kneeText.Foreground = Brushes.Green;
                        else { }
                        //this.kneeText.Foreground = Brushes.Red;
                    }
                    else
                    {
                        // this.squatDepthText.Foreground = Brushes.Red;
                        //this.kneeText.Foreground = Brushes.Black;
                    }
                }

                if (squatDepth >= 0.45 && curSquatState == SquatState.SQUAT_IN_PROGRESS && (curMilliseconds - squatStartTime) > 1500)
                {
                    // squat has ended
                    int average = (this.curRep.scores[0] + this.curRep.scores[1]) / 2;

                    this.makeRep(average, curRep);
                    curRep = new Rep();
                    //this.sets[sets.Count - 1].Add(curRep); // add curRep to the current set
                    curSquatState = SquatState.SQUAT_START;
                    //this.currentStateText.Text = "Squat over, begin squat";
                    repCount++;
                    //this.squatDepthText.Text = squatDepth.ToString("0.000");
                    //this.repCountText.Text = repCount.ToString("0");
                    if (maxSquatDepth < 0.3)
                    {
                        goodReps++;
                        //this.goodRepsText.Text = goodReps.ToString("0.000");
                    }
                    else
                    {
                        badReps++;
                    }
                }
            }
        }

        private void scoreThings(Skeleton skeleton)
        {
            int kneeScore, depthScore;
            kneeScore = scoreKneeWidth(skeleton);
            depthScore = scoreDepth(skeleton);

            if (kneeScore > curRep.scores[0])
            {
                curRep.scores[0] = kneeScore;
            }
            if (depthScore > curRep.scores[1])
            {
                curRep.scores[1] = depthScore;
            }

            //Console.WriteLine("Scores: " + kneeScore + " " + depthScore);

            //kneeText.Text = curRep.scores[0].ToString();
            //depthText.Text = curRep.scores[1].ToString();

        }



        private int scoreKneeWidth(Skeleton skeleton)
        {

            double hipHeight = skeleton.Joints[JointType.HipCenter].Position.Y;
            double kneeHeight = (skeleton.Joints[JointType.KneeLeft].Position.Y + skeleton.Joints[JointType.KneeRight].Position.Y) / 2.0;
            double squatDepth = hipHeight - kneeHeight;

            double kneeRightX = skeleton.Joints[JointType.KneeRight].Position.X;
            double kneeLeftX = skeleton.Joints[JointType.KneeLeft].Position.X;
            double kneeWidth = kneeRightX - kneeLeftX;

            if (squatDepth > .3)
                return -1;

            if (kneeWidth > .6)
                return 100;
            else if (kneeWidth < .3)
                return 0;
            else
                return (int)((kneeWidth - .3) * 100.0 / .3);
        }

        private int scoreDepth(Skeleton skeleton)
        {
            double hipHeight = skeleton.Joints[JointType.HipCenter].Position.Y;
            double kneeHeight = (skeleton.Joints[JointType.KneeLeft].Position.Y + skeleton.Joints[JointType.KneeRight].Position.Y) / 2.0;
            double squatDepth = hipHeight - kneeHeight;
            double hipZCoord = skeleton.Joints[JointType.HipCenter].Position.Z;
            //this.squatDepthText.Text = squatDepth.ToString("0.000");

            //this.currentDepthText.Text = hipZCoord.ToString("0.000");

            if (squatDepth < .2)
                return 100;
            else if (squatDepth > .35)
                return 0;
            else
                return (int)((.35 - squatDepth) * 100.0 / .15);
        }

        private void audioCuesCB_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("changing cb");
            this.audioCues = (bool)audioCuesCB.IsChecked;
        }



    }
}
