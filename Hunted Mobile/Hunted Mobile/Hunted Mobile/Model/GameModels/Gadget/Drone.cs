using Hunted_Mobile.Model.Response.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public class Drone : Gadget {
        public Drone(GadgetData data) : base(data) {
        }

        public override void Activate(Player user) {
            throw new NotImplementedException();
        }
    }
}
