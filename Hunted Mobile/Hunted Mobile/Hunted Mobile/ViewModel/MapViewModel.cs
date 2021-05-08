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
using Hunted_Mobile.Enum;
using Hunted_Mobile.View;
using System.Linq;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel : BaseViewModel {
        private const int LOOT_PICKUP_TIME_IN_SECONDES = 5,
            ARREST_THIEF_TIME_IN_SECONDES = 5,
            LOOT_PICKUP_MAX_DISTANCE_IN_METERS = 10,
            POLICE_ARREST_DISTANCE_IN_METERS = 10,
            PICK_UP_LOOT_SCORE = 50,
            ARREST_THIEF_SCORE = 200;

        const string PAUSE_TITLE = "Gepauzeerd",
            END_TITLE = "Het spel is afgelopen!",
            PAUSE_DESCRIPTION = "Momenteel is het spel gepauzeerd door de spelleider. Wanneer de pauze voorbij is, zal het spel weer hervat worden.",
            END_DESCRIPTION = "Ga terug naar de spelleider!",
            LOOT_TAG = "loot",
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
        private Timer lootTimer;
        private Timer arrestingTimer;
        private Pin playerPin;
        private Thief thiefToBeArrested;
        private bool isEnabled = true;
        private bool gameHasEnded = false;
        private bool isHandlingLoot = false;
        private bool openMainMapMenu = false;
        private bool mainMapMenuButtonVisible = true;
        private bool hasFinishedHandlingLoot = false;
        private bool hasFinishedArrestingThief = false;
        private bool isArrestingThief = false;
        private Resource chatIcon;
        private readonly Resource policeBadgeIcon;
        private readonly Resource moneyBagIcon;
        private String selectedMainMenuOption = "";

        private Countdown _countdown;
        private int _hours;
        private int _minutes;
        private int _seconds;
        private bool initialTimerStart = true;
        private DateTime dateTimeNow;

        public int Hours {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        public int Minutes {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }

        public int Seconds {
            get => _seconds;
            set => SetProperty(ref _seconds, value);
        }

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
                selectedLoot = value != null ? value : new Loot(0);

                OnPropertyChanged("SelectedLoot");
                OnPropertyChanged(nameof(IsCloseToSelectedLoot));
                OnPropertyChanged(nameof(IsFarFromSelectedLoot));
            }
        }

        public Thief ThiefToBeArrested {
            get => thiefToBeArrested;
            set {
                thiefToBeArrested = value;
                OnPropertyChanged("ThiefToBeArrested");
            }
        }

        public bool IsHandlingLoot {
            get => isHandlingLoot;
            set {
                isHandlingLoot = value;
                if(value) HasFinishedHandlingLoot = false;

                OnPropertyChanged("IsHandlingLoot");
                OnPropertyChanged(nameof(IsCloseToSelectedLoot));
                OnPropertyChanged(nameof(IsFarFromSelectedLoot));
            }
        }

        public bool IsArrestingThief {
            get => isArrestingThief;
            set {
                isArrestingThief = value;
                if(value) HasFinishedArrestingThief = false;

                OnPropertyChanged("IsArrestingThief");
                OnPropertyChanged("IsCloseToSelectedThief");
                OnPropertyChanged("IsFarFromSelectedThief");
            }
        }

        public bool HasFinishedHandlingLoot {
            get => hasFinishedHandlingLoot;
            set {
                hasFinishedHandlingLoot = value;
                if(value) IsHandlingLoot = false;

                OnPropertyChanged("HasFinishedHandlingLoot");
            }
        }

        public bool HasFinishedArrestingThief {
            get => hasFinishedArrestingThief;
            set {
                hasFinishedArrestingThief = value;
                if(value) IsArrestingThief = false;
                OnPropertyChanged("HasFinishedArrestingThief");
            }
        }

        public string SelectedMainMenuOption {
            get => selectedMainMenuOption;
            set {
                selectedMainMenuOption = value;

                OnPropertyChanged("SelectedMainMenuOption");
            }
        }

        public bool IsCloseToSelectedLoot {
            get {
                if(IsHandlingLoot && mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && SelectedLoot != null) {
                    return mapModel.PlayingUser.Location.DistanceToOtherInMeters(SelectedLoot.Location) <= LOOT_PICKUP_MAX_DISTANCE_IN_METERS;
                }
                return false;
            }
        }

        public bool IsCloseToSelectedThief {
            get {
                if(IsArrestingThief && mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && ThiefToBeArrested != null) {
                    return mapModel.PlayingUser.Location.DistanceToOtherInMeters(ThiefToBeArrested.Location) <= POLICE_ARREST_DISTANCE_IN_METERS;
                }
                return false;
            }
        }

        public bool IsFarFromSelectedLoot => !IsCloseToSelectedLoot;
        public bool IsFarFromSelectedThief => !IsCloseToSelectedThief;
        public bool VisibleOverlay => !IsEnabled;
        public bool Initialized { get; private set; }
        public string TitleOverlay => GameHasEnded ? END_TITLE : PAUSE_TITLE;
        public string DescriptionOverlay => GameHasEnded ? END_DESCRIPTION : PAUSE_DESCRIPTION;

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
            _countdown = new Countdown();
            dateTimeNow = DateTime.Now;
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

        private void HandlePinClicked(object sender, PinClickedEventArgs args) {
            string tag = $"{args.Pin.Tag}";

            if(tag == LOOT_TAG && mapModel.PlayingUser is Thief) {
                OnLootClicked(args.Pin.Position);
            }
            else if(tag == THIEF_TAG && mapModel.PlayingUser is Police) {
                OnThiefClicked(args.Pin.Position);
            }
        }

        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(messagesView);
        });

        /// <summary>
        /// Navigate to the RootPage
        /// </summary>
        public ICommand ExitGameCommand => new Xamarin.Forms.Command(async (e) => {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new MainPage());
            RemovePreviousNavigation();
            await webSocketService.Disconnect();
        });

        public ICommand NavigateToPlayersOverviewCommand => new Xamarin.Forms.Command((e) => {
            SelectedMainMenuOption = MainMenuOptions.DisplayUsersOption;
            NavigateToPlayersOverview();
        });

        public ICommand PickupLootCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedHandlingLoot = true;
        });

        public ICommand ClosePickingLootCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedHandlingLoot = false;
        });

        public ICommand CancelPickUpLootCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedHandlingLoot = false;
            IsHandlingLoot = false;
        });

        public ICommand ArrestThiefCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedArrestingThief = true;
        });

        public ICommand ClosArrestingThiefCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedArrestingThief = false;
        });

        public ICommand CancelArrestingThiefCommand => new Xamarin.Forms.Command((e) => {
            HasFinishedArrestingThief = false;
            IsArrestingThief = false;
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

        public ICommand Button_PressedArrestingThief => new Xamarin.Forms.Command((e) => {
            arrestingTimer = new Timer();

            // Interval is set with milisecondes
            arrestingTimer.Interval = ARREST_THIEF_TIME_IN_SECONDES * 1000;
            arrestingTimer.Elapsed += SuccessfullyArrestThief;
            arrestingTimer.Start();
        });

        public ICommand Button_ReleasedArrestingThief => new Xamarin.Forms.Command((e) => {
            if(arrestingTimer != null) {
                arrestingTimer.Stop();
                arrestingTimer = null;
            }
        });

        private void SuccessfullyPickedUpLoot(object sender, EventArgs e) {
            lootTimer.Stop();
            lootTimer = null;
            HasFinishedHandlingLoot = true;

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
            arrestingTimer.Stop();
            arrestingTimer = null;
            HasFinishedArrestingThief = true;

            Task.Run(async () => {
                // Get latest score of game (in feature this should be replaced with socket event)
                gameModel = await gameRepository.GetGame(gameModel.Id);
                gameModel.PoliceScore += ARREST_THIEF_SCORE;

                bool isCaught = await userRepository.CatchThief(ThiefToBeArrested.Id);
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
            if(initialTimerStart) {
                _countdown.EndDate = gameModel.EndTime;
                initialTimerStart = false;
            }
            else {
                _countdown.EndDate = DateTime.Now.AddSeconds(timeLeft);
            }
            _countdown.Start();

            _countdown.Ticked += OnCountdownTicked;
            _countdown.Completed += OnCountdownCompleted;
        }

        public void StopCountdown() {
            _countdown.Ticked -= OnCountdownTicked;
            _countdown.Completed -= OnCountdownCompleted;
        }

        void OnCountdownTicked() {
            Hours = _countdown.RemainTime.Hours;
            Minutes = _countdown.RemainTime.Minutes;
            Seconds = _countdown.RemainTime.Seconds;

            var totalSeconds = (gameModel.EndTime - dateTimeNow).TotalSeconds;
            var remainSeconds = _countdown.RemainTime.TotalSeconds;
        }

        void OnCountdownCompleted() {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
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

                mapView.PinClicked += HandlePinClicked;

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
            GameHasEnded = true;
            IsEnabled = false;

            StopIntervalTimer();
        }

        private void PauseGame(JObject data) {
            IsEnabled = false;
            StopCountdown();

            StopIntervalTimer();
        }

        private void ResumeGame(JObject data) {
            IsEnabled = true;
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
            Mapsui.UI.Forms.Position mapsuiPosition = new Mapsui.UI.Forms.Position(newLocation.Latitude, newLocation.Longitude);
            mapView.MyLocationLayer.UpdateMyLocation(mapsuiPosition, true);

            DisplayPlayerPin();

            OnPropertyChanged(nameof(IsCloseToSelectedLoot));
            OnPropertyChanged(nameof(IsFarFromSelectedLoot));

            if(!Initialized) {
                await userRepository.Update(mapModel.PlayingUser.Id, mapModel.PlayingUser.Location);
                Initialized = true;
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

        private void NavigateToPlayersOverview() {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(playersOverview);
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
                if(user.Location != null && user.UserName != null && !user.IsCaught && mapModel.PlayingUser.GetType() == user.GetType()) {

                    mapView.Pins.Add(new Pin(mapView) {
                        Label = user.UserName,
                        Color = user is Thief ? thiefPinColor : policePinColor,
                        Position = new Mapsui.UI.Forms.Position(user.Location.Latitude, user.Location.Longitude),
                        Scale = 0.666f,
                        Tag = user is Thief ? THIEF_TAG : null,
                        Transparency = 0.25f,
                    });
                }
            }

            // Closest thief for player
            List<Player> thiefs = new List<Player>();
            Player closestThief = new Player();
            bool firstThief = true;

            if(mapModel.PlayingUser is Police) {
                foreach(var user in mapModel.GetUsers()) {
                    if(user is Thief) {
                        thiefs.Add(user);
                    }
                }

                foreach(var thief in thiefs) {
                    if(firstThief) {
                        closestThief = thief;
                        firstThief = false;
                    }

                    if(mapModel.PlayingUser.Location.DistanceToOtherInMeters(thief.Location) < mapModel.PlayingUser.Location.DistanceToOtherInMeters(closestThief.Location)) {
                        closestThief = thief;
                    }
                }

                if(thiefs.Count != 0) {
                    mapView.Pins.Add(new Pin(mapView) {
                        Label = closestThief.UserName,
                        Color = Xamarin.Forms.Color.Red,
                        Position = new Mapsui.UI.Forms.Position(closestThief.Location.Latitude, closestThief.Location.Longitude),
                        Scale = 0.666f,
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

        private void OnLootClicked(Position position) {
            SelectedLoot = mapModel.FindLoot(new Location(position));
            IsHandlingLoot = SelectedLoot != null;
        }

        private void OnThiefClicked(Position position) {
            thiefToBeArrested = mapModel.FindThief(new Location(position));
            IsArrestingThief = thiefToBeArrested != null;
        }
    }
}
