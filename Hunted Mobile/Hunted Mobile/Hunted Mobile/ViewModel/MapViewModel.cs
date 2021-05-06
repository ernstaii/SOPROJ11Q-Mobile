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
using Xamarin.Forms;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel : BaseViewModel {
        private MapView _mapView;
        private View.Messages _messagesView;
        private readonly Model.Map _mapModel;
        private readonly Game _gameModel;
        private readonly LootRepository _lootRepository;
        private readonly UserRepository _userRepository;
        private readonly GpsService _gpsService;
        private Timer _intervalUpdateTimer;
        private Pin _playerPin;
        private WebSocketService _webSocketService;

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

        public bool Initialized { get; private set; }

        const string PAUSE_TITLE = "Gepauzeerd",
            END_TITLE = "Het spel is afgelopen!",
            PAUSE_DESCRIPTION = "Momenteel is het spel gepauzeerd door de spelleider. Wanneer de pauze voorbij is, zal het spel weer hervat worden.",
            END_DESCRIPTION = "Ga terug naar de spelleider!";

        public string TitleOverlay => GameHasEnded ? END_TITLE : PAUSE_TITLE;
        public string DescriptionOverlay => GameHasEnded ? END_DESCRIPTION : PAUSE_DESCRIPTION;

        public MapViewModel(Game gameModel, Model.Map mapModel, GpsService gpsService, LootRepository lootRepository, UserRepository userRepository) {
            _mapModel = mapModel;
            _gameModel = gameModel;
            _gpsService = gpsService;
            _messagesView = new View.Messages(_gameModel.Id);
            _lootRepository = lootRepository;
            _userRepository = userRepository;

            if(_gameModel.Status == GameStatus.Paused) {
                PauseGame(null);
            }

            if(_gameModel.Status == GameStatus.Finished) {
                EndGame(null);
            }
        }

        private async Task PollLoot() {
            var lootList = await _lootRepository.GetAll(_gameModel.Id);
            _mapModel.SetLoot(lootList);
        }

        private async Task PollUsers() {
            var userList = new List<User>();
            foreach(User user in await _userRepository.GetAll(_gameModel.Id)) {
                if(user.Id != _mapModel.PlayingUser.Id) {
                    userList.Add(user);
                }
            }
            _mapModel.SetUsers(userList);
        }

        private void IntervalOfGame(JObject data) {
            StartIntervalTimer();

            List<User> userList = new List<User>();

            foreach(JObject user in data.GetValue("users")) {

                Location location = new Location((string) user.GetValue("location"));
                int userId = -1;
                int.TryParse((string) user.GetValue("id"), out userId);

                User newUser = new User();
                newUser.Id = userId;
                newUser.UserName = ((string) user.GetValue("username"));
                newUser.Location = location;
                newUser.Role = ((string) user.GetValue("role"));

                userList.Add(newUser);
            }

            _mapModel.SetUsers(userList);

            DisplayOtherPins();
        }

        public void SetMapView(MapView mapView) {
            if(mapView != null) {
                bool initializedBefore = _mapView != null;
                _mapView = mapView;

                if(!initializedBefore) {
                    InitializeMap();
                }
            }
        }

        private void InitializeMap() {
            AddOsmLayerToMapView();
            AddGameBoundary();
            LimitViewportToGame();

            Task.Run(async () => {
                if(!_gpsService.GpsHasStarted()) {
                    await _gpsService.StartGps();
                }
                _gpsService.LocationChanged += MyLocationUpdated;

                await PollLoot();
                DisplayOtherPins();

                await StartSocket();

                StartIntervalTimer();

                Timer initialPlayerUpdateTimer = new Timer(5000);
                initialPlayerUpdateTimer.AutoReset = false;
                initialPlayerUpdateTimer.Elapsed += async (object sender, ElapsedEventArgs args) => {
                    await PollUsers();
                    DisplayOtherPins();
                    Initialized = true;
                    initialPlayerUpdateTimer.Dispose();
                };
                initialPlayerUpdateTimer.Start();
            });
        }

        private void StopIntervalTimer() {
            if(_intervalUpdateTimer != null) {
                _intervalUpdateTimer.Stop();
                _intervalUpdateTimer.Dispose();
                _intervalUpdateTimer = null;
            }
        }

        private void StartIntervalTimer(float secondsBeforeGameInterval = 5) {
            StopIntervalTimer();
            _intervalUpdateTimer = new Timer((_gameModel.Interval - secondsBeforeGameInterval) * 1000);
            _intervalUpdateTimer.AutoReset = false;
            _intervalUpdateTimer.Elapsed += PreIntervalUpdate;
            _intervalUpdateTimer.Start();
        }

        private async void PreIntervalUpdate(object sender = null, ElapsedEventArgs args = null) {
            StopIntervalTimer();

            // Send the current user's location to the database
            await _userRepository.Update(_mapModel.PlayingUser.Id, _mapModel.PlayingUser.Location);
        }

        private async Task StartSocket() {
            try {
                _webSocketService = new WebSocketService(_gameModel.Id);
                if(!WebSocketService.Connected) {
                    await _webSocketService.Connect();
                }

                _webSocketService.ResumeGame += ResumeGame;
                _webSocketService.PauseGame += PauseGame;
                _webSocketService.EndGame += EndGame;

                _webSocketService.IntervalEvent += IntervalOfGame;
            }
            catch(Exception ex) {
                Console.WriteLine("An error occurred when connecting the web socket: " + ex.StackTrace);
            }
        }

        private void EndGame(JObject data) {
            GameHasEnded = true;
            IsEnabled = false;

            StopIntervalTimer();
        }

        private void PauseGame(JObject data) {
            IsEnabled = false;

            StopIntervalTimer();
        }

        private void ResumeGame() {
            IsEnabled = true;

            StartIntervalTimer();
        }

        /// <summary>
        /// Action to execute when the device location has updated
        /// </summary>
        private async void MyLocationUpdated(Location newLocation) {
            _mapModel.PlayingUser.Location = newLocation;

            // Send update to the map view
            Mapsui.UI.Forms.Position mapsuiPosition = new Mapsui.UI.Forms.Position(newLocation.Latitude, newLocation.Longitude);
            _mapView.MyLocationLayer.UpdateMyLocation(mapsuiPosition, true);

            DisplayPlayerPin();

            if(!Initialized) {
                await _userRepository.Update(_mapModel.PlayingUser.Id, _mapModel.PlayingUser.Location);
            }
        }

        private void CenterMapOnLocation(Location center, double zoomResolution) {
            Mapsui.Geometries.Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            _mapView.Navigator.CenterOn(centerPoint);

            _mapView.Navigator.NavigateTo(centerPoint, zoomResolution);
        }

        /// <summary>
        /// Ensures the map panning is limited to given number around a given center location
        /// </summary>
        private void LimitMapViewport(Location center, int limit = 100000) {
            _mapView.Map.Limiter = new ViewportLimiterKeepWithin();
            Mapsui.Geometries.Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            Mapsui.Geometries.Point min = new Mapsui.Geometries.Point(centerPoint.X - limit, centerPoint.Y - limit);
            Mapsui.Geometries.Point max = new Mapsui.Geometries.Point(centerPoint.X + limit, centerPoint.Y + limit);
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
                    Fill = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(0, 0, 0, 0)),
                    Outline = new Pen {
                        Color = Mapsui.Styles.Color.Red,
                        Width = 2,
                        PenStyle = PenStyle.DashDotDot,
                        PenStrokeCap = PenStrokeCap.Round
                    }
                }
            };
        }

        private void DisplayPlayerPin() {
            if(_playerPin == null) {
                _playerPin = new Pin(_mapView) {
                    Label = _mapModel.PlayingUser.UserName,
                    Color = Xamarin.Forms.Color.FromRgb(39, 96, 203)
                };
            }

            _playerPin.Position = new Mapsui.UI.Forms.Position(_mapModel.PlayingUser.Location.Latitude, _mapModel.PlayingUser.Location.Longitude);

            if(!_mapView.Pins.Contains(_playerPin)) {
                _mapView.Pins.Add(_playerPin);
            }
        }

        /// <summary>
        /// Displays pins for all game objects with a location
        /// </summary>
        private void DisplayOtherPins() {
            _mapView.Pins.Clear();

            if(_playerPin != null) {
                _mapView.Pins.Add(_playerPin);
            }

            //TODO: the null checks here should probably be resolved elsewhere
            // Players
            foreach(var user in _mapModel.GetUsers()) {
                if(user.Location != null && user.UserName != null) {
                    _mapView.Pins.Add(new Pin(_mapView) {
                        Label = user.UserName,
                        Color = Xamarin.Forms.Color.Black,
                        Position = new Mapsui.UI.Forms.Position(user.Location.Latitude, user.Location.Longitude),
                        Scale = 0.666f,
                    });
                }
            }

            // Loot
            foreach(var loot in _mapModel.GetLoot()) {
                if(loot.Name != null && loot.Location != null) {
                    _mapView.Pins.Add(new Pin(_mapView) {
                        Label = loot.Name,
                        Color = Xamarin.Forms.Color.Gold,
                        Position = new Mapsui.UI.Forms.Position(loot.Location.Latitude, loot.Location.Longitude),
                        Scale = 0.5f,
                    });
                }
            }
        }


        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(_messagesView);
        });

        /// <summary>
        /// Navigate to the RootPage
        /// </summary>
        public ICommand ExitGameCommand => new Xamarin.Forms.Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopToRootAsync();
            await _webSocketService.Disconnect();
        });
    }
}
