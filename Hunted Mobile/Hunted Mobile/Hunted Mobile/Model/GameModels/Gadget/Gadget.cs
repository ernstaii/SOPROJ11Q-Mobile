using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Repository;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public abstract class Gadget {
        public int Id { get; }
        public string Name { get; }
        public bool InUse => Location != null && Location.IsSet();
        public Location Location { get; }

        protected Gadget(GadgetData data, Location location) {
            Id = data.id;
            Name = data.name;
            Location = location;
        }

        public abstract void Activate(Player user);
    }
}
