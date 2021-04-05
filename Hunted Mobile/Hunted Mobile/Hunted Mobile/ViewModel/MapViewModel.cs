using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using Mapsui;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.UI;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Forms;
using Mapsui.Utilities;
using Mapsui.Widgets;

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

using System;
using System.Collections.Generic;
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

            #region Temporary code (test data)
            _model.AddUser(new Thief(345) {
                Name = "Henk",
                Location = new Location() {
                    Lattitude = 51.7,
                    Longitude = 5.55
                }
            });
            _model.AddUser(new Police(346) {
                Name = "Piet",
                Location = new Location() {
                    Lattitude = 51.8,
                    Longitude = 5.5
                }
            });
            _model.PlayingUser = new Police(123) {
                Name = "Hans",
                Location = new Location() {
                    Lattitude = 51.7612,
                    Longitude = 5.5140
                }
            };
            #endregion

            LimitMapViewport(_model.PlayingUser.Location, 20000);
            CenterMapOnLocation(_model.PlayingUser.Location, 45);

            if(!CrossGeolocator.Current.IsListening) {
                StartGPS();
            }
        }

        private void MyLocationUpdated(object sender, PositionEventArgs e) {
            _model.PlayingUser.Location = new Location(e.Position);

            Mapsui.UI.Forms.Position mapsuiPosition = new Mapsui.UI.Forms.Position(e.Position.Latitude, e.Position.Longitude);
            _view.MyLocationLayer.UpdateMyLocation(mapsuiPosition, true);
            _view.MyLocationLayer.UpdateMySpeed(e.Position.Speed);

            _model.SetCircleBoundary(new Mapsui.UI.Forms.Position(51.7, 5.2), new Distance(20000));

            List<Point> pointList = new List<Point>();
            pointList.Add(new Mapsui.UI.Forms.Position(51.779043, 5.506003).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.761559, 5.491387).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.743866, 5.506616).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.755662, 5.553818).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.772993, 5.546168).ToMapsui());

            _model.SetPolygonBoundary(pointList);
            GetLoot(1);
        }

        private async void StartGPS() {
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

            CrossGeolocator.Current.PositionChanged += MyLocationUpdated;
        }

        private void CenterMapOnLocation(Location center, double zoomResolution) {
            Point centerPoint = new Mapsui.UI.Forms.Position(center.Lattitude, center.Longitude).ToMapsui();
            _view.Navigator.CenterOn(centerPoint);

            _view.Navigator.NavigateTo(centerPoint, zoomResolution);
        }

        private void LimitMapViewport(Location center, int limit = 100000) {
            _view.Map.Limiter = new ViewportLimiterKeepWithin();
            Point centerPoint = new Mapsui.UI.Forms.Position(center.Lattitude, center.Longitude).ToMapsui();
            Point min = new Point(centerPoint.X - limit, centerPoint.Y - limit);
            Point max = new Point(centerPoint.X + limit, centerPoint.Y + limit);
            _view.Map.Limiter.PanLimits = new BoundingBox(min, max);
        }

        private void ZoomMap(double resolution) {
            _view.Navigator.ZoomTo(resolution);
        }

        /// Displays pins for all game objects with a location
        private void DisplayPins() {
            _view.Pins.Clear();

            // Players
            foreach(var user in _model.GetUsers()) {
                _view.Pins.Add(new Pin(_view) {
                    Label = user.Name,
                    Color = Xamarin.Forms.Color.Black,
                    Position = new Mapsui.UI.Forms.Position(user.Location.Lattitude, user.Location.Longitude),
                    Scale = 0.666f,
                });
            }

            // Loot
            foreach(var loot in _model.GetLoot()) {
                _view.Pins.Add(new Pin(_view) {
                    Label = loot.Name,
                    Color = Xamarin.Forms.Color.Gold,
                    Position = new Mapsui.UI.Forms.Position(loot.Location.Lattitude, loot.Location.Longitude),
                    Scale = 0.5f,
                    // TODO change icon of loot
                });
            }
            
            // Playing player
            _view.Pins.Add(new Pin(_view) {
                Label = _model.PlayingUser.Name,
                Color = Xamarin.Forms.Color.FromRgb(39, 96, 203),
                Position = new Mapsui.UI.Forms.Position(_model.PlayingUser.Location.Lattitude, _model.PlayingUser.Location.Longitude),
            });

            // Boundary as a circle
            _view.Drawables.Add(_model.GameBoundary);

            // Boundary as a polygon
            _view.Map.Layers.Add(CreateLayer());
        }

        private Mapsui.Layers.ILayer CreateLayer() {
            MemoryProvider test = new MemoryProvider(_model.PolygonBoundary);
            return new Layer("Polygon") {
                DataSource = test,
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
        private async Task GetLoot(int gameId) {
            var lootList = await _lootRepository.GetAll(gameId);

            foreach(var loot in lootList) {
                _model.AddLoot(loot);
            }

            await Task.Run(() => DisplayPins());
        }
    }
}
