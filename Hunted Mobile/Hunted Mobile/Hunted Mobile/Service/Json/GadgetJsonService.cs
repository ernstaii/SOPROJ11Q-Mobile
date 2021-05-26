using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Model.Response.Json;

using System.Collections.Generic;

namespace Hunted_Mobile.Service.Json {
    public class GadgetJsonService : JsonConversionService<IEnumerable<Gadget>, GadgetData> {
        public GadgetJsonService() {
        }

        public override IEnumerable<Gadget> ToObject(GadgetData data) {
            Location location = new LocationJsonService().ToObject(data.pivot.location);
            var gadgets = new List<Gadget>();
            for(int i = 0; i < data.pivot.amount; i++) {
                switch(data.name.ToLower()) {
                    case GadgetName.SMOKE_SCREEN:
                        gadgets.Add(new SmokeScreen(data, location));
                        break;
                    case GadgetName.ALARM:
                        gadgets.Add(new Alarm(data, location));
                        break;
                    case GadgetName.DRONE:
                        gadgets.Add(new Drone(data, location));
                        break;
                    default:
                        break;
                }
            }
            return gadgets;
        }


    }
}