using Hunted_Mobile.Model;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Json;

using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GameRepository {
        public async Task<Game> GetGame(int gameId) {
            HttpClientResponse response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"games/{gameId}"));

            return new GameJsonService().ToObject(response.ResponseContent);
        }

        public async Task<bool> UpdateThievesScore(int gameId, int score) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch($"games/{gameId}/thieves-score/{score}"));

            return response.IsSuccessful;
        }

        public async Task<bool> UpdatePoliceScore(int gameId, int score) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch($"games/{gameId}/police-score/{score}"));

            return response.IsSuccessful;
        }
    }
}
