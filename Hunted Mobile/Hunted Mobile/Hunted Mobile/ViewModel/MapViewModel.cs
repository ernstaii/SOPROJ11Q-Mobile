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
using Hunted_Mobile.View;
using System.Linq;
using Hunted_Mobile.Model.Response;
using Hunted_Mobile.Service.Map;
using Hunted_Mobile.Enum;
using Hunted_Mobile.Service.Preference;
using Hunted_Mobile.Service.Builder;
using Hunted_Mobile.Model.GameModels.Gadget;

namespace Hunted_Mobile.ViewModel {
    public class MapViewModel : BaseViewModel {
        private const int LOOT_PICKUP_TIME_IN_SECONDES = 5,
            ARREST_THIEF_TIME_IN_SECONDES = 5,
            LOOT_PICKUP_MAX_DISTANCE_IN_METERS = 20,
            POLICE_ARREST_DISTANCE_IN_METERS = 20,
            PICK_UP_LOOT_SCORE = 1,
            ARREST_THIEF_SCORE = 1;

        const string LOOT_TAG = "loot",
            THIEF_TAG = PlayerRole.THIEF;

        private readonly Model.Map mapModel;
        private GpsService gpsService;
        private readonly WebSocketService webSocketService;
        private Loot selectedLoot = new Loot();
        private Game gameModel;
        private readonly View.Messages messagesView;
        private readonly MessageViewModel messageViewModel;
        private PlayersOverviewPage playersOverview;
        private readonly InformationPage informationPage;
        private readonly GadgetsPage gadgetsOverview;
        private readonly GadgetOverviewViewModel gadgetOverviewViewModel;
        private Timer intervalUpdateTimer;
        private Timer holdingButtonTimer;
        private Thief selectedThief;
        private bool openMainMapMenu = false;
        private bool mainMapMenuButtonVisible = true;
        private readonly Countdown countdown;
        private MapViewService mapViewService;
        private readonly DateTime dateTimeNow;
        private string logoImage;
        private bool roleToggle = false;

        #region Properties
#pragma warning disable IDE1006 // Naming Styles
        private MapView mapView {
            get => mapViewService?.MapView;
            set {
                if(mapViewService != null) {
                    mapViewService.MapView = value;
                }
            }
        }
#pragma warning restore IDE1006 // Naming Styles

        public string CounterDisplay => countdown.RemainTime.ToString(@"hh\:mm\:ss");
        public MapDialog MapDialog { get; private set; }
        public MapIconsService Icons { get; } = new MapIconsService();

        public MapDialogOptions MapDialogOption {
            get => MapDialog.SelectedDialog;
            set {
                MapDialog.SelectedDialog = value;
                ToggleEnableStatusOnMapView();
            }
        }
        public string LogoImage {
            get => logoImage;
            set {
                logoImage = value;
                OnPropertyChanged("LogoImage");
            }
        }

        public Game GameModel {
            get => gameModel;
            set {
                gameModel = value;
                appViewModel.ColourTheme = gameModel.ColourTheme;
                OnPropertyChanged("GameModel");
            }
        }

        private readonly AppViewModel appViewModel;

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

        public bool IsCloseToSelectedLoot {
            get {
                if(mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && SelectedLoot != null) {
                    return mapModel.PlayingUser.Location.DistanceToOtherInMeters(SelectedLoot.Location) <= LOOT_PICKUP_MAX_DISTANCE_IN_METERS;
                }
                return false;
            }
        }

