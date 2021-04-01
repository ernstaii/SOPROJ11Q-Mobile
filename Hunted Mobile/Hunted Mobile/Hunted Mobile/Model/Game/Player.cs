using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Game {
    public abstract class Player : User {
        protected Player(int id, string name = "Placeholder", Location location = new Location()) : base(id, name, location) {
        }
    }
}
