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
            gadgets;

        public MapIconsService() {
            chat = UnitOfWork.Instance.ResourceRepository.GetGuiImage("chat.png");
            menu = UnitOfWork.Instance.ResourceRepository.GetGuiImage("menu.png");
            users = UnitOfWork.Instance.ResourceRepository.GetGuiImage("users.png");
            gadgets = UnitOfWork.Instance.ResourceRepository.GetGuiImage("backpack.png");
            close = UnitOfWork.Instance.ResourceRepository.GetGuiImage("close.png");

            OnPropertyChanged(nameof(Chat));
            OnPropertyChanged(nameof(Menu));
            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(Gadgets));

            // Gadgets icons:
            // alarm, smoke, drone, police, thief

            // Roles icons:
            // police, thief
        }

        public UriImageSource Chat {
            get => new UriImageSource() {
                Uri = chat.Uri,
                CachingEnabled = false
            };
        }

        public UriImageSource Menu {
            get => new UriImageSource() {
                Uri = menu.Uri,
                CachingEnabled = false
            };
        }

        public UriImageSource Users {
            get => new UriImageSource() {
                Uri = users.Uri,
                CachingEnabled = false
            };
        }

        public UriImageSource Gadgets {
            get => new UriImageSource() {
                Uri = gadgets.Uri,
                CachingEnabled = false
            };
        }

        public UriImageSource Close {
            get => new UriImageSource() {
                Uri = close.Uri,
                CachingEnabled = false
            };
        }

        private UriImageSource GetIconImageSource() {
            return null;
        }
    }
}
