using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Service.Factory;

using System.Collections.Generic;

namespace Hunted_Mobile.Service.Json {
    public class GadgetJsonService : JsonConversionService<IEnumerable<Gadget>, GadgetData> {
        public GadgetJsonService() {
        }

        public override IEnumerable<Gadget> ToObject(GadgetData data) {
            Location location = new LocationJsonService().ToObjectFromCsv(data.pivot.location);
            var gadgets = new List<Gadget>();
            var gadgetFactory = new GadgetFactory();
            for(int i = 0; i < data.pivot.amount; i++) {
                gadgets.Add(gadgetFactory.GetGadget(data, location));
            }
            return gadgets;
        }


    }
}