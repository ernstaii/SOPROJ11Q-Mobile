﻿using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Preference;
using Hunted_Mobile.View;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class LobbyViewModel : BaseViewModel {
        private List<Player> users = new List<Player>();
        private Game gameModel = new Game();
        private readonly Player currentUser;
        private readonly Lobby page;
        private readonly WebSocketService webSocketService;
        private readonly GameSessionPreference gameSessionPreference;
        private bool isloading;

        public Game GameModel {
            get => gameModel;
            set {
                gameModel = value;
                OnPropertyChanged("GameModel");
            }
        }

        public bool IsLoading {
            get => isloading;
            set {
                isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public List<Player> Users {
            get => users;
            set {
                users = value;
                OnPropertyChanged("Thiefs");
                OnPropertyChanged("Police");
            }
        }

        public ObservableCollection<Player> Thiefs {
            get => new ObservableCollection<Player>(Users.Where(user => user is Thief).ToList());
        }

        public ObservableCollection<Player> Police {
            get => new ObservableCollection<Player>(Users.Where(user => user is Police).ToList());
        }

        public LobbyViewModel(Lobby page, Player currentUser) {
            this.page = page;
            this.currentUser = currentUser;
            gameModel.Id = this.currentUser.InviteKey.GameId;
            gameSessionPreference = new GameSessionPreference();
            SaveCurrentGame();

            webSocketService = new WebSocketService(gameModel.Id);

            IsLoading = true;
            Task.Run(async () => await LoadUsers());
            Task.Run(async () => {
                await StartSocket();
                
                // The socket-event should be set first, then the status can be checked
                await CheckForStatus();
                IsLoading = false;
            });
        }

        private async Task StartSocket() {
            if(!WebSocketService.Connected) {
                await webSocketService.Connect();
            }

            webSocketService.StartGame += StartGame;
        }

        private async Task LoadUsers() {
            Users = await UnitOfWork.Instance.UserRepository.GetAll(GameModel.Id);
        }

        private async Task CheckForStatus() {
            GameModel = await UnitOfWork.Instance.GameRepository.GetGame(gameModel.Id);

            if(GameModel.Status == GameStatus.ONGOING || GameModel.Status == GameStatus.PAUSED || GameModel.Status == GameStatus.FINISHED) {
                StartGameWithoutLoadingGame();
            }
        }
        private void SaveCurrentGame() {
            gameSessionPreference.SetGame(gameModel.Id);
            gameSessionPreference.SetUser(currentUser.Id);
        }
        private async void StartGame() {
            GameModel = await UnitOfWork.Instance.GameRepository.GetGame(gameModel.Id);
            NavigateToMapPage();
        }

        private void StartGameWithoutLoadingGame() {
            NavigateToMapPage();
        }

        // To manually navigate to a different page, the mainthread need to be approached
        private void NavigateToMapPage() {
            try {
                Map mapModel = new Map() {
                    PlayingUser = currentUser
                };

                var mapPage = new MapPage(new MapViewModel(GameModel, mapModel, new Service.Gps.GpsService()));

                Device.BeginInvokeOnMainThread(() => {
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(mapPage, true);
                    webSocketService.StartGame -= StartGame;
                });
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