        public bool IsCloseToSelectedThief {
            get {
                if(mapModel != null && mapModel.PlayingUser != null && mapModel.PlayingUser.Location != null && SelectedThief != null) {
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

        public bool DroneActive { get; private set; }
        #endregion

        public MapViewModel(Game gameModel, Model.Map mapModel, AppViewModel appViewModel) {
            MapDialog = new MapDialog();
            this.mapModel = mapModel;
            this.appViewModel = appViewModel;
            GameModel = gameModel;
            var gameIdStr = gameModel.Id.ToString();
            messageViewModel = new MessageViewModel(gameIdStr, gameModel.ColourTheme);
            messagesView = new View.Messages(messageViewModel);
            webSocketService = new WebSocketService(gameIdStr);
            playersOverview = new View.PlayersOverviewPage(new PlayersOverviewViewModel(new List<Player>() { mapModel.PlayingUser }, webSocketService));
            informationPage = new InformationPage(new InformationPageViewModel(gameModel.ColourTheme, Icons));
            gadgetOverviewViewModel = new GadgetOverviewViewModel(webSocketService, mapModel, gameModel.ColourTheme, gameModel.Id);
            gadgetsOverview = new View.GadgetsPage(gadgetOverviewViewModel);
            countdown = new Countdown();
            dateTimeNow = DateTime.Now;
            Task.Run(async () => await UpdatePlayerLocation());
            BeforeStartCountdown();
            StartCountdown(0);
            SetGameLogo();

            if(gameModel.Status == GameStatus.PAUSED) {
                PauseGame(null);
            }
            if(gameModel.Status == GameStatus.FINISHED) {
                EndGame(null);
            }
        }

        private void HandlePinClickedCommand(object sender, PinClickedEventArgs args) {
            var pinTag = args.Pin?.Tag?.ToString() ?? "";
            bool containsId = pinTag.Contains(".");

            if(containsId && pinTag.Contains(LOOT_TAG) || containsId && pinTag.Contains(THIEF_TAG)) {
                int.TryParse(pinTag.Split('.')[1], out int id);

                if(pinTag.Contains(LOOT_TAG) && mapModel.PlayingUser is Thief) {
                    OnLootClicked(id);
                }
                else if(pinTag.Contains(THIEF_TAG) && mapModel.PlayingUser is Police) {
                    OnThiefClicked(id);
                }
            }
        }

        public ICommand ToggleRole => new Xamarin.Forms.Command((e) => {
            if(mapModel.PlayingUser is FakePolice) {
                roleToggle = !roleToggle;
                Icons.RoleName = roleToggle ? "police" : "fakepolice";
                DisplayAllPins();
            }
        });

        public ICommand NavigateToPlayersOverviewCommand => new Xamarin.Forms.Command((e) => NavigateToPlayersOverview());

        public ICommand NavigateToGadgetsOverviewCommand => new Xamarin.Forms.Command((e) => NavigateToGadgetsOverview());
        public ICommand NavigateToInformationPageCommand => new Xamarin.Forms.Command((e) => NavigateToInformationPage());

        public ICommand NavigateToMessagePageCommand => new Command((e) => NavigateToMessagePage());


        public ICommand ReleasingMapDialogActionButtonCommand => new Xamarin.Forms.Command((e) => {
            if(holdingButtonTimer != null) {
                holdingButtonTimer.Stop();
                holdingButtonTimer = null;
            }
        });

        public ICommand HoldingMapDialogActionButtonCommand => new Xamarin.Forms.Command((e) => {
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
            if(MapDialogOption == MapDialogOptions.DISPLAY_END_GAME) {
                ExitGame();
            }

            MapDialogOption = MapDialogOptions.NONE;
        });

        public ICommand CloseMapDialogCommand => new Xamarin.Forms.Command((e) => {
            MapDialogOption = MapDialogOptions.NONE;
        });

        public ICommand OpenMainMapMenuCommand => new Xamarin.Forms.Command((e) => {
            OpenMainMapMenu = true;
        });

        public ICommand CloseMainMapMenuCommand => new Xamarin.Forms.Command((e) => {
            OpenMainMapMenu = false;
        });

        private async void PreIntervalUpdate(object sender = null, ElapsedEventArgs args = null) {
            StopIntervalTimer();
            StartWarnForLateIntervalTimer(15);

            // Send the current user's location to the database
            await UpdatePlayerLocation();
        }

        private void SuccessfullyPickedUpLoot(object sender, EventArgs e) {
            holdingButtonTimer.Stop();
            holdingButtonTimer = null;
            MapDialogOption = MapDialogOptions.DISPLAY_PICKUP_LOOT_SUCCESFULLY;

            if(SelectedLoot == null) {
                DependencyService.Get<Toast>().Show("De buit is niet meer geselecteerd");
                return;
            }
            MapDialog.DisplayPickedUpLootSuccessfully(SelectedLoot.Name);

            Task.Run(async () => {
                // User should be a thief here since a police can't open the dialog
                bool deleted = await UnitOfWork.Instance.LootRepository.Delete(SelectedLoot.Id);
                if(deleted) {
                    await UnitOfWork.Instance.GameRepository.UpdateThievesScore(gameModel.Id, PICK_UP_LOOT_SCORE);
                }
            });
        }

        private void SuccessfullyArrestThief(object sender, EventArgs e) {

            holdingButtonTimer.Stop();
            holdingButtonTimer = null;

            if(SelectedThief == null) {
                DependencyService.Get<Toast>().Show("De dief is niet meer geselecteerd");
                MapDialogOption = MapDialogOptions.NONE;
                return;
            }

            MapDialogOption = MapDialogOptions.DISPLAY_ARREST_THIEF_SUCCESFULLY;
            MapDialog.DisplayArrestedThiefSuccessfully(SelectedThief.UserName);

            Task.Run(async () => {
                bool isCaught = await UnitOfWork.Instance.UserRepository.CatchThief(SelectedThief.Id);
                if(isCaught) {
                    await UnitOfWork.Instance.GameRepository.UpdatePoliceScore(gameModel.Id, ARREST_THIEF_SCORE);
                }
            });
        }


        private async Task PollLoot() {
            var lootList = await UnitOfWork.Instance.LootRepository.GetAll(gameModel.Id);
            mapModel.Loot = lootList;
        }

        private async Task PollUsers() {
            var userList = new List<Player>();
            foreach(Player user in await UnitOfWork.Instance.UserRepository.GetAll(gameModel.Id)) {
                if(user.Id != mapModel.PlayingUser.Id) {
                    userList.Add(user);
                }
            }
            mapModel.Players = userList;

            playersOverview = new PlayersOverviewPage(
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
            OnCountdownTicked();
        }

        private void SetGameLogo() {
            LogoImage = UnitOfWork.Instance.GameRepository.GetLogoUrl(gameModel.Id);

            OnPropertyChanged(nameof(LogoImage));
        }

        private async void ScoreUpdated(ScoreUpdatedEventData data) {
            gameModel.ThievesScore = data.ThiefScore;
            gameModel.PoliceScore = data.PoliceScore;

            OnPropertyChanged(nameof(PlayingUserScore));
            OnPropertyChanged(nameof(PlayingUserScoreDisplay));

            await PollLoot();
            await PollUsers();
            DisplayAllPins();
        }

        private void IntervalOfGame(IntervalEventData data) {
            StopIntervalTimer();
            StartIntervalTimer();

            DroneActive = data.DroneActive;

            Location playingUserLocation = mapModel.PlayingUser.Location;
            var newPlayer = new List<Player>();

            foreach(PlayerBuilder builder
                in mapModel.PlayingUser is Thief
                ? data.PlayerBuilders.Concat(data.SmokeScreenedPlayerBuilders)
                : data.PlayerBuilders) {
                var player = builder.ToPlayer();
                if(player != null) {
                    var gadgets = mapModel.Players.Where(p => p.Id == player.Id).FirstOrDefault()?.Gadgets;
                    if(gadgets != null) {
                        player.Gadgets = gadgets;
                    }

                    newPlayer.Add(player);

                    if(player.Id == mapModel.PlayingUser.Id) {
                        mapModel.PlayingUser = player;
                    }
                }
            }
            mapModel.PlayingUser.Location = playingUserLocation;

            mapModel.Players = newPlayer;
            mapModel.Loot = data.Loot;

            DisplayAllPins();
        }

        public void SetMapView(MapView mapView) {
            bool initializedBefore = this.mapView != null;
            mapViewService = new MapViewService(mapView, mapModel.PlayingUser, LOOT_PICKUP_MAX_DISTANCE_IN_METERS, Icons);

            if(!initializedBefore) {
                InitializeMap();
            }

            if(MapDialogOption != MapDialogOptions.DISPLAY_PAUSE && MapDialogOption != MapDialogOptions.DISPLAY_END_GAME) {
                DisplayCaughtScreenOrDisplayNone();
            }
        }

        private void RemovePreviousNavigation() {
            var navigation = Application.Current.MainPage.Navigation;
            while(navigation.NavigationStack.Count > 1) {
                navigation.RemovePage(navigation.NavigationStack.First());
            }
        }

        private void InitializeMap() {
            AddOsmLayerToMapView();

            Icons.RoleName = mapModel.PlayingUser.GetType().Name;

            Task.Run(async () => {
                await AddGameBoundary();
                LimitViewportToGame();

                gpsService = new GpsService(mapModel.GameBoundary.GetCenter(), mapModel.GameBoundary.GetDiameter());
                if(!gpsService.GpsHasStarted()) {
                    await gpsService.StartGps();
                }

                gpsService.LocationChanged += MyLocationUpdated;
                mapView.PinClicked += HandlePinClickedCommand;

                await StartSocket();
                StartIntervalTimer();
                
                await ReloadGadgets();
                RemovePreviousNavigation();
            });
        }

        private async Task ReloadGadgets() {
            var playersWithGadgets = await UnitOfWork.Instance.GadgetRepository.GetAll(gameModel.Id);
            if(playersWithGadgets != null && playersWithGadgets.Count > 0) {
                mapModel.Players.Clear();
                foreach(PlayerBuilder playerWithGadgets in playersWithGadgets) {
                    if(playerWithGadgets.Id == mapModel.PlayingUser.Id) {
                        mapModel.PlayingUser = playerWithGadgets.ToPlayer();
                    }
                    else {
                        mapModel.Players.Add(playerWithGadgets.ToPlayer());
                    }
                }
                DisplayAllPins();
            }
        }

        private void BeforeStartCountdown() {
            double diffInSecondes = (gameModel.StartTime - dateTimeNow).TotalSeconds;
            gameModel.EndTime.AddSeconds(-diffInSecondes);
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
            intervalUpdateTimer.Elapsed += PreIntervalUpdate;
            intervalUpdateTimer.Start();
        }

        private void StartWarnForLateIntervalTimer(float secondsToAwait) {
            if(intervalUpdateTimer == null) {
                intervalUpdateTimer = new Timer();
                intervalUpdateTimer.AutoReset = true;
            }
            intervalUpdateTimer.Interval = secondsToAwait * 1000;
            intervalUpdateTimer.Elapsed += (object sender, ElapsedEventArgs args) => {
                DependencyService.Get<Toast>().Show("Het verwachte spel interval was niet aangekomen");
            };
            if(!intervalUpdateTimer.Enabled) {
                intervalUpdateTimer.Start();
            }
        }

        private async Task StartSocket() {
            try {
                if(!WebSocketService.Online) {
                    await webSocketService.Connect();
                }

                webSocketService.ResumeGame += ResumeGame;
                webSocketService.PauseGame += PauseGame;
                webSocketService.EndGame += EndGame;
                webSocketService.ThiefCaught += ThiefStatusChanged;
                webSocketService.ThiefReleased += ThiefStatusChanged;
                webSocketService.IntervalEvent += IntervalOfGame;
                webSocketService.ScoreUpdated += ScoreUpdated;
                webSocketService.GadgetsUpdated += GadgetsUpdated;
                webSocketService.ThiefFakePoliceToggle += ThiefFakePoliceToggle;
            }
            catch(Exception e) {
                DependencyService.Get<Toast>().Show("(#1) Er was een probleem met het initialiseren van de web socket (MapViewModel)");
                UnitOfWork.Instance.ErrorRepository.Create(e);
            }
        }

        private void ThiefFakePoliceToggle(PlayerEventData data) {
            Thief updatingPlayer = mapModel.Thiefs.Where(player => player.Id == data.PlayerBuilder.Id).FirstOrDefault();

            if(updatingPlayer == null) {
                if(data.PlayerBuilder.Id == mapModel.PlayingUser.Id && mapModel.PlayingUser is Thief) {
                    updatingPlayer = mapModel.PlayingUser as Thief;
                }
            }

            if(updatingPlayer != null) {
                mapModel.Players.Remove(updatingPlayer);

                // At this point we know for sure the player is a thief so we can call ToThief
                updatingPlayer = data.PlayerBuilder.ToThief();

                mapModel.Players.Add(updatingPlayer);

                if(updatingPlayer.Id == mapModel.PlayingUser.Id) {
                    roleToggle = false;
                    Location myLocation = mapModel.PlayingUser.Location;
                    mapModel.PlayingUser = updatingPlayer;
                    mapViewService.Player = updatingPlayer;
                    mapViewService.UpdatePlayerPinLocation(myLocation);
                    DisplayAllPins();
                    Icons.RoleName = updatingPlayer is FakePolice ? "fakepolice" : "thief";
                }
            }
        }

        private void GadgetsUpdated(GadgetsUpdatedEventData data) {
            Player updatingPlayer = mapModel.Players.Where(player => player.Id == data.PlayerBuilder.Id).FirstOrDefault();

            if(updatingPlayer != null) {
                updatingPlayer.Gadgets = new List<Gadget>(data.Gadgets);
            }
        }

        private void EndGame(EventData data) {
            MapDialogOption = MapDialogOptions.DISPLAY_END_GAME;
            MapDialog.DisplayEndScreen();
            StopCountdown();

            StopIntervalTimer();
        }

        private void PauseGame(EventData data) {
            MapDialogOption = MapDialogOptions.DISPLAY_PAUSE;
            MapDialog.DisplayPauseScreen();
            StopCountdown();

            StopIntervalTimer();
        }

        private void ResumeGame(EventData data) {
            DisplayCaughtScreenOrDisplayNone();

            StartCountdown(data.TimeLeft);
            StartIntervalTimer();

            // Check if user is still in boundaries
            HandlePlayerBoundaries(WasWithinBoundary(), IsWithinBoundary(mapModel.PlayingUser.Location));
        }

        private void ThiefStatusChanged(PlayerEventData data) {
            CaughtPlayingUser(data);

            Task.Run(async () => {
                await PollUsers();
                DisplayAllPins();
            });
        }

        private void DisplayCaughtScreenOrDisplayNone() {
            try {
                if(mapModel.PlayingUser is Police) {
                    MapDialogOption = MapDialogOptions.NONE;
                    return;
                }

                var user = mapModel.PlayingUser as Thief;

                if(user.IsCaught) {
                    MapDialogOption = MapDialogOptions.DISPLAY_ARRESTED_SCREEN;
                    MapDialog.DisplayArrestedScreen();
                }
                else {
                    MapDialogOption = MapDialogOptions.NONE;
                }
            }
            catch(Exception e) {
                DependencyService.Get<Toast>().Show("Er is iets misgegaan met de weergave van de boef die is opgepakt.");
            }
        }

        private void CaughtPlayingUser(PlayerEventData data) {
            if(mapModel.PlayingUser.Id == data.PlayerBuilder.Id) {
                mapModel.PlayingUser = data.PlayerBuilder
                    .ToPlayer();

                DisplayCaughtScreenOrDisplayNone();
            }
        }

        private bool WasWithinBoundary() {
            // Unset coordinates are considered within bounds to prevent incorrect notifications
            return !mapModel.PlayingUser.Location.IsSet() || mapModel.GameBoundary.Contains(mapModel.PlayingUser.Location);
        }

        private bool IsWithinBoundary(Location newLocation) {
            return mapModel.GameBoundary.Contains(newLocation);
        }

        /// <summary>
        /// Action to execute when the device location has updated
        /// </summary>
        private async void MyLocationUpdated(Location newLocation) {
            bool wasWithinBoundary = WasWithinBoundary();
            bool isWithinBoundary = IsWithinBoundary(newLocation);
            mapModel.PlayingUser.Location = newLocation;

            // Send update to the map view
            MapsuiPosition position = new MapsuiPosition(newLocation.Latitude, newLocation.Longitude);
            mapView.MyLocationLayer.UpdateMyLocation(position, true);

            mapViewService.UpdatePlayerPinLocation(mapModel.PlayingUser.Location);
            OnPropertyChanged(nameof(IsCloseToSelectedLoot));

            if(!Initialized) {
                Initialized = true;
                await UpdatePlayerLocation();
            }

            HandlePlayerBoundaries(wasWithinBoundary, isWithinBoundary);

            await CheckAlarmTriggering();
        }

        private async Task UpdatePlayerLocation() {
            await UnitOfWork.Instance.UserRepository.Update(mapModel.PlayingUser.Id, mapModel.PlayingUser.Location);
        }

        private async void HandlePlayerBoundaries(bool wasWithinBoundary, bool isWithinBoundary) {
            var overwritableScreens = new MapDialogOptions[] {
                MapDialogOptions.NONE,
                MapDialogOptions.DISPLAY_ARREST_THIEF_SUCCESFULLY,
                MapDialogOptions.DISPLAY_PICKUP_LOOT_SUCCESFULLY,
            };

            // Only display the boundary screen if there is no other screen visible
            if(overwritableScreens.Contains(MapDialogOption) && !isWithinBoundary) {
                MapDialogOption = MapDialogOptions.DISPLAY_BOUNDARY_SCREEN;
                MapDialog.DisplayBoundaryScreen();
            }
            else if(isWithinBoundary && MapDialogOption == MapDialogOptions.DISPLAY_BOUNDARY_SCREEN) {
                DisplayCaughtScreenOrDisplayNone();
            }

            if(isWithinBoundary && !wasWithinBoundary) {
                await PostNotificationAboutPlayer(mapModel.PlayingUser.UserName + " bevindt zich weer binnen de spelgrenzen.");
            }
            else if(!isWithinBoundary && wasWithinBoundary) {
                await PostNotificationAboutPlayer(mapModel.PlayingUser.UserName + " heeft de spelgrenzen verlaten!");
            }
        }

        private async Task<bool> CheckAlarmTriggering() {
            if(mapModel.PlayingUser is Thief) {
                foreach(Player player in mapModel.Players) {
                    if(player.Gadgets != null) {
                        foreach(Alarm alarm in player.Gadgets.Where((gadget) => gadget is Alarm).Select((gadget) => (Alarm) gadget)) {
                            if(mapModel.PlayingUser.Location.DistanceToOtherInMeters(alarm.Location) < alarm.TriggerRangeInMeters) {
                                return await UnitOfWork.Instance.GadgetRepository.TriggerAlarm(mapModel.PlayingUser.Id);
                            }
                        }
                    }
                }
            }
            return false;
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
            mapView.Map.Limiter.ZoomLimits = new MinMax(0, limit * 0.002);
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
            Boundary boundary = await UnitOfWork.Instance.BorderMarkerRepository.GetBoundary(gameModel.Id);

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

        private void NavigateToPlayersOverview() {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(playersOverview);
        }

        private async void NavigateToGadgetsOverview() {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(gadgetsOverview);
            gadgetOverviewViewModel.Update();
        }

        private async void NavigateToInformationPage() {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(informationPage);
        }

        private void NavigateToMessagePage() {
            Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(messagesView);
        }

        private void PlayingUserToPolice() {
            mapModel.PlayingUser = new Police(
                mapModel.PlayingUser.Id,
                mapModel.PlayingUser.UserName,
                mapModel.PlayingUser.InviteKey,
                mapModel.PlayingUser.Location,
                mapModel.PlayingUser.Status,
                mapModel.PlayingUser.Gadgets,
                mapModel.PlayingUser.TriggeredAlarm
            );
        }

        /// <summary>
        /// Displays pins for all game objects with a location
        /// </summary>
        private void DisplayAllPins() {
            mapView.Pins.Clear();

            Player player = mapModel.Players.Where((p) => p.Id == mapModel.PlayingUser.Id).FirstOrDefault();
            if(roleToggle) {
                PlayingUserToPolice();
            }
            mapViewService.Player = mapModel.PlayingUser;

            foreach(var thief in mapModel.Thiefs) {
                mapViewService.AddTeamMatePin(thief);
            }

            foreach(var police in mapModel.Police) {
                mapViewService.AddTeamMatePin(police);
            }

            // If current user has role as Police
            if(mapModel.PlayingUser is Police) {
                mapViewService.AddPoliceStationPin(gameModel.PoliceStationLocation);
                foreach(Thief thief in mapModel.Thiefs) {
                    if(thief is FakePolice) {
                        // See fake police as normal police
                        mapViewService.AddPolicePin(thief);
                    }
                    else if(thief.TriggeredAlarm || DroneActive) {
                        mapViewService.AddThiefPin(thief);
                    }
                }
                if(!DroneActive && mapModel.Thiefs.Count > 0) {
                    var closestThief = GetClosestThief();
                    if(closestThief != null) {
                        mapViewService.AddThiefPin(closestThief);
                    }
                }
            }
            // If current user has role as Thief
            else {
                foreach(var loot in mapModel.Loot) {
                    mapViewService.AddLootPin(loot);
                }
            }

            mapModel.PlayingUser = player;
        }

        private Player GetClosestThief() {
            Player closestThief = null;

            foreach(var thief in mapModel.Thiefs) {
                // Fake police should not show up as thief
                if(!(thief is FakePolice)) {
                    if(closestThief == null) {
                        closestThief = thief;
                    }
                    else if(mapModel.PlayingUser.Location.DistanceToOtherInMeters(thief.Location) < mapModel.PlayingUser.Location.DistanceToOtherInMeters(closestThief.Location)) {
                        closestThief = thief;
                    }
                }
            }

            return closestThief;
        }

        private void OnLootClicked(int lootId) {
            SelectedLoot = mapModel.FindLoot(lootId);
            MapDialogOption = MapDialogOptions.DISPLAY_PICKUP_LOOT;
            MapDialog.DisplayPickingUpLoot(SelectedLoot.Name, IsCloseToSelectedLoot);
        }

        private void OnThiefClicked(int userId) {
            SelectedThief = mapModel.FindThief(userId);
            MapDialogOption = MapDialogOptions.DISPLAY_ARREST_THIEF;
            MapDialog.DisplayArrestingThief(SelectedThief.UserName, IsCloseToSelectedThief);
        }

        private void ToggleEnableStatusOnMapView() {
            if(mapView != null) {
                mapView.Content.IsEnabled = MapDialogOption == MapDialogOptions.NONE;
            }
        }

        private async void ExitGame() {
            GameSessionPreference.ClearUserAndGame();
            appViewModel.ResetColor();
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new MainPage(appViewModel));
            RemovePreviousNavigation();
            await webSocketService.Disconnect();
        }

        private async Task<bool> PostNotificationAboutPlayer(string message) {
            return await UnitOfWork.Instance.NotificationRepository.Create(
                message,
                gameModel.Id,
                mapModel.PlayingUser.Id
            );
        }
    }
}
