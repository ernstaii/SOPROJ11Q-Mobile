using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Service {
    public class ConvertResponseService {
        public async static Task<JObject> ConvertJObject(HttpResponseMessage response) {
            // Check if request went successfully
            if(response.IsSuccessStatusCode) {
                var contents = await response.Content.ReadAsStringAsync();

                JObject output = null;

                try {
                    output = (JObject) JsonConvert.DeserializeObject(contents);
                }
                catch(Exception e) {
                    // Validation of Laravel is also ending up here, maybe something for later to be resolved
                    Console.WriteLine(e);
                }

                return output;
            }

            return null;
        }

        public async static Task<JArray> ConvertJArray(HttpResponseMessage response) {
            // Check if request went successfully
            if(response.IsSuccessStatusCode) {
                var contents = await response.Content.ReadAsStringAsync();

                JArray output = null;

                try {
                    output = JArray.Parse(contents);
                }
                catch(Exception e) {
                    Console.WriteLine(e);
                }

                return output;
            }

            return null;
        }

        public async static Task<string>ConvertRaw(HttpResponseMessage response){
            // Check if request went successfully
            if(response.IsSuccessStatusCode) {
                var contents = await response.Content.ReadAsStringAsync();

                return contents;
            }
            else return null;
        }
    }
}
