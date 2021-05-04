using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model {
    public class Game {
        public int Id { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public int Interval { get; set; }
        public int TimeLeft { get; set; }
        public Location PoliceStationLocation { get; set; }
    }
}
