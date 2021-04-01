using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<InviteKey> Get(string inviteCode) {
            // TODO: For test purposes only
            // inviteCode = "156M"; 
            string url = $"http://192.168.42.182:8000/api/invite-key/{inviteCode}";

            var response = await new HttpClient().GetAsync(url);

            var result = await ConvertResponseService.Convert(response);

            return result != null ? new InviteKey() {
                Value = (string) result.GetValue("value"),
                GameId = (int) result.GetValue("game_id")
            } : null;
        }
    }
}
