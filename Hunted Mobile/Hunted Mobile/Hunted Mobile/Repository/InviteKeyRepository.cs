using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class InviteKeyRepository {
        public async Task<InviteKey> Get(string inviteCode) {
            // TODO: For test purposes only
            // inviteCode = "156M"; 
            // inviteCode = "BF3V";
            string url = $"http://192.168.236.189:8080/api/invite-key/{inviteCode}";

            var response = await new HttpClient().GetAsync(url);

            var result = await ConvertResponseService.Convert(response);

            return result != null ? new InviteKey() {
                Value = (string) result.GetValue("value"),
                GameId = (int) result.GetValue("game_id")
            } : null;
        }
    }
}
