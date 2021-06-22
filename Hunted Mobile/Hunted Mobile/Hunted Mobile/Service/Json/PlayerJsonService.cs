using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Service.Builder;

using System;
using System.Collections.Generic;

namespace Hunted_Mobile.Service.Json {
    public class PlayerJsonService : JsonConversionService<PlayerBuilder, UserData> {
        public PlayerJsonService() {
        }

        public override PlayerBuilder ToObject(UserData data) {
            var allGadgets = new List<Gadget>();
            foreach(var gadgets in new GadgetJsonService().ToObjects(data.gadgets?.ToString())) {
                foreach(var gadget in gadgets) {
                    allGadgets.Add(gadget);
                }
            }

            return new PlayerBuilder()
                .SetId(data.id)
                .SetInviteKey(new InviteKey() {
                    Role = data.role,
                    UserId = data.id
                })
                .SetLocation(new LocationJsonService().ToObjectFromCsv(data.location))
                .SetStatus(data.status)
                .SetUsername(data.username)
                .SetTriggeredAlarm(data.triggered_alarm)
                .SetCaughtAt(data.caught_at)
                .SetFakePolice(data.is_fake_agent)
                .SetGadgets(allGadgets);
        }

        public PlayerBuilder ToObject(string json, InviteKey inviteKey) {
            PlayerBuilder builder = ToObject(json);
            builder.SetInviteKey(inviteKey);
            inviteKey.UserId = builder.Id;

            return builder;
        }
    }
}
