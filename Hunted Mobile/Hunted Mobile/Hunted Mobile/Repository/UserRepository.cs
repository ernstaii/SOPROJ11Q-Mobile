using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class UserRepository {
        public async Task<User> Create(InviteKey inviteKey, string username) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("users", new {
                username = username,
                invite_key = inviteKey.Value,
            }));

            var responseLoc = response.GetStringValue("location");

            return new User((int) response.GetNumberValue("id")) {
                UserName = username,
                InviteKey = inviteKey,
                ErrorMessages = response.ErrorMessages,
                Location = string.IsNullOrWhiteSpace(responseLoc) ? null : new Location(responseLoc)
            };
        }

        // Get all users that are linked to a game
        public async Task<List<User>> GetAll(int gameId) {
            var response = new HttpClientResponse() {
                HasMultipleResults = true,
            };
            await response.Convert(HttpClientRequestService.GetAll($"games/{gameId}/users-with-role"));

            var output = new List<User>();

            // Looping through the result
            foreach(JObject item in response.Items) {
                try {
                    output.Add(new User((int) item.GetValue("id")) {
                        UserName = item.GetValue("username")?.ToString(),
                        Role = item.GetValue("role")?.ToString() ?? "thief",
                        Location = new Location(item.GetValue("location")?.ToString()),
                        CaughtAt = item.GetValue("caught_at")?.ToString(),
                        Status = item.GetValue("status")?.ToString(),
                    });
                    ;
                }
                catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }

            return output;
        }

        public async Task<bool> Update(int userId, Location location) {
            var response = new HttpClientResponse();

            await response.Convert(HttpClientRequestService.Update($"users/{userId}", new {
                location = location.ToCsvString()
            }));

            return response.ResponseContent != null;
        }
    }
}
