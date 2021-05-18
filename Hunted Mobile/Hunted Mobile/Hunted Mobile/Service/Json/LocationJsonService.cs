using Hunted_Mobile.Model;
using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json.Linq;

using System;

namespace Hunted_Mobile.Service.Json {
    public class LocationJsonService : JsonConversionService<Location, LocationData> {
        public LocationJsonService() {
        }

        public override string ToJson(Location location) {
            return location.ToCsvString();
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

        protected override Location ToObject(LocationData data) {
            return ToObjectFromCsv(data.location);
        }
    }
}
