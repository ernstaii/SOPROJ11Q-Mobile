using Hunted_Mobile.Model;

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.Service.Gps {
    public class GpsService {
        private const byte DEFERRAL_DISTANCE_IN_METERS = 4;

        private readonly TimeSpan deferralTime = TimeSpan.FromSeconds(14);
        private DateTime lastLocationHandle = DateTime.MinValue;

        public Location CenterOfCurrentGame { get; set; }
        public double DiameterOfCurrentGame { get; set; }

        public delegate void LocationChangedEvent(Location location);
        /// <summary>
        /// Event called whenever the GPS has started and the current device's location changes
        /// </summary>
        public event LocationChangedEvent LocationChanged;

        public GpsService(Location centerOfGame, double diameterOfGame) {
            CrossGeolocator.Current.PositionChanged += HandlerLocationChangedEvent;
            CrossGeolocator.Current.PositionError += PositionError;

            CenterOfCurrentGame = centerOfGame;
            DiameterOfCurrentGame = diameterOfGame;
        }

        private void PositionError(object sender, PositionErrorEventArgs e) {
            DependencyService.Get<Toast>().Show("GPS error: " + e.Error.ToString());
        }

        /// <summary>
        ///  Handles PositionChanged events from CrossGeolocator.Current by calling the local LocationChanged event property
        /// </summary>
        private void HandlerLocationChangedEvent(object sender, PositionEventArgs args = null) {
            var now = DateTime.Now;
            if(now.Subtract(lastLocationHandle).TotalSeconds > deferralTime.TotalSeconds * 0.75) {
                var location = new Location(args?.Position);
                if(location.DistanceToOther(CenterOfCurrentGame) < DiameterOfCurrentGame) {
                    lastLocationHandle = now;
                    LocationChanged?.Invoke(location);
                }
            }
        }

        public async Task StartGps() {
            if(!CrossGeolocator.Current.IsListening) {
                await CrossGeolocator.Current.StartListeningAsync(
                    deferralTime,
                    DEFERRAL_DISTANCE_IN_METERS,
                    false,
                    new ListenerSettings {
                        ActivityType = ActivityType.Fitness,
                        AllowBackgroundUpdates = false,
                        DeferLocationUpdates = true,
                        DeferralDistanceMeters = DEFERRAL_DISTANCE_IN_METERS,
                        DeferralTime = deferralTime,
                        ListenForSignificantChanges = false,
                        PauseLocationUpdatesAutomatically = true
                    }
                );
            }
        }

        public bool GpsHasStarted() {
            return CrossGeolocator.Current.IsListening;
        }
    }
}
