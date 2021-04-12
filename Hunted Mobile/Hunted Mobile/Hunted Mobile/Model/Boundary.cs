using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model {
    public class Boundary {
        public List<Location> Points { get; }

        public Boundary() {
            Points = new List<Location>();
        }

        public Mapsui.Geometries.Polygon ToPolygon() {
            var polygon = new Mapsui.Geometries.Polygon();
            foreach(Location point in Points) {
                polygon.ExteriorRing.Vertices.Add(point.ToMapsuiPoint());
            }
            return polygon;
        }

        public Location GetCenter() {
            if(Points.Count > 0) {
                Location center = new Location(0, 0);

                foreach(Location location in Points) {
                    center.Latitude += location.Latitude;
                    center.Longitude += location.Longitude;
                }

                center.Latitude /= Points.Count;
                center.Longitude /= Points.Count;

                return center;
            }
            else return null;
        }

        public Location GetPointFurthestNorth() {
            Location furthestNorth = null;

            foreach(Location location in Points) {
                if(furthestNorth == null || location.Longitude > furthestNorth.Longitude) {
                    furthestNorth = location;
                }
            }

            return furthestNorth;
        }

        public Location GetPointFurthestSouth() {
            Location furthestSouth = null;

            foreach(Location location in Points) {
                if(furthestSouth == null || location.Longitude < furthestSouth.Longitude) {
                    furthestSouth = location;
                }
            }

            return furthestSouth;
        }

        public Location GetPointFurthestEast() {
            Location furthestEast = null;

            foreach(Location location in Points) {
                if(furthestEast == null || location.Latitude > furthestEast.Latitude) {
                    furthestEast = location;
                }
            }

            return furthestEast;
        }

        public Location GetPointFurthestWest() {
            Location furthestWest = null;

            foreach(Location location in Points) {
                if(furthestWest == null || location.Latitude < furthestWest.Latitude) {
                    furthestWest = location;
                }
            }

            return furthestWest;
        }
    }
}
