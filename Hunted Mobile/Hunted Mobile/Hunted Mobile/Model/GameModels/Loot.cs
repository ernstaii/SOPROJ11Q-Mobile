using Hunted_Mobile.ViewModel;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class Loot : BaseViewModel {
        private readonly int _id;
        private bool _isHandlingLoot = false;
        private bool _hasFinishedHandlingLoot = false;

        public Location Location { get; set; }
        public string Name { get; set; }

        public Loot(int id) {
            _id = id;
        }

        public bool IsHandlingLoot {
            get => _isHandlingLoot;
            set {
                _isHandlingLoot = value;
                if(value)
                    HasFinishedHandlingLoot = false;

                OnPropertyChanged("IsHandlingLoot");
            }
        }

        public bool HasFinishedHandlingLoot {
            get => _hasFinishedHandlingLoot;
            set {
                _hasFinishedHandlingLoot = value;
                if(value)
                    IsHandlingLoot = false;

                OnPropertyChanged("HasFinishedHandlingLoot");
            }
        }
    }
}
