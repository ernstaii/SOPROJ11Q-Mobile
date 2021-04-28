using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository
{
    public class BorderMarkerRepository {
        public async Task<List<Location>> GetAll(int gameId) {
            var response = new HttpClientResponse() {
                HasMultipleResults = true,
            };
            await response.Convert(HttpClientRequestService.GetAll($"games/{gameId}/border-markers"));

            List<Location> result = new List<Location>();

            foreach(JObject item in response.Items) {
                try {
                    result.Add(new Location(item.GetValue("location").ToString()));
                }
                catch(Exception) {
                    result.Add(new Location("5.000000,51.000000"));
                }
            }


            return result;
        }
    }
}
