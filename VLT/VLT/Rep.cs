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

        public Rep()
        {
            scores = new int[2];
            scores[0] = -1;
            scores[1] = -1;
        }

        public string getFeedback()
        {
            string advice = "Feeback: ";
            
            return advice;
        }

        public List<Label> getProblems()
        {
            List<Label> probs = new List<Label>();
            if (scores[0] < 95)
            {
                Label l = new Label();
                if (scores[0] < 50)
                    l.Background = new SolidColorBrush(Colors.Red);
                else
                    l.Background = new SolidColorBrush(Colors.Yellow);
                l.Content = "Knees too close";
                l.Tag = "Your knees are not far out enough at the bottom of the rep - you need to focus on pushing them out at the bottom. ";
                probs.Add(l);
            }
            else
            {
                Label l = new Label();
                l.Background = new SolidColorBrush(Colors.Green);
                l.Content = "Good Job with your knee width!";
                l.Tag = "Nice job!";
                probs.Add(l);
            }
            if (scores[1] < 95)
            {
                Label l = new Label();
                if (scores[1] < 50)
                    l.Background = new SolidColorBrush(Colors.Red);
                else
                    l.Background = new SolidColorBrush(Colors.Yellow); 
                l.Content = "Insufficient Depth";
                l.Tag = "You aren't getting deep enough at the bottom of your squat - try to get your hips below your knees. ";
                probs.Add(l);
            }
            else
            {
                Label l = new Label();
                l.Background = new SolidColorBrush(Colors.Green);
                l.Content = "Good Job with your knee depth!";
                l.Tag = "Nice job!";
                probs.Add(l);
            }
            return probs;
        }
    }
}
