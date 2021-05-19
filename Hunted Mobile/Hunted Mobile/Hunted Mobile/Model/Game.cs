using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace Hunted_Mobile.Model {
    public class Game {
        private string colourTheme;

        public int Id { get; set; }
        public int Duration { get; set; }
        public string ColourTheme {
            get => colourTheme;
            set {
                colourTheme = value != null && value.Contains("#") ? value : Color.RoyalBlue.ToHex();
            }
        }
        public string Status { get; set; }
        public int Interval { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public int TimeLeft { get; set; }
        public int ThievesScore { get; set; }
        public int PoliceScore { get; set; }
        public Location PoliceStationLocation { get; set; }
    }
}
