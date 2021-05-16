using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using MapsuiPosition = Mapsui.UI.Forms.Position;
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
using Hunted_Mobile.View;
using System.Linq;
using Hunted_Mobile.Service.Map;
using Hunted_Mobile.Enum;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel : BaseViewModel {
        private const int LOOT_PICKUP_TIME_IN_SECONDES = 5,
            ARREST_THIEF_TIME_IN_SECONDES = 5,
            LOOT_PICKUP_MAX_DISTANCE_IN_METERS = 10,
            POLICE_ARREST_DISTANCE_IN_METERS = 10,
            PICK_UP_LOOT_SCORE = 50,
            ARREST_THIEF_SCORE = 200;

        const string LOOT_TAG = "loot",
            THIEF_TAG = "thief";

        private readonly Xamarin.Forms.Color policePinColor = Xamarin.Forms.Color.FromRgb(39, 96, 203);
        private readonly Xamarin.Forms.Color thiefPinColor = Xamarin.Forms.Color.Black;
        private readonly Model.Map mapModel;
        private readonly LootRepository lootRepository;
        private readonly UserRepository userRepository;
        private readonly InviteKeyRepository inviteKeyRepository;
        private readonly BorderMarkerRepository borderMarkerRepository;
        private readonly GameRepository gameRepository;
        private readonly GpsService gpsService;
        private readonly WebSocketService webSocketService;
        private Loot selectedLoot = new Loot(0);
        private Game gameModel;
        private MapView mapView;
        private readonly View.Messages messagesView;
        private View.PlayersOverviewPage playersOverview;
        private Timer intervalUpdateTimer;
        private Timer holdingButtonTimer;
        private Pin playerPin;
        private Thief selectedThief;
        private bool openMainMapMenu = false;
        private bool mainMapMenuButtonVisible = true;
        private bool isHoldingButton = false;
        private readonly Resource chatIcon;
        private readonly Resource policeBadgeIcon;
        private readonly Resource moneyBagIcon;
        private readonly Countdown countdown;

        public string CounterDisplay => countdown.RemainTime.ToString(@"hh\:mm\:ss");
        public MapDialog MapDialog { get; private set; } = new MapDialog();

        public MapDialogOptions MapDialogOption {
            get => MapDialog.SelectedDialog;
            set {
                MapDialog.SelectedDialog = value;
                ToggleEnableStatusOnMapView();
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
            }
        }

        public Thief SelectedThief {
            get => selectedThief;
            set {
                selectedThief = value;
                OnPropertyChanged("SelectedThief");
            }
        }

        public bool IsHoldingButton {
            get => isHoldingButton;
            set {
                isHoldingButton = value;
            }
        }

        public bool IsCloseToSelectedLoot {
            get {
                if(IsHoldingButton && mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && SelectedLoot != null) {
                    return mapModel.PlayingUser.Location.DistanceToOtherInMeters(SelectedLoot.Location) <= LOOT_PICKUP_MAX_DISTANCE_IN_METERS;
                }
                return false;
            }
        }

        public bool IsCloseToSelectedThief {
            get {
                if(IsHoldingButton && mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && SelectedThief != null) {
                    return mapModel.PlayingUser.Location.DistanceToOtherInMeters(SelectedThief.Location) <= POLICE_ARREST_DISTANCE_IN_METERS;
                }
                return false;
            }
        }

        public bool Initialized { get; private set; }

        public int PlayingUserScore {
            get {
                if(mapModel != null && gameModel != null) {
                    return mapModel.PlayingUser is Thief ? gameModel.ThievesScore : gameModel.PoliceScore;
                }
                return 0;
            }
        }

        public string PlayingUserScoreDisplay => "Score: " + PlayingUserScore;

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
            webSocketService = new WebSocketService(gameModel.Id);
            playersOverview = new View.PlayersOverviewPage(new PlayersOverviewViewModel(new List<Player>() { mapModel.PlayingUser }, webSocketService));
            this.lootRepository = lootRepository;
            this.userRepository = userRepository;
            this.gameRepository = gameRepository;
            this.inviteKeyRepository = inviteKeyRepository;
            this.borderMarkerRepository = borderMarkerRepository;
            countdown = new Countdown();
            StartCountdown(0);

            chatIcon = resourceRepository.GetGuiImage("chat.png");
            OnPropertyChanged(nameof(ChatIcon));
            policeBadgeIcon = resourceRepository.GetMapImage("police-badge.png");
            moneyBagIcon = resourceRepository.GetMapImage("money-bag.png");

            if(gameModel.Status == GameStatus.PAUSED) {
                PauseGame(null);
            }
            if(gameModel.Status == GameStatus.FINISHED) {
                EndGame(null);
            }
        }

        private void HandlePinClickedCommand(object sender, PinClickedEventArgs args) {
            string tag = $"{args.Pin.Tag}";

            if(tag == LOOT_TAG && mapModel.PlayingUser is Thief) {
                OnLootClicked(args.Pin.Position);
            }
            else if(tag == THIEF_TAG && mapModel.PlayingUser is Police) {
                OnThiefClicked(args.Pin.Position);
            }
        }

        public ICommand NavigateToPlayersOverviewCommand => new Xamarin.Forms.Command((e) => NavigateToPlayersOverview());

        public ICommand NavigateToMessagePageCommand => new Command((e) => NavigateToMessagePage());


        public ICommand ReleasingMapDialogActionButtonCommand => new Xamarin.Forms.Command((e) => {
            if(holdingButtonTimer != null) {
                holdingButtonTimer.Stop();
                holdingButtonTimer = null;
            }

            IsHoldingButton = false;
        });

        public ICommand HoldingMapDialogActionButtonCommand => new Xamarin.Forms.Command((e) => {
            IsHoldingButton = true;
            holdingButtonTimer = new Timer();

            // Interval is set with milisecondes
            if(MapDialogOption == MapDialogOptions.DISPLAY_PICKUP_LOOT) {
                holdingButtonTimer.Interval = LOOT_PICKUP_TIME_IN_SECONDES * 1000;
                holdingButtonTimer.Elapsed += SuccessfullyPickedUpLoot;
            }
            else if(MapDialogOption == MapDialogOptions.DISPLAY_ARREST_THIEF) {
                holdingButtonTimer.Interval = ARREST_THIEF_TIME_IN_SECONDES * 1000;
                holdingButtonTimer.Elapsed += SuccessfullyArrestThief;
            }

            holdingButtonTimer.Start();
        });

        public ICommand PressedMapDialogActionButtonCommand => new Xamarin.Forms.Command((e) => {
            if(MapDialogOption == MapDialogOptions.DISPLAY_ARREST_THIEF_SUCCESFULLY || MapDialogOption == MapDialogOptions.DISPLAY_PICKUP_LOOT_SUCCESFULLY) {
                MapDialogOption = MapDialogOptions.NONE;
            }
            else if(MapDialogOption == MapDialogOptions.DISPLAY_END_GAME) {
                ExitGame();
            }
        });

        public ICommand CloseMapDialogCommand => new Xamarin.Forms.Command((e) => {
            IsHoldingButton = false;
            MapDialogOption = MapDialogOptions.NONE;
        });

        public ICommand OpenMainMapMenuCommand => new Xamarin.Forms.Command((e) => {
            OpenMainMapMenu = true;
        });

        public ICommand CloseMainMapMenuCommand => new Xamarin.Forms.Command((e) => {
            OpenMainMapMenu = false;
        });

        private void SuccessfullyPickedUpLoot(object sender, EventArgs e) {
            holdingButtonTimer.Stop();
            holdingButtonTimer = null;
            MapDialogOption = MapDialogOptions.DISPLAY_PICKUP_LOOT_SUCCESFULLY;
            MapDialog.DisplayPickedUpLootSuccessfully(SelectedLoot.Name);

            Task.Run(async () => {
                // Get latest score of game (in feature this should be replaced with socket event)
                gameModel = await gameRepository.GetGame(gameModel.Id);
                gameModel.ThievesScore += PICK_UP_LOOT_SCORE;

                // User should be a thief here since a police can't open the dialog
                bool deleted = await lootRepository.Delete(SelectedLoot.Id);
                if(deleted) {
                    await gameRepository.UpdateThievesScore(gameModel.Id, gameModel.ThievesScore);

                    OnPropertyChanged(nameof(PlayingUserScore));
                    OnPropertyChanged(nameof(PlayingUserScoreDisplay));
                    await PollLoot();
                    DisplayOtherPins();
                }
            });
        }

        private void SuccessfullyArrestThief(object sender, EventArgs e) {
            holdingButtonTimer.Stop();
            holdingButtonTimer = null;
            MapDialogOption = MapDialogOptions.DISPLAY_ARREST_THIEF_SUCCESFULLY;
            MapDialog.DisplayArrestedThiefSuccessfully(SelectedThief.UserName);

            Task.Run(async () => {
                // Get latest score of game (in feature this should be replaced with socket event)
                gameModel = await gameRepository.GetGame(gameModel.Id);
                gameModel.PoliceScore += ARREST_THIEF_SCORE;

                bool isCaught = await userRepository.CatchThief(SelectedThief.Id);
                if(isCaught) {
                    await gameRepository.UpdatePoliceScore(gameModel.Id, gameModel.PoliceScore);
                    OnPropertyChanged(nameof(PlayingUserScore));
                    OnPropertyChanged(nameof(PlayingUserScoreDisplay));
                }
            });
        }


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

            playersOverview = new View.PlayersOverviewPage(
                new PlayersOverviewViewModel(
                    new List<Player>(userList) { mapModel.PlayingUser },
                    webSocketService
                )
            );
        }

        public void StartCountdown(double timeLeft) {
            if(countdown.InitialTimerStart) {
                countdown.EndDate = gameModel.EndTime;
                countdown.InitialTimerStart = false;
            }
            else {
                countdown.EndDate = DateTime.Now.AddSeconds(timeLeft);
            }

            countdown.Start();
            countdown.Ticked += OnCountdownTicked;
            countdown.Completed += OnCountdownCompleted;
        }

        public void StopCountdown() {
            countdown.Ticked -= OnCountdownTicked;
            countdown.Completed -= OnCountdownCompleted;
        }

        void OnCountdownTicked() {
            OnPropertyChanged(nameof(CounterDisplay));
        }

        void OnCountdownCompleted() {
            countdown.RemainTime = new TimeSpan(0, 0, 0);
        }

        private void IntervalOfGame(JObject data) {
            StartIntervalTimer();

            List<Player> userList = new List<Player>();

            foreach(JObject user in data.GetValue("users")) {
                int userId = -1;
                int.TryParse((string) user.GetValue("id"), out userId);

                if(userId != mapModel.PlayingUser.Id) {
                    Location location = new Location((string) user.GetValue("location"));
                    bool wasThief = (mapModel.GetUserById(userId) is Thief);
                    Player newUser = new Player();
                    if(user.GetValue("role").ToString() == "thief") {
                        newUser = new Thief(newUser);
                    }
                    else newUser = new Police(newUser);

                    newUser.Id = userId;
                    newUser.UserName = ((string) user.GetValue("username"));
                    newUser.Location = location;

                    userList.Add(newUser);
                }
            }

            List<Loot> lootList = new List<Loot>();

            foreach(JObject loot in data.GetValue("loot")) {
                int id = int.Parse(loot.GetValue("id")?.ToString());
                Location location = new Location(loot.GetValue("location")?.ToString());

                Loot newLoot = new Loot(id);
                newLoot.Name = loot.GetValue("name")?.ToString();
                newLoot.Location = location;

                lootList.Add(newLoot);
            }

            mapModel.SetUsers(userList);
            mapModel.SetLoot(lootList);

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

        private void RemovePreviousNavigation() {
            var navigation = Application.Current.MainPage.Navigation;
            while(navigation.NavigationStack.Count > 1) {
                navigation.RemovePage(navigation.NavigationStack.First());
            }
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

                await StartSocket();

                StartIntervalTimer();

                mapView.PinClicked += HandlePinClickedCommand;

                RemovePreviousNavigation();
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
                if(!WebSocketService.Connected) {
                    await webSocketService.Connect();
                }

                webSocketService.ResumeGame += ResumeGame;
                webSocketService.PauseGame += PauseGame;
                webSocketService.EndGame += EndGame;
                webSocketService.ThiefCaught += ThiefStatusChanged;
                webSocketService.ThiefReleased += ThiefStatusChanged;
                webSocketService.IntervalEvent += IntervalOfGame;
            }
            catch(Exception ex) {
                Console.WriteLine("An error occurred when connecting the web socket: " + ex.StackTrace);
            }
        }

        private void EndGame(JObject data) {
            MapDialogOption = MapDialogOptions.DISPLAY_END_GAME;
            MapDialog.DisplayEndScreen();

            StopIntervalTimer();
        }

        private void PauseGame(JObject data) {
            MapDialogOption = MapDialogOptions.DISPLAY_PAUSE;
            MapDialog.DisplayPauseScreen();
            StopCountdown();

            StopIntervalTimer();
        }

        private void ResumeGame(JObject data) {
            MapDialogOption = MapDialogOptions.NONE;
            StartCountdown(Convert.ToDouble(data.GetValue("timeLeft")));
            StartIntervalTimer();
        }

        private void ThiefStatusChanged(JObject data) {
            Task.Run(async () => {
                await PollUsers();
                DisplayOtherPins();
            });
        }

        /// <summary>
        /// Action to execute when the device location has updated
        /// </summary>
        private async void MyLocationUpdated(Location newLocation) {
            mapModel.PlayingUser.Location = newLocation;

            // Send update to the map view
            MapsuiPosition position = new MapsuiPosition(newLocation.Latitude, newLocation.Longitude);
            mapView.MyLocationLayer.UpdateMyLocation(position, true);

            DisplayPlayerPin();
            OnPropertyChanged(nameof(IsCloseToSelectedLoot));

            if(!Initialized) {
                await userRepository.Update(mapModel.PlayingUser.Id, mapModel.PlayingUser.Location);
                Initialized = true;
            }
        }

        private void CenterMapOnLocation(Location center, double zoomResolution) {
            Mapsui.Geometries.Point centerPoint = new MapsuiPosition(center.Latitude, center.Longitude).ToMapsui();
            mapView.Navigator.CenterOn(centerPoint);
            mapView.Navigator.NavigateTo(centerPoint, zoomResolution);
        }

        /// <summary>
        /// Ensures the map panning is limited to given number around a given center location
        /// </summary>
        private void LimitMapViewport(Location center, int limit = 100000) {
            mapView.Map.Limiter = new ViewportLimiterKeepWithin();
            Mapsui.Geometries.Point centerPoint = new MapsuiPosition(center.Latitude, center.Longitude).ToMapsui();
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

            playerPin.Position = new MapsuiPosition(mapModel.PlayingUser.Location.Latitude, mapModel.PlayingUser.Location.Longitude);

            if(!mapView.Pins.Contains(playerPin)) {
                mapView.Pins.Add(playerPin);
            }
        }

        private void NavigateToPlayersOverview() {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(playersOverview);
        }

        private void NavigateToMessagePage() {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(messagesView);
        }

        /// <summary>
        /// Displays pins for all game objects with a location
        /// </summary>
        private void DisplayOtherPins() {
            mapView.Pins.Clear();

            if(playerPin != null) {
                mapView.Pins.Add(playerPin);
            }

            // Players
            foreach(var user in mapModel.GetUsers()) {
                if(!user.IsCaught && mapModel.PlayingUser.GetType() == user.GetType()) {
                    mapView.Pins.Add(new Pin(mapView) {
                        Label = user.UserName,
                        Color = user is Thief ? thiefPinColor : policePinColor,
                        Position = new MapsuiPosition(user.Location.Latitude, user.Location.Longitude),
                        Scale = 0.666f,
                        Tag = user is Thief ? THIEF_TAG : null,
                        Transparency = 0.25f,
                    });
                }
            }

            // If current user has role as Police
            if(mapModel.PlayingUser is Police) {
                Player closestThief = null;

                foreach(var thief in mapModel.Thiefs) {
                    if(closestThief == null) {
                        closestThief = thief;
                    }
                    else if(mapModel.PlayingUser.Location.DistanceToOtherInMeters(thief.Location) < mapModel.PlayingUser.Location.DistanceToOtherInMeters(closestThief.Location)) {
                        closestThief = thief;
                    }
                }

                if(closestThief != null) {
                    mapView.Pins.Add(new Pin(mapView) {
                        Label = closestThief.UserName,
                        Color = Xamarin.Forms.Color.Red,
                        Position = new MapsuiPosition(closestThief.Location.Latitude, closestThief.Location.Longitude),
                        Scale = 0.666f,
                        Tag = THIEF_TAG,
                        Transparency = 0.25f,
                    });
                }
            }

            // Loot
            foreach(var loot in mapModel.GetLoot()) {
                mapView.Pins.Add(new Pin(mapView) {
                    Label = loot.Name,
                    Position = new MapsuiPosition(loot.Location.Latitude, loot.Location.Longitude),
                    Scale = 1.0f,
                    Tag = LOOT_TAG,
                    Icon = moneyBagIcon.Data,
                    Type = PinType.Icon,
                });
            }

            // Police station
            if(gameModel.PoliceStationLocation != null) {
                mapView.Pins.Add(new Pin(mapView) {
                    Label = "Politie station",
                    Position = new MapsuiPosition(gameModel.PoliceStationLocation.Latitude, gameModel.PoliceStationLocation.Longitude),
                    Scale = 1.0f,
                    Icon = policeBadgeIcon.Data,
                    Type = PinType.Icon,
                });
            }
        }

        private void OnLootClicked(Position position) {
            SelectedLoot = mapModel.FindLoot(new Location(position));
            MapDialogOption = MapDialogOptions.DISPLAY_PICKUP_LOOT;
            MapDialog.DisplayPickingUpLoot(SelectedLoot.Name);
        }

        private void OnThiefClicked(Position position) {
            SelectedThief = mapModel.FindThief(new Location(position));
            MapDialogOption = MapDialogOptions.DISPLAY_ARREST_THIEF;
            MapDialog.DisplayArrestingThief(SelectedThief.UserName);
        }

        private void ToggleEnableStatusOnMapView() {
            mapView.Content.IsEnabled = MapDialogOption == MapDialogOptions.NONE;
        }

        private void ExitGame() {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new MainPage());
            RemovePreviousNavigation();
            webSocketService.Disconnect();
        }
    }
}
