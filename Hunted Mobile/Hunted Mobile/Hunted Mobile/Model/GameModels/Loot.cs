using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class Loot {
        private readonly int _id;
        public Location Location { get; set; }
        public string Name { get; set; }

        public Loot(int id) {
            _id = id;
        }
    }
}
