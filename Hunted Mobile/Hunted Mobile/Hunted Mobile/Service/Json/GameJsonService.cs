using Hunted_Mobile.Model;
using Hunted_Mobile.Model.Response.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class GameJsonService : JsonConversionService<Game, GameData> {
        public override Game ToObject(GameData data) {
            return new Game() {
                Duration = data.duration,
                Id = data.id,
                Interval = data.interval,
                PoliceScore = data.police_score,
                PoliceStationLocation = new LocationJsonService().ToObjectFromCsv(data.police_station_location),
                StartTime = data.started_at,
                Status = data.status,
                ThievesScore = data.thieves_score,
                TimeLeft = data.time_left,
                EndTime = DateTime.Now.AddSeconds(data.time_left)
            };
        }
    }
}
