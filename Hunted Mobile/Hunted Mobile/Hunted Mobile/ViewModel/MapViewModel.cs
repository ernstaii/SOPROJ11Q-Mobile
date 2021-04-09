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
using Plugin.Geolocator.Abstractions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hunted_Mobile.Service.Gps;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel {
        private readonly MapView _mapView;
        private readonly Model.Map _mapModel;
        private readonly GpsService _gpsService;
        public LootRepository _lootRepository = new LootRepository();

        public MapViewModel(MapView view) {
            var map = new Mapsui.Map {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom });

            view.Map = map;
            view.MyLocationLayer.Enabled = false;

            _mapView = view;
            _mapModel = new Model.Map();
            _gpsService = new GpsService();

            #region Temporary code (test data)
            _mapModel.AddUser(new Thief(345) {
                Name = "Henk",
                Location = new Location() {
                    Latitude = 51.769043,
                    Longitude = 5.516003
                }
            });
            _mapModel.AddUser(new Police(346) {
                Name = "Piet",
                Location = new Location() {
                    Latitude = 51.757423,
                    Longitude = 5.523745
                }
            });
            _mapModel.PlayingUser = new Police(123) {
                Name = "Hans",
                Location = new Location() {
                    Latitude = 51.770031,
                    Longitude = 5.534014
                }
            };
            #endregion

            LimitMapViewport(_mapModel.PlayingUser.Location, 5000);
            CenterMapOnLocation(_mapModel.PlayingUser.Location, 10);

            if(!_gpsService.GpsHasStarted()) {
                _gpsService.StartGps();
            }
            _gpsService.LocationChanged += MyLocationUpdated;
        }

        private void MyLocationUpdated(Location location) {
            _mapModel.PlayingUser.Location = location;

            Mapsui.UI.Forms.Position mapsuiPosition = new Mapsui.UI.Forms.Position(location.Latitude, location.Longitude);
            _mapView.MyLocationLayer.UpdateMyLocation(mapsuiPosition, true);

            _mapModel.SetCircleBoundary(new Mapsui.UI.Forms.Position(51.7, 5.2), new Distance(20000));

            List<Point> pointList = new List<Point>();
            pointList.Add(new Mapsui.UI.Forms.Position(51.779043, 5.506003).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.761559, 5.491387).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.743866, 5.506616).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.755662, 5.553818).ToMapsui());
            pointList.Add(new Mapsui.UI.Forms.Position(51.772993, 5.546168).ToMapsui());

            _mapModel.SetPolygonBoundary(pointList);
            GetLoot(1);
        }

        private void CenterMapOnLocation(Location center, double zoomResolution) {
            Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            _mapView.Navigator.CenterOn(centerPoint);

            _mapView.Navigator.NavigateTo(centerPoint, zoomResolution);
        }

        private void LimitMapViewport(Location center, int limit = 100000) {
            _mapView.Map.Limiter = new ViewportLimiterKeepWithin();
            Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            Point min = new Point(centerPoint.X - limit, centerPoint.Y - limit);
            Point max = new Point(centerPoint.X + limit, centerPoint.Y + limit);
            _mapView.Map.Limiter.PanLimits = new BoundingBox(min, max);
        }

        private void ZoomMap(double resolution) {
            _mapView.Navigator.ZoomTo(resolution);
        }

        /// Displays pins for all game objects with a location
        private void DisplayPins() {
            _mapView.Pins.Clear();

            // Players
            foreach(var user in _mapModel.GetUsers()) {
                _mapView.Pins.Add(new Pin(_mapView) {
                    Label = user.Name,
                    Color = Xamarin.Forms.Color.Black,
                    Position = new Mapsui.UI.Forms.Position(user.Location.Latitude, user.Location.Longitude),
                    Scale = 0.666f,
                });
            }
            
            // Playing player
            _mapView.Pins.Add(new Pin(_mapView) {
                Label = _mapModel.PlayingUser.Name,
                Color = Xamarin.Forms.Color.FromRgb(39, 96, 203),
                Position = new Mapsui.UI.Forms.Position(_mapModel.PlayingUser.Location.Latitude, _mapModel.PlayingUser.Location.Longitude),
            });

            // Boundary as a circle
            _mapView.Drawables.Add(_mapModel.GameBoundary);

            // Boundary as a polygon
            _mapView.Map.Layers.Add(CreateLayer());

            // Loot
            foreach(var loot in _mapModel.GetLoot()) {
                _mapView.Pins.Add(new Pin(_mapView) {
                    Label = loot.Name,
                    Color = Xamarin.Forms.Color.Gold,
                    Position = new Mapsui.UI.Forms.Position(loot.Location.Latitude, loot.Location.Longitude),
                    Scale = 0.5f,
                    // TODO change icon of loot
                });
            }
        }

        private Mapsui.Layers.ILayer CreateLayer() {
            MemoryProvider test = new MemoryProvider(_mapModel.PolygonBoundary);
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
                _mapModel.AddLoot(loot);
            }

            await Task.Run(() => DisplayPins());
        }
    }
}
