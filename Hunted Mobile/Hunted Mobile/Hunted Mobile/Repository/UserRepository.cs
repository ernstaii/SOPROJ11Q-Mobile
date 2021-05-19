using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Json;

using System;
using System.Collections.Generic;
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

            Player newPlayer = new PlayerJsonService().ToObject(response.ResponseContent, player.InviteKey);
            newPlayer.ErrorMessages = response.ErrorMessages;

            return newPlayer;
        }

        // Get all users that are linked to a game
        public async Task<List<Player>> GetAll(int gameId) {
            var usersResponse = new HttpClientResponse() {
                HasMultipleResults = true,
            };

            await usersResponse.Convert(HttpClientRequestService.GetAll($"games/{gameId}/users-with-role"));

            var result = new List<Player>(
                new PlayerJsonService().ToObjects(usersResponse.ResponseContent)
            );

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
