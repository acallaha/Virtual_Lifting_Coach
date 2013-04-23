using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Kinect;
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
    /// Interaction logic for teachMePage.xaml
    /// </summary>
    public partial class teachMePage : Page
    {
        public teachMePage()
        {
            InitializeComponent();
        }

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
        /// Possible squat states
        /// </summary>
        enum SquatTeachState { ST_INTRO, ST_FEET, ST_BOTTOM, ST_INTEGRATE, ST_DONE };

        SquatTeachState curSTState = SquatTeachState.ST_INTRO;

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


        double curSTMilliseconds = System.DateTime.Now.TimeOfDay.TotalMilliseconds;

        double STRoundStartTime = 0;
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

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Blank the text used in editing the screen
            this.currentFeedback.Text = "";
            this.currentInstruction.Text = "";
            this.goodRepsLabel.Text = "";
            this.goodRepsText.Text = "";

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
                this.currentInstruction.Text = "   No Kinect Found";
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void PageClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
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

            teachSquatLoop(skeleton);
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

        private void teachSquatLoop(Skeleton skeleton)
        {

            curSTMilliseconds = System.DateTime.Now.TimeOfDay.TotalMilliseconds;

            // give extra time for the welcome
            if (curSTState == SquatTeachState.ST_INTRO && curSTMilliseconds - STRoundStartTime < 5000)
            {
                return;
            }

            // make sure the user sees the feet state
            if (curSTState == SquatTeachState.ST_FEET && curSTMilliseconds - STRoundStartTime < 3500)
            {
                return;
            }

            // don't let the user progress through the rounds too quickkly
            if (curSTMilliseconds - STRoundStartTime < 500)
            {
                return;
            }

            STRoundStartTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
            switch (curSTState)
            {
                case SquatTeachState.ST_INTRO:
                    teachSquatIntro(skeleton);
                    break;
                case SquatTeachState.ST_FEET:
                    teachSquatFeet(skeleton);
                    break;
                case SquatTeachState.ST_BOTTOM:
                    teachSquatBottom(skeleton);
                    break;
                case SquatTeachState.ST_INTEGRATE:
                    teachSquatIntegrate(skeleton);
                    break;
                case SquatTeachState.ST_DONE:
                    teachSquatDone(skeleton);
                    break;
                default:
                    currentInstruction.Text = "ERROR: unknown curSTState";
                    break;
            }
        }


        private void teachSquatIntro(Skeleton skeleton)
        {
            this.currentInstruction.Text = "Welcome! We're going to teach you how to do a proper squat, one step at a time.";
            // System.Threading.Thread.Sleep(2000); TODO: check time each time we're called, don't sleep in UI
            curSTState = SquatTeachState.ST_FEET;
        }
        private void teachSquatFeet(Skeleton skeleton)
        {
            double kneeRightX = skeleton.Joints[JointType.KneeRight].Position.X;
            double kneeLeftX = skeleton.Joints[JointType.KneeLeft].Position.X;
            double kneeWidth = kneeRightX - kneeLeftX;

            double shoulderRightX = skeleton.Joints[JointType.ShoulderRight].Position.X;
            double shoulderLeftX = skeleton.Joints[JointType.ShoulderLeft].Position.X;
            double shoudlerWidth = shoulderRightX - shoulderLeftX;


            this.currentInstruction.Text = "To begin, please stand in front of the Kinect with your feet shoulder width apart";

            if (shoudlerWidth - kneeWidth > 0.1)
            {
                this.currentFeedback.Text = "Your knees are too close together. Please move them further apart";
            }
            else if (shoudlerWidth - kneeWidth < -0.1)
            {
                this.currentFeedback.Text = "Your knees are too far apart. Please move closer together";
            }
            else
            {
                this.currentFeedback.Text = "Nice Job! Your knees are in the correct position. Onto the next step";

                curSTState = SquatTeachState.ST_BOTTOM;
            }
        }

        private void teachSquatBottom(Skeleton skeleton)
        {
            double kneeRightX = skeleton.Joints[JointType.KneeRight].Position.X;
            double kneeLeftX = skeleton.Joints[JointType.KneeLeft].Position.X;
            double kneeWidth = kneeRightX - kneeLeftX;

            double hipHeight = skeleton.Joints[JointType.HipCenter].Position.Y;
            double kneeHeight = (skeleton.Joints[JointType.KneeLeft].Position.Y + skeleton.Joints[JointType.KneeRight].Position.Y) / 2.0;
            double squatDepth = hipHeight - kneeHeight;

            this.currentInstruction.Text = "Now we'll focus on the bottom of the squat. Begin by lowering your hips and pushing your knees out. Keep your back straight. Your hips should be lower than your knees. Be careful not to rock up onto your toes";

            if (squatDepth > 0.25)
            {
                this.currentFeedback.Text = "You need to go lower. Be sure your hips are at least as low as your knees";
                return;
            }



            if (kneeWidth < 0.5)
            {
                this.currentFeedback.Text = "Your knees are too close together. Please move them further apart";
                return;
            }
            else if (kneeWidth > 1.0)
            {
                this.currentFeedback.Text = "Your knees are too far apart. Please move closer together";
                return;
            }
            else
            {
                this.currentFeedback.Text = "Nice Job! Your squat is deep enough and your knees are in the correct position. Onto the next step";
                curSTState = SquatTeachState.ST_INTEGRATE;
            }
        }

        private void teachSquatIntegrate(Skeleton skeleton)
        {

            this.currentInstruction.Text = "Now we'll integrate the previous two states. Begin in the standing position with your feet shoulder's width apart. Drop into the seated squat position as we practiced. To raise yourself, drive your hips forward to return to the original position. We'll do three good squats before moving to the next round";

            setState(skeleton);
            this.goodRepsLabel.Text = "Good Reps Count:   ";
            this.goodRepsText.Text = goodReps.ToString();
            this.currentFeedback.Text = "";
            if (goodReps >= 3)
            {
                this.currentFeedback.Text = "All done!";
                curSTState = SquatTeachState.ST_DONE;
            }
        }

        private void teachSquatDone(Skeleton skeleton)
        {
            this.currentInstruction.Text = "Congratulations! Now you know how to squat. Why don't you try a quick lift?";
            // System.Threading.Thread.Sleep(2000); TODO: check time each time we're called, don't sleep in UI
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
            double maxSquatDepth = double.PositiveInfinity;

            curMilliseconds = System.DateTime.Now.TimeOfDay.TotalMilliseconds;

            if (squatDepth < 0.45 && curSquatState == SquatState.SQUAT_START)
            {
                // squat has begun
                curSquatState = SquatState.SQUAT_IN_PROGRESS;
                squatStartTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
            }

            if (squatDepth < 0.45 && curSquatState == SquatState.SQUAT_IN_PROGRESS)
            {
                // keep track of max depth
                if (squatDepth < maxSquatDepth)
                {
                    maxSquatDepth = squatDepth;
                }

                scoreThings(skeleton);

            }

            if (squatDepth >= 0.45 && curSquatState == SquatState.SQUAT_IN_PROGRESS && (curMilliseconds - squatStartTime) > 1500)
            {
                // squat has ended
                curSquatState = SquatState.SQUAT_START;
                repCount++;

                if (curRep.scores[0] > 90 && curRep.scores[1] > 90)
                {
                    goodReps++;
                }
            }

        }

        private void scoreThings(Skeleton skeleton)
        {
            if (curSquatState == SquatState.SQUAT_START)
            {
                curRep = new Rep();
            }

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

            if (squatDepth < .2)
                return 100;
            else if (squatDepth > .35)
                return 0;
            else
                return (int)((.35 - squatDepth) * 100.0 / .15);
        }

    }
}
