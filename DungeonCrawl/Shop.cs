using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
    internal class Shop
    {
        public List<Item> inventory;
        public Vector2 position;

        public Shop(Vector2 position)
        {
            this.position = position;
            this.inventory = new List<Item>();
        }

        public void RestockShop(Random random)
        {
            inventory.Clear();

            // Add a few random items
            int itemCount = random.Next(3, 6);
            for (int i = 0; i < itemCount; i++)
            {
                ItemType type = (ItemType)random.Next(0, 3); // Exclude treasure type
                int quality = random.Next(1, 4); // Higher quality items in shop

                Item item = type switch
                {
                    ItemType.Weapon => CreateShopItem("Magic Sword", type, quality + 1),
                    ItemType.Armor => CreateShopItem("Enchanted Armor", type, quality),
                    ItemType.Potion => CreateShopItem("Health Potion", type, quality * 2),
                    _ => CreateShopItem("Unknown Item", type, 1)
                };

                inventory.Add(item);
            }
        }

        private Item CreateShopItem(string name, ItemType type, int quality)
        {
            Item item = new Item();
            item.name = name;
            item.type = type;
            item.quality = quality;
            item.position = Vector2.Zero; // Not placed on map
            return item;
        }
    }

}
