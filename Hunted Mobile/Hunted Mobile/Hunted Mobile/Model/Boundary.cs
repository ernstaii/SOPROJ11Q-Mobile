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

        public double GetDiameter() {
            double diameter = 0;
            foreach(Location firstLocation in Points) {
                foreach(Location secondLocation in Points) {
                    var distance = firstLocation.DistanceToOther(secondLocation);
                    if(distance > diameter) {
                        diameter = distance;
                    }
                }
            }
            return diameter;
        }
    }
}
