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
using Hunted_Mobile.Model.Resource;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel : BaseViewModel {
        private const int LOOT_PICKUP_TIME_IN_SECONDES = 5,
            LOOT_PICKUP_MAX_DISTANCE_IN_METERS = 10;

        const string PAUSE_TITLE = "Gepauzeerd",
            END_TITLE = "Het spel is afgelopen!",
            PAUSE_DESCRIPTION = "Momenteel is het spel gepauzeerd door de spelleider. Wanneer de pauze voorbij is, zal het spel weer hervat worden.",
            END_DESCRIPTION = "Ga terug naar de spelleider!",
            LOOT_TAG = "loot";

        private readonly Xamarin.Forms.Color policePinColor = Xamarin.Forms.Color.FromRgb(39, 96, 203);
        private readonly Xamarin.Forms.Color thiefPinColor = Xamarin.Forms.Color.Black;
        private readonly Model.Map mapModel;
        private readonly LootRepository lootRepository;
        private readonly UserRepository userRepository;
        private readonly InviteKeyRepository inviteKeyRepository;
        private readonly BorderMarkerRepository borderMarkerRepository;
        private readonly GameRepository gameRepository;
        private readonly GpsService gpsService;
        private WebSocketService webSocketService;
        private Loot selectedLoot = new Loot(0);
        private Game gameModel;
        private MapView mapView;
        private readonly View.Messages messagesView;
        private Timer intervalUpdateTimer;
        private Timer lootTimer;
        private Pin playerPin;
        private bool isEnabled = true;
        private bool gameHasEnded = false;
        private bool isHandlingLoot = false;
        private bool openMainMapMenu = false;
        private bool mainMapMenuButtonVisible = true;
        private bool hasFinishedHandlingLoot = false;
        private Resource chatIcon;
        private readonly Resource policeBadgeIcon;
        private readonly Resource moneyBagIcon;

        /// <summary>
        /// This property will disable the touch of the user with the mapView
        /// </summary>
        public bool IsEnabled {
            get => isEnabled;
            set {
                isEnabled = value;
                if(mapView != null && mapView.Content != null)
                    mapView.Content.IsEnabled = isEnabled;

                OnPropertyChanged("IsEnabled");
                OnPropertyChanged("VisibleOverlay");
                OnPropertyChanged("TitleOverlay");
                OnPropertyChanged("DescriptionOverlay");
            }
        }

        public bool GameHasEnded {
            get => gameHasEnded;
            set {
                gameHasEnded = value;

                OnPropertyChanged("GameHasEnded");
            }
        }
        public bool OpenMainMapMenu {
            get => openMainMapMenu;
            set {
                mainMapMenuButtonVisible = !value;
                openMainMapMenu = value;
                OnPropertyChanged("MainMapMenuButtonVisible");
                OnPropertyChanged("OpenMainMapMenu");
            }
        }

        public bool MainMapMenuButtonVisible => mainMapMenuButtonVisible;

        public Loot SelectedLoot {
            get => selectedLoot;
            set {
                selectedLoot = value;

                OnPropertyChanged("SelectedLoot");
                OnPropertyChanged(nameof(IsCloseToSelectedLoot));
                OnPropertyChanged(nameof(IsFarFromSelectedLoot));
            }
        }

        public bool IsHandlingLoot {
            get => isHandlingLoot;
            set {
                isHandlingLoot = value;
                if(value)
                    HasFinishedHandlingLoot = false;

                OnPropertyChanged("IsHandlingLoot");
                OnPropertyChanged(nameof(IsCloseToSelectedLoot));
                OnPropertyChanged(nameof(IsFarFromSelectedLoot));
            }
        }

        public bool HasFinishedHandlingLoot {
            get => hasFinishedHandlingLoot;
            set {
                hasFinishedHandlingLoot = value;
                if(value)
                    IsHandlingLoot = false;

                OnPropertyChanged("HasFinishedHandlingLoot");
            }
        }

        public bool IsCloseToSelectedLoot {
            get {
                if(IsHandlingLoot && mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && SelectedLoot != null && SelectedLoot.Location != null) {
                    return mapModel.PlayingUser.Location.DistanceToOtherInMeters(SelectedLoot.Location) <= LOOT_PICKUP_MAX_DISTANCE_IN_METERS;
                }
                else return false;
            }
        }

        public bool IsFarFromSelectedLoot => !IsCloseToSelectedLoot;
        public bool VisibleOverlay => !IsEnabled;
        public bool Initialized { get; private set; }

        public string TitleOverlay => GameHasEnded ? END_TITLE : PAUSE_TITLE;

        public string DescriptionOverlay => GameHasEnded ? END_DESCRIPTION : PAUSE_DESCRIPTION;

        public int PlayingUserScore {
            get {
                if(mapModel != null && gameModel != null) {
                    if(mapModel.PlayingUser is Thief) {
                        return gameModel.ThievesScore;
                    }
                    else if(mapModel.PlayingUser is Police) {
                        return gameModel.PoliceScore;
                    }
                }
                return 0;
            }
        }

        public UriImageSource ChatIcon {
            get => new UriImageSource() {
                Uri = chatIcon.Uri,
                CachingEnabled = false
            };
        }

        public MapViewModel(Game gameModel, Model.Map mapModel, GpsService gpsService, LootRepository lootRepository, UserRepository userRepository, GameRepository gameRepository, InviteKeyRepository inviteKeyRepository, BorderMarkerRepository borderMarkerRepository, ResourceRepository resourceRepository) {
            this.mapModel = mapModel;
            this.gameModel = gameModel;
            this.gpsService = gpsService;
            messagesView = new View.Messages(this.gameModel.Id);
            this.lootRepository = lootRepository;
            this.userRepository = userRepository;
            this.gameRepository = gameRepository;
            this.inviteKeyRepository = inviteKeyRepository;
            this.borderMarkerRepository = borderMarkerRepository;

            chatIcon = resourceRepository.GetGuiImage("chat.png");
            OnPropertyChanged(nameof(ChatIcon));
            policeBadgeIcon = resourceRepository.GetMapImage("police-badge.png");
            moneyBagIcon = resourceRepository.GetMapImage("money-bag.png");
        }

        private void HandlePinClicked(object sender, PinClickedEventArgs args) {
            if($"{args.Pin.Tag}" == LOOT_TAG) {
                var loot = mapModel.FindLoot(new Location(args.Pin.Position));

                if(loot != null) {
                    SelectedLoot = loot;
                    IsHandlingLoot = true;
                }
            }
        }

        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(messagesView);
        });

        /// <summary>
        /// Navigate to the RootPage
        /// </summary>
        public ICommand ExitGameCommand => new Xamarin.Forms.Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopToRootAsync();
            await webSocketService.Disconnect();
        });

        public ICommand PickupLootCommand => new Xamarin.Forms.Command((e) => {
            // Instant finishing off
            HasFinishedHandlingLoot = true;
        });

        public ICommand ClosePickingLootCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedHandlingLoot = false;
        });

        public ICommand CancelPickUpLootCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedHandlingLoot = false;
            IsHandlingLoot = false;
        });

        public ICommand OpenMainMapMenuCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedHandlingLoot = false;
            OpenMainMapMenu = true;
        });

        public ICommand CloseMainMapMenuCommand => new Xamarin.Forms.Command((e) => {
            OpenMainMapMenu = false;
        });

        public ICommand Button_PressedPickupLoot => new Xamarin.Forms.Command((e) => {
            lootTimer = new Timer();

            // Interval is set with milisecondes
            lootTimer.Interval = LOOT_PICKUP_TIME_IN_SECONDES * 1000;
            lootTimer.Elapsed += SuccessfullyPickedUpLoot;
            lootTimer.Start();
        });

        public ICommand Button_ReleasedPickupLoot => new Xamarin.Forms.Command((e) => {
            if(lootTimer != null) {
                lootTimer.Stop();
                lootTimer = null;
            }
        });


        private async Task PollLoot() {
            var lootList = await lootRepository.GetAll(gameModel.Id);
            mapModel.SetLoot(lootList);
        }

        private async Task PollUsers() {
            var userList = new List<Player>();
            foreach(Player user in await userRepository.GetAll(gameModel.Id)) {
                if(user.Id != mapModel.PlayingUser.Id) {
                    userList.Add(user);
                }
            }
            mapModel.SetUsers(userList);
        }

        private void IntervalOfGame(JObject data) {
            StartIntervalTimer();

            List<Player> userList = new List<Player>();

            foreach(JObject user in data.GetValue("users")) {
                int userId = -1;
                int.TryParse((string) user.GetValue("id"), out userId);

                if(userId != mapModel.PlayingUser.Id) {
                    Location location = new Location((string) user.GetValue("location"));
                    bool wasThief = mapModel.GetUserById(userId) is Thief;
                    Player newUser = new Player();
                    if(wasThief) {
                        newUser = new Thief(newUser);
                    }
                    else newUser = new Police(newUser);

                    newUser.Id = userId;
                    newUser.UserName = ((string) user.GetValue("username"));
                    newUser.Location = location;

                    userList.Add(newUser);
                }
            }

            mapModel.SetUsers(userList);

            DisplayOtherPins();
        }

        public void SetMapView(MapView mapView) {
            bool initializedBefore = this.mapView != null;
            this.mapView = mapView;
            DisableDefaultMapViewOptions();

            if(!initializedBefore) {
                InitializeMap();
            }
        }


        // In the Mockups, these options are not visible, so this method makes sure that the options are hidden
        private void DisableDefaultMapViewOptions() {
            mapView.IsZoomButtonVisible = false;
            mapView.IsNorthingButtonVisible = false;
            mapView.IsMyLocationButtonVisible = false;
        }

        private void InitializeMap() {
            AddOsmLayerToMapView();

            Task.Run(async () => {
                await AddGameBoundary();
                LimitViewportToGame();

                if(!gpsService.GpsHasStarted()) {
                    await gpsService.StartGps();
                }

                gpsService.LocationChanged += MyLocationUpdated;

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

                mapView.PinClicked += HandlePinClicked;
            });
        }

        private void StopIntervalTimer() {
            if(intervalUpdateTimer != null) {
                intervalUpdateTimer.Stop();
                intervalUpdateTimer.Dispose();
                intervalUpdateTimer = null;
            }
        }

        private void StartIntervalTimer(float secondsBeforeGameInterval = 5) {
            StopIntervalTimer();
            intervalUpdateTimer = new Timer((gameModel.Interval - secondsBeforeGameInterval) * 1000);
            intervalUpdateTimer.AutoReset = false;
            intervalUpdateTimer.Elapsed += PreIntervalUpdate;
            intervalUpdateTimer.Start();
        }

        private async void PreIntervalUpdate(object sender = null, ElapsedEventArgs args = null) {
            StopIntervalTimer();

            // Send the current user's location to the database
            await userRepository.Update(mapModel.PlayingUser.Id, mapModel.PlayingUser.Location);
        }

        private async Task StartSocket() {
            try {
                webSocketService = new WebSocketService(gameModel.Id);
                if(!WebSocketService.Connected) {
                    await webSocketService.Connect();
                }

                webSocketService.ResumeGame += ResumeGame;
                webSocketService.PauseGame += PauseGame;
                webSocketService.EndGame += EndGame;

                webSocketService.IntervalEvent += IntervalOfGame;
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

        private void ResumeGame(JObject data) {
            IsEnabled = true;

            StartIntervalTimer();
        }

        /// <summary>
        /// Action to execute when the device location has updated
        /// </summary>
        private async void MyLocationUpdated(Location newLocation) {
            mapModel.PlayingUser.Location = newLocation;

            // Send update to the map view
            Mapsui.UI.Forms.Position mapsuiPosition = new Mapsui.UI.Forms.Position(newLocation.Latitude, newLocation.Longitude);
            mapView.MyLocationLayer.UpdateMyLocation(mapsuiPosition, true);

            DisplayPlayerPin();

            OnPropertyChanged(nameof(IsCloseToSelectedLoot));
            OnPropertyChanged(nameof(IsFarFromSelectedLoot));

            if(!Initialized) {
                await userRepository.Update(mapModel.PlayingUser.Id, mapModel.PlayingUser.Location);
            }
        }

        private void CenterMapOnLocation(Location center, double zoomResolution) {
            Mapsui.Geometries.Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            mapView.Navigator.CenterOn(centerPoint);
            mapView.Navigator.NavigateTo(centerPoint, zoomResolution);
        }

        /// <summary>
        /// Ensures the map panning is limited to given number around a given center location
        /// </summary>
        private void LimitMapViewport(Location center, int limit = 100000) {
            mapView.Map.Limiter = new ViewportLimiterKeepWithin();
            Mapsui.Geometries.Point centerPoint = new Mapsui.UI.Forms.Position(center.Latitude, center.Longitude).ToMapsui();
            Mapsui.Geometries.Point min = new Mapsui.Geometries.Point(centerPoint.X - limit, centerPoint.Y - limit);
            Mapsui.Geometries.Point max = new Mapsui.Geometries.Point(centerPoint.X + limit, centerPoint.Y + limit);
            mapView.Map.Limiter.PanLimits = new BoundingBox(min, max);
        }

        /// <summary>
        /// Ensures the map panning is limited to the game's boundary
        /// </summary>
        private void LimitViewportToGame() {
            Location center = mapModel.GameBoundary.GetCenter();
            double diameter = mapModel.GameBoundary.GetDiameter();
            int viewPortSizeMultiplier = 70000;
            LimitMapViewport(center, (int) (diameter * viewPortSizeMultiplier));

            BoundingBox gameArea = new BoundingBox(new List<Geometry>() { mapModel.GameBoundary.ToPolygon() });

            while(!mapView.Map.Limiter.PanLimits.Contains(gameArea)) {
                viewPortSizeMultiplier += 5000;
                LimitMapViewport(center, (int) (diameter * viewPortSizeMultiplier));
            }

            CenterMapOnLocation(center, diameter * 175);
        }

        private void ZoomMap(double resolution) {
            mapView.Navigator.ZoomTo(resolution);
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

            mapView.Map = map;
            mapView.MyLocationLayer.Enabled = false;
        }

        /// <summary>
        /// Adds the visual game boundary as a polygon
        /// </summary>
        private async Task AddGameBoundary() {
            List<Location> locations = await borderMarkerRepository.GetAll(gameModel.Id);
            Boundary boundary = new Boundary();

            foreach(Location location in locations)
                boundary.Points.Add(location);

            mapModel.GameBoundary = boundary;
            mapView.Map.Layers.Add(CreateBoundaryLayer());
        }

        /// <summary>
        /// Creates a layer to display the game boundary
        /// </summary>
        private ILayer CreateBoundaryLayer() {
            MemoryProvider memoryProvider = new MemoryProvider(mapModel.GameBoundary.ToPolygon());
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
            if(playerPin == null) {
                playerPin = new Pin(mapView) {
                    Label = mapModel.PlayingUser.UserName,
                    Color = mapModel.PlayingUser is Thief ? thiefPinColor : policePinColor,
                };
            }

            playerPin.Position = new Mapsui.UI.Forms.Position(mapModel.PlayingUser.Location.Latitude, mapModel.PlayingUser.Location.Longitude);

            if(!mapView.Pins.Contains(playerPin)) {
                mapView.Pins.Add(playerPin);
            }
        }

        /// <summary>
        /// Displays pins for all game objects with a location
        /// </summary>
        private void DisplayOtherPins() {
            mapView.Pins.Clear();

            if(playerPin != null) {
                mapView.Pins.Add(playerPin);
            }

            //TODO: the null checks here should probably be resolved elsewhere
            // Players
            foreach(var user in mapModel.GetUsers()) {
                if(user.Location != null && user.UserName != null) {
                    mapView.Pins.Add(new Pin(mapView) {
                        Label = user.UserName,
                        Color = user is Thief ? thiefPinColor : policePinColor,
                        Position = new Mapsui.UI.Forms.Position(user.Location.Latitude, user.Location.Longitude),
                        Scale = 0.75f,
                        Transparency = 0.25f,
                    });
                }
            }

            // Loot
            foreach(var loot in mapModel.GetLoot()) {
                if(loot.Name != null && loot.Location != null) {
                    mapView.Pins.Add(new Pin(mapView) {
                        Label = loot.Name,
                        Position = new Mapsui.UI.Forms.Position(loot.Location.Latitude, loot.Location.Longitude),
                        Scale = 1.0f,
                        Tag = LOOT_TAG,
                        Icon = moneyBagIcon.Data,
                        Type = PinType.Icon,
                    });
                }
            }

            // Police station
            if(gameModel.PoliceStationLocation != null) {
                mapView.Pins.Add(new Pin(mapView) {
                    Label = "Politie station",
                    Position = new Mapsui.UI.Forms.Position(gameModel.PoliceStationLocation.Latitude, gameModel.PoliceStationLocation.Longitude),
                    Scale = 1.0f,
                    Icon = policeBadgeIcon.Data,
                    Type = PinType.Icon,
                });
            }
        }

        private void SuccessfullyPickedUpLoot(object sender, EventArgs e) {
            lootTimer.Stop();
            lootTimer = null;
            HasFinishedHandlingLoot = true;

            Task.Run(async () => {
                Game game = await gameRepository.GetGame(gameModel.Id);

                // User should be a thief here since a police can't open the dialog
                bool deleted = await lootRepository.Delete(SelectedLoot.Id);
                if(deleted) {
                    await gameRepository.UpdateThievesScore(game.Id, game.ThievesScore + 50);
                    gameModel = await gameRepository.GetGame(gameModel.Id);
                    OnPropertyChanged(nameof(PlayingUserScore));
                    await PollLoot();
                    DisplayOtherPins();
                }
            });
        }
    }
}
