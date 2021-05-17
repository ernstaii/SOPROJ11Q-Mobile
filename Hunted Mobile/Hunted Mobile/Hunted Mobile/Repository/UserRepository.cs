using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Json;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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

            if(newUser.InviteKey.Role == "thief") return new Thief(newUser);
            else if(newUser.InviteKey.Role == "police") return new Police(newUser);
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
            foreach(var userJson in new ConvertFromJsonService(usersResponse.ResponseContent).ToArray()) {
                result.Add(new ConvertFromJsonService(userJson).ToPlayer());
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
    }
}
