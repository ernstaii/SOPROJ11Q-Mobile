using Hunted_Mobile.Model.Game;

using System.Collections.Generic;

namespace Hunted_Mobile.Model {
    public class Map {
        private readonly List<User> _users = new List<User>();

        private readonly List<Loot> _loot = new List<Loot>();
        public User PlayingUser { get; set; }

        public Map() {

        }

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
    }
}
