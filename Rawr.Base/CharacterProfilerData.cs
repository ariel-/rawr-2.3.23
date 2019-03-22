using System.Collections.Generic;
using System;
using System.Diagnostics;
#if !RAWR3 && !SILVERLIGHT
using System.Windows.Forms;
#endif
using System.IO;
using System.Reflection;

namespace Rawr
{
	/* CharacterProfilerCharacter
	 * @author Charinna
	 * This class is a container of Character classes that imports its data from a saved variables
	 * file created by the CharacterProfiler mod.
	 * It automatically populates the character with the currently equipped gear as
	 * well as make available for optimization all items in the character's inventory
	 * with their current gemming & enchants.
	 */
	public class CharacterProfilerCharacter
	{
		public string Name
		{
			get { return m_sName; }
		}

		public string Summary
		{
			get { return "Level " + m_iLevel + " " + m_sRace + " " + m_sClass; }
		}

		public Character Character
		{
			get { return m_character; }
		}

		static readonly string[] s_asEquippableTooltipKeywords =
        {
            // English
            "<br>Head",
            "<br>Neck",
            "<br>Shoulder",
            "<br>Back",
            "<br>Chest",
            "<br>Wrist",
            "<br>Hands",
            "<br>Waist",
            "<br>Legs",
            "<br>Feet",
            "<br>Finger",
            "<br>Trinket",
            "<br>Main Hand",
            "<br>One-Hand",
            "<br>Off Hand",
            "<br>Held In Off-hand",
            "<br>Two-Hand",
            "<br>Relic",
            "<br>Ranged",
            "<br>Thrown",
            "<br>Projectile",

            // German
            "<br>Kopf",
            "<br>Hals",
            "<br>Schulter",
            "<br>Rücken",
            "<br>Brust",
            "<br>Handgelenke",
            "<br>Hände",
            "<br>Taille",
            "<br>Beine",
            "<br>Füße",
            "<br>Finger",
            "<br>Schmuck",
            "<br>Waffenhand",
            "<br>Einhändig",
            "<br>Schildhand",
            "<br>In Schildhand geführt",
            "<br>Zweihändig",
            "<br>Relikt",
            "<br>Distanz",
            "<br>Wurfwaffe",
            "<br>Projektil",

            // French
            "<br>Tête",
            "<br>Cou",
            "<br>Épaules",
            "<br>Dos",
            "<br>Torse",
            "<br>Poignets",
            "<br>Mains",
            "<br>Taille",
            "<br>Jambes",
            "<br>Pieds",
            "<br>Doigt",
            "<br>Bijou",
            "<br>Main droite",
            "<br>À une main",
            "<br>Main gauche",
            "<br>Tenu en main gauche",
            "<br>Deux mains",
            "<br>Relique",
            "<br>À distance",
            "<br>Armes de jet",
            "<br>Projectile",

            // Russian
            "<br>Голова",
            "<br>Шея",
            "<br>Плечо",
            "<br>Спина",
            "<br>Грудь",
            "<br>Запястья",
            "<br>Кисти рук",
            "<br>Пояс",
            "<br>Ноги",
            "<br>Ступни",
            "<br>Палец",
            "<br>Аксессуар",
            "<br>Правая рука",
            "<br>Одноручное",
            "<br>Левая рука",
            "<br>Двуручное",
            "<br>Реликвия",
            "<br>Дальний бой",
            "<br>Метательное",
            "<br>Projectile",
        };

		static CharacterProfilerCharacter()
		{
		}

		static int getEnchant(SavedVariablesDictionary item)
		{
			string sItemString = item["Item"] as string;
			char[] acSplitCharacters = { ':' };
			return Int32.Parse(sItemString.Split(acSplitCharacters)[1]);
		}

		static int getEnchantBySlot(SavedVariablesDictionary characterInfo, string sSlot)
		{
			SavedVariablesDictionary equipment = (SavedVariablesDictionary)characterInfo["Equipment"];

			if (equipment.ContainsKey(sSlot))
			{
				SavedVariablesDictionary item = equipment[sSlot] as SavedVariablesDictionary;
				return getEnchant(item);
			}
			else
			{
				return 0;
			}
		}

