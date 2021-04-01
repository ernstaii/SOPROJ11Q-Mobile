using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class User {
        private readonly int _id;
        public Location Location { get; set; }
        public string Name { get; set; }
        public string InviteKey { get; set; }
        public int Role { get; set; }

        public User(int id) {
            _id = id;
        }
    }
}
