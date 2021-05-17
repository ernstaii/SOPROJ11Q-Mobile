using Hunted_Mobile.Model.Response;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class IntervalEventJsonService : JsonConversionService<IntervalEventData, Model.Response.Json.IntervalEventData> {
        public override string ToJson(IntervalEventData @object) {
            throw new NotImplementedException();
        }

        protected override IntervalEventData ToObject(Model.Response.Json.IntervalEventData data) {
            return new IntervalEventData() {
                Loot = new LootJsonService().ToObjects(data.loot),
                Message = data.message,
                Players = new PlayerJsonService().ToObjects(data.users)
            };
        }
    }
}
