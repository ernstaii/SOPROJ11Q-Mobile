using Hunted_Mobile.Model;
using Hunted_Mobile.Model.Response.Json;

using System;

namespace Hunted_Mobile.Service.Json {
    public class LocationJsonService : JsonConversionService<Location, JsonResponseData> {
        public LocationJsonService() {
        }

        public override string ToJson(Location location) {
            return location.ToCsvString();
        }

        public override Location ToObject(string json) {
            return ToObjectFromCsv(
                ToJObject(json).GetValue("location")?.ToString()
            );
        }

        public Location ToObjectFromCsv(string commaSeparatedValues) {
            try {
                string[] split = commaSeparatedValues.Split(',');
                double latitude = double.Parse(split[0]);
                double longitude = double.Parse(split[1]);

                return new Location(latitude, longitude);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message + " occurred while parsing comma separated location: " + commaSeparatedValues);

                return new Location();
            }
        }

        protected override Location ToObject(JsonResponseData data) {
            throw new NotImplementedException();
        }
    }
}
