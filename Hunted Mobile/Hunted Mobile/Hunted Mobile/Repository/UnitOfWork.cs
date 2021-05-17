using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Repository {
    public class UnitOfWork {
        private BorderMarkerRepository bordersRepo;
        private GameRepository gamesRepo;
        private InviteKeyRepository inviteKeysRepo;
        private LootRepository lootRepo;
        private ResourceRepository resourcesRepo;
        private UserRepository usersRepo;
        private NotificationRepository notificationsRepo;
        private static readonly UnitOfWork instance = new UnitOfWork();

        public static UnitOfWork Instance => instance;

        public BorderMarkerRepository BorderMarkerRepository {
            get {
                if(bordersRepo == null) {
                    bordersRepo = new BorderMarkerRepository();
                }
                return bordersRepo;
            }
        }

        public GameRepository GameRepository {
            get {
                if(gamesRepo == null) {
                    gamesRepo = new GameRepository();
                }
                return gamesRepo;
            }
        }

        public InviteKeyRepository InviteKeyRepository {
            get {
                if(inviteKeysRepo == null) {
                    inviteKeysRepo = new InviteKeyRepository();
                }
                return inviteKeysRepo;
            }
        }

        public LootRepository LootRepository {
            get {
                if(lootRepo == null) {
                    lootRepo = new LootRepository();
                }
                return lootRepo;
            }
        }

        public ResourceRepository ResourceRepository {
            get {
                if(resourcesRepo == null) {
                    resourcesRepo = new ResourceRepository();
                }
                return resourcesRepo;
            }
        }

        public UserRepository UserRepository {
            get {
                if(usersRepo == null) {
                    usersRepo = new UserRepository();
                }
                return usersRepo;
            }
        }

        public NotificationRepository NotificationRepository {
            get {
                if(notificationsRepo == null) {
                    notificationsRepo = new NotificationRepository();
                }
                return notificationsRepo;
            }
        }
    }
}
