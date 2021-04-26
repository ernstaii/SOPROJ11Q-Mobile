using Hunted_Mobile.Model.GameModels;

using Mapsui.Geometries;
using Mapsui.UI.Forms;
using Mapsui.UI.Objects;

using System.Collections.Generic;
using System.Linq;

namespace Hunted_Mobile.Model {
    public class Map {
        private List<User> _users = new List<User>();

        private List<Loot> _loot = new List<Loot>();
        public User PlayingUser { get; set; }

        public Boundary GameBoundary { get; set; }

        public Map() { }

        public void AddUser(User user) {
            _users.Add(user);
        }
        public void RemoveUser(User user) {
            _users.Remove(user);
        }
        public IEnumerable<User> GetUsers() {
            return _users.AsReadOnly();
        }
        public void SetUsers(IEnumerable<User> users) {
            _users = new List<User>(users);
        }

        public void AddLoot(Loot loot) {
            _loot.Add(loot);
        }
        public void RemoveLoot(Loot loot) {
            _loot.Remove(loot);
        }
        public IEnumerable<Loot> GetLoot() {
            return _loot.AsReadOnly();
        }
        public void SetLoot(IEnumerable<Loot> loot) {
            _loot = new List<Loot>(loot);
        }
        public Loot FindLoot(Location location) {
            return _loot.FirstOrDefault(loot => loot.Location.Equals(location));
        }
    }
}
