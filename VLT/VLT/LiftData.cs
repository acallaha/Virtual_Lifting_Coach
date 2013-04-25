using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VLT
{
    class LiftData
    {

        private String FILE_NAME = 
            "C:\\Users\\Matthew Drabick\\Documents\\GitHub\\Virtual_Lifting_Coach\\VLT\\VLT\\LiftData.txt";
        public LiftData()
        {

        }

        public Stack<String> getUserData(String username)
        {
            Stack<String> userData = new Stack<String>();

            using (StreamReader sr = new StreamReader(this.FILE_NAME)) {
                String line;
                while ((line = sr.ReadLine()) != null) {
                    char[] sep = {'\t'};
                    String[] words = line.Split(sep);
                    if (words[0].CompareTo(username) == 0) {
                        userData.Push(line);
                        Console.WriteLine(line);
                    }
                }
                Console.WriteLine(userData.Count());
                Console.WriteLine("--------------------");
            }

            return userData;
        }
        public void writeLine(String user, String exercise, int sets, int reps, int rating ) 
        {
            StreamWriter sw = File.AppendText(this.FILE_NAME);
            
            sw.WriteLine(user + "\t" + DateTime.Now + "\t" + exercise + "\t" + sets + "\t" + reps + "\t" + rating);

            sw.Close();
        }

    }
}
