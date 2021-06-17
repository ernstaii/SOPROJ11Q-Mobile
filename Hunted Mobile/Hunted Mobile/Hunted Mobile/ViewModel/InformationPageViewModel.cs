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

        public InformationPageViewModel(string colourTheme, MapIconsService service) {
            this.colourTheme = colourTheme;
            LoadAllIcons(service);
        }

        private void LoadAllIcons(MapIconsService service) {
            iconList = new List<IconInformation>();

            PrepareIcon("Informatie", "Op deze pagina staat informatie over het spel.", service.Information);
            PrepareIcon("Menu", "Met deze knop wordt het menu geopend.", service.Menu);
            PrepareIcon("Spelers", "Met deze knop wordt het speleroverzicht geopend.", service.Users);
            PrepareIcon("Gadgets", "Met deze knop wordt het gadgetsoverzicht geopend.", service.Gadgets);
            PrepareIcon("Sluiten", "Met deze knop wordt het menu gesloten.", service.Close);

            PrepareIcon("Agent", "Dit is een indicatie van uw rol als agent.", service.PoliceRole, "#FFFFFF");
            PrepareIcon("Boef", "Dit is een indicatie van uw rol als boef.", service.ThiefRole, "#FFFFFF");
            PrepareIcon("Nep agent", "Dit is een indicatie van uw rol als nep agent. Met deze rol zijn alle agenten zichtbaar en bij het klikken op de knop wordt de weergave aangepast. Hierdoor wordt u minder snel herkent door de andere agenten.", service.FakePoliceRole, "#FFFFFF");

            PrepareIcon("Alarm", "Met deze gadget worden tijdelijk alle boeven zichtbaar in een gebied.", service.Alarm);
            PrepareIcon("Rookbom", "Met deze gadget kan een boef verdwijnen op de kaart van een politie.", service.Smoke);
            PrepareIcon("Drone", "Met deze gadget worden tijdelijk alle boeven zichtbaar.", service.Drone);

            PrepareIcon("Politie station", "Dit is een indicatie van waar het politiebureau is gelokaliseerd.", service.PoliceBadge, "#FFFFFF");
            PrepareIcon("Buit", "Dit is een indicatie van een buit.", service.MoneyBag, "#FFFFFF");
            PrepareIcon("Boef pin", "Dit is een indicatie waar een boef loopt.", service.BlackPin, "#FFFFFF");
            PrepareIcon("Politie pin", "Dit is een indicatie waar een agent loopt.", service.BluePin, "#FFFFFF");

            Icons = new ObservableCollection<IconInformation>(iconList);
        }

        private void PrepareIcon(string name, string description, UriImageSource icon, string imageColor = null) {
            iconList.Add(new IconInformation(name, description, icon, imageColor ?? this.colourTheme, this.colourTheme));
        }
    }
}
