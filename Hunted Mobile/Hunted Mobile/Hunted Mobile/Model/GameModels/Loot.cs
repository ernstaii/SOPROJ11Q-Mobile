using System;

namespace Hunted_Mobile.Model.GameModels {
    public class Loot {
        public int Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }

        public Loot(int id) {
            Id = id;
        }
    }
}
