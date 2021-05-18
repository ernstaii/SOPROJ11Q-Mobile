using Hunted_Mobile.Service;

using Mapsui.Geometries;

using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

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

        public Location(string commaSeparatedLatitudeLongitude) {
            string[] split = commaSeparatedLatitudeLongitude.Split(',');
            try {
                Latitude = double.Parse(split[0]);
                Longitude = double.Parse(split[1]);
            }
            catch(Exception ex) {
                DependencyService.Get<Toast>().Show("Er was een probleem met het splitsen van de locatie variabelen");
            }
        }

        public Location(double latitude, double longitude) {
            this.Latitude = latitude;
            this.Longitude = longitude;
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

        public double DistanceToOther(Location other) {
            return Math.Sqrt(
                Math.Pow(other.Latitude - Latitude, 2) + Math.Pow(other.Longitude - Longitude, 2)
            );
        }

        // https://stackoverflow.com/a/11172685
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Mathatmatical notations")]
        public double DistanceToOtherInMeters(Location other) {
            var R = 6378.137; // Radius of earth in KM
            var dLat = other.Latitude * Math.PI / 180 - Latitude * Math.PI / 180;
            var dLon = other.Longitude * Math.PI / 180 - Longitude * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Latitude * Math.PI / 180) * Math.Cos(other.Latitude * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return d * 1000; // meters
        }

        public string ToCsvString() {
            return $"{Latitude},{Longitude}";
        }

        public bool Equals(Location location) {
            return location.Latitude.Equals(Latitude) && location.Longitude.Equals(Longitude);
        }
    }
}
