using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Map;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class InformationPageViewModel : BaseViewModel {
        public class IconInformation : BaseViewModel {
            private string imageColor;
            private string borderColor;
            private string description;
            private string name;
            private UriImageSource icon;

            public UriImageSource Icon {
                get => icon; set {
                    icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
            public string ImageColor {
                get => imageColor;
                set {
                    imageColor = value;
                    OnPropertyChanged(nameof(ImageColor));
                }
            }
            public string BorderColor {
                get => borderColor;
                set {
                    borderColor = value;
                    OnPropertyChanged(nameof(BorderColor));
                }
            }

            public string Description {
                get => description;
                set {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }

            public string Name {
                get => name;
                set {
                    name = value;
                    OnPropertyChanged(nameof(name));
                }
            }

            public IconInformation(string name, string description, UriImageSource icon, string background, string color = null) {
                ImageColor = background;
                BorderColor = color ?? background;
                Description = description;
                Name = name;
                Icon = icon;
            }
        }

        private ObservableCollection<IconInformation> icons;
        private List<IconInformation> iconList;
        private readonly string colourTheme;

        public ObservableCollection<IconInformation> Icons {
            get => icons;
            set {
                if(value == null) {
                    icons = new ObservableCollection<IconInformation>();
                }
                else icons = value;

                OnPropertyChanged(nameof(Icons));
            }
        }

        public InformationPageViewModel(string colourTheme, MapIconsService iconService) {
            this.colourTheme = colourTheme;
            try {
                LoadAllIcons(iconService);
            }
            catch(Exception e) {

            }
        }

        private void LoadAllIcons(MapIconsService iconService) {
            iconList = new List<IconInformation>();

            PrepareIcon("Informatie", "beschrijving", iconService.Information);
            PrepareIcon("Menu", "beschrijving", iconService.Menu);
            PrepareIcon("Spelers", "beschrijving", iconService.Users);
            PrepareIcon("Gadgets", "beschrijving", iconService.Gadgets);
            PrepareIcon("Sluiten", "beschrijving", iconService.Close);

            PrepareIcon("Politie", "beschrijving", iconService.PoliceRole, "#FFFFFF");
            PrepareIcon("Boef", "beschrijving", iconService.ThiefRole, "#FFFFFF");
            PrepareIcon("Nep agent", "beschrijving", iconService.FakePoliceRole, "#FFFFFF");

            PrepareIcon("Alarm", "beschrijving", iconService.Alarm);
            PrepareIcon("Rookbom", "beschrijving", iconService.Smoke);
            PrepareIcon("Drone", "beschrijving", iconService.Drone);

            PrepareIcon("Politie station", "beschrijving", iconService.PoliceBadge);
            PrepareIcon("Buit", "beschrijving", iconService.MoneyBag);
            PrepareIcon("Boef pin", "beschrijving", iconService.BluePin);
            PrepareIcon("Politie pin", "beschrijving", iconService.BluePin);

            Icons = new ObservableCollection<IconInformation>(iconList);
        }

        private void PrepareIcon(string name, string description, UriImageSource icon, string imageColor = null) {
            iconList.Add(new IconInformation(name, description, icon, imageColor ?? this.colourTheme, this.colourTheme));
        }
    }
}
