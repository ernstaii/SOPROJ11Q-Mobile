using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.Repository {
    public class UserRepository {
        public async Task<Player> Create(Player player) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("users", new {
                username = player.UserName,
                invite_key = player.InviteKey.Value,
            }));

            var responseLoc = response.GetStringValue("location");

            Type t = typeof(Thief);

            Player newUser = new Player() {
                Id = response.GetNumberValue("id"),
                InviteKey = player.InviteKey,
                UserName = player.UserName,
                ErrorMessages = response.ErrorMessages,
                Location = string.IsNullOrWhiteSpace(responseLoc) ? null : new Location(responseLoc)
            };
            newUser.InviteKey.UserId = newUser.Id;

            if(newUser.InviteKey.Role == PlayerRole.THIEF) return new Thief(newUser);
            else if(newUser.InviteKey.Role == PlayerRole.POLICE) return new Police(newUser);
            else return newUser;
        }

        // Get all users that are linked to a game
        public async Task<List<Player>> GetAll(int gameId) {
            var usersResponse = new HttpClientResponse() {
                HasMultipleResults = true,
            };

            await usersResponse.Convert(HttpClientRequestService.GetAll($"games/{gameId}/users-with-role"));

            var result = new List<Player>();

            // Looping through the result
            foreach(JObject item in usersResponse.Items) {
                try {
                    string role = item.GetValue("role")?.ToString();
                    int userId = (int) item.GetValue("id");

                    Player user = new Player() {
                        Id = userId,
                        UserName = item.GetValue("username")?.ToString(),
                        Location = new Location(item.GetValue("location")?.ToString()),
                        CaughtAt = item.GetValue("caught_at")?.ToString(),
                        Status = item.GetValue("status")?.ToString(),
                        InviteKey = new InviteKey() {
                            Role = role
                        },
                    };

                    if(role == PlayerRole.THIEF) result.Add(new Thief(user));
                    else if(role == PlayerRole.POLICE) result.Add(new Police(user));
                    else result.Add(user);
                }
                catch(Exception ex) {
                    DependencyService.Get<Toast>().Show("Er was een probleem met het ophalen van de spelers");
                }
            }

            return result;
        }

        public async Task<bool> CatchThief(int userId) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch($"users/{userId}/catch"));

            return response.IsSuccessful;
        }

        public async Task<bool> Update(int userId, Location location) {
            var response = new HttpClientResponse();

            await response.Convert(HttpClientRequestService.Put($"users/{userId}", new {
                location = location.ToCsvString()
            }));

            return response.ResponseContent != null;
        }

        public async Task<Player> GetUser(int userId, int gameId) {
            HttpClientResponse response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"users/{userId}"));

            if(response.Status == System.Net.HttpStatusCode.NotFound) return null;

            var role = response.GetStringValue("role");
            var user = new Player() {
                Id = userId,
                UserName = response.GetStringValue("username"),
                Location = new Location(response.GetStringValue("location")),
                CaughtAt = response.GetStringValue("caught_at"),
                Status = response.GetStringValue("status"),
                InviteKey = new InviteKey() {
                    Role = role,
                    GameId = gameId,
                },
            };

            if(role == PlayerRole.THIEF) return new Thief(user);
            else if(role == PlayerRole.POLICE) return new Police(user);
            return user;
        }
    }
}
