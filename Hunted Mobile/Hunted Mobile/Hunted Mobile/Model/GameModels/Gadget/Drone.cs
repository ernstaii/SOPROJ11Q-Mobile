using Hunted_Mobile.Model.Response.Json;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public class Drone : Gadget {
        public Drone(GadgetData data) : base(data) {
            Description = "Bij de volgende locatieupdate zijn boeven ook te zien op de kaart";
        }
    }
}
