using Mapsui.UI.Forms;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model {
    public class Location {
        public double Lattitude { get; set; }
        public double Longitude { get; set; }

        public Location() {

        }

        public Location(double Lattitude, double Longitude) {
            this.Lattitude = Lattitude;
            this.Longitude = Longitude;
        }

        public Location(Position position) {
            Lattitude = position.Latitude;
            Longitude = position.Longitude;
        }

        public Location(Plugin.Geolocator.Abstractions.Position position) {
            Lattitude = position.Latitude;
            Longitude = position.Longitude;
        }
    }
}
