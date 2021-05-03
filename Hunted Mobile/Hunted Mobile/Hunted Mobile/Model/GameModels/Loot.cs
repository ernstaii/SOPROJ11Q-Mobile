using System;

namespace Hunted_Mobile.Model.GameModels {
    public class Loot {
        private readonly int id;

        public Location Location { get; set; }
        public string Name { get; set; }

        public Loot(int id) {
            this.id = id;
        }
    }
}
