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
        public async Task<User> Create(InviteKey inviteKey, string username) {
            // Prepare parameters inside List
            var content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("invite_key", inviteKey.Value),
                    new KeyValuePair<string, string>("role", inviteKey.Role)
                });

            var response = await new HttpClient().PostAsync(HttpClientService.GetUrl("users"), content);
            var result = await ConvertResponseService.ConvertJObject(response);

            return result != null ? new User((int) result.GetValue("id")) {
                Location = null,
                Name = (string) result.GetValue("name"),
                InviteKey = inviteKey,
                Role = (string) result.GetValue("role"),
            } : null;
        }

        // Get all users that are linked to a game
        public async Task<List<User>> GetAll(int gameId) {
            var response = await new HttpClient().GetAsync(HttpClientService.GetUrl($"game/{gameId}/users"));
            var result = await ConvertResponseService.ConvertJArray(response);

            var output = new List<User>();

            // Looping through the result
            foreach(JObject item in result) {
                string role = item.GetValue("role").ToString();

                output.Add(new User((int) item.GetValue("id")) {
                    Name = item.GetValue("username").ToString(),
                    Location = null,
                    InviteKey = new InviteKey() {
                        GameId = gameId,
                        Role = role,
                        Value = item.GetValue("invite_key").ToString()
                    },
                    Role = role,
                });
            }

            return output;
        }
    }
}
