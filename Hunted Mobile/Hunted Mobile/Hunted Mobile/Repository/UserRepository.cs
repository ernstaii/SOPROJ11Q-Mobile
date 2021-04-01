using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class UserRepository {
        public async Task<User> Create(InviteKey inviteKey) {
            string url = $"http://192.168.42.182:8000/users";

            // Prepare parameters inside List
            var content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("name", "value"),
                    new KeyValuePair<string, string>("invite_key", "value"),
                    new KeyValuePair<string, string>("role", "value"),
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

        public async Task<User> GetAll(int gameId) {
            string url = $"http://192.168.42.182:8000/game/{gameId}/users";

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
