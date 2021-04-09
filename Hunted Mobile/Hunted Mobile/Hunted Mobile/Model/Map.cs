using Hunted_Mobile.Model.GameModels;

using Mapsui.Geometries;
using Mapsui.UI.Forms;
using Mapsui.UI.Objects;

using System.Collections.Generic;

namespace Hunted_Mobile.Model {
    public class Map {
        private readonly List<User> _users = new List<User>();

        private readonly List<Loot> _loot = new List<Loot>();
        public User PlayingUser { get; set; }

        public Boundary GameBoundary { get; set; }

        public Map() { }

        public void AddUser(User user) {
            _users.Add(user);
        }
        public void RemoveUser(User user) {
            _users.Remove(user);
        }
        public void AddLoot(Loot loot) {
            _loot.Add(loot);
        }
        public void RemoveLoot(Loot loot) {
            _loot.Remove(loot);
        }
        public IEnumerable<User> GetUsers() {
            return _users.AsReadOnly();
        }
        public IEnumerable<Loot> GetLoot() {
            return _loot.AsReadOnly();
        }
        
        /*public void SetCircleBoundary(Position center, Distance radius) {
            GameBoundary = new Circle() {
                Center = center,
                StrokeColor = Xamarin.Forms.Color.Red,
                FillColor = Xamarin.Forms.Color.Transparent,
                Radius = radius,
                Quality = 360.0
            };
        }*/
        public void SetPolygonBoundary(Boundary boundary) {
            GameBoundary = boundary;
        }
    }
}