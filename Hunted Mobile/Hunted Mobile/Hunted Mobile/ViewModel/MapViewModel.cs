using Hunted_Mobile.Model;
using Hunted_Mobile.Model.Game;

using Mapsui;
using Mapsui.Projection;
using Mapsui.UI.Forms;
using Mapsui.Utilities;
using Mapsui.Widgets;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel {
        private readonly MapView _view;
        private readonly Model.Map _model;

        public MapViewModel(MapView view) {
            var map = new Mapsui.Map {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };

            var tileLayer = OpenStreetMap.CreateTileLayer();

            map.Layers.Add(tileLayer);
            map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom });

            view.Map = map;
            view.MyLocationLayer.Enabled = false;

            _view = view;
            _model = new Model.Map();

            // Temporary code. Phone location can be obtained with: _view.MyLocationLayer.MyLocation
            _model.AddUser(new Thief(345) {
                Name = "Henk",
                Location = new Location() {
                    Lattitude = 25.5,
                    Longitude = 33.554325
                }
            });
            _model.AddUser(new Police(346) {
                Name = "Piet",
                Location = new Location() {
                    Lattitude = 24.223,
                    Longitude = 34.0078
                }
            });
            _model.PlayingUser = new Police(123) {
                Name = "Hans",
                Location = new Location() {
                    Lattitude = 26.789,
                    Longitude = 33.99912
                }
            };
            _model.AddLoot(new Loot(137) {
                Name = "Vlijmen",
                Location = new Location() {
                    Lattitude = 51.69,
                    Longitude = 5.22
                }
            });
            _model.AddLoot(new Loot(138) {
                Name = "Geen Vlijmen",
                Location = new Location() {
                    Lattitude = 50.69,
                    Longitude = 4.22
                }
            });
            DisplayPins();
        }

        /**
         * Displays pins for all game objects with a location
         * **/
        private void DisplayPins() {
            _view.Pins.Clear();

            // Players
            foreach(var user in _model.GetUsers()) {
                _view.Pins.Add(new Pin(_view) {
                    Label = user.Name,
                    Color = Xamarin.Forms.Color.Black,
                    Position = new Position(user.Location.Lattitude, user.Location.Longitude),
                    Scale = 0.666f,
                });
            }

            // Loot
            foreach(var loot in _model.GetLoot()) {
                _view.Pins.Add(new Pin(_view) {
                    Label = loot.Name,
                    Color = Xamarin.Forms.Color.Gold,
                    Position = new Position(loot.Location.Lattitude, loot.Location.Longitude),
                    Scale = 0.5f,
                    // TODO change icon of loot
                });
            }
            
            // Playing player
            _view.Pins.Add(new Pin(_view) {
                Label = _model.PlayingUser.Name,
                Color = Xamarin.Forms.Color.FromRgb(39, 96, 203),
                Position = new Position(_model.PlayingUser.Location.Lattitude, _model.PlayingUser.Location.Longitude),
            });
        }
    }
}
