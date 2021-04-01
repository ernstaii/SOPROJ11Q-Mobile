using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Game {
    public class User {
        private readonly int _id;
        public Location Location { get; set; }
        public string Name { get; set; }

        public User(int id, string name = "Placeholder", Location location = new Location()) {
            _id = id;
            Name = name;
            Location = location;
        }
    }
}
