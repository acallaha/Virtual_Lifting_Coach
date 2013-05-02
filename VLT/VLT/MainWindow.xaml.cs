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

    }
}
