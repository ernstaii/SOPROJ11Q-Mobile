using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Service.Builder;

using System;

namespace Hunted_Mobile.Service.Json {
    public class PlayerJsonService : JsonConversionService<PlayerBuilder, UserData> {
        public PlayerJsonService() {
        }

        public override string ToJson(PlayerBuilder builder) {
            return ConvertToJson(new UserData {
                id = builder.Id,
                username = builder.UserName,
                location = builder.Location.ToCsvString(),
                status = builder.Status,
                caught_at = builder.CaughtAt,
                role = builder.InviteKey.Role,
                triggered_alarm = builder.TriggeredAlarm
            });
        }

        public override PlayerBuilder ToObject(UserData data) {
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
                .SetCaughtAt(data.caught_at);
        }

        public PlayerBuilder ToObject(string json, InviteKey inviteKey) {
            PlayerBuilder builder = ToObject(json);
            builder.SetInviteKey(inviteKey);
            inviteKey.UserId = builder.Id;

            return builder;
        }
    }
}
