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

            string[] location = response.Item.GetValue("police_station_location").ToString().Split(',');
            return new Game() {
                Id = response.GetNumberValue("id"),
                Duration = response.GetNumberValue("duration"),
                Interval = response.GetNumberValue("interval"),
                TimeLeft = response.GetNumberValue("time_left"),
                Status = response.GetStringValue("status"),
                PoliceStationLocation = new Location() {
                    Latitude = double.Parse(location[0]),
                    Longitude = double.Parse(location[1])
                },
            };
        }
    }
}
