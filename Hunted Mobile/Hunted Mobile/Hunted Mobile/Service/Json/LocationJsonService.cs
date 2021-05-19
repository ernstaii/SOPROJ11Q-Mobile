using Hunted_Mobile.Model;
using Hunted_Mobile.Model.Response.Json;

using System;

using Xamarin.Forms;

namespace Hunted_Mobile.Service.Json {
    public class LocationJsonService : JsonConversionService<Location, LocationData> {
        public LocationJsonService() {
        }

        public override string ToJson(Location location) {
            return location.ToCsvString();
        }

        public Location ToObjectFromCsv(string commaSeparatedValues) {
            if(string.IsNullOrEmpty(commaSeparatedValues)) {
                return new Location();
            }
            else try {
                string[] split = commaSeparatedValues.Split(',');
                double latitude = double.Parse(split[0]);
                double longitude = double.Parse(split[1]);

                return new Location(latitude, longitude);
            }
            catch(Exception) {
                DependencyService.Get<Toast>().Show("Er was een probleem met het splitsen van de locatie variabelen");

                return new Location();
            }
        }

        public override Location ToObject(LocationData data) {
            return ToObjectFromCsv(data.location);
        }
    }
}
