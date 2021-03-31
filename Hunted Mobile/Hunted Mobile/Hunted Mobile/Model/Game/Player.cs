using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Game {
    public abstract class Player : User {
        protected Player(int id) : base(id) {
        }
    }
}
