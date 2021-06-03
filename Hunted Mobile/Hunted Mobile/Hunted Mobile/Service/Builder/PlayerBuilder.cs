using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.GameModels.Gadget;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hunted_Mobile.Service.Builder {
    public class PlayerBuilder {
        public int Id { get; private set; }
        public Location Location { get; private set; }
        public InviteKey InviteKey { get; private set; }
        public string UserName { get; private set; }
        public string Status { get; private set; }
        public ICollection<Gadget> Gadgets { get; private set; }
        public bool TriggeredAlarm { get; private set; }
        public string CaughtAt { get; private set; }
        public bool FakePolice { get; private set; }

        public PlayerBuilder() {

        }

        public PlayerBuilder SetId(int id) {
            Id = id;
            return this;
        }

        public PlayerBuilder SetLocation(Location location) {
            Location = location;
            return this;
        }

        public PlayerBuilder SetInviteKey(InviteKey inviteKey) {
            InviteKey = inviteKey;
            return this;
        }

        public PlayerBuilder SetUsername(string username) {
            UserName = username;
            return this;
        }

        public PlayerBuilder SetStatus(string status) {
            Status = status;
            return this;
        }

        public PlayerBuilder SetGadgets(ICollection<Gadget> gadgets) {
            Gadgets = gadgets;
            return this;
        }

        public PlayerBuilder SetTriggeredAlarm(bool triggeredAlarm) {
            TriggeredAlarm = triggeredAlarm;
            return this;
        }

        public PlayerBuilder SetCaughtAt(string caughtAt) {
            CaughtAt = caughtAt;
            return this;
        }

        public PlayerBuilder SetFakePolice(bool fakePolice) {
            FakePolice = fakePolice;
            return this;
        }

        public Player ToPlayer() {
            switch(InviteKey?.Role.ToLower()) {
                case PlayerRole.THIEF:
                    var thief = new Thief(Id, UserName, InviteKey, Location, Status, Gadgets, TriggeredAlarm, CaughtAt);
                    if(FakePolice) {
                        return new FakePolice(thief);
                    }
                    else return thief;
                case PlayerRole.POLICE:
                    return new Police(Id, UserName, InviteKey, Location, Status, Gadgets, TriggeredAlarm);
                default:
                    return null; // throw new ArgumentException("InviteKey did not have a value for Role")
            }
        }
    }
}
