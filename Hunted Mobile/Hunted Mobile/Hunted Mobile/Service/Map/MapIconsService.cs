using Hunted_Mobile.Enum;
using Hunted_Mobile.Model.Resource;
using Hunted_Mobile.Repository;
using Hunted_Mobile.ViewModel;

using Xamarin.Forms;

namespace Hunted_Mobile.Service.Map {
    public class MapIconsService : BaseViewModel {
        private readonly Resource chat,
            menu,
            users,
            close,
            gadgets,
            alarm,
            smoke,
            drone,
            information,
            thiefRole,
            policeRole,
            fakePoliceRole,
            blackPin,
            bluePin;
#pragma warning disable IDE1006 // Naming Styles
        public readonly Resource PoliceBadgeResource,
            MoneyBagResource;
#pragma warning restore IDE1006 // Naming Styles
        private string roleName;

        public MapIconsService() {
            chat = UnitOfWork.Instance.ResourceRepository.GetGuiImage("chat.png");
            menu = UnitOfWork.Instance.ResourceRepository.GetGuiImage("menu.png");
            users = UnitOfWork.Instance.ResourceRepository.GetGuiImage("users.png");
            gadgets = UnitOfWork.Instance.ResourceRepository.GetGuiImage("backpack.png");
            close = UnitOfWork.Instance.ResourceRepository.GetGuiImage("close.png");
            alarm = UnitOfWork.Instance.ResourceRepository.GetGuiImage("alarm.png");
            smoke = UnitOfWork.Instance.ResourceRepository.GetGuiImage("smoke.png");
            drone = UnitOfWork.Instance.ResourceRepository.GetGuiImage("drone.png");
            information = UnitOfWork.Instance.ResourceRepository.GetGuiImage("information.png");
            thiefRole = UnitOfWork.Instance.ResourceRepository.GetGuiImage("thief.png");
            policeRole = UnitOfWork.Instance.ResourceRepository.GetGuiImage("police.png");
            fakePoliceRole = UnitOfWork.Instance.ResourceRepository.GetGuiImage("fake_police.png");
            PoliceBadgeResource = UnitOfWork.Instance.ResourceRepository.GetMapImage("police-badge.png");
            MoneyBagResource = UnitOfWork.Instance.ResourceRepository.GetMapImage("money-bag.png");
            blackPin = UnitOfWork.Instance.ResourceRepository.GetGuiImage("black_pin.png");
            bluePin = UnitOfWork.Instance.ResourceRepository.GetGuiImage("blue_pin.png");

            OnPropertyChanged(nameof(Chat));
            OnPropertyChanged(nameof(Menu));
            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(Gadgets));
            OnPropertyChanged(nameof(Alarm));
            OnPropertyChanged(nameof(Smoke));
            OnPropertyChanged(nameof(Drone));
            OnPropertyChanged(nameof(Information));
            OnPropertyChanged(nameof(ThiefRole));
            OnPropertyChanged(nameof(PoliceRole));
            OnPropertyChanged(nameof(FakePoliceRole));
            OnPropertyChanged(nameof(PoliceBadge));
            OnPropertyChanged(nameof(MoneyBag));
            OnPropertyChanged(nameof(BlackPin));
            OnPropertyChanged(nameof(BluePin));
        }

        public string RoleName {
            get => roleName;
            set {
                roleName = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public UriImageSource Chat => GetUriImageSource(chat);

        public UriImageSource Menu => GetUriImageSource(menu);

        public UriImageSource Users => GetUriImageSource(users);

        public UriImageSource Gadgets => GetUriImageSource(gadgets);

        public UriImageSource Alarm => GetUriImageSource(alarm);

        public UriImageSource Smoke => GetUriImageSource(smoke);

        public UriImageSource Drone => GetUriImageSource(drone);

        public UriImageSource Information => GetUriImageSource(information);

        public UriImageSource Close => GetUriImageSource(close);

        public UriImageSource ThiefRole => GetUriImageSource(thiefRole);

        public UriImageSource PoliceRole => GetUriImageSource(policeRole);

        public UriImageSource FakePoliceRole => GetUriImageSource(fakePoliceRole);

        public UriImageSource PoliceBadge => GetUriImageSource(PoliceBadgeResource);

        public UriImageSource MoneyBag => GetUriImageSource(MoneyBagResource);

        public UriImageSource BlackPin => GetUriImageSource(blackPin);

        public UriImageSource BluePin => GetUriImageSource(bluePin);

        public UriImageSource Role {
            get {
                switch(RoleName.ToLower()) {
                    case PlayerRole.THIEF:
                        return ThiefRole;
                    case PlayerRole.POLICE:
                        return PoliceRole;
                    case "fakepolice":
                        return FakePoliceRole;
                    default:
                        return null;
                }
            }
        }

        private UriImageSource GetUriImageSource(Resource resource) {
            return new UriImageSource() {
                Uri = resource.Uri,
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0)
            };
        }
    }
}