		static string getGearString(SavedVariablesDictionary item, bool replaceAsterisks)
		{
			string sItemString = item["Item"] as string;
			char[] acSplitCharacters = { ':' };
			string[] asItemElements = sItemString.Split(acSplitCharacters);

			if (item.ContainsKey("Gem"))
			{
				SavedVariablesDictionary gems = item["Gem"] as SavedVariablesDictionary;

				string sItemSlotString = "";
				for (long lGemSlot = 1; lGemSlot <= 3; lGemSlot++)
				{
					sItemSlotString += ".";

					if (gems.ContainsKey(lGemSlot))
					{
						string sGemItemString = (gems[lGemSlot] as SavedVariablesDictionary)["Item"] as string;
						sItemSlotString += sGemItemString.Split(acSplitCharacters)[0];
					}
					else
					{
                        sItemSlotString += "0";
					}
				}

                if (replaceAsterisks && sItemSlotString == ".0.0.0")
                {
                    return asItemElements[0] + ".*.*.*";
                }
                else
                {
                    return asItemElements[0] + sItemSlotString;
                }
			}
			else
			{
				if (replaceAsterisks)
				{
                    return asItemElements[0] + ".*.*.*";
                }
				else
				{
                    return asItemElements[0] + ".0.0.0";
                }
			}
		}

		static string getGearStringBySlot(SavedVariablesDictionary characterInfo, string sSlot, bool replaceAsterisks)
		{
			SavedVariablesDictionary equipment = (SavedVariablesDictionary)characterInfo["Equipment"];

			if (equipment.ContainsKey(sSlot))
			{
				SavedVariablesDictionary item = equipment[sSlot] as SavedVariablesDictionary;
				string enchant = getEnchant(item).ToString();
				if (enchant == "0" && replaceAsterisks)
				{
					enchant = "*";
				}
				return getGearString(item, replaceAsterisks) + "." + enchant;
			}
			else
			{
				return null;
			}
		}

		/*
		 * This function is used to help populate the optimizer list.
		 * Rather than add every item in every bag slot, this filter is used
		 * to try to limit the items only to equippable ones.
		 * Note however that the current implentation is a bit of a hack
		 * that involves searching item Tooltips.
		 * Returns true if the item is equippable.
		 */
		static bool isEquippable(SavedVariablesDictionary itemInfo)
		{
			if (itemInfo != null && itemInfo.ContainsKey("Tooltip"))
			{
				string sTooltip = itemInfo["Tooltip"] as string;

				foreach (string sNeedle in s_asEquippableTooltipKeywords)
				{
					if (sTooltip.IndexOf(sNeedle) != -1)
					{
						return true;
					}
				}
			}

			return false;
		}

		static bool addEquippedItemForOptimization(List<string> asOptimizableItems,
			SavedVariablesDictionary characterInfo, string sSlot)
		{
			string sItem = getGearStringBySlot(characterInfo, sSlot, true);

			if (sItem != null)
			{
                char[] acSplitCharacters = { '.' };
                string[] asItemElements = sItem.Split(acSplitCharacters);
                if (sItem.Substring(asItemElements[0].Length) == ".*.*.*.*")
                {
                    sItem = asItemElements[0];
                }
                asOptimizableItems.Add(sItem);
				return true;
			}

			return false;
		}

		static void addPossessionsForOptimization(List<string> asOptimizableItems,
			SavedVariablesDictionary characterInfo)
		{
			string[] asSources = { "Inventory", "Bank" };

			foreach (string sSource in asSources)
			{
				if (characterInfo.ContainsKey(sSource))
				{
					SavedVariablesDictionary bags = characterInfo[sSource] as SavedVariablesDictionary;

					foreach (object oBag in bags.Values)
					{
						SavedVariablesDictionary bag = oBag as SavedVariablesDictionary;
						SavedVariablesDictionary contents = bag["Contents"] as SavedVariablesDictionary;

						foreach (object oItem in contents.Values)
						{
							SavedVariablesDictionary item = oItem as SavedVariablesDictionary;

							if (isEquippable(item))
							{
                                string enchant = getEnchant(item).ToString();
                                if (enchant == "0")
                                {
                                    enchant = "*";
                                }
                                string item2 = getGearString(item, true) + "." + enchant;
                                char[] acSplitCharacters = { '.' };
                                string[] asItemElements = item2.Split(acSplitCharacters);
                                if (item2.Substring(asItemElements[0].Length) == ".*.*.*.*")
                                {
                                    item2 = asItemElements[0];
                                }
                                asOptimizableItems.Add(item2);
							}
						}
					}
				}
			}
		}

