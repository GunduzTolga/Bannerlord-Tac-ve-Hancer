using System.Collections.Generic;
using System.Linq;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.ObjectSystem;

namespace CheatMenu.Cheats
{
    /// <summary>Envantere eşya ekleme. Satılık olmayan özel eşyalar (NotMerchandise) listelenmez.</summary>
    public static class ItemSpawner
    {
        public sealed class Category
        {
            public Category(string label, ItemObject.ItemTypeEnum type)
            {
                Label = label;
                Type = type;
            }

            public string Label { get; }
            public ItemObject.ItemTypeEnum Type { get; }
        }

        public static readonly Category[] Categories =
        {
            new Category("Tek elli silah", ItemObject.ItemTypeEnum.OneHandedWeapon),
            new Category("İki elli silah", ItemObject.ItemTypeEnum.TwoHandedWeapon),
            new Category("Mızrak / uzun silah", ItemObject.ItemTypeEnum.Polearm),
            new Category("Yay", ItemObject.ItemTypeEnum.Bow),
            new Category("Arbalet", ItemObject.ItemTypeEnum.Crossbow),
            new Category("Ok", ItemObject.ItemTypeEnum.Arrows),
            new Category("Cıvata", ItemObject.ItemTypeEnum.Bolts),
            new Category("Fırlatma silahı", ItemObject.ItemTypeEnum.Thrown),
            new Category("Kalkan", ItemObject.ItemTypeEnum.Shield),
            new Category("Kafa zırhı", ItemObject.ItemTypeEnum.HeadArmor),
            new Category("Gövde zırhı", ItemObject.ItemTypeEnum.BodyArmor),
            new Category("Bacak zırhı", ItemObject.ItemTypeEnum.LegArmor),
            new Category("Eldiven", ItemObject.ItemTypeEnum.HandArmor),
            new Category("Pelerin", ItemObject.ItemTypeEnum.Cape),
            new Category("At", ItemObject.ItemTypeEnum.Horse),
            new Category("At koşumu", ItemObject.ItemTypeEnum.HorseHarness),
            new Category("Ticaret malı", ItemObject.ItemTypeEnum.Goods),
            new Category("Hayvan", ItemObject.ItemTypeEnum.Animal),
        };

        public static IEnumerable<ItemObject> ItemsOfType(ItemObject.ItemTypeEnum type) =>
            MBObjectManager.Instance.GetObjectTypeList<ItemObject>()
                .Where(item => item.ItemType == type && !item.NotMerchandise);

        public static string Label(ItemObject item) =>
            item.Name + " (T" + ((int)item.Tier + 1) + ", " + item.Value.ToString("N0") + " dinar)";

        public static ImageIdentifier Image(ItemObject item) => new ItemImageIdentifier(item, null);

        public static void Add(ItemObject item, int count)
        {
            MobileParty.MainParty.ItemRoster.AddToCounts(item, count);
            MenuUI.Success(count + " x " + item.Name + " envantere eklendi");
        }
    }
}
