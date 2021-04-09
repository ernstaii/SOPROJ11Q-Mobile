using Hunted_Mobile.Model;

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Gps {
    public class GpsService {
        public delegate void LocationChangedEvent(Location location);
        /// <summary>
        /// Event called whenever the GPS has started and the current device's location changes
        /// </summary>
        public event LocationChangedEvent LocationChanged;

        public GpsService() {
            CrossGeolocator.Current.PositionChanged += HandlerLocationChangedEvent;
        }

        /// <summary>
        ///  Handles PositionChanged events from CrossGeolocator.Current by calling the local LocationChanged event property
        /// </summary>
        private void HandlerLocationChangedEvent(object sender, PositionEventArgs args) {
            LocationChanged(new Location(args.Position));
        }

        public async void StartGps() {
            //TODO settings may need to change
            await CrossGeolocator.Current.StartListeningAsync(
                TimeSpan.FromSeconds(1),
                1,
                false,
                new ListenerSettings {
                    ActivityType = ActivityType.Fitness,
                    AllowBackgroundUpdates = false,
                    DeferLocationUpdates = true,
                    DeferralDistanceMeters = 1,
                    DeferralTime = TimeSpan.FromSeconds(5),
                    ListenForSignificantChanges = false,
                    PauseLocationUpdatesAutomatically = true
                }
            );
        }

        public bool GpsHasStarted() {
            return CrossGeolocator.Current.IsListening;
        }
    }
}