        static int getTalentPointsFromTree(SavedVariablesDictionary talent_tree, string spec, TalentDataAttribute talent)
		{
			int points = 0;

			if (talent_tree.ContainsKey(spec))
			{
				SavedVariablesDictionary spec_tree = talent_tree[spec] as SavedVariablesDictionary;

				if (spec_tree.ContainsKey(talent.Name))
				{
					SavedVariablesDictionary talent_info = spec_tree[talent.Name] as SavedVariablesDictionary;

					string rank_info = talent_info["Rank"] as string;

					int split_pos = rank_info.IndexOf(':');
					string points_str = rank_info.Remove(split_pos);

					points = (int)Int32.Parse(points_str);
				}
				else
				{
					Debug.WriteLine("Talent Not Found: " + talent);
				}
			}
			else
			{
                // we're most likely dealing with non-English data, try to determine by position in tree
                foreach (SavedVariablesDictionary tree in talent_tree.Values)
                {
                    if ((long)tree["Order"] == talent.Tree + 1)
                    {
                        string loc = talent.Row + ":" + talent.Column;
                        foreach (object t in tree.Values)
                        {
                            SavedVariablesDictionary td = t as SavedVariablesDictionary;
                            if (td != null)
                            {
                                if ((string)td["Location"] == loc)
                                {
                                    string rank_info = td["Rank"] as string;

                                    int split_pos = rank_info.IndexOf(':');
                                    string points_str = rank_info.Remove(split_pos);

                                    points = (int)Int32.Parse(points_str);
                                    return points;
                                }
                            }
                        }
                    }
                }
				Debug.WriteLine("Talent Tree Not Found: " + spec);
			}

			return points;
		}

