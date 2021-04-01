using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Game {
    public class Police : Player {
        public Police(int id, string name = "Placeholder", Location location = new Location()) : base(id, name, location) {
        }
    }
}
