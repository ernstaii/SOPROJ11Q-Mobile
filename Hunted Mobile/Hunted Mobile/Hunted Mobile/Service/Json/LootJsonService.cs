using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class LootJsonService : JsonConversionService<Loot, LootData> {
        public override string ToJson(Loot @object) {
            throw new NotImplementedException();
        }

        protected override Loot ToObject(LootData data) {
            return new Loot(data.id) {
                Location = new LocationJsonService().ToObjectFromCsv(data.location),
                Name = data.name
            };
        }
    }
}
