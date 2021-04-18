using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<InviteKey> Get(string inviteCode) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"invite-key/{inviteCode}"));

            return new InviteKey() {
                Value = inviteCode,
                GameId = response.GetNumberValue("game_id"),
                Role = response.GetStringValue("role"),
                ErrorMessages = response.ErrorMessages
            };
        }
    }
}
