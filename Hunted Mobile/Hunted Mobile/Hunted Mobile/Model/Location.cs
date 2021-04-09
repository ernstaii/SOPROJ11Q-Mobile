using Mapsui.Geometries;

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

        public Location(Mapsui.UI.Forms.Position position) {
            Latitude = position.Latitude;
            Longitude = position.Longitude;
        }

        public Location(Plugin.Geolocator.Abstractions.Position position) {
            Latitude = position.Latitude;
            Longitude = position.Longitude;
        }

        public Mapsui.Geometries.Point ToMapsuiPoint() {
            return AsMapsuiPosition().ToMapsui();
        }

        public Mapsui.UI.Forms.Position AsMapsuiPosition() {
            return new Mapsui.UI.Forms.Position(Latitude, Longitude);
        }

        public Plugin.Geolocator.Abstractions.Position AsGeolocatorPosition() {
            return new Plugin.Geolocator.Abstractions.Position(Latitude, Longitude);
        }
    }
}
