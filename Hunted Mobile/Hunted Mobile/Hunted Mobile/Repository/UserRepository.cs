using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using System.Collections.Generic;
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
            var result = await ConvertResponseService.Convert(response);

            return result != null ? new User((int) result.GetValue("id")) {
                Location = null,
                Name = (string) result.GetValue("name"),
                InviteKey = (string) result.GetValue("invite_key"),
                Role = (int) result.GetValue("role"),
            } : null;
        }

        // TODO: This UserStory or next?
        public async Task<User> GetAll(int gameId) {
            string url = $"http://192.168.236.189:8080/game/{gameId}/users";

            var response = await new HttpClient().GetAsync(url);
            var result = await ConvertResponseService.Convert(response);

            // TODO: Read total players inside a game
            return null;

            return result != null ? new User((int) result.GetValue("id")) {
                Location = null,
                Name = (string) result.GetValue("name"),
                InviteKey = (string) result.GetValue("invite_key"),
                Role = (int) result.GetValue("role"),
            } : null;
        }
    }
}
