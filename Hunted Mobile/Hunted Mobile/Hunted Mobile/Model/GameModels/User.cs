using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class User {
        public readonly int Id;
        private string _userName;
        public Location Location { get; set; }
        public InviteKey InviteKey { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public User(int id) {
            Id = id;
        }

        public User() {
        }
    }
}
