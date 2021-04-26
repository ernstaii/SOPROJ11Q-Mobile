using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<List<InviteKey>> GetAll(string inviteCode) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"invite-keys/{inviteCode}"));

            List<InviteKey> result = new List<InviteKey>();

            try {
                result.Add(new InviteKey() {
                    Value = inviteCode,
                    GameId = response.GetNumberValue("game_id"),
                    UserId = 0,
                    Role = response.GetStringValue("role").ToString(),
                    ErrorMessages = response.ErrorMessages
                });
            }
            catch(Exception) {
                result.Add(new InviteKey() {
                    Value = inviteCode,
                    GameId = 0,
                    UserId = 0,
                    Role = null,
                    ErrorMessages = response.ErrorMessages.Count() > 0 ? response.ErrorMessages : new Dictionary<string, string>() {
                        { "value", "Er is iets misgegaan"}
                    }
                });
            }

            return result;
        }
    }
}
