using Hunted_Mobile.Model.GameModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hunted_Mobile.Model {
    public class Map {
        private readonly ObservableCollection<Player> players = new ObservableCollection<Player>();
        private readonly ObservableCollection<Loot> loot = new ObservableCollection<Loot>();
        private Player playingUser;

        public ICollection<Player> Players {
            get => players;
            set {
                players.Clear();

                foreach(var item in value) {
                    players.Add(item);
                }
                if(!players.Contains(playingUser)) {
                    players.Add(playingUser);
                }
            }
        }
        public ICollection<Loot> Loot {
            get => loot;
            set {
                loot.Clear();

                foreach(var item in value) {
                    loot.Add(item);
                }
            }
        }

        public IReadOnlyCollection<Police> Police => Players.Where(user => user is Police).Select(user => (Police) user).ToList();
        public IReadOnlyCollection<Thief> Thiefs => Players.Where(user => user is Thief).Select(user => (Thief) user).Where(thief => !thief.IsCaught).ToList();

        public Player PlayingUser {
            get => playingUser; 
            set {
                players.Remove(PlayingUser);
                players.Add(value);
                playingUser = value;
            }
        }
        public Boundary GameBoundary { get; set; }

        public Map() {
            players.CollectionChanged += CollectionChanged;
            loot.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if(e.NewItems != null) {
                foreach(var item in e.NewItems) {
                    if(item is Player) {
                        if(!((Player) item).IsValid) {
                            throw new OperationCanceledException("Could not add invalid player " + item);
                        }
                    }
                    else if(item is Loot) {
                        if(!((Loot) item).IsValid) {
                            throw new OperationCanceledException("Could not add invalid loot " + loot);
                        }
                    }
                }
            }
        }

        public Loot FindLoot(int id) {
            return Loot.FirstOrDefault(loot => loot.Id.Equals(id));
        }

        public Thief FindThief(int id) {
            return Thiefs.FirstOrDefault(thief => thief.Id.Equals(id));
        }
    }
}
