using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace Hunted_Mobile.Model {
    public class Countdown : BindableObject {
        private TimeSpan remainTime;
        public bool InitialTimerStart { get; set; } = true;
        public event Action Completed;
        public event Action Ticked;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Countdown() {
            StartDate = DateTime.Now;
        }

        public TimeSpan RemainTime {
            get => remainTime;
            set {
                remainTime = value;
                OnPropertyChanged();
            }
        }

        public void Start(int seconds = 1) {
            Device.StartTimer(TimeSpan.FromSeconds(seconds), () => {
                RemainTime = (EndDate - DateTime.Now);
                var ticked = RemainTime.TotalSeconds > 1;

                if(ticked) {
                    Ticked?.Invoke();
                }
                else {
                    Completed?.Invoke();
                }

                return ticked;
            });
        }
    }
}
