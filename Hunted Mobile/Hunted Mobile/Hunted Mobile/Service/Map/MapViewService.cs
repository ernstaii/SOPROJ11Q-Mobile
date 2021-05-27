using Hunted_Mobile.Model;

using MapsuiPosition = Mapsui.UI.Forms.Position;
using Mapsui.UI.Forms;
using Hunted_Mobile.Model.GameModels;
using Xamarin.Forms;
using System;
using Hunted_Mobile.Model.Resource;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Enum;

namespace Hunted_Mobile.Service.Map {
    public class MapViewService {
        const string LOOT_TAG = "loot",
            THIEF_TAG = PlayerRole.THIEF;

        private readonly Color policePinColor = Xamarin.Forms.Color.FromRgb(39, 96, 203),
            thiefPinColor = Xamarin.Forms.Color.Black;
        private readonly Resource policeBadgeIcon;
        private readonly Resource moneyBagIcon;
        private readonly Player player;
        private Pin playerPin;

        public MapView MapView { get; set; }

        public MapViewService(MapView mapView, Player player) {
            MapView = mapView;
            this.player = player;
            SetPlayerPin();

            // Enableling this buttons is something for a different PR
            DisableDefaultMapViewOptions();

            policeBadgeIcon = UnitOfWork.Instance.ResourceRepository.GetMapImage("police-badge.png");
            moneyBagIcon = UnitOfWork.Instance.ResourceRepository.GetMapImage("money-bag.png");
        }

        public void UpdatePlayerPinLocation(Location location) {
            playerPin.Position = new MapsuiPosition(location.Latitude, location.Longitude);
            AddPlayerPin();
        }

        public void AddPlayerPin() {
            if(!MapView.Pins.Contains(playerPin)) {
                MapView.Pins.Add(playerPin);
            }
        }

        /// <summary>
        /// This methode will only display the user if the person is in the same team as the player
        /// </summary>
        /// <param name="player"></param>
        public void AddTeamMatePin(Player player) {
            if(this.player.GetType() == player.GetType()) {
                MapView.Pins.Add(new Pin(MapView) {
                    Label = player.UserName,
                    Color = player is Thief ? thiefPinColor : policePinColor,
                    Position = new MapsuiPosition(player.Location.Latitude, player.Location.Longitude),
                    Scale = 0.666f,
                    Tag = player is Thief ? THIEF_TAG : null,
                    Transparency = 0.25f,
                });
            }
        }

        public void AddClosestThiefPin(Player thief) {
            MapView.Pins.Add(new Pin(MapView) {
                Label = thief.UserName,
                Color = Xamarin.Forms.Color.Black,
                Position = new Mapsui.UI.Forms.Position(thief.Location.Latitude, thief.Location.Longitude),
                Scale = 0.666f,
                Tag = THIEF_TAG,
                Transparency = 0.25f,
            });
        }

        public void AddLootPin(Loot loot) {
            MapView.Pins.Add(new Pin(MapView) {
                Label = loot.Name,
                Position = new MapsuiPosition(loot.Location.Latitude, loot.Location.Longitude),
                Scale = 1.0f,
                Tag = LOOT_TAG,
                Icon = moneyBagIcon.Data,
                Type = PinType.Icon,
            });
        }

        public void AddPoliceStationPin(Location policeStationLocation) {
            if(policeStationLocation != null) {
                MapView.Pins.Add(new Pin(MapView) {
                    Label = "Politie station",
                    Position = new MapsuiPosition(policeStationLocation.Latitude, policeStationLocation.Longitude),
                    Scale = 1.0f,
                    Icon = policeBadgeIcon.Data,
                    Type = PinType.Icon,
                });
            }
        }

        private void SetPlayerPin() {
            playerPin = new Pin(MapView) {
                Label = player.UserName,
                Color = player is Thief ? thiefPinColor : policePinColor,
            };
        }

        private void DisableDefaultMapViewOptions() {
            MapView.IsZoomButtonVisible = false;
            MapView.IsNorthingButtonVisible = false;
            MapView.IsMyLocationButtonVisible = false;
        }
    }
}
