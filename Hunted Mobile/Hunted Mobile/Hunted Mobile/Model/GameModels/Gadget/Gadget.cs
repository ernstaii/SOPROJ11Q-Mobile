using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public abstract class Gadget {
        public void Use() {
            // TODO: remove gadget from database and from player's collection
        }

        public abstract bool CanBeUsed();
    }
}
