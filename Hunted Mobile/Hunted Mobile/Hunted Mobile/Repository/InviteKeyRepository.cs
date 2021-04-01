using Hunted_Mobile.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<InviteKey> Get(string inviteCode) {
            // TODO: For test purposes only
            // inviteCode = "156M"; 
            string url = $"http://192.168.42.182:8000/api/invite-key/{inviteCode}";

            var response = await new HttpClient().GetAsync(url);

            // Check if request went successfully
            if(response.IsSuccessStatusCode) {

                // Content of responses
                var contents = await response.Content.ReadAsStringAsync();

                // Convert to JObject
                var result = (JObject) JsonConvert.DeserializeObject(contents);

                return result != null ? new InviteKey() {
                    Value = (string) result.GetValue("value"),
                    GameId = (int) result.GetValue("game_id")
                } : null;
            }

            return null;
        }
    }
}
