using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class UserRepository {
        public async Task<User> Create(InviteKey inviteKey, string username) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("users", new {
                username = username,
                invite_key = inviteKey.Value,
                role = inviteKey.Role,
                game_id = inviteKey.GameId
            }));

            return new User(response.GetNumberValue("id")) {
                Location = null,
                UserName = username,
                InviteKey = inviteKey,
                Role = response.GetStringValue("role"),
                ErrorMessages = response.ErrorMessages,
            };
        }

        // Get all users that are linked to a game
        public async Task<List<User>> GetAll(int gameId) {
            var response = new HttpClientResponse() {
                HasMultipleResults = true,
            };
            await response.Convert(HttpClientRequestService.GetAll($"game/{gameId}/users"));

            var output = new List<User>();

            // Looping through the result
            foreach(JObject item in response.Items) {
                string role = item.GetValue("role")?.ToString();

                try {
                    output.Add(new User((int) item.GetValue("id")) {
                        UserName = item.GetValue("username").ToString(),
                        Location = null,
                        InviteKey = new InviteKey() {
                            GameId = gameId,
                            Role = role,
                            Value = item.GetValue("invite_key").ToString()
                        },
                        Role = role,
                        GameId = gameId,
                    });
                }
                catch(Exception e) {
                    Console.WriteLine(e.ToString());
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
