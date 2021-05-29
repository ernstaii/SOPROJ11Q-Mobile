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
        public bool InUse { get; }

        protected Gadget(GadgetData data) {
            Id = data.id;
            Name = data.name;
            InUse = data.pivot.in_use;
        }

        public abstract void Activate(Player user);
    }
}
