using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class UserRepository {
        public async Task<User> Create(InviteKey inviteKey, string username) {
            // Prepare parameters inside List
            var response = await HttpClientService.Create("users", new {
                username = username,
                invite_key = inviteKey.Value,
                role = inviteKey.Role
            });

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
            var response = await HttpClientService.GetAll($"game/{gameId}/users");
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
