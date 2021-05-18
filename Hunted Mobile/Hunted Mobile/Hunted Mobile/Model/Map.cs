using Hunted_Mobile.Model.GameModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hunted_Mobile.Model {
    public class Map {
        private readonly ObservableCollection<Player> players = new ObservableCollection<Player>();
        private readonly ObservableCollection<Loot> loot = new ObservableCollection<Loot>();

        public ICollection<Player> Players {
            get => players;
            set {
                players.Clear();

                foreach(var item in value) {
                    players.Add(item);
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

        public IReadOnlyCollection<Police> Police => Players.Where(user => user is Police).Select(user => new Police(user)).ToList();
        public IReadOnlyCollection<Thief> Thiefs => Players.Where(user => user is Thief).Select(user => new Thief(user)).ToList();

        public Player PlayingUser { get; set; }
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
                            e.NewItems.Remove(item);
                        }
                    }
                    else if(item is Loot) {
                        if(!((Loot) item).IsValid) {
                            e.NewItems.Remove(item);
                        }
                    }
                }
            }
        }

        public Loot FindLoot(Location location) {
            return Loot.FirstOrDefault(loot => loot.Location.Equals(location));
        }

        internal Thief FindThief(Location location) {
            return Thiefs.FirstOrDefault(thief => thief.Location.Equals(location));
        }
    }
}
