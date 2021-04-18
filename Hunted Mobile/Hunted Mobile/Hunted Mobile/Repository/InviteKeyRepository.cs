using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<List<InviteKey>> GetAll(string inviteCode) {
            var response = new HttpClientResponse() {
                HasMultipleResults = true,
            };

            await response.Convert(HttpClientRequestService.GetAll($"invite-key/{inviteCode}"));

            List<InviteKey> result = new List<InviteKey>();

            try {
                if(!response.IsSuccessful) {
                    result.Add(new InviteKey() {
                        Value = inviteCode,
                        GameId = 0,
                        Role = null,
                        ErrorMessages = response.ErrorMessages
                    });
                }
                else {
                    foreach(JObject item in response.Items) {
                        result.Add(new InviteKey() {
                            GameId = (int) item.GetValue("game_id"),
                            Role = item.GetValue("role").ToString(),
                            Value = item.GetValue("value").ToString()
                        });
                    }
                }
            }
            catch(Exception e) {
            }

            return result;
        }
    }
}
