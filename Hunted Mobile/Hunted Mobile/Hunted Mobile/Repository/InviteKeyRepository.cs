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

            await response.Convert(HttpClientRequestService.Get($"invite-keys/{inviteCode}"));

            List<InviteKey> result = new List<InviteKey>();
            result.Add(new InviteKey() {
                Value = inviteCode,
                GameId = (int) response.GetNumberValue("game_id"),
                Role = response.GetStringValue("role").ToString(),
                ErrorMessages = response.ErrorMessages
            });

            return result;
        }
    }
}
