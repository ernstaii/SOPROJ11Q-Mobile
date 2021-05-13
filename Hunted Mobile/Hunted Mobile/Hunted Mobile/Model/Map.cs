using Hunted_Mobile.Model.GameModels;

using Mapsui.Geometries;
using Mapsui.UI.Forms;
using Mapsui.UI.Objects;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hunted_Mobile.Model {
    public class Map {
        private List<Player> users = new List<Player>();

        private List<Loot> loot = new List<Loot>();

        public Player PlayingUser { get; set; }
        public Boundary GameBoundary { get; set; }

        public List<Police> Police => users.Where(user => user is Police).Select(user => new Police(user)).ToList();
        public List<Thief> Thiefs => users.Where(user => user is Thief).Select(user => new Thief(user)).ToList();

        public Map() { }

        public void AddUser(Player user) {
            if(ValidUser(user)) {
                users.Add(user);
            }
        }

        public void RemoveUser(Player user) {
            users.Remove(user);
        }

        public IEnumerable<Player> GetUsers() {
            return users.AsReadOnly();
        }

        public Player GetUserById(int id) {
            return users.FirstOrDefault(user => user.Id == id);
        }

        public void SetUsers(IEnumerable<Player> users) {
            this.users = users.Where(user => ValidUser(user)).ToList();
        }

        public void AddLoot(Loot loot) {
            if(ValidLoot(loot)) {
                this.loot.Add(loot);
            }
        }

        public void RemoveLoot(Loot loot) {
            this.loot.Remove(loot);
        }

        public IEnumerable<Loot> GetLoot() {
            return loot.Where(item => ValidLoot(item)).ToList().AsReadOnly();
        }

        public void SetLoot(IEnumerable<Loot> loot) {
            this.loot = new List<Loot>(loot);
        }

        public Loot FindLoot(Location location) {
            return loot.FirstOrDefault(loot => loot.Location.Equals(location));
        }

        internal Thief FindThief(Location location) {
            return Thiefs.FirstOrDefault(thief => thief.Location.Equals(location));
        }

        internal bool ValidUser(Player user) => user.UserName != null && user.Location != null;
        internal bool ValidLoot(Loot loot) => loot .Name != null && loot.Location != null;
    }
}
