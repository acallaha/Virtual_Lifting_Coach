using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VLT
{
    [Serializable]
    public class Set
    {
        public int weight;
        public List<Rep> reps;

        public Set() {
            this.weight = 0;
            this.reps = new List<Rep>();
        }
    }
}
