using Hunted_Mobile.Model;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Json;

using System;
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

        public async Task<string> GetLogoUrl(int gameId) {
            /*HttpClientResponse response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"games/{gameId}/logo"));*/

            /*string test = "";

            try {
                test = response.ResponseContent;
            }
            catch(Exception e) {

            }*/

            return HttpClientRequestService.GetUrl($"games/{gameId}/logo");
        }

        public async Task<bool> UpdatePoliceScore(int gameId, int score) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch($"games/{gameId}/police-score/{score}"));

            return response.IsSuccessful;
        }
    }
}
