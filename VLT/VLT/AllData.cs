using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VLT 
{
    [Serializable]
    public class AllData
    {
        public List<Session> sessions;

        public AllData() {
            this.sessions = new List<Session>();
        }
    }
}
