using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GameRepository {
        public async Task<Game> GetGame(int gameId) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"games/{gameId}"));

            return new Game() {
                Id = response.GetNumberValue("id"),
                Duration = response.GetNumberValue("duration"),
                Interval = response.GetNumberValue("interval"),
                TimeLeft = response.GetNumberValue("time_left"),
                Status = response.GetStringValue("status"),
                ThievesScore = response.GetNumberValue("thieves_score"),
                PoliceScore = response.GetNumberValue("police_score"),
            };
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
