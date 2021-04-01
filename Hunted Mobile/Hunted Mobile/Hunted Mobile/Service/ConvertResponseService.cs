using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Service {
    public class ConvertResponseService {
        public async static Task<JObject> Convert(HttpResponseMessage response) {
            // Check if request went successfully
            if(response.IsSuccessStatusCode) {
                var contents = await response.Content.ReadAsStringAsync();

                // Convert to JObject
                return (JObject) JsonConvert.DeserializeObject(contents);
            }

            return null;
        }
    }
}
