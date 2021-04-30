using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public abstract class Player : User {
        protected Player(int id) : base(id) {
        }
    }
}
