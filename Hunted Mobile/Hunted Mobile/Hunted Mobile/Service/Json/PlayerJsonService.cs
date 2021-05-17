using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;

using System;

namespace Hunted_Mobile.Service.Json {
    public class PlayerJsonService : JsonConversionService<Player, UserData> {
        public PlayerJsonService() {
        }

        public override string ToJson(Player player) {
            return ConvertToJson(new UserData {
                id = player.Id,
                username = player.UserName,
                location = player.Location.ToCsvString(),
                status = player.Status,
                caught_at = player is Thief ? ((Thief) player).CaughtAt : null,
                role = player.InviteKey.Role
            });
        }

        protected override Player ToObject(UserData data) {
            Player player = new Player() {
                Id = data.id,
                InviteKey = new InviteKey() {
                    Role = data.role,
                    UserId = data.id
                },
                Location = new LocationJsonService().ToObjectFromCsv(data.location),
                Status = data.status,
                UserName = data.username
            };

            if(data.role == "thief") {
                var thief = new Thief(player);
                thief.CaughtAt = data.caught_at;
                return thief;
            }
            else if(data.role == "police") {
                return new Police(player);
            }
            else throw new ArgumentException("User json data did not contain role");
        }
    }
}
