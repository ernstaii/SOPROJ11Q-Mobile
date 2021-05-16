using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hunted_Mobile.Service {
    public class JsonConversionService {

        public JsonConversionService() {

        }

        protected T ConvertFromJson<T>(string json) where T : DataModel {
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected string ConvertToJson(DataModel dataModel) {
            return JsonConvert.SerializeObject(dataModel);
        }

        protected JArray ToJArray(string json) {
            return JArray.Parse(json);
        }

        protected JObject ToJObject(string json) {
            return JsonConvert.DeserializeObject<JObject>(json);
        }

        public ReadOnlyCollection<string> ToArray(string json) {
            List<string> items = new List<string>();
            foreach(var item in ToJArray(json)) {
                items.Add(item.ToString());
            }
            return items.AsReadOnly();
        }

        public string ConvertToJson(Player player) {
            return ConvertToJson(new UserData {
                id = player.Id,
                username = player.UserName,
                location = player.Location.ToCsvString(),
                status = player.Status,
                caught_at = player is Thief ? ((Thief) player).CaughtAt : null,
                role = player.InviteKey.Role
            });
        }

        public Player ConvertUserFromJson(string json) {
            UserData data = ConvertFromJson<UserData>(json);
            Player player = new Player() {
                Id = data.id,
                InviteKey = new InviteKey() {
                    Role = data.role,
                    UserId = data.id
                },
                Location = new Location(data.location),
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
