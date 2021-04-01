using Hunted_Mobile.Model.GameModels;

using System.Collections.Generic;

namespace Hunted_Mobile.Model {
    public class Map {
        private readonly List<User> _users = new List<User>();
        public User PlayingUser { get; set; }

        public Map() {

        }

        public void AddUser(User user) {
            _users.Add(user);
        }
        public void RemoveUser(User user) {
            _users.Remove(user);
        }
        public IEnumerable<User> GetUsers() {
            return _users.AsReadOnly();
        }
    }
}
