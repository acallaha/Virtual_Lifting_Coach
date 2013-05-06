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
using Microsoft.Kinect;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace VLT
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Session session;
        public static AllData data;
        //public static String data_filepath;
        private static Random rand; // for making fake data

        public MainWindow()
        {

            MainWindow.session = new Session();
            MainWindow.data = new AllData();

            DeserializeFromXML();
            InitializeComponent();
            mainFrame.Navigate(new SelectExercise());
            Console.WriteLine("It initialized");

            /* A better way to do the filepath... not worth the time it took and still doesnt work.
            MainWindow.data_filepath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", ""), "LiftData.xml");
            Console.WriteLine(MainWindow.data_filepath);
             */
        }
        static public void SerializeToXML(AllData data)
        {
            IFormatter formatter = new BinaryFormatter();
            
            using (FileStream stream = new FileStream(@"LiftData.xml", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, data);
            }
           
        }

        static public void DeserializeFromXML()
        {
            
            IFormatter formatter = new BinaryFormatter();
            
            using (FileStream stream = new FileStream(@"LiftData.xml", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                MainWindow.data = (AllData)formatter.Deserialize(stream);
            }
           
        }

        static public List<Rep> fakeRepList(int n)
        {
            List<Rep> repList = new List<Rep>();
            for (int i = 0; i < n; i++)
            {
                Rep r = new Rep();
                r.exercise = "Back Squat";
                r.scores[0] = MainWindow.rand.Next(0, 100);
                r.scores[1] = MainWindow.rand.Next(0, 100);
                repList.Add(r);
            }
            return repList;
        }

        static public List<Set> fakeSetList(int n)
        {
            List<Set> setList = new List<Set>();
            for (int i = 0; i < n; i++)
            {
                Set s = new Set();
                s.weight = MainWindow.rand.Next(100, 200);
                s.reps = fakeRepList(MainWindow.rand.Next(5,16));
                setList.Add(s);
            }

            return setList;
        }


        static public List<Workout> fakeWorkoutList(int n)
        {
            List<Workout> workoutList = new List<Workout>();
            for (int i = 0; i < n; i++)
            {
                Workout w = new Workout();
                w.exercise = "Back Squat";
                w.sets = fakeSetList(MainWindow.rand.Next(3,8));
                workoutList.Add(w);
            }

            return workoutList;
        }

        static public List<Session> fakeSessionList(int n)
        {
            List<Session> sessionList = new List<Session>();
            for (int i = 0; i < n; i++)
            {
                Session s = new Session();
                s.date = s.date.AddDays(i);
                s.username = "Guest";
                s.workouts = fakeWorkoutList(1);
                sessionList.Add(s);
            }

            return sessionList;
        }

        static public void writeFakeData()
        {
            MainWindow.rand = new Random();
            MainWindow.data = new AllData();
            MainWindow.data.sessions = fakeSessionList(25);
            MainWindow.SerializeToXML(MainWindow.data);

        }

    }
}
