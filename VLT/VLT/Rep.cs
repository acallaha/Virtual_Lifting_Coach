using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace VLT
{
    [Serializable]
    public class Rep
    {
        public string exercise;
        public int[] scores;
        public double weight;

        public Rep()
        {
            scores = new int[2];
            scores[0] = -1;
            scores[1] = -1;
        }


        public List<String> getAdviceList()
        {
            List<String> adviceList = new List<String>();

            if (scores[0] > 95)
            {
                adviceList.Add("Nice Job!");
            }
            else
            {
                adviceList.Add("Your knees are not far out enough at the bottom of the rep - you need to focus on pushing them out at the bottom. ");
            }

            if (scores[0] > 95)
            {
                adviceList.Add("Nice Job!");
            }
            else
            {
                adviceList.Add("You aren't getting deep enough at the bottom of your squat - try to get your hips below your knees. ");
            }

            return adviceList;
        }

        public List<Label> getProblems()
        {
            List<Label> probs = new List<Label>();
            Color c;
            if (scores[0] < 95)
            {
                Label l = new Label();
                if (scores[0] < 50)
                {
                    c = Colors.Red;
                    c.A = 127;
                    l.Background = new SolidColorBrush(c);
                }
                else
                {
                    c = Colors.Yellow;
                    c.A = 127;
                    l.Background = new SolidColorBrush(c);
                }
                l.Content = "Knees too close\t\t\t-->";
                l.Tag = "Your knees are not far out enough at the bottom of the rep - you need to focus on pushing them out at the bottom. ";
                probs.Add(l);
            }
            else
            {
                Label l = new Label();
                c = Colors.Green;
                c.A = 127;
                l.Background = new SolidColorBrush(c);
                l.Content = "Good job with your knee width!\t-->";
                l.Tag = "Nice job!";
                probs.Add(l);
            }
            if (scores[1] < 95)
            {
                Label l = new Label();
                if (scores[1] < 50)
                {
                    c = Colors.Red;
                    c.A = 127;
                    l.Background = new SolidColorBrush(c);
                }
                else
                {
                    c = Colors.Yellow;
                    c.A = 127;
                    l.Background = new SolidColorBrush(c);
                }
                l.Content = "Insufficient depth\t\t\t-->";
                l.Tag = "You aren't getting deep enough at the bottom of your squat - try to get your hips below your knees. ";
                probs.Add(l);
            }
            else
            {
                Label l = new Label();
                c = Colors.Green;
                c.A = 127;
                l.Background = new SolidColorBrush(c);
                l.Content = "Good job with your depth!\t\t-->";
                l.Tag = "Nice job!";
                probs.Add(l);
            }
            return probs;
        }
    }
}
