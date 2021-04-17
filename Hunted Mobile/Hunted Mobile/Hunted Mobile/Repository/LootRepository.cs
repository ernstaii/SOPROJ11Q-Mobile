using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;


namespace Hunted_Mobile.Repository {
    public class LootRepository {
        // Get all loot that is linked to a game
        public async Task<List<Loot>> GetAll(int gameId) {
            var response = await HttpClientService.Get($"game/{gameId}/loot");
            Console.WriteLine(response + "HAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            var result = await ConvertResponseService.ConvertJArray(response);

            var output = new List<Loot>();

            // Looping through the result
            foreach(JObject item in result) {
                var location = item.GetValue("location").ToString().Split(',');

                output.Add(new Loot((int) item.GetValue("id")) {
                    Name = item.GetValue("name").ToString(),
                    Location = new Location() {
                        Latitude = double.Parse(location[0]),
                        Longitude = double.Parse(location[1])
                    }
                });
            }

            return output;
        }
    }
}
