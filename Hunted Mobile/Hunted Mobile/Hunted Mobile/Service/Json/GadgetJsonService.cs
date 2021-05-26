using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Model.Response.Json;

namespace Hunted_Mobile.Service.Json {
    public class GadgetJsonService : JsonConversionService<Gadget, GadgetData> {
        public GadgetJsonService() {
        }

        public override Gadget ToObject(GadgetData data) {
            Location location = new LocationJsonService().ToObject(data.location);
            switch(data.name.ToLower()) {
                case "rookgordijn":
                    return new SmokeScreen(data, location);
                case "alarm":
                    return new Alarm(data, location);
                case "Drone":
                    return new Drone(data, location);
                default:
                    return default;
            }
        }
    }
}