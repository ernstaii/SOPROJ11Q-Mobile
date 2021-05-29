using Hunted_Mobile.Model.Response.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public class Alarm : Gadget {
        public Location Location { get; }

        public Alarm(GadgetData data, Location location) : base(data) {
            Location = location;
        }

        public override void Activate(Player user) {
            throw new NotImplementedException();
        }
    }
}
