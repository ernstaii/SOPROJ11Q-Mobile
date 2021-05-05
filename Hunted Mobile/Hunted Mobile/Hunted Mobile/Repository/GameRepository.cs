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
            HttpClientResponse response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"games/{gameId}"));

            return new Game() {
                Id = response.GetNumberValue("id"),
                Duration = response.GetNumberValue("duration"),
                Interval = response.GetNumberValue("interval"),
                EndTime = DateTime.Now.AddSeconds(response.GetNumberValue("time_left")),
                Status = response.GetStringValue("status"),
                PoliceStationLocation = new Location(
                    response.GetStringValue("police_station_location")
                )
            };
        }

        public async Task<int> GetTimeLeftOfGame(int gameId) {
            HttpClientResponse response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"games/{gameId}/time_left"));

            return response.GetNumberValue("time_left");
        }
    }
}
