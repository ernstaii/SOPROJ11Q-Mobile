using Hunted_Mobile.Model.Response;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class EventJsonService : JsonConversionService<EventData, Model.Response.Json.EventData> {
        public override EventData ToObject(Model.Response.Json.EventData data) {
            return new EventData {
                Message = data.message,
                TimeLeft = data.timeLeft
            };
        }
    }

    public class IntervalEventJsonService : JsonConversionService<IntervalEventData, Model.Response.Json.IntervalEventData> {
        public override IntervalEventData ToObject(Model.Response.Json.IntervalEventData data) {
            return new IntervalEventData {
                Loot = new LootJsonService().ToObjects(data.loot),
                Message = data.message,
                Players = new PlayerJsonService().ToObjects(data.users),
                TimeLeft = data.timeLeft,
                DroneActive = data.drone_is_active,
            };
        }
    }

    public class PlayerEventJsonService : JsonConversionService<PlayerEventData, Model.Response.Json.UserEventData> {
        public override PlayerEventData ToObject(Model.Response.Json.UserEventData data) {
            return new PlayerEventData {
                Player = new PlayerJsonService().ToObject(data.user),
                Message = data.message,
                TimeLeft = data.timeLeft
            };
        }
    }

    public class ScoreUpdatedEventJsonService : JsonConversionService<ScoreUpdatedEventData, Model.Response.Json.ScoreUpdatedEventData> {
        public override ScoreUpdatedEventData ToObject(Model.Response.Json.ScoreUpdatedEventData data) {
            return new ScoreUpdatedEventData {
                Message = data.message,
                PoliceScore = data.police_score,
                ThiefScore = data.thief_score,
                TimeLeft = data.timeLeft
            };
        }
    }

    public class GadgetsUpdatedEventJsonService : JsonConversionService<GadgetsUpdatedEventData, Model.Response.Json.GadgetsUpdatedEventData> {
        public override GadgetsUpdatedEventData ToObject(Model.Response.Json.GadgetsUpdatedEventData data) {
            return new GadgetsUpdatedEventData {
                Message = data.message,
                TimeLeft = data.timeLeft,
                Gadgets = new GadgetJsonService().ToObjects(data.gadgets),
                Player = new PlayerJsonService().ToObject(data.user)
            };
        }
    }
}
