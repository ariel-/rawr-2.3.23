using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Rawr
{
	/*
    public class Armory
    {
		public static void GetCharacter(string name, string realm, CharacterRegion region, Action<Character> callback)
		{
			new CharacterRequest(name, realm, region, callback);
		}

        public static void GetItem(int id, Action<Item> callback)
        {
            new ItemRequest(id, callback);
        }

        public static void GetItemIdByName(string itemName, Action<int> callback)
        {
            new ItemIdRequest(itemName, callback);
        }
    }

    public class ItemIdRequest
    {
        private readonly string itemName;
        public string ItemName { get { return itemName; } }

        private readonly Action<int> callback;
        public Action<int> Callback { get { return callback; } }

        public XDocument ItemSearch { get; set; }

        public int Result { get; set; }

        public void Invoke()
        {
            callback(Result);
        }

        public ItemIdRequest(string itemName, Action<int> callback)
        {
            this.itemName = itemName;
            this.callback = callback;
            GetItemId();
        }
        public void GetItemId()
        {
            new NetworkUtils(new EventHandler(ItemSearchReady)).DownloadItemSearch(ItemName);
        }

        private void ItemSearchReady(object sender, EventArgs e)
        {
            NetworkUtils network = sender as NetworkUtils;
            ItemSearch = network.Result;
            try
            {
                List<XElement> items_nodes = new List<XElement>(ItemSearch.SelectNodes("/page/armorySearch/searchResults/items/item"));
                // we only want a single match, even if its not exact
                if (items_nodes.Count == 1)
                {
                    int id = Int32.Parse(items_nodes[0].Attribute("id").Value);
                    Result = id;
                }
                else
                {
                    // choose an exact match if it exists
                    foreach (XElement node in items_nodes)
                    {
                        if (node.Attribute("name").Value == itemName)
                        {
                            int id = Int32.Parse(items_nodes[0].Attribute("id").Value);
                            Result = id;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessaging.ReportError("Get Item", ex, "Rawr encountered an error getting Item Id from Armory: " + ItemName);
            }
            Invoke();
        }
    }

    public class ItemRequest
    {

        private readonly int id;
        public int Id { get { return id; } }

        private readonly Action<Item> callback;
        public Action<Item> Callback { get { return callback; } }

        public XDocument Tooltip { get; set; }
        public XDocument ItemInfo { get; set; }

        public Item Result { get; set; }

        public void Invoke()
        {
            callback(Result);
        }

        public ItemRequest(int id, Action<Item> callback)
        {
            this.id = id;
            this.callback = callback;
            GetItem();
        }
        public void GetItem()
        {
            new NetworkUtils(new EventHandler(ItemTooltipReady)).DownloadItemToolTipSheet(Id);
        }

        private void ItemTooltipReady(object sender, EventArgs e)
        {
            NetworkUtils network = sender as NetworkUtils;

            Tooltip = network.Result;
            network.DocumentReady -= new EventHandler(ItemTooltipReady);
            network.DocumentReady += new EventHandler(ItemInformationReady);
            network.DownloadItemInformation(Id);
        }

        private void ItemInformationReady(object sender, EventArgs e)
        {
            NetworkUtils network = sender as NetworkUtils;
            ItemInfo = network.Result;
            try
            {
                ItemLocation location = LocationFactory.Create(Tooltip, ItemInfo, Id.ToString());

                if (Tooltip == null || Tooltip.SelectSingleNode("/page/itemTooltips/itemTooltip") == null)
                {
                    StatusMessaging.ReportError("Get Item", null, "No item returned from Armory for " + Id);
                    return;
                }

                ItemQuality quality = ItemQuality.Common;
                ItemType type = ItemType.None;
                ItemSlot socketColor1 = ItemSlot.None;
                ItemSlot socketColor2 = ItemSlot.None;
                ItemSlot socketColor3 = ItemSlot.None;
                Stats socketStats = new Stats();
                string name = string.Empty;
                string iconPath = string.Empty;
                string setName = string.Empty;
                ItemSlot slot = ItemSlot.None;
                Stats stats = new Stats();
                int inventoryType = -1;
                int classId = -1;
                string subclassName = string.Empty;
                int minDamage = 0;
                int maxDamage = 0;
                ItemDamageType damageType = ItemDamageType.Physical;
                float speed = 0f;
                List<string> requiredClasses = new List<string>();
                bool unique = false;
                int itemLevel = 0;

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/name")) { name = node.Value; }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/icon")) { iconPath = node.Value; }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/maxCount")) { unique = node.Value == "1"; }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/overallQualityId")) { quality = (ItemQuality)Enum.Parse(typeof(ItemQuality), node.Value, false); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/classId")) { classId = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/equipData/inventoryType")) { inventoryType = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/equipData/subclassName")) { subclassName = node.Value; }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/damageData/damage/min")) { minDamage = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/damageData/damage/max")) { maxDamage = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/damageData/damage/type")) { damageType = (ItemDamageType)Enum.Parse(typeof(ItemDamageType), node.Value, false); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/damageData/speed")) { speed = float.Parse(node.Value, System.Globalization.CultureInfo.InvariantCulture); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/setData/name")) { setName = node.Value; }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/allowableClasses/class")) { requiredClasses.Add(node.Value); }

                foreach (XAttribute attr in ItemInfo.SelectNodes("page/itemInfo/item").Attributes("level")) { itemLevel = int.Parse(attr.Value); }

                if (inventoryType >= 0)
                    slot = GetItemSlot(inventoryType, classId);
                if (!string.IsNullOrEmpty(subclassName))
                    type = GetItemType(subclassName, inventoryType, classId);

                // fix class restrictions on BOP items that can only be made by certain classes
                switch (Id)
                {
                    case 35181:
                    case 32495:
                        requiredClasses.Add("Priest");
                        break;
                    case 32476:
                    case 35184:
                    case 32475:
                    case 34355:
                        requiredClasses.Add("Shaman");
                        break;
                    case 32474:
                    case 34356:
                        requiredClasses.Add("Hunter");
                        break;
                    case 46106:
                    case 32479:
                    case 32480:
                    case 46109:
                        requiredClasses.Add("Druid");
                        break;
                    case 32478:
                    case 34353:
                        requiredClasses.Add("Druid");
                        requiredClasses.Add("Rogue");
                        break;
                }

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusAgility")) { stats.Agility = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusAttackPower")) { stats.AttackPower = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/armor")) { stats.Armor = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusDefenseSkillRating")) { stats.DefenseRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusDodgeRating")) { stats.DodgeRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusParryRating")) { stats.ParryRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusBlockRating")) { stats.BlockRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusBlockValue")) { stats.BlockValue = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/blockValue")) { stats.BlockValue = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusResilienceRating")) { stats.Resilience = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusStamina")) { stats.Stamina = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusIntellect")) { stats.Intellect = int.Parse(node.Value); }

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusStrength")) { stats.Strength = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusHitRating")) { stats.HitRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusHasteRating")) { stats.HasteRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusCritRating")) { stats.CritRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusExpertiseRating")) { stats.ExpertiseRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusArmorPenetration")) { stats.ArmorPenetrationRating = int.Parse(node.Value); }

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/arcaneResist")) { stats.ArcaneResistance = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/fireResist")) { stats.FireResistance = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/frostResist")) { stats.FrostResistance = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/natureResist")) { stats.NatureResistance = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/shadowResist")) { stats.ShadowResistance = int.Parse(node.Value); }

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusCritSpellRating")) { stats.CritRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusHitSpellRating")) { stats.HitRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusHasteSpellRating")) { stats.HasteRating = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusSpellPower")) { stats.SpellPower = int.Parse(node.Value); }

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusMana")) { stats.Mana = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusSpirit")) { stats.Spirit = int.Parse(node.Value); }
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/bonusManaRegen")) { stats.Mp5 = int.Parse(node.Value); }

                if (slot == ItemSlot.Finger ||
                    slot == ItemSlot.MainHand ||
                    slot == ItemSlot.Neck ||
                    (slot == ItemSlot.OffHand && type != ItemType.Shield) ||
                    slot == ItemSlot.OneHand ||
                    slot == ItemSlot.Trinket ||
                    slot == ItemSlot.TwoHand)
                {
                    stats.BonusArmor += stats.Armor;
                    stats.Armor = 0f;
                }

                if (slot == ItemSlot.Back)
                {
                    float baseArmor = 0;
                    switch (quality)
                    {
                        case ItemQuality.Temp:
                        case ItemQuality.Poor:
                        case ItemQuality.Common:
                        case ItemQuality.Uncommon:
                            baseArmor = (float)itemLevel * 1.19f + 5.1f;
                            break;

                        case ItemQuality.Rare:
                            baseArmor = ((float)itemLevel + 26.6f) * 16f / 25f;
                            break;

                        case ItemQuality.Epic:
                        case ItemQuality.Legendary:
                        case ItemQuality.Artifact:
                        case ItemQuality.Heirloom:
                            baseArmor = ((float)itemLevel + 358f) * 7f / 26f;
                            break;
                    }

                    baseArmor = (float)Math.Floor(baseArmor);
                    stats.BonusArmor = stats.Armor - baseArmor;
                    stats.Armor = baseArmor;
                }

                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/spellData/spell"))
                {
                    bool isEquip = false;
                    bool isUse = false;
                    string spellDesc = null;
                    foreach (XElement childNode in node.Elements())
                    {
                        if (childNode.Name == "trigger")
                        {
                            isEquip = childNode.Value == "1";
                            isUse = childNode.Value == "0";
                        }
                        if (childNode.Name == "desc")
                            spellDesc = childNode.Value;
                    }

                    //parse Use/Equip lines
                    if (isUse) SpecialEffects.ProcessUseLine(spellDesc, stats, true, Id);
                    if (isEquip) SpecialEffects.ProcessEquipLine(spellDesc, stats, true, itemLevel, Id);
                }

                List<XElement> socketNodes = new List<XElement>(Tooltip.SelectNodes("page/itemTooltips/itemTooltip/socketData/socket"));
                if (socketNodes.Count > 0) socketColor1 = (ItemSlot)Enum.Parse(typeof(ItemSlot), socketNodes[0].Attribute("color").Value, false);
                if (socketNodes.Count > 1) socketColor2 = (ItemSlot)Enum.Parse(typeof(ItemSlot), socketNodes[1].Attribute("color").Value, false);
                if (socketNodes.Count > 2) socketColor3 = (ItemSlot)Enum.Parse(typeof(ItemSlot), socketNodes[2].Attribute("color").Value, false);
                string socketBonusesString = string.Empty;
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/socketData/socketMatchEnchant")) { socketBonusesString = node.Value.Trim('+'); }
                if (!string.IsNullOrEmpty(socketBonusesString))
                {
                    try
                    {
                        List<string> socketBonuses = new List<string>();
                        string[] socketBonusStrings = socketBonusesString.Split(new string[] { " and ", " & ", ", " }, StringSplitOptions.None);
                        foreach (string socketBonusString in socketBonusStrings)
                        {
                            if (socketBonusString.LastIndexOf('+') > 2 && socketBonusString.LastIndexOf('+') < socketBonusString.Length - 3)
                            {
                                socketBonuses.Add(socketBonusString.Substring(0, socketBonusString.IndexOf(" +")));
                                socketBonuses.Add(socketBonusString.Substring(socketBonusString.IndexOf(" +") + 1));
                            }
                            else
                                socketBonuses.Add(socketBonusString);
                        }
                        foreach (string socketBonus in socketBonuses)
                        {
                            int socketBonusValue = 0;
                            if (socketBonus.IndexOf(' ') > 0) socketBonusValue = int.Parse(socketBonus.Substring(0, socketBonus.IndexOf(' ')));
                            switch (socketBonus.Substring(socketBonus.IndexOf(' ') + 1))
                            {
                                case "Agility":
                                    socketStats.Agility = socketBonusValue;
                                    break;
                                case "Stamina":
                                    socketStats.Stamina = socketBonusValue;
                                    break;
                                case "Dodge Rating":
                                    socketStats.DodgeRating = socketBonusValue;
                                    break;
                                case "Parry Rating":
                                    socketStats.ParryRating = socketBonusValue;
                                    break;
                                case "Block Rating":
                                    socketStats.BlockRating = socketBonusValue;
                                    break;
                                case "Block Value":
                                    socketStats.BlockValue = socketBonusValue;
                                    break;
                                case "Defense Rating":
                                    socketStats.DefenseRating = socketBonusValue;
                                    break;
                                case "Hit Rating":
                                    socketStats.HitRating = socketBonusValue;
                                    break;
                                case "Haste Rating":
                                    socketStats.HasteRating = socketBonusValue;
                                    break;
                                case "Expertise Rating":
                                    socketStats.ExpertiseRating = socketBonusValue;
                                    break;
                                case "Armor Penetration Rating":
                                    socketStats.ArmorPenetrationRating = socketBonusValue;
                                    break;
                                case "Strength":
                                    socketStats.Strength = socketBonusValue;
                                    break;
                                case "Healing":
                                    //case "Healing +4 Spell Damage":
                                    //case "Healing +3 Spell Damage":
                                    //case "Healing +2 Spell Damage":
                                    //case "Healing +1 Spell Damage":
                                    //case "Healing and +4 Spell Damage":
                                    //case "Healing and +3 Spell Damage":
                                    //case "Healing and +2 Spell Damage":
                                    //case "Healing and +1 Spell Damage":
                                    if (socketBonusValue == 0)
                                        socketStats.SpellPower = (float)Math.Round(int.Parse(socketBonuses[0].Substring(0, socketBonuses[0].IndexOf(' '))) / 1.88f);
                                    else
                                        socketStats.SpellPower = (float)Math.Round(socketBonusValue / 1.88f);
                                    break;
                                case "Spell Damage":
                                    // Only update Spell Damage if its not already set (Incase its an old heal bonus)
                                    if (socketStats.SpellPower == 0)
                                        socketStats.SpellPower = socketBonusValue;
                                    //sockets.Stats.Healing = socketBonusValue;
                                    break;
                                case "Spell Power":
                                    socketStats.SpellPower = socketBonusValue;
                                    break;
                                case "Crit Rating":
                                case "Crit Strike Rating":
                                case "Critical Rating":
                                case "Critical Strike Rating":
                                    socketStats.CritRating = socketBonusValue;
                                    break;
                                case "Attack Power":
                                    socketStats.AttackPower = socketBonusValue;
                                    break;
                                case "Weapon Damage":
                                    socketStats.WeaponDamage = socketBonusValue;
                                    break;
                                case "Resilience":
                                case "Resilience Rating":
                                    socketStats.Resilience = socketBonusValue;
                                    break;
                                //case "Spell Damage and Healing":
                                //    sockets.Stats.SpellDamageRating = socketBonusValue;
                                //    sockets.Stats.Healing = socketBonusValue;
                                //    break;
                                case "Spell Hit Rating":
                                    socketStats.HitRating = socketBonusValue;
                                    break;
                                case "Intellect":
                                    socketStats.Intellect = socketBonusValue;
                                    break;
                                case "Spell Crit":
                                case "Spell Crit Rating":
                                case "Spell Critical":
                                case "Spell Critical Rating":
                                case "Spell Critical Strike Rating":
                                    socketStats.CritRating = socketBonusValue;
                                    break;
                                case "Spell Haste Rating":
                                    socketStats.HasteRating = socketBonusValue;
                                    break;
                                case "Spirit":
                                    socketStats.Spirit = socketBonusValue;
                                    break;
                                case "Mana every 5 seconds":
                                case "Mana ever 5 Sec":
                                case "mana per 5 sec":
                                case "mana per 5 sec.":
                                case "Mana per 5 sec.":
                                case "Mana per 5 Seconds":
                                    socketStats.Mp5 = socketBonusValue;
                                    break;
                            }
                        }
                    }
                    catch { }
                }
                foreach (XElement nodeGemProperties in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/gemProperties"))
                {
                    List<string> gemBonuses = new List<string>();
                    string[] gemBonusStrings = nodeGemProperties.Value.Split(new string[] { " and ", " & ", ", " }, StringSplitOptions.None);
                    foreach (string gemBonusString in gemBonusStrings)
                    {
                        if (gemBonusString.IndexOf('+') != gemBonusString.LastIndexOf('+'))
                        {
                            gemBonuses.Add(gemBonusString.Substring(0, gemBonusString.IndexOf(" +")));
                            gemBonuses.Add(gemBonusString.Substring(gemBonusString.IndexOf(" +") + 1));
                        }
                        else
                            gemBonuses.Add(gemBonusString);
                    }
                    foreach (string gemBonus in gemBonuses)
                    {
                        if (gemBonus == "Spell Damage +6")
                        {
                            stats.SpellPower = 6.0f;
                        }
                        else if (gemBonus == "2% Increased Armor Value from Items")
                        {
                            stats.BaseArmorMultiplier = 0.02f;
                        }
                        else if (gemBonus == "Stamina +6")
                        {
                            stats.Stamina = 6.0f;
                        }
                        else if (gemBonus == "Chance to restore mana on spellcast")
                        {
                            stats.ManaRestoreOnCast_5_15 = 300; // IED
                        }
                        else if (gemBonus == "Chance on spellcast - next spell cast in half time" || gemBonus == "Chance to Increase Spell Cast Speed")
                        {
                            stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCast, new Stats() { HasteRating = 320 }, 6, 45, 0.15f));
                        }
                        else if (gemBonus == "+10% Shield Block Value")
                        {
                            stats.BonusBlockValueMultiplier = 0.1f;
                        }
                        else if (gemBonus == "+2% Intellect")
                        {
                            stats.BonusIntellectMultiplier = 0.02f;
                        }
                        else if (gemBonus == "2% Reduced Threat")
                        {
                            stats.ThreatReductionMultiplier = 0.02f;
                        }
                        else if (gemBonus == "3% Increased Critical Healing Effect")
                        {
                            stats.BonusCritHealMultiplier = 0.03f;
                        }
                        else
                        {
                            try
                            {
                                int gemBonusValue = int.Parse(gemBonus.Substring(0, gemBonus.IndexOf(' ')).Trim('+').Trim('%'));
                                switch (gemBonus.Substring(gemBonus.IndexOf(' ') + 1).Trim())
                                {
                                    case "to All Stats":
                                    case "All Stats":
                                        stats.Agility = gemBonusValue;
                                        stats.Strength = gemBonusValue;
                                        stats.Stamina = gemBonusValue;
                                        stats.Intellect = gemBonusValue;
                                        stats.Spirit = gemBonusValue;
                                        break;
                                    case "Resist All":
                                        stats.ArcaneResistance = gemBonusValue;
                                        stats.FireResistance = gemBonusValue;
                                        stats.FrostResistance = gemBonusValue;
                                        stats.NatureResistance = gemBonusValue;
                                        stats.ShadowResistance = gemBonusValue;
                                        break;
                                    case "Increased Critical Damage":
                                        stats.BonusCritMultiplier = (float)gemBonusValue / 100f;
                                        stats.BonusSpellCritMultiplier = (float)gemBonusValue / 100f; // both melee and spell crit use the same text, would have to disambiguate based on other stats
                                        break;
                                    case "Agility":
                                        stats.Agility = gemBonusValue;
                                        break;
                                    case "Stamina":
                                        stats.Stamina = gemBonusValue;
                                        break;
                                    case "Dodge Rating":
                                        stats.DodgeRating = gemBonusValue;
                                        break;
                                    case "Parry Rating":
                                        stats.ParryRating = gemBonusValue;
                                        break;
                                    case "Block Rating":
                                        stats.BlockRating = gemBonusValue;
                                        break;
                                    case "Defense Rating":
                                        stats.DefenseRating = gemBonusValue;
                                        break;
                                    case "Hit Rating":
                                        stats.HitRating = gemBonusValue;
                                        break;
                                    case "Haste Rating":
                                        stats.HasteRating = gemBonusValue;
                                        break;
                                    case "Expertise Rating":
                                        stats.ExpertiseRating = gemBonusValue;
                                        break;
                                    case "Armor Penetration Rating":
                                        stats.ArmorPenetrationRating = gemBonusValue;
                                        break;
                                    case "Strength":
                                        stats.Strength = gemBonusValue;
                                        break;
                                    case "Crit Rating":
                                    case "Crit Strike Rating":
                                    case "Critical Rating":
                                    case "Critical Strike Rating":
                                        stats.CritRating = gemBonusValue;
                                        break;
                                    case "Attack Power":
                                        stats.AttackPower = gemBonusValue;
                                        break;
                                    case "Weapon Damage":
                                        stats.WeaponDamage = gemBonusValue;
                                        break;
                                    case "Resilience":
                                    case "Resilience Rating":
                                        stats.Resilience = gemBonusValue;
                                        break;
                                    case "Spell Hit Rating":
                                        stats.HitRating = gemBonusValue;
                                        break;
                                    case "Spell Haste Rating":
                                        stats.HasteRating = gemBonusValue;
                                        break;
                                    case "Spell Damage":
                                        // Ignore spell damage from gem if Healing has already been applied, as it might be a "9 Healing 3 Spell" gem. 
                                        if (stats.SpellPower == 0)
                                            stats.SpellPower = gemBonusValue;
                                        break;
                                    case "Spell Damage and Healing":
                                        stats.SpellPower = gemBonusValue;
                                        break;
                                    case "Healing":
                                        stats.SpellPower = (float)Math.Round(gemBonusValue / 1.88f);
                                        break;
                                    case "Spell Power":
                                        stats.SpellPower = gemBonusValue;
                                        break;
                                    case "Spell Crit":
                                    case "Spell Crit Rating":
                                    case "Spell Critical":
                                    case "Spell Critical Rating":
                                        stats.CritRating = gemBonusValue;
                                        break;
                                    case "Mana every 5 seconds":
                                    case "Mana ever 5 Sec":
                                    case "mana per 5 sec":
                                    case "mana per 5 sec.":
                                    case "Mana per 5 Seconds":
                                        stats.Mp5 = gemBonusValue;
                                        break;
                                    case "Intellect":
                                        stats.Intellect = gemBonusValue;
                                        break;
                                    case "Spirit":
                                        stats.Spirit = gemBonusValue;
                                        break;
                                }
                            }
                            catch { }
                        }
                    }
                }
                string desc = string.Empty;
                foreach (XElement node in Tooltip.SelectNodes("page/itemTooltips/itemTooltip/desc")) { desc = node.Value; }
                if (desc.Contains("Matches any socket"))
                {
                    slot = ItemSlot.Prismatic;
                }
                else if (desc.Contains("Matches a "))
                {
                    bool red = desc.Contains("Red");
                    bool blue = desc.Contains("Blue");
                    bool yellow = desc.Contains("Yellow");
                    slot = red && blue && yellow ? ItemSlot.Prismatic :
                        red && blue ? ItemSlot.Purple :
                        blue && yellow ? ItemSlot.Green :
                        red && yellow ? ItemSlot.Orange :
                        red ? ItemSlot.Red :
                        blue ? ItemSlot.Blue :
                        yellow ? ItemSlot.Yellow :
                        ItemSlot.None;
                }
                else if (desc.Contains("meta gem slot"))
                    slot = ItemSlot.Meta;

                Item item = new Item()
                {
                    Id = Id,
                    Name = name,
                    Quality = quality,
                    Type = type,
                    IconPath = iconPath,
                    Slot = slot,
                    SetName = setName,
                    Stats = stats,
                    SocketColor1 = socketColor1,
                    SocketColor2 = socketColor2,
                    SocketColor3 = socketColor3,
                    SocketBonus = socketStats,
                    MinDamage = minDamage,
                    MaxDamage = maxDamage,
                    DamageType = damageType,
                    Speed = speed,
                    RequiredClasses = string.Join("|", requiredClasses.ToArray()),
                    Unique = unique,
                    ItemLevel = itemLevel,
                };
                Result = item;
            }
            catch (Exception ex)
            {
                StatusMessaging.ReportError("Get Item", ex, "Rawr encountered an error getting Item from Armory: " + Id);
            }
            Invoke();
        }

        private static ItemType GetItemType(string subclassName, int inventoryType, int classId)
        {
            switch (subclassName)
            {
                case "Cloth":
                    return ItemType.Cloth;

                case "Leather":
                    return ItemType.Leather;

                case "Mail":
                    return ItemType.Mail;

                case "Plate":
                    return ItemType.Plate;

                case "Dagger":
                    return ItemType.Dagger;

                case "Fist Weapon":
                    return ItemType.FistWeapon;

                case "Axe":
                    if (inventoryType == 17)
                        return ItemType.TwoHandAxe;
                    else
                        return ItemType.OneHandAxe;

                case "Mace":
                    if (inventoryType == 17)
                        return ItemType.TwoHandMace;
                    else
                        return ItemType.OneHandMace;

                case "Sword":
                    if (inventoryType == 17)
                        return ItemType.TwoHandSword;
                    else
                        return ItemType.OneHandSword;

                case "Polearm":
                    return ItemType.Polearm;

                case "Staff":
                    return ItemType.Staff;

                case "Shield":
                    return ItemType.Shield;

                case "Bow":
                    return ItemType.Bow;

                case "Crossbow":
                    return ItemType.Crossbow;

                case "Gun":
                    return ItemType.Gun;

                case "Wand":
                    return ItemType.Wand;

                case "Thrown":
                    return ItemType.Thrown;

                case "Idol":
                    return ItemType.Idol;

                case "Libram":
                    return ItemType.Libram;

                case "Totem":
                    return ItemType.Totem;

                case "Arrow":
                    return ItemType.Arrow;

                case "Bullet":
                    return ItemType.Bullet;

                case "Quiver":
                    return ItemType.Quiver;

                case "Ammo Pouch":
                    return ItemType.AmmoPouch;

                case "Sigil":
                    return ItemType.Sigil;

                default:
                    return ItemType.None;
            }
        }

        private static ItemSlot GetItemSlot(int inventoryType, int classId)
        {
            switch (classId)
            {
                case 6:
                    return ItemSlot.Projectile;

                case 11:
                    return ItemSlot.ProjectileBag;
            }

            switch (inventoryType)
            {
                case 1:
                    return ItemSlot.Head;

                case 2:
                    return ItemSlot.Neck;

                case 3:
                    return ItemSlot.Shoulders;

                case 16:
                    return ItemSlot.Back;

                case 5:
                case 20:
                    return ItemSlot.Chest;

                case 4:
                    return ItemSlot.Shirt;

                case 19:
                    return ItemSlot.Tabard;

                case 9:
                    return ItemSlot.Wrist;

                case 10:
                    return ItemSlot.Hands;

                case 6:
                    return ItemSlot.Waist;

                case 7:
                    return ItemSlot.Legs;

                case 8:
                    return ItemSlot.Feet;

                case 11:
                    return ItemSlot.Finger;

                case 12:
                    return ItemSlot.Trinket;

                case 13:
                    return ItemSlot.OneHand;

                case 17:
                    return ItemSlot.TwoHand;

                case 21:
                    return ItemSlot.MainHand;

                case 14:
                case 22:
                case 23:
                    return ItemSlot.OffHand;

                case 15:
                case 25:
                case 26:
                case 28:
                    return ItemSlot.Ranged;

                case 24:
                    return ItemSlot.Projectile;

                case 27:
                    return ItemSlot.ProjectileBag;

                default:
                    return ItemSlot.None;
            }
        }
    }

    public class CharacterRequest
    {
        private readonly string name;
        public string Name { get { return name; } }

        private readonly string realm;
        public string Realm { get { return realm; } }

        private readonly CharacterRegion region;
        public CharacterRegion Region { get { return region; } }

        private readonly Action<Character> callback;
        public Action<Character> Callback { get { return callback; } }

        public XDocument CharacterSheet { get; set; }
        public XDocument TalentTree { get; set; }

        private int ItemsToLoad;
        private int TotalItemsToLoad;

        public Character Result { get; set; }

        public void Invoke()
        {
            if (ItemsToLoad == 0 && TalentTree == null && CharacterSheet == null && callback != null) 
                callback(Result);
        }

        public CharacterRequest(string name, string realm, CharacterRegion region, Action<Character> callback)
        {
            this.name = name;
            this.realm = realm;
            this.region = region;
            this.callback = callback;
            ItemsToLoad = 0;
            GetCharacter();
        }

        public void GetCharacter()
        {
            StatusMessaging.UpdateStatus("Get Character From Armory", "Downloading Character Definition");
            StatusMessaging.UpdateStatus("Get Talent Tree From Armory", "Queued");
            new NetworkUtils(CharacterSheet_Completed).GetCharacterSheetDocument(name, realm, region);
        }

        private void CharacterSheet_Completed(object sender, EventArgs e)
        {
            NetworkUtils network = sender as NetworkUtils;

            CharacterSheet = network.Result;
            XDocument docCharacter = CharacterSheet;

            try
            {
                XElement characterInfo = docCharacter.Element("page").Element("characterInfo");
                StatusMessaging.UpdateStatus("Get Character From Armory", "Processing Character Definition");

                XElement characterElement = characterInfo.Element("character");
                CharacterRace race = (CharacterRace)int.Parse(characterElement.Attribute("raceId").Value);
                CharacterClass charClass = (CharacterClass)int.Parse(characterElement.Attribute("classId").Value);
                string name = characterElement.Attribute("name").Value;
                string realm = characterElement.Attribute("realm").Value;
				
				StatusMessaging.UpdateStatus("Get Character From Armory", "A");

                Dictionary<CharacterSlot, string> items = new Dictionary<CharacterSlot, string>();
                foreach (XElement itemNode in characterInfo.Element("characterTab").Element("items").Elements("item"))
                {
                    int slot = int.Parse(itemNode.Attribute("slot").Value) + 1;
                    CharacterSlot cslot = Character.GetCharacterSlotFromId(slot);
                    items[cslot] = string.Format("{0}.{1}.{2}.{3}.{4}", itemNode.Attribute("id").Value,
                        itemNode.Attribute("gem0Id").Value, itemNode.Attribute("gem1Id").Value, itemNode.Attribute("gem2Id").Value,
                        itemNode.Attribute("permanentenchant").Value);
                }

                BossList bosslist = new BossList();
                BossHandler boss = bosslist.TheEZModeBoss_Called;

				StatusMessaging.UpdateStatus("Get Character From Armory", "B");

                EnsureItemsLoaded(items.Values);

				StatusMessaging.UpdateStatus("Get Character From Armory", "C");

                Result = new Character(name, realm, Region, race, boss,
                    items.ContainsKey(CharacterSlot.Head) ? items[CharacterSlot.Head] : null,
                    items.ContainsKey(CharacterSlot.Neck) ? items[CharacterSlot.Neck] : null,
                    items.ContainsKey(CharacterSlot.Shoulders) ? items[CharacterSlot.Shoulders] : null,
                    items.ContainsKey(CharacterSlot.Back) ? items[CharacterSlot.Back] : null,
                    items.ContainsKey(CharacterSlot.Chest) ? items[CharacterSlot.Chest] : null,
                    items.ContainsKey(CharacterSlot.Shirt) ? items[CharacterSlot.Shirt] : null,
                    items.ContainsKey(CharacterSlot.Tabard) ? items[CharacterSlot.Tabard] : null,
                    items.ContainsKey(CharacterSlot.Wrist) ? items[CharacterSlot.Wrist] : null,
                    items.ContainsKey(CharacterSlot.Hands) ? items[CharacterSlot.Hands] : null,
                    items.ContainsKey(CharacterSlot.Waist) ? items[CharacterSlot.Waist] : null,
                    items.ContainsKey(CharacterSlot.Legs) ? items[CharacterSlot.Legs] : null,
                    items.ContainsKey(CharacterSlot.Feet) ? items[CharacterSlot.Feet] : null,
                    items.ContainsKey(CharacterSlot.Finger1) ? items[CharacterSlot.Finger1] : null,
                    items.ContainsKey(CharacterSlot.Finger2) ? items[CharacterSlot.Finger2] : null,
                    items.ContainsKey(CharacterSlot.Trinket1) ? items[CharacterSlot.Trinket1] : null,
                    items.ContainsKey(CharacterSlot.Trinket2) ? items[CharacterSlot.Trinket2] : null,
                    items.ContainsKey(CharacterSlot.MainHand) ? items[CharacterSlot.MainHand] : null,
                    items.ContainsKey(CharacterSlot.OffHand) ? items[CharacterSlot.OffHand] : null,
                    items.ContainsKey(CharacterSlot.Ranged) ? items[CharacterSlot.Ranged] : null,
                    items.ContainsKey(CharacterSlot.Projectile) ? items[CharacterSlot.Projectile] : null,
                    null);
				Result.Class = charClass;
				if (ItemsToLoad == 0) InitializeAvailableItemList();

				StatusMessaging.UpdateStatus("Get Character From Armory", "D");

                StatusMessaging.UpdateStatus("Get Talent Tree From Armory", "Downloading Talent Tree");
                StatusMessaging.UpdateStatusFinished("Get Character From Armory");
                network.DocumentReady -= new EventHandler(CharacterSheet_Completed);
                network.DocumentReady += new EventHandler(TalentTree_Completed);
                network.GetTalentTreeDocument(Name, Realm, Region);
            }
            catch
            {
                //StatusMessaging.UpdateStatusFinished("Get Character From Armory");
                //StatusMessaging.UpdateStatusFinished("Get Talent Tree From Armory");
                //StatusMessaging.ReportError("Get Character From Armory", null, "No character returned from the Armory");
                Result = null;
                TalentTree = null;
                CharacterSheet = null;
                //Invoke();
            }
        }

        private void EnsureItemsLoaded(IEnumerable<string> items)
        {
			foreach (string item in items)
            {
				string[] itemids = item.Split('.');
                for (int i = 0; i < 4; i++)
                {
					int id = int.Parse(itemids[i]);
					if (id > 0 && !ItemCache.ContainsItemId(id))
                    {
						ItemsToLoad++;
                        TotalItemsToLoad++;
                        StatusMessaging.UpdateStatus("Get Items from Armory",
                            string.Format("{0} of {1} Loaded", TotalItemsToLoad - ItemsToLoad, TotalItemsToLoad));
                        new ItemRequest(id, ItemLoaded);
                    }
                }
            }
		}

        private void ItemLoaded(Item item)
        {
            ItemCache.AddItem(item, false);
            ItemsToLoad--;
            if (ItemsToLoad == 0)
            {
                StatusMessaging.UpdateStatusFinished("Get Items from Armory");
                InitializeAvailableItemList();
                Invoke();
            }
            else StatusMessaging.UpdateStatus("Get Items from Armory",
                string.Format("{0} of {1} Loaded", TotalItemsToLoad - ItemsToLoad, TotalItemsToLoad));
        }

        private void TalentTree_Completed(object sender, EventArgs e)
        {
            NetworkUtils network = sender as NetworkUtils;

            TalentTree = network.Result;

            XElement activeGroup = null;
            StatusMessaging.UpdateStatus("Get Talent Tree From Armory", "Processing Talent Tree");
			foreach (XElement group in TalentTree.Element("page").Element("characterInfo")
                .Element("talents").Elements("talentGroup"))
            {
                if (group.Attribute("active") != null) activeGroup = group;
            }
            string talentCode = activeGroup.Element("talentSpec").Attribute("value").Value;
            switch (Result.Class)
            {
                case CharacterClass.Warrior:
                    Result.WarriorTalents = new WarriorTalents(talentCode);
                    if (Result.WarriorTalents.Devastate > 0) Result.CurrentModel = "ProtWarr";
                    else Result.CurrentModel = "DPSWarr";
                    break;
                case CharacterClass.Paladin:
                    Result.PaladinTalents = new PaladinTalents(talentCode);
                    if (Result.PaladinTalents.HolyShield > 0) Result.CurrentModel = "ProtPaladin";
                    else if (Result.PaladinTalents.CrusaderStrike > 0) Result.CurrentModel = "Retribution";
                    else Result.CurrentModel = "Healadin";
                    break;
                case CharacterClass.Hunter:
                    Result.HunterTalents = new HunterTalents(talentCode);
                    Result.CurrentModel = "Hunter";
                    break;
                case CharacterClass.Rogue:
                    Result.RogueTalents = new RogueTalents(talentCode);
                    Result.CurrentModel = "Rogue";
                    break;
                case CharacterClass.Priest:
                    Result.PriestTalents = new PriestTalents(talentCode);
                    if (Result.PriestTalents.Shadowform > 0) Result.CurrentModel = "ShadowPriest";
                    else Result.CurrentModel = "HealPriest";
                    break;
                case CharacterClass.Shaman:
                    Result.ShamanTalents = new ShamanTalents(talentCode);
                    if (Result.ShamanTalents.ElementalMastery > 0) Result.CurrentModel = "Elemental";
                    else if (Result.ShamanTalents.Stormstrike > 0) Result.CurrentModel = "Enhance";
                    else Result.CurrentModel = "RestoSham";
                    break;
                case CharacterClass.Mage:
                    Result.MageTalents = new MageTalents(talentCode);
                    Result.CurrentModel = "Mage";
                    break;
                case CharacterClass.Warlock:
                    Result.WarlockTalents = new WarlockTalents(talentCode);
                    Result.CurrentModel = "Warlock";
                    break;
                case CharacterClass.Druid:
                    Result.DruidTalents = new DruidTalents(talentCode);
                    if (Result.DruidTalents.ProtectorOfThePack > 0) Result.CurrentModel = "Bear";
                    else if (Result.DruidTalents.LeaderOfThePack > 0) Result.CurrentModel = "Cat";
                    else if (Result.DruidTalents.MoonkinForm > 0) Result.CurrentModel = "Moonkin";
                    else Result.CurrentModel = "Tree";
                    break;
                case CharacterClass.DeathKnight:
                    Result.DeathKnightTalents = new DeathKnightTalents(talentCode);
                    if (Result.DeathKnightTalents.Anticipation > 0) Result.CurrentModel = "TankDK";
                    else Result.CurrentModel = "DPSDK";
                    break;
                default:
                    break;
            }
            TalentsBase talents = Result.CurrentTalents;
            Dictionary<string, PropertyInfo> glyphProperty = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo pi in talents.GetType().GetProperties())
            {
                GlyphDataAttribute[] glyphDatas = pi.GetCustomAttributes(typeof(GlyphDataAttribute), true) as GlyphDataAttribute[];
                if (glyphDatas.Length > 0)
                {
                    GlyphDataAttribute glyphData = glyphDatas[0];
                    glyphProperty[glyphData.Name] = pi;
                }
            }

            foreach (XElement glyph in activeGroup.Element("glyphs").Elements("glyph"))
            {
                PropertyInfo pi;
                if (glyphProperty.TryGetValue(glyph.Attribute("name").Value, out pi))
                {
                    pi.SetValue(talents, true, null);
                }
            }
			StatusMessaging.UpdateStatus("Get Talent Tree From Armory", Result.CurrentModel);
			ApplyRacialandProfessionBuffs();

            StatusMessaging.UpdateStatusFinished("Get Talent Tree From Armory");
            TalentTree = null;
            CharacterSheet = null;
            Invoke();
        }

        /// <summary>
        /// 09.01.01 - TankConcrete
        /// Sets up the initial list of gear that the toon has available. Method looks the
        /// gear that is currently equipped and adds it to the "AvailableItems" list if it
        /// is not already there.
        /// 
        /// If a new item is added to the "AvailableItems" list, the "unsaved" changes flag
        /// will be updated so the user knows to save.
        /// </summary>
        /// <param name="currentChar"></param>
		private void InitializeAvailableItemList()
		{
			for (CharacterSlot slot = 0; slot < (CharacterSlot)19; slot++)
			{
				ItemInstance item = Result[slot];
				if ((object)item != null && item.Id != 0)
				{
					if (!Result.AvailableItems.Contains(item.Id.ToString())) Result.AvailableItems.Add(item.Id.ToString());
					Enchant enchant = item.Enchant;
					if (enchant != null && enchant.Id != 0)
					{
						string enchantString = (-1 * (enchant.Id + (10000 * (int)enchant.Slot))).ToString();
						if (!Result.AvailableItems.Contains(enchantString)) Result.AvailableItems.Add(enchantString);
					}
				}
			}
		}

        private void ApplyRacialandProfessionBuffs()
        {
            if (Result.Race == CharacterRace.Draenei) Result.ActiveBuffs.Add(Buff.GetBuffByName("Heroic Presence"));

            foreach (XElement profession in CharacterSheet.Element("page").Element("characterInfo")
                .Element("characterTab").Element("professions").Elements("skill"))
            {   // apply profession buffs if max skill
                if (profession.Attribute("name").Value == "Mining" && profession.Attribute("value").Value == "450")
                    Result.ActiveBuffs.Add(Buff.GetBuffByName("Toughness"));
                if (profession.Attribute("name").Value == "Skinning" && profession.Attribute("value").Value == "450")
                    Result.ActiveBuffs.Add(Buff.GetBuffByName("Master of Anatomy"));
                if (profession.Attribute("name").Value == "Blacksmithing" && int.Parse(profession.Attribute("value").Value) >= 400)
                {
                    Result.WristBlacksmithingSocketEnabled = true;
                    Result.HandsBlacksmithingSocketEnabled = true;
                }
            }

			StatusMessaging.UpdateStatus("Get Talent Tree From Armory", Result.CurrentModel);
			Calculations.GetModel(Result.CurrentModel).SetDefaults(Result);
        }
    }
	*/
}
