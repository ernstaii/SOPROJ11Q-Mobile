using Mapsui.Geometries;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model {
    /// <summary>
    /// A general map location model
    /// Use this within our models and wherever else it is suitable
    /// </summary>
    public class Location {
        /// <summary>
        /// Horizontal map coordinate
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Vertical map coordinate
        /// </summary>
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

        public double DistanceToOther(Location location) {
            return Math.Sqrt(
                Math.Pow(location.Latitude - Latitude, 2) + Math.Pow(location.Longitude - Longitude, 2)
            );
        }
    }
}
