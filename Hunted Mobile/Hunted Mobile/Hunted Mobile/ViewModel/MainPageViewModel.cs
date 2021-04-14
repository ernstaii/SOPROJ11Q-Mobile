using Hunted_Mobile.Model;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class MainPageViewModel : INotifyPropertyChanged {
        private InviteKey _inviteKeyModel = new InviteKey();
        private bool _isloading = false;
        public InviteKey InviteKeyModel {
            get => _inviteKeyModel;
            set {
                _inviteKeyModel = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("InviteKeyModel"));
            }
        }

        public bool SubmitButtonIsEnable {
            get => _isloading;
            set {
                _isloading = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SubmitButtonIsEnable"));
            }
        }

        private InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();
        private MainPage _page;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid { get; set; }

        public MainPageViewModel(MainPage page) {
            _page = page;
        }

        public async Task Get() {
            IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page);

            if(IsValid) {
                var result = await _inviteKeyRepository.Get(InviteKeyModel.Value);

                if(result != null)
                    InviteKeyModel = result;
                else
                    IsValid = false;
            }
        }

        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await Get();

            // Navigate when InviteKey is valid
            if(IsValid) {
                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new EnterUsername(InviteKeyModel));
            }

            SubmitButtonIsEnable = true;
        });

        // Change the IsEnabled of SubmitButton
        /*public void EnableButton(bool enabled = true) {
            this._page.SubmitInviteCodeButton.IsEnabled = enabled;
            OnPropertyChanged(nameof(this.SubmitInviteCodeButton));
        }*/
    }
}
