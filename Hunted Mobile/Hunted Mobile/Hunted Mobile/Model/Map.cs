using Hunted_Mobile.Model.GameModels;

using Mapsui.Geometries;
using Mapsui.UI.Forms;
using Mapsui.UI.Objects;

using System.Collections.Generic;
using System.Linq;

namespace Hunted_Mobile.Model {
    public class Map {
        private List<Player> users = new List<Player>();

        private List<Loot> loot = new List<Loot>();

        public Player PlayingUser { get; set; }
        public Boundary GameBoundary { get; set; }

        public Map() { }

        public void AddUser(Player user) {
            users.Add(user);
        }
        public void RemoveUser(Player user) {
            users.Remove(user);
        }
        public IEnumerable<Player> GetUsers() {
            return users.AsReadOnly();
        }
        public void SetUsers(IEnumerable<Player> users) {
            users = new List<Player>(users);
        }

        public void AddLoot(Loot loot) {
            this.loot.Add(loot);
        }

        public void RemoveLoot(Loot loot) {
            this.loot.Remove(loot);
        }

        public IEnumerable<Loot> GetLoot() {
            return loot.AsReadOnly();
        }

        public void SetLoot(IEnumerable<Loot> loot) {
            this.loot = new List<Loot>(loot);
        }

        public Loot FindLoot(Location location) {
            return loot.FirstOrDefault(loot => loot.Location.Equals(location));
        }
    }
}
