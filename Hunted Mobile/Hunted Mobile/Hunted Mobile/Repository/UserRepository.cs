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

            User user = new User(
                response.GetNumberValue("id"),
                username,
                inviteKey
            ) {
                ErrorMessages = response.ErrorMessages,
                Location = string.IsNullOrWhiteSpace(responseLoc) ? null : new Location(responseLoc)
            };

            inviteKey.UserId = user.Id;

            if(inviteKey?.Role == "thief") {
                return new Thief(user);
            }
            else if(inviteKey?.Role == "police") {
                return new Police(user);
            }
            else return user;
        }

        // Get all users that are linked to a game
        public async Task<List<User>> GetAll(int gameId, InviteKeyRepository inviteKeyRepository) {
            var usersResponse = new HttpClientResponse() {
                HasMultipleResults = true,
            };

            await usersResponse.Convert(HttpClientRequestService.GetAll($"games/{gameId}/users-with-role"));

            var inviteKeys = await inviteKeyRepository.GetAll(gameId);

            var result = new List<User>();

            // Looping through the result
            foreach(JObject item in usersResponse.Items) {
                try {
                    int userId = (int) item.GetValue("id");

                    InviteKey inviteKey = inviteKeys.Find((key) => { return key.UserId == userId; });

                    User user = new User(userId, item.GetValue("username")?.ToString(), inviteKey);
                    user.Location = new Location(item.GetValue("location")?.ToString());

                    if(inviteKey?.Role == "thief") {
                        result.Add(new Thief(user));
                    }
                    else if(inviteKey?.Role == "police") {
                        result.Add(new Police(user));
                    }
                    else result.Add(user);
                }
                catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
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
