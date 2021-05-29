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

        public UriImageSource Chat => GetUriImageSource(chat);

        public UriImageSource Menu => GetUriImageSource(menu);

        public UriImageSource Users => GetUriImageSource(users);

        public UriImageSource Gadgets => GetUriImageSource(gadgets);

        public UriImageSource Close => GetUriImageSource(close);

        private UriImageSource GetUriImageSource(Resource resource) {
            return new UriImageSource() {
                Uri = resource.Uri,
                CachingEnabled = false
            };
        }
    }
}
