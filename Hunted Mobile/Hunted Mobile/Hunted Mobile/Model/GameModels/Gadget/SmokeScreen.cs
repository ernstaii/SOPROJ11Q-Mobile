using Hunted_Mobile.Model.Response.Json;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public class SmokeScreen : Gadget {
        public SmokeScreen(GadgetData data) : base(data) {
            Description = "Zorgt ervoor dat je locatie niet te zien is in de volgende locatieupdate";
        }
    }
}
