using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hunted_Mobile.Service.Json {
    public class ConvertToJsonService {

        private ConvertToJsonService() {
        }

        protected string ConvertToJson(DataModel dataModel) {
            return JsonConvert.SerializeObject(dataModel);
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
    }
}
