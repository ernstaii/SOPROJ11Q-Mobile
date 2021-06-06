using System;

namespace Hunted_Mobile.Model.GameModels {
    public class Loot {
        public int Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public bool IsValid => Name != null && Location != null;

        public Loot() {
        }
    }
}
