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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Gps;
using System.Windows.Input;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel : BaseViewModel {
        private readonly MapView _mapView;
        private readonly Model.Map _mapModel;
        private readonly LootRepository _lootRepository;
        private readonly GpsService _gpsService;
        private readonly WebSocketService _webSocketService;

        private bool _isEnabled = true;
        private bool _gameHasEnded = false;

        /// <summary>
        /// This property will disable the touch of the user with the mapView
        /// </summary>
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                if(_mapView != null && _mapView.Content != null)
                    _mapView.Content.IsEnabled = _isEnabled;

                OnPropertyChanged("IsEnabled");
                OnPropertyChanged("VisibleOverlay");
                OnPropertyChanged("TitleOverlay");
                OnPropertyChanged("DescriptionOverlay");
            }
        }
        public bool GameHasEnded {
            get => _gameHasEnded;
            set {
                _gameHasEnded = value;

                OnPropertyChanged("GameHasEnded");
            }
        }

        /// <summary>
        /// The oposite of the enable-state
        /// </summary>
        public bool VisibleOverlay => !IsEnabled;

        const string PAUSE_TITLE = "Gepauzeerd",
            END_TITLE = "Het spel is afgelopen!",
            PAUSE_DESCRIPTION = "Momenteel is het spel gepauzeerd door de spelleider. Wanneer de pauze voorbij is, zal het spel weer hervat worden.",
            END_DESCRIPTION = "Ga terug naar de spelleider!";

        public string TitleOverlay => GameHasEnded ? END_TITLE : PAUSE_TITLE;
        public string DescriptionOverlay => GameHasEnded ? END_DESCRIPTION : PAUSE_DESCRIPTION;

        public MapViewModel(MapView view, Game gameModel) {
            _mapView = view;
            _mapModel = new Model.Map();
            _gpsService = new GpsService();
            _lootRepository = new LootRepository();

            AddOsmLayerToMapView();

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

            AddGameBoundary();
            LimitViewportToGame();

            if(!_gpsService.GpsHasStarted()) {
                _gpsService.StartGps();
            }

            _gpsService.LocationChanged += MyLocationUpdated;

            _webSocketService = new WebSocketService(gameModel.Id);
            Task.Run(async () => await StartSocket());
        }

        private async Task StartSocket() {
            try {
                if(!WebSocketService.Connected) {
                    await _webSocketService.Connect();
                }

                _webSocketService.ResumeGame += ResumeGame;
                _webSocketService.PauseGame += PauseGame;
                _webSocketService.EndGame += EndGame;
            }
            catch {
            }
        }

        private void EndGame() {
            GameHasEnded = true;
            IsEnabled = false;
        }

        private void PauseGame() {
            IsEnabled = false;
        }

        private void ResumeGame() {
            IsEnabled = true;
        }

        /// <summary>
        /// Action to execute when the device location has updated
        /// </summary>
        private async void MyLocationUpdated(Location newLocation) {
            await Task.Run(async () => {
                _mapModel.PlayingUser.Location = newLocation;

                // Send update to the map view
                Mapsui.UI.Forms.Position mapsuiPosition = new Mapsui.UI.Forms.Position(newLocation.Latitude, newLocation.Longitude);
                _mapView.MyLocationLayer.UpdateMyLocation(mapsuiPosition, true);

                // todo: this call should later move to an update interval
                await UpdateLoot(1);

                DisplayPins();
            });
        }

        private void CenterMapOnLocation(Location center, double zoomResolution) {
            Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            _mapView.Navigator.CenterOn(centerPoint);

            _mapView.Navigator.NavigateTo(centerPoint, zoomResolution);
        }

        /// <summary>
        /// Ensures the map panning is limited to given number around a given center location
        /// </summary>
        private void LimitMapViewport(Location center, int limit = 100000) {
            _mapView.Map.Limiter = new ViewportLimiterKeepWithin();
            Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            Point min = new Point(centerPoint.X - limit, centerPoint.Y - limit);
            Point max = new Point(centerPoint.X + limit, centerPoint.Y + limit);
            _mapView.Map.Limiter.PanLimits = new BoundingBox(min, max);
        }

        /// <summary>
        /// Ensures the map panning is limited to the game's boundary
        /// </summary>
        private void LimitViewportToGame() {
            Location center = _mapModel.GameBoundary.GetCenter();
            double diameter = _mapModel.GameBoundary.GetDiameter();
            int viewPortSizeMultiplier = 70000;
            LimitMapViewport(center, (int) (diameter * viewPortSizeMultiplier));

            BoundingBox gameArea = new BoundingBox(new List<Geometry>() { _mapModel.GameBoundary.ToPolygon() });

            while(!_mapView.Map.Limiter.PanLimits.Contains(gameArea)) {
                viewPortSizeMultiplier += 5000;
                LimitMapViewport(center, (int) (diameter * viewPortSizeMultiplier));
            }

            CenterMapOnLocation(center, diameter * 175);
        }

        private void ZoomMap(double resolution) {
            _mapView.Navigator.ZoomTo(resolution);
        }

        /// <summary>
        /// Adds the required OpenStreetMap layer to the mapView
        /// </summary>
        private void AddOsmLayerToMapView() {
            var map = new Mapsui.Map {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom });

            _mapView.Map = map;
            _mapView.MyLocationLayer.Enabled = false;
        }

        /// <summary>
        /// Adds the visual game boundary as a polygon
        /// </summary>
        private void AddGameBoundary() {
            Boundary boundary = new Boundary();
            boundary.Points.Add(new Location(51.779043, 5.506003));
            boundary.Points.Add(new Location(51.761559, 5.491387));
            boundary.Points.Add(new Location(51.743866, 5.506616));
            boundary.Points.Add(new Location(51.755662, 5.553818));
            boundary.Points.Add(new Location(51.772993, 5.546168));

            _mapModel.GameBoundary = boundary;

            _mapView.Map.Layers.Add(CreateBoundaryLayer());
        }

        /// <summary>
        /// Creates a layer to display the game boundary
        /// </summary>
        private ILayer CreateBoundaryLayer() {
            MemoryProvider memoryProvider = new MemoryProvider(_mapModel.GameBoundary.ToPolygon());
            return new Layer("Polygon") {
                DataSource = memoryProvider,
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

        /// <summary>
        /// Displays pins for all game objects with a location
        /// </summary>
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

        /// <summary>
        /// Gets all the loot from the database and updates the _model
        /// </summary>
        private async Task UpdateLoot(int gameId) {
            var lootList = await _lootRepository.GetAll(gameId);

            _mapModel.SetLoot(lootList);
        }

        /// <summary>
        /// Navigate to the RootPage
        /// </summary>
        public ICommand ExitGameCommand => new Xamarin.Forms.Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopToRootAsync();
            await _webSocketService.Disconnect();
        });
    }
}
