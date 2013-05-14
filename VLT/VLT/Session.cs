using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VLT
{
    [Serializable]
    public class Session
    {
        public String username;
        public List<Workout> workouts;
        public DateTime date;

        public Session() {
            this.username = "";
            this.workouts = new List<Workout>();
            this.date = DateTime.Now;

        }
    }
}
