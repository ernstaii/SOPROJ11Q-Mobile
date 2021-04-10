using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<InviteKey> Get(string inviteCode) {
            var response = await new HttpClient().GetAsync(HttpClientService.GetUrl($"invite-key/{inviteCode}"));
            var result = await ConvertResponseService.ConvertJObject(response);

            return result != null ? new InviteKey() {
                Value = (string) result.GetValue("value"),
                GameId = (int) result.GetValue("game_id"),
                Role = (string) result.GetValue("role")
            } : null;
        }
    }
}
