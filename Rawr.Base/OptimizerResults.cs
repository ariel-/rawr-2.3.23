using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    public class OptimizerResults
    {
        Character _before;
        Character _after;
        bool _talents;

        public OptimizerResults(Character before, Character after, bool talents)
        {
            _before = before.Clone();
            _after = after.Clone();
            _talents = talents;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            reportItemDifferences(sb);
            sb.AppendLine();
            return sb.ToString();
        }

        private void reportItemDifferences(StringBuilder sb)
        {
            Enchant noEnchant = Enchant.FindEnchant(0, ItemSlot.None, _before);
            Item noItem = new Item();
            noItem.Name = "No item equipped";
            Item noGem = new Item();
            ItemInstance noItemInstance = new ItemInstance();
            ItemInstance[] beforeItems = _before.GetItems();
            ItemInstance[] afterItems = _after.GetItems();
            int changes = 0;
            for (int i = 1; i < Character.SlotCount; i++)
            {
                CharacterSlot slot = (CharacterSlot)i;
                if (slot != CharacterSlot.Shirt && slot != CharacterSlot.Tabard)
                {
                    ItemInstance before = beforeItems[i] ?? noItemInstance;
                    ItemInstance after = afterItems[i] ?? noItemInstance;
                    bool swappedRingTrinket = false;
                    if (slot == CharacterSlot.Finger1 || slot == CharacterSlot.Trinket1)
                    {
                        ItemInstance after2 = afterItems[i+1] ?? noItemInstance;
                        if (before != noItemInstance && after2 != noItemInstance && before.Equals(after2))
                        {
                            swappedRingTrinket = true;
                            i++; // force skip over finger2 and trinket2.
                        }
                    }
                    if (!(before == noItemInstance && after == noItemInstance) && !before.Equals(after) && !swappedRingTrinket)
                    {
                        string itemname = (before.Item ?? noItem).Name;
                        string gem1name = before.Gem1 == null ? "" : "with " + before.Gem1.Name;
                        string gem2name = before.Gem2 == null ? "" : ", " + before.Gem2.Name;
                        string gem3name = before.Gem3 == null ? "" : ", " + before.Gem3.Name;
                        string enchantname = before.Enchant == null ? "" : ", " + before.Enchant.Name;
                        sb.AppendFormat("{0}: Changed {1} {2}{3}{4}{5}", slot, itemname, gem1name, gem2name, gem3name, enchantname);
                        sb.AppendLine();
                        itemname = (after.Item ?? noItem).Name;
                        gem1name = after.Gem1 == null ? "" : "with " + after.Gem1.Name;
                        gem2name = after.Gem2 == null ? "" : ", " + after.Gem2.Name;
                        gem3name = after.Gem3 == null ? "" : ", " + after.Gem3.Name;
                        enchantname = after.Enchant == null ? "" : ", " + after.Enchant.Name;
                        sb.AppendFormat("               to {0} {1}{2}{3}{4}", itemname, gem1name, gem2name, gem3name, enchantname);
                        sb.AppendLine();
                        changes++;
                    }
                }
            }
            if (changes == 0)
                sb.AppendLine("No item changes found");
        }
    }
}
