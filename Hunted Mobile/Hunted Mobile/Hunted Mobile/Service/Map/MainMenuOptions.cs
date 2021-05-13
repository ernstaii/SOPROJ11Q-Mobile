using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service.Map {
    public class MainMenuOptions {
        public string SelectedMainMenuOption { get; set; }

        public class Options {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
            public static readonly string DisplayUsersOption = "USERS_IN_GAME",
                DisplayLootOption = "TOTAL_LOOT_OF_GAME",
                DisplayGadgetsOption = "GADGETS_OF_USER";
        }

        public MainMenuOptions() { }
    }

}
