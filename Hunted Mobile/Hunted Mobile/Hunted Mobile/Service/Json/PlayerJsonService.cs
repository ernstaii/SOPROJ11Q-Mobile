using Hunted_Mobile.Enum;
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
                role = player.InviteKey.Role,
                triggered_alarm = player.TriggeredAlarm,
                is_fake_agent = player is FakePolice
            });
        }

        public override Player ToObject(UserData data) {
            Player player = new Player() {
                Id = data.id,
                InviteKey = new InviteKey() {
                    Role = data.role,
                    UserId = data.id
                },
                Location = new LocationJsonService().ToObjectFromCsv(data.location),
                Status = data.status,
                UserName = data.username,
                TriggeredAlarm = data.triggered_alarm,
            };

            if(data.is_fake_agent) {
                return new FakePolice(new Thief(player, data.caught_at));
            }
            else if(data.role == PlayerRole.THIEF) {
                return new Thief(player, data.caught_at);
            }
            else if(data.role == PlayerRole.POLICE) {
                return new Police(player);
            }
            else return player;
        }

        public Player ToObject(string json, InviteKey inviteKey) {
            Player player = ToObject(json);
            player.InviteKey = inviteKey;
            inviteKey.UserId = player.Id;

            if(inviteKey.Role == PlayerRole.THIEF) {
                return new Thief(
                    player, 
                    player is Thief ? ((Thief) player).CaughtAt : null
                );
            }
            else if(inviteKey.Role == PlayerRole.POLICE) {
                return new Police(player);
            }
            else throw new ArgumentException("InviteKey did not have a value for Role");
        }
    }
}
