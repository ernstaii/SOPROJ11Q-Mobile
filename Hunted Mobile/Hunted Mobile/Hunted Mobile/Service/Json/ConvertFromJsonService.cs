using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hunted_Mobile.Service.Json {
    public class ConvertFromJsonService {
        public string Json { get; }

        public ConvertFromJsonService(string json) {
            Json = json;
        }

        protected T ConvertFromJson<T>() where T : JsonResponseData {
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
            return ToPlayer(data);
        }

        protected Player ToPlayer(UserData data) {
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

        protected Player[] ToPlayer(UserData[] data) {
            Player[] players = new Player[data.Length];
            for(int i = 0; i < players.Length; i++) {
                players[i] = ToPlayer(data[i]);
            }
            return players;
        }

        public Location ToLocation() {
            string locationData = ToJObject().GetValue("location")?.ToString();
            return new Location(locationData);
        }

        public InviteKey ToInviteKey() {
            InviteKeyData data = ConvertFromJson<InviteKeyData>();
            return new InviteKey() {
                GameId = data.game_id,
                Role = data.role,
                UserId = data.user_id,
                Value = data.value,
            };
        }

        public Loot ToLoot() {
            LootData data = ConvertFromJson<LootData>();
            return ToLoot(data);
        }

        protected Loot ToLoot(LootData data) {
            return new Loot(data.id) {
                Location = new Location(data.location),
                Name = data.name
            };
        }

        protected Loot[] ToLoot(LootData[] data) {
            Loot[] loot = new Loot[data.Length];
            for(int i = 0; i < loot.Length; i++) {
                loot[i] = ToLoot(data[i]);
            }
            return loot;
        }

        public Model.Response.IntervalEventData ToIntervalEvent() {
            IntervalEventData data = ConvertFromJson<IntervalEventData>();
            return new Model.Response.IntervalEventData() {
                Loot = ToLoot(data.loot),
                Message = data.message,
                Players = ToPlayer(data.users)
            };
        }
    }
}
