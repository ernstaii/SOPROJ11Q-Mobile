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
                    double distance = firstLocation.DistanceToOther(secondLocation);
                    if(distance > diameter) {
                        diameter = distance;
                    }
                }
            }
            return diameter;
        }

        // http://www.alienryderflex.com/polygon/
        // 
        //  The function will return true if the point x,y (location) is inside the
        //  polygon (boundary), or false if it is not.
        //  If the point is exactly on the edge of the polygon, then the function
        //  may return true or false.
        //
        //  Note that division by zero is avoided because the division is protected
        //  by the "if" clause which surrounds it.
        public bool Contains(Location location) {
            int j = Points.Count - 1;
            bool oddNodes = false;

            double testingX = location.Latitude;
            double testingY = location.Longitude;

            for(int i = 0; i < Points.Count; i++) {
                double currentBoundaryPointY = Points[i].Longitude;
                double currentBoundaryPointX = Points[i].Latitude;
                double previousBoundaryPointY = Points[j].Longitude;
                double previousBoundaryPointX = Points[j].Latitude;
                if((currentBoundaryPointY < testingY && previousBoundaryPointY >= testingY
                    || previousBoundaryPointY < testingY && currentBoundaryPointY >= testingY)
                    && (currentBoundaryPointX <= testingX || previousBoundaryPointX <= testingX)) {
                    if(currentBoundaryPointX + 
                        (testingY - currentBoundaryPointY) / (previousBoundaryPointY - currentBoundaryPointY) *
                        (previousBoundaryPointX - currentBoundaryPointX) < testingX) {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }
    }
}
