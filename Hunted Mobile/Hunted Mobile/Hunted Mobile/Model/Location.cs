using Mapsui.UI.Forms;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model {
    public class Location {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location() {

        }

        public Location(double Latitude, double Longitude) {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public Location(Position position) {
            Latitude = position.Latitude;
            Longitude = position.Longitude;
        }

        public Location(Plugin.Geolocator.Abstractions.Position position) {
            Latitude = position.Latitude;
            Longitude = position.Longitude;
        }
    }
}