		void setTalentsFromTree(SavedVariablesDictionary characterInfo)
		{
            if (!characterInfo.ContainsKey("Talents"))
            {
#if RAWR3 || SILVERLIGHT
                //new ErrorWindow() { Message = "Not yet implemented." }.Show();
#else
                MessageBox.Show("Talent data was not found, and must be manually added.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                return;
            }

			SavedVariablesDictionary talent_tree = characterInfo["Talents"] as SavedVariablesDictionary;

			TalentsBase Talents = m_character.CurrentTalents;

			if (Talents != null)
			{
				List<string> treeNames = new List<string>((string[])Talents.GetType().GetField("TreeNames").GetValue(Talents));

				//TalentTree currentTree;
				foreach (PropertyInfo pi in Talents.GetType().GetProperties())
				{
					TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
					if (talentDatas.Length > 0)
					{
						TalentDataAttribute talentData = talentDatas[0];

						int points = getTalentPointsFromTree(talent_tree, treeNames[talentData.Tree], talentData);
						m_character.CurrentTalents.Data[talentData.Index] = points;
					}
				}
			}
		}

		/*
		public Character(string name, string realm, CharacterRegion region, CharacterRace race,
		 * string head, string neck, string shoulders, string back, string chest, string shirt, string tabard,
				string wrist, string hands, string waist, string legs, string feet, string finger1, string finger2, string trinket1, string trinket2,
		 * string mainHand, string offHand, string ranged, string projectile, string projectileBag,
			int enchantHead, int enchantShoulders, int enchantBack, int enchantChest, int enchantWrist, 
		 * int enchantHands, int enchantLegs, int enchantFeet, int enchantFinger1, int enchantFinger2, int enchantMainHand, int enchantOffHand, int enchantRanged)
		 * */

		public CharacterProfilerCharacter(string sName, string sRealm, SavedVariablesDictionary characterInfo)
		{
			m_characterInfo = characterInfo;
			m_sName = sName;
            m_sRealm = sRealm;
			m_iLevel = (int)(characterInfo["Level"] as long?);

			m_sRace = (string)characterInfo["Race"];
			m_sClass = (string)characterInfo["Class"];
        }

		public void loadFromInfo()
        {
			CharacterRace race = (CharacterRace)(m_characterInfo["RaceId"] as long?);
			CharacterClass charClass = (CharacterClass)(m_characterInfo["ClassId"] as long?);

			// it might be possible to get this from the Locale field, but I'd need data from the other regions
			CharacterRegion charRegion = CharacterRegion.US;

            m_character = new Character(m_sName, m_sRealm,
			    charRegion,
			    race,
                new BossOptions(),
		        getGearStringBySlot(m_characterInfo, "Head", false),
                getGearStringBySlot(m_characterInfo, "Neck", false),
                getGearStringBySlot(m_characterInfo, "Shoulder", false),
                getGearStringBySlot(m_characterInfo, "Back", false),
                getGearStringBySlot(m_characterInfo, "Chest", false),
                getGearStringBySlot(m_characterInfo, "Shirt", false),
                getGearStringBySlot(m_characterInfo, "Tabard", false),
                getGearStringBySlot(m_characterInfo, "Wrist", false),
                getGearStringBySlot(m_characterInfo, "Hands", false),
                getGearStringBySlot(m_characterInfo, "Waist", false),
                getGearStringBySlot(m_characterInfo, "Legs", false),
                getGearStringBySlot(m_characterInfo, "Feet", false),
                getGearStringBySlot(m_characterInfo, "Finger0", false),
                getGearStringBySlot(m_characterInfo, "Finger1", false),
                getGearStringBySlot(m_characterInfo, "Trinket0", false),
                getGearStringBySlot(m_characterInfo, "Trinket1", false),
                getGearStringBySlot(m_characterInfo, "MainHand", false),
                getGearStringBySlot(m_characterInfo, "SecondaryHand", false),
                getGearStringBySlot(m_characterInfo, "Ranged", false),
                getGearStringBySlot(m_characterInfo, "Ammo", false),
		        null // Not sure what projectile bag is called
			        /*null, //TODO: Find ExtraWristSocket
			        null, //TODO: Find ExtraHandsSocket
			        null, //TODO: Find ExtraWaistSocket
			        getEnchantBySlot(characterInfo, "Head"),
			        getEnchantBySlot(characterInfo, "Shoulder"),
			        getEnchantBySlot(characterInfo, "Back"),
			        getEnchantBySlot(characterInfo, "Chest"),
			        getEnchantBySlot(characterInfo, "Wrist"),
			        getEnchantBySlot(characterInfo, "Hands"),
			        getEnchantBySlot(characterInfo, "Legs"),
			        getEnchantBySlot(characterInfo, "Feet"),
			        getEnchantBySlot(characterInfo, "Finger0"),
			        getEnchantBySlot(characterInfo, "Finger1"),
			        getEnchantBySlot(characterInfo, "MainHand"),
			        getEnchantBySlot(characterInfo, "SecondaryHand"),
			        getEnchantBySlot(characterInfo, "Ranged")*/
			);

			// set the character class
			Character.Class = charClass;

			// only try and load the talents if they actually have them
			if (m_iLevel >= 10)
			{
				// create an empty talent tree
				switch (charClass) {
					case CharacterClass.Warrior:
                        m_character.WarriorTalents = new WarriorTalents();
                        //m_character.WarriorTalentsCata = new WarriorTalentsCata();
                        break;
					case CharacterClass.Paladin: m_character.PaladinTalents = new PaladinTalents(); break;
					case CharacterClass.Hunter: m_character.HunterTalents = new HunterTalents(); break;
					case CharacterClass.Rogue: m_character.RogueTalents = new RogueTalents(); break;
					case CharacterClass.Priest: m_character.PriestTalents = new PriestTalents(); break;
					case CharacterClass.Shaman: m_character.ShamanTalents = new ShamanTalents(); break;
					case CharacterClass.Mage: m_character.MageTalents = new MageTalents(); break;
					case CharacterClass.Warlock: m_character.WarlockTalents = new WarlockTalents(); break;
					case CharacterClass.Druid: m_character.DruidTalents = new DruidTalents(); break;
					case CharacterClass.DeathKnight: m_character.DeathKnightTalents = new DeathKnightTalents(); break;
					default: break;
				}

				// load up the talents
				setTalentsFromTree(m_characterInfo);
			}

			// Populate available items
			// Note that some of these items cannot be enchanted
			// But they should correctly return ".0" for their enchants.
			List<string> asOptimizableItems = new List<string>();
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Head");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Neck");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Shoulder");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Back");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Chest");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Shirt");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Tabard");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Wrist");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Hands");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Waist");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Legs");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Feet");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Finger0");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Finger1");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Trinket0");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Trinket1");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "MainHand");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "SecondaryHand");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Ranged");
			addEquippedItemForOptimization(asOptimizableItems, m_characterInfo, "Ammo");

			addPossessionsForOptimization(asOptimizableItems, m_characterInfo);

			m_character.AvailableItems = asOptimizableItems;
		}

		string m_sName;
        string m_sRealm;
		string m_sRace;
		string m_sClass;
		int m_iLevel;
		SavedVariablesDictionary m_characterInfo;
		Character m_character;
	}

	public class CharacterProfilerRealm
	{
		string m_sName;
		List<CharacterProfilerCharacter> m_aCharacters = new List<CharacterProfilerCharacter>();

		public CharacterProfilerRealm(string sName)
		{
			m_sName = sName;
		}

		public string Name { get { return m_sName; } }
        
		public List<CharacterProfilerCharacter> Characters { get { return m_aCharacters; } }
	}

	public class CharacterProfilerFailedImport
	{
		string m_sRealm;
		string m_sCharacter;
		string m_sError;

		public CharacterProfilerFailedImport(string sRealm, string sCharacter, string sError)
		{
			m_sRealm = sRealm;
			m_sCharacter = sCharacter;
			m_sError = sError;
		}

		public string Realm
		{
			get { return m_sRealm; }
		}

		public string Character
		{
			get { return m_sCharacter; }
		}

		public string Error
		{
			get { return m_sError; }
		}
	}

	public class CharacterProfilerData
	{
		List<CharacterProfilerRealm> m_realms = new List<CharacterProfilerRealm>();
		List<CharacterProfilerFailedImport> m_errors = new List<CharacterProfilerFailedImport>();

		public List<CharacterProfilerRealm> Realms { get { return m_realms; } }

		public List<CharacterProfilerFailedImport> Errors { get { return m_errors; } }

		public CharacterProfilerData(string sFileName)
		{
			SavedVariablesDictionary savedVariables = SavedVariablesParser.parse(sFileName);

			// TODO: check the version

			if (!savedVariables.ContainsKey("myProfile"))
			{
                throw new InvalidDataException("Expected myProfile variable in file.");
            }

			SavedVariablesDictionary realms = (SavedVariablesDictionary)savedVariables["myProfile"];

			foreach (string sRealm in realms.Keys)
			{
				bool bHaveCharacters = false;

				CharacterProfilerRealm realm = new CharacterProfilerRealm(sRealm);

				SavedVariablesDictionary characterContainer = (SavedVariablesDictionary)realms[sRealm];
				SavedVariablesDictionary characters = (SavedVariablesDictionary)characterContainer["Character"];

				foreach (string sCharacter in characters.Keys)
				{
					try {
						SavedVariablesDictionary characterInfo = (SavedVariablesDictionary)characters[sCharacter];
						CharacterProfilerCharacter character = new CharacterProfilerCharacter(sCharacter, sRealm, characterInfo);
						realm.Characters.Add(character);
						bHaveCharacters = true;
					} catch (Exception error) {
						m_errors.Add(new CharacterProfilerFailedImport(sRealm, sCharacter, error.ToString()));
					}
				}

				if (bHaveCharacters) { m_realms.Add(realm); }
			}
		}
	}

}
