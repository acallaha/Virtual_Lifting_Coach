using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VLT
{
    [Serializable]
    public class Workout
    {
        public String exercise;
        public List<Set> sets;

        public Workout() {
            this.exercise = "";
            this.sets = new List<Set>();
        }
    }
}
