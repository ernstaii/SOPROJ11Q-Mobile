using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Forms;
using Mapsui.Utilities;
using Mapsui.Widgets;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel {
        private readonly MapView _view;
        private readonly Model.Map _model;
        public LootRepository _lootRepository = new LootRepository();

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

            GetLoot(13);

            _model.SetCircleBoundary(new Position(51.7, 5.2), new Distance(20000));

            List<Point> pointList = new List<Point>();
            pointList.Add(new Position(51.779043, 5.506003).ToMapsui());
            pointList.Add(new Position(51.761559, 5.491387).ToMapsui());
            pointList.Add(new Position(51.743866, 5.506616).ToMapsui());
            pointList.Add(new Position(51.755662, 5.553818).ToMapsui());
            pointList.Add(new Position(51.772993, 5.546168).ToMapsui());

            _model.SetPolygonBoundary(pointList);
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

            // Boundary as a circle
            _view.Drawables.Add(_model.GameBoundary);

            // Boundary as a polygon
            _view.Map.Layers.Add(CreateLayer());
        }

        private Mapsui.Layers.ILayer CreateLayer() {
            return new Layer("Polygon") {
                DataSource = new MemoryProvider(_model.PolygonBoundary),
                Style = new VectorStyle {
                    Fill = new Brush(new Color(0, 0, 0, 0)),
                    Outline = new Pen {
                        Color = Color.Red,
                        Width = 2,
                        PenStyle = PenStyle.DashDotDot,
                        PenStrokeCap = PenStrokeCap.Round
                    }
                }
            };
        }

        // Gets all the loot from the database and adds it to the _model
        private async void GetLoot(int gameId) {
            var lootList = await _lootRepository.GetAll(gameId);

            foreach(var loot in lootList) {
                _model.AddLoot(loot);
            }
        }
    }
}
