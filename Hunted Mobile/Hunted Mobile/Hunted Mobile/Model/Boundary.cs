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
    }
}
