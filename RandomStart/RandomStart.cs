using BepInEx;
using RoR2;

namespace RandomStart
{
    [BepInPlugin("com.softashell.randomstart", "RandomSart", "1.0")]
    public class RandomStart : BaseUnityPlugin
    {
        public void Awake()
        {
            // Called once for each player in game
            On.RoR2.PlayerCharacterMasterController.Start += (orig, self) =>
            {
                orig(self);

                if (self.master.teamIndex != TeamIndex.Player)
                    return;

                Logger.Log(BepInEx.Logging.LogLevel.Debug, "PlayerCharacterMasterController.Start");

                GiveStartingItems(self);
            };
        }

        public void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.F2))
            {
                // Give items to first player
                GiveStartingItems(PlayerCharacterMasterController.instances[0]);
            }
            */
        }

        internal void GiveStartingItems(PlayerCharacterMasterController player)
        {
            RoR2.Chat.AddMessage($"Giving {player.GetDisplayName()} a care package");

            const int whiteItems = 5;
            const int greenItems = 3;
            const int redItems = 0;

            for (int i = 0; i < whiteItems; i++)
            {
                //And then finally drop it infront of the player.
                CreatePickup(player, GetRandomPickup(1));
            }

            for (int i = 0; i < greenItems; i++)
            {
                //And then finally drop it infront of the player.
                CreatePickup(player, GetRandomPickup(2));
            }

            for (int i = 0; i < redItems; i++)
            {
                //And then finally drop it infront of the player.
                CreatePickup(player, GetRandomPickup(3));
            }
        }

        internal PickupIndex GetRandomPickup(int tier)
        {
            var dropList = Run.instance.availableTier1DropList;

            switch (tier)
            {
                case 2:
                    dropList = Run.instance.availableTier2DropList;
                    break;

                case 3:
                    dropList = Run.instance.availableTier3DropList;
                    break;
            }

            var rng = Run.instance.treasureRng.RangeInt(0, dropList.Count);

            return dropList[rng];
        }

        internal void CreatePickup(PlayerCharacterMasterController player, PickupIndex item)
        {
            if (!player)
                return;

            // Get player position and drop item in front of it
            //var transform = player.master.GetBodyObject().transform;
            //PickupDropletController.CreatePickupDroplet(item, transform.position, transform.forward * 20f);

            // Directly add item to inventory
            player.master.inventory.GiveItem(item.itemIndex);
        }
    }
}