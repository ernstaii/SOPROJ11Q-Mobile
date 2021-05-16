using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Hunted_Mobile.Service.Json {
    public class ConvertFromJsonService {
        public string Json { get; }

        public ConvertFromJsonService(string json) {
            Json = json;
        }

        protected T ConvertFromJson<T>() where T : DataModel {
            return JsonConvert.DeserializeObject<T>(Json);
        }

        protected JArray ToJArray() {
            return JArray.Parse(Json);
        }

        protected JObject ToJObject() {
            return JsonConvert.DeserializeObject<JObject>(Json);
        }

        public ReadOnlyCollection<string> ToArray() {
            List<string> items = new List<string>();
            foreach(var item in ToJArray()) {
                items.Add(item.ToString());
            }
            return items.AsReadOnly();
        }

        public Player ToPlayer() {
            UserData data = ConvertFromJson<UserData>();
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
