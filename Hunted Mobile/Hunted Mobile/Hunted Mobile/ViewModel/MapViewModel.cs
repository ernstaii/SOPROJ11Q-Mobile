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

            

            _model.AddUser(new Thief(345, "Henk", new Location(25.5, 33.554325)));
            _model.AddUser(new Police(346, "Piet", new Location(24.223, 34.0078)));
            _model.PlayingUser = new Police(123, "Hans", _view.MyLocationLayer.MyLocation);

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
            
            // Playing player
            _view.Pins.Add(new Pin(_view) {
                Label = _model.PlayingUser.Name,
                Color = Xamarin.Forms.Color.FromRgb(39, 96, 203),
                Position = new Position(_model.PlayingUser.Location.Lattitude, _model.PlayingUser.Location.Longitude),
            });
        }
    }
}
