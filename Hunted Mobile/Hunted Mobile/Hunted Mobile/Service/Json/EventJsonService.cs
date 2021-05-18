using Hunted_Mobile.Model.Response;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class EventJsonService : JsonConversionService<EventData, Model.Response.Json.EventData> {
        protected override EventData ToObject(Model.Response.Json.EventData data) {
            return new EventData() {
                Message = data.message,
                TimeLeft = data.timeLeft
            };
        }
    }

    public class IntervalEventJsonService : JsonConversionService<IntervalEventData, Model.Response.Json.IntervalEventData> {
        protected override IntervalEventData ToObject(Model.Response.Json.IntervalEventData data) {
            return new IntervalEventData() {
                Loot = new LootJsonService().ToObjects(data.loot),
                Message = data.message,
                Players = new PlayerJsonService().ToObjects(data.users),
                TimeLeft = data.timeLeft
            };
        }
    }
}
