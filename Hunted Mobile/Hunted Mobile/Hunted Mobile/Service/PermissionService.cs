using Android.Content.PM;

using System;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace Hunted_Mobile.Service {
    public static class PermissionService {
        private static PermissionStatus gpsPermission;
        public static bool HasGpsPermission => gpsPermission == PermissionStatus.Granted;
        public static Task AskPermissionForLocation() {
            gpsPermission = Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>().Result;

            if(!HasGpsPermission && gpsPermission != PermissionStatus.Disabled && gpsPermission != PermissionStatus.Restricted) {
                try {
                    return Permissions.RequestAsync<Permissions.LocationWhenInUse>()
                        .ContinueWith(x => {
                            gpsPermission = Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>().Result;
                        });
                }
                catch(Exception e) {

                }
            }

            return Task.FromResult(0);
        }

        public static void CheckPermissionLocation() {
            if(!HasGpsPermission) {
                noGpsPermission();
            }
        }

        private static void noGpsPermission() {
            DependencyService.Get<Toast>().Show("De app heeft geen toestemming om gebruik te maken van locatie. Zonder de locatie is het spel niet speelbaar.");
        }
    }
}
