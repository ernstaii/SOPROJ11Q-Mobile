using Hunted_Mobile.Model;
using Hunted_Mobile.Model.Response.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class InviteKeyJsonService : JsonConversionService<InviteKey, InviteKeyData> {
        protected override InviteKey ToObject(InviteKeyData data) {
            return new InviteKey() {
                GameId = data.game_id,
                Role = data.role,
                UserId = data.user_id,
                Value = data.value,
            };
        }
    }
}
