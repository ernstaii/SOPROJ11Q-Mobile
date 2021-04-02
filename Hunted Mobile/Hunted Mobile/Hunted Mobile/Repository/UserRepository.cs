using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class UserRepository {
        public async Task<User> Create(string inviteKey, string username) {
            string url = $"http://192.168.236.189:8080/api/users";

            // Prepare parameters inside List
            // TODO: Should location be submitted as well?
            var content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("invite_key", inviteKey),
                });

            var response = await new HttpClient().PostAsync(url, content);
            var result = await ConvertResponseService.ConvertJObject(response);

            return result != null ? new User((int) result.GetValue("id")) {
                Location = null,
                Name = (string) result.GetValue("name"),
                InviteKey = (string) result.GetValue("invite_key"),
                Role = (string) result.GetValue("role"),
            } : null;
        }

        // Get all users that are linked to a game
        // TODO: Should location be submitted as well?
        public async Task<List<User>> GetAll(int gameId) {
            string url = $"http://192.168.236.189:8080/api/game/{gameId}/users";

            var response = await new HttpClient().GetAsync(url);
            var result = await ConvertResponseService.ConvertJArray(response);

            var output = new List<User>();

            // Looping through the result
            foreach(JObject item in result) {
                output.Add(new User((int) item.GetValue("id")) {
                    Name = item.GetValue("username").ToString(),
                    Location = null,
                    InviteKey = item.GetValue("invite_key").ToString(),
                    Role = item.GetValue("role").ToString(),
                });
            }

            return output;
        }
    }
}
