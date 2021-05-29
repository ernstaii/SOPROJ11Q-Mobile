using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Model.Response.Json;

namespace Hunted_Mobile.Service.Factory {
    public class GadgetFactory {
        public GadgetFactory() {
        }

        public Gadget GetGadget(GadgetData data, Location location) {
            switch(data.name.ToLower()) {
                case GadgetName.SMOKE_SCREEN:
                    return new SmokeScreen(data);
                case GadgetName.ALARM:
                    return new Alarm(data, location);
                case GadgetName.DRONE:
                    return new Drone(data);
                 default:
                    return null;
            }
        }
    }
}
