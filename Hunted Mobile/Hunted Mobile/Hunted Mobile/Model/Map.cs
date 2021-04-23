using Hunted_Mobile.Model.GameModels;

using System.Collections.Generic;

namespace Hunted_Mobile.Model {
    public class Map {
        private List<User> users = new List<User>();

        private List<Loot> loot = new List<Loot>();
        public User PlayingUser { get; set; }

        public Boundary GameBoundary { get; set; }

        public Map() { }

        public void AddUser(User user) {
            users.Add(user);
        }
        public void RemoveUser(User user) {
            users.Remove(user);
        }
        public IEnumerable<User> GetUsers() {
            return users.AsReadOnly();
        }
        public void SetUsers(IEnumerable<User> users) {
            this.users = new List<User>(users);
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
    }
}