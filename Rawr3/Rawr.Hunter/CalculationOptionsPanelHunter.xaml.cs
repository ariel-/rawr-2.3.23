using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Xml.Serialization;
using Rawr.Base;
using System.Text;

namespace Rawr.Hunter {
    public partial class CalculationOptionsPanelHunter : ICalculationOptionsPanel
    {
        #region Instance Variables
        private PetAttacks[] familyList = null;
        private List<ComboBox> ShotPriorityBoxes = new List<ComboBox>();
        private Dictionary<string, string> FAQStuff = new Dictionary<string, string>();
        private Dictionary<string, string> PNStuff = new Dictionary<string, string>();
        private int _CurrentSpec;
        private int CurrentSpec
        {
            get { return _CurrentSpec; }
            set { _CurrentSpec = value; }
        }
        public enum Specs { BeastMaster = 1, Marksman, Survival }
        #endregion

        #region Constructors
        public CalculationOptionsPanelHunter()
        {
            _loadingCalculationOptions = true;
            InitializeComponent();
            SV_Page01.SetIsMouseWheelScrollingEnabled(true);
            PetBuffs.Character = Character;
            //
            SetUpFAQ();
            SetUpPatchNotes();
            SetUpOther();
            //
            // This should now be the only group of 10 lines, the rest are loop-capable
            ShotPriorityBoxes.Clear();
            ShotPriorityBoxes.Add(CB_ShotPrio_01);
            ShotPriorityBoxes.Add(CB_ShotPrio_02);
            ShotPriorityBoxes.Add(CB_ShotPrio_03);
            ShotPriorityBoxes.Add(CB_ShotPrio_04);
            ShotPriorityBoxes.Add(CB_ShotPrio_05);
            ShotPriorityBoxes.Add(CB_ShotPrio_06);
            ShotPriorityBoxes.Add(CB_ShotPrio_07);
            ShotPriorityBoxes.Add(CB_ShotPrio_08);
            ShotPriorityBoxes.Add(CB_ShotPrio_09);
            ShotPriorityBoxes.Add(CB_ShotPrio_10);

            //CB_CalculationToGraph.Items.Add(Graph.GetCalculationNames());
            _loadingCalculationOptions = false;
        }

        private void SetUpFAQ()
        {
            FAQStuff.Add(
            "Why does my toon do 0 DPS?",
            @"There are a couple possible reasons this could occur.
1) You don't have a Ranged Weapon, all DPS is tied to having a Ranged Weapon.
2) Your Situational settings on the Fight Info tab are set such that you ave no ability to get any DPS out during the fight."
            );
            FAQStuff.Add(
            "Why does the Optimizer sometimes lower my DPS?",
            @"The Optimizer operates on a Random Seed method, which is why it works at all. Sometimes it can't use that random seed to find a set that is better than what you are currently wearing."
            );
            FAQStuff.Add(
            "Why does the Optimizer sometimes just rearrange my Gems?",
            @"This is a result of a the flaw of logic in the final push that the Optimizer uses, if your total DPS is the same and it was just the Gems that got swapped around, keep your existing set. Astrylian is working on an eventual solution to this problem."
            );
            FAQStuff.Add(
            "Why is my Crit value so low compared to in-game?",
            @"Boss level affects your Crit value. Level 83 has about a 4.8% drop, this is mentioned in the Crit Value tooltip."
            );
            FAQStuff.Add(
            "Why do T9 items sometimes show as less value than T8 items (and subsequently T8 to T7)?",
            @"Set Bonuses can have a serious impact on DPS, getting that 2nd or 4th piece can mean more or less for your character at specific gear-sets. It could also be a factor of Meta Gem Requirements if you have that active."
            );
            FAQStuff.Add(
            "Why do Blood Frenzy/Savage Combat, Trauma/Mangle, Rampage/Leader of the Pack, Battle Shout/Blessing of Might, Commanding Shout/Blood Pact Buffs sometimes show 0 value or get cleared?",
            @"One of the most repeated issue submissions for DPSWarr, this is actually intended functionality. When your character is Maintaining this Buff themselves, we disable the Buff Version so that the Talent can have value instead and we can get a better DPS calculation. We also disable the Buff version to prevent Double-Dipping (getting buff twice, once as Buff and once as Talent).
1) Blood Frenzy/Savage Combat: Disabled on having Blood Frenzy Talent (Arms)
2) Trauma/Mangle: Disabled on having Trauma Talent (Arms)
3) Rampage/Leader of the Pack: Disabled on having Rampage Talent (Fury)
4) Battle Shout/Blessing of Might: Disabled on Maintaining Ability
5) Commanding Shout/Blood Pact: Disabled on Maintaining Ability
6) Presently we do NOT model the following abilities this way: Sunder Armor, Thunder Clap, Demoralizing Shout, Hamstring. Sunder because of the stacking effect we have yet to model and the others because their Buffs are currently not relevant to DPSWarr."
            );
            FAQStuff.Add(
            "Why aren't items with Resilience relevant?",
            @"Rawr is for PvE, not PvP."
            );
            FAQStuff.Add(
            "Why are the stats wrong for my x level (non-80) character when I load from Armory?",
            @"Rawr is for end-game PvE, meaning you should be level 80. Rawr does not factor things for leveling as there is no point, you will replace the item in a couple of levels anyway and all your raiding gear will end up requiring level 80 to wear."
            );
            FAQStuff.Add(
            "Why can't I select X weapon type or Y Armor Type?",
            @"Some weapon types are pointless to factor in, Staves and one handed weapons definitely being the big part of this. Same for Armor, though we can wear cloth, cloth can't physically boost our DPS in any way compared to Plate. Leather and Mail at top end items have a chance to beat out your DPS plate in some circumstances. If you want to enable Leather and Mail you can by use of Refine Types of Items Listed from the Tools menu."
            );
            CB_FAQ_Questions.Items.Add("All");
            string[] arr = new string[FAQStuff.Keys.Count];
            FAQStuff.Keys.CopyTo(arr,0);
            foreach (string s in arr) { CB_FAQ_Questions.Items.Add(s); }
            CB_FAQ_Questions.SelectedIndex = 0;
            CB_FAQ_Questions_SelectedIndexChanged(null, null);
        }
        private void SetUpPatchNotes()
        { // No Significant Changes due to short period of time between releases.
PNStuff.Add(
"v2.3.3 (Unreleased)",
@"- Fix for Issue 15431: Hunter Hit Optimizer Req wasn't working correctly
- Fix for issue 15430: Pet Talents not saving correctly
- Fix for Issue 15412: Running OoM just before the end of the fight was causing it to use full viper uptime instead of minimal needed
");
PNStuff.Add(
"v2.3.2 (Dec 12, 2009 14:39)",
@"- Corrected the pet talent descriptions called in the pet talents comparison chart (was possible to call the incorrect rank's info)
- Added a couple of missing Special Effect Triggers to Relevancy
- Renamed the Set Bonuses to include '(T?)' where ? is the Tier number
- Reworked the T9 4P to use new Stat PetAttackPower
- Added Survivability to Hunter, This is separated for Hunter and Pet and can be Scaled/Weighted from 0.0 to 10.0 (default 1.0). This includes simple things that improve survivability like reduced damage taken, or ability heals that occur on self, and healing potions, etc
- Added Pet Dodge Percentage to the Stats Pane so users can see when their pet is getting dodged (due to the Hunter lacking Hit)
- Fixed a typo in the tooltip for the Rabid pet talent
- Heavy Reworking of the Pet Stats, these need to be stabilized a bit but the end-result is a much more accurate depiction of Pet Stats instead of the really weird way they were being done before.
- Added a new series of gemming templates
- Adjusted the special effects formula with regard to pet attacks and steady shot hit intervals
- Fixed a bug with Hit Rating not getting full value (I had revered teh anti-dodge handling for pets)
- Fixed Attack Power usage on Hunter & Pet (was migrating it all to AttackPower stat but shots n stuff were using RangedAttackPower)
- Refactored CombatTable to a new file
- Added PetAttackTable class (derivative of CombatTable, set to use pet stats and abilities)
- Couple of minor fixes to Pet Stats
- Fixed a bug where Bestial Wrath could cause Aspect of the Beast Uptime to go Infinite when you don't have the talent
- Some corrections to Crit Calcs
- Fixed a bug with Aspect of the Beast uptime during Bestial Wrath calculation (was doubling hunter DPS /facepalm)
- Fixed the damage adjustments to Volley (still need to add multiple target count)
- Added several talent's survivability values
- Fixed Aspect of the Hawk/DragonHawk AP bonus addage
- Fixed Pet Haste effect bonuses (had to add a recalculate after special effects in pet)
- Fixed a couple of survivability related talent values
- Changed Stat addage to Accumulate for better performance
- Added Longevity Cd reduc to Call of the Wild handling
- Did some work on the Intervals for Special Effects, integrating pet frequencies
- Fixed a bug with CombatTable for Pets
- Fixed Pet Spell Miss variable (didn't realize the Spell miss function in StatConversion was inverse)
- Removed some dead code
- Fixed a scope damage issue
- Fixed an issue with Mana Regen stats (at least I think I did)
- Improved Attack Intervals for Pet Special Effects
- Fix for Serpent Sting proc related damage
");
PNStuff.Add(
"v2.3.1 (Dec 10, 2009 03:47)",
@"- Corrected a couple of Get Stat points for Mana Recovery options
- Fixed a problem with Hunter's Hit translating to Pet's Dodged attacks
NOTICE: A new issue has been identified, if you do not select the option to use Aspect of the Viper but you are running out of Mana before end of fight, there's presently no penalty. I'll need to fix this but it's not a simple correction so please be patient and until then at least use Viper at 'Just Enough'
- Fixed some back end naming for Aspect of the Viper Usage
- Your DPS is now appropriately penalized when you are running OoM and not using an Aspect of the Viper Uptime, giving heavy favor to Mana Regen methods (pots, Mp5, etc). When using Viper, those stats reduce in value but don't go away unless your time to OoM is more than the fight duration
- Added a method by which users could get off the specialized chart setups after having viewed them. Since it doesn't auto-revert after seeing one of those and going back to a normal chart, you can now select one of the Custom DPS charts to revert the view then go to a normal chart.
- Added a new Custom Chart 'Pet Talents' looks and feels just like the regular talent chart but works on Pet Talents, even shows the information about the talent at it's current rank on mouse-over
");
PNStuff.Add(
"v2.3.0 (Dec 08, 2009 17:04)",
@"- Was doing some work on the PetTalents but it didn't pan out, leaving it in but commented until a better solution is found
- Fix for Expose Weakness not being applied
- Changed T9 4P to use the stat itself instead of relying on the Buff Name
- Working with Pet Stats to ensure proper scaling
- More back end work
- Removed Avoidance talent from PetTalents, users may see their talents reset or look funky in next version but it shouldn't crash
- Fix for Issue 15240: Pet Talent Changes Cause Crash (was a talent indexing issue)
");
PNStuff.Add(
"v2.2.28 (Dec 06, 2009 22:28)",
@"- Fix for issue 14739: Optimize Requirement '% Chance to Miss' error (calcOpts wasn't being set before the call to it in the requirement, added set before call)
- Fix for bug with Hit Rating where more of it would hurt SpecialEffects
- Applied Patch 4398:
  * Adding tab to show shot rotation details
  * NOTE: This patch was applied but it presently doesn't have a function (it's just visual). Will add function later
- Fixed Strength stat from Gear pulling, also made it add to attack power like it should
- Added Profession Handling for Buffs, will auto-activate buff bonuses if you have the related Profession selected.
- Added Automated Buff Anti-Double-Dipping for Trueshot Aura. If you are maintaining it yourself, then it ignores the Buff version and uses your Talents instead. (This needs more verification)
- Fixed Crit Rating Procs so they will actually add to your Crit %
- Moved all references to the QuickShots effect to GetCharacterStats as a simple PhysicalHaste SpecialEffect (will add aspect downtime to it later)
- Moved all references to Call of the Wild to GetCharacterStats
- Moved all References to Trueshot Aura to GetCharacterStats
- Changed the RAP tooltip to just show base, agi, str bonuses (will add gear later)
- Fixed the activator that would turn off Buff version Trueshot aura when you have the talent yourself
- Added Hunter's Mark to Buff disabling when you have the relevant talents/glyphs (this still isn't working the way that I want)
- Moved Aspect of the Hawk and Dragonhawk's AP handling to GetCharacterStats
- Moved all references to Hunter vs Wild AP to GetCharacterStats
- Moved all references to Careful Aim AP to GetCharacterStats
- Fixed a Ranged Haste on Procs bug
- Redid hunter T8 4 pc to use a different trigger and method (and removed the old method stat placeholder from Stats)
- Moved all references to Expose Weakness AP to GetCharacterStats
- Moved all references to Furious Howl AP to GetCharacterStats
- Added a couple of files to begin work on converting shots to classes (will make them easier to manage and prevent some things from not affecting others)
- Fixed the Details tab resizing
- Aligned more of the back end for the new Ability system
- Started adding in shots using the Ability class method from DPSWarr (will not do any rotation work until all of this is settled first)
- Finally fixed the Buffs that Double-Dip. I had added the function that does the special handling but GetCharacterStats wasn't using it, D'OH!
- More shot data setups
- Moved TFB above OP in the priority queue, because OP's use is limited by TFB
- Corrected a bug in Arms where Slam would try to use more rage than you had available, which led to negative DPS while the rotation tried to correct itself
- All models using Prof enchant hiding have been changed to use the global sets on the Stats Pane and Options > General Settings > Hide enchants based on professions. Models that were handling this manually have been edited to the new method, models that didn't have it are now on it as the back end changed
- Profession bonus Buffs (Toughness and the like) are now updated when you update your professions in all models. Models that were handling this manually have been edited to the new method, models that didn't have it are now on it as the back end changed
- Added a new Stats for global Mana Cost Reducs by Percentage (for the Beast Within talent)
- Changed BonusDamageMultiplier usage in Hunter to not be restrictive (which was just dumb). It can now handle sources beyond just Buffs and be used in more places
- Changed handling of The Beast Within and Bestial Wrath. Now these are handled using SpecialEffects and better modifiers, though I do need to add more of it into Pet damage calculation. Also fixed teh Duration from 18 to 10 sec
- Changed the Armor Damage Reduction Formula for Pets to use the global one (just without ArP Rating)
- Added some Traps and Volley as selectable in your rotations (started out as a way to give value to several glyphs that were not being used and just kinda blew up from there)
- Fixed a bug I accidentally made with Armor Ignorance on Pets
- Removed the emulate Spreadsheet Bugs option
- Added a PTR Mode to enable 3.3 changes. Most affect stuff Hunter isn't set up to handle, but CullingTheHerd has been added to the Pet Talents so it can be selected

Completely reworked the Pet Talents storage and interface (these steps will lead towards storing Pet Specs alongside Hunter Specs and also makes it a lot easier for me to maintain)
- Interface on the options pane now looks more like the actual talent trees, though I still have some more tweaking to do
- Back end stores the talent info as a string, just like normal talents. Way better than the monstrously sized PetTalents section in the XML files before

- Added support for T10 2PC & 4PC, the values are in there but the uptimes are wrong, won't be correct until new rotation setup is created
- Fixed a couple missing relevant Triggers for Hunter Special Effects
- Removed Pot selection from Options Pane, migrating this to use Buff version of pots instead
- Added Nature Damage as a relevant stat

- Removed option to 'calculate uptimes like the spreadsheet' since the back end for it no longer exists
- Added Multiple Targets % to control Volley usage and reworked Volley's activation to enforce it's channeling
- Moved Traps to a separate section on the Stats Pane from Shots
- Removed Racials from the Stats Pane (back end is there)
- Adjusted the display of some of the Mana Regen stats
- Added the RAP boost coeff to Volley
- Added a failure reason to Volley for not having Multiple Targets active and at >0%
- Fixed a potential bug with certain gear setups that would break Mana Regen (causing it to go to Infinity)
- Made the Randomize Procs on Rotation Test option gray out if not using Shot Rotation Test
- Added the PetBuffSelector directly to the Options Pane instead of using a back end function to add it on load (previous builder didn't know how to add it)
- Moved Pet stuff to a subfolder
");
            PNStuff.Add(
            "v2.2.27 (Nov 10, 2009 03:45)",
            @"- Fix for broken Trolls and Dwaves.");
            PNStuff.Add(
            "v2.2.26 (Nov 09, 2009 01:53)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.25 (Nov 08, 2009 20:15)",
            @"Please note that this Versions's notes will be difficult to read due to the nature of the commits occurring here.

- Applied Patch 4276 (by Drizz): Here is the small patches I made to the hunter module. I have tried to put a comment '// <date> Drizz: ...' at each of the places where I have been doing changes or adding something. List
Have tested from a MM spec perspective. So BM and Surv spec talents and shots (e.g. Black Arrow, Exploding Shot have not yet been tested/corrected)
CalculationsHunter.cs
* Added display of Piercingshot element in stats view.
* Trinket: Added handling of Heroic version of Death's Choice/Verdict
* Added handling of Talent Piercingshots
* Trinket: Added handling of Banner of Victory
* Corrected the WildQuiver DPS to include the ViperUptime penalty
* Aligned Calculation with Spreadsheet on CritAdjust for several shots
* Moved the armorReduction to be multiplied in later
* Changed Buff check to check for 'Windrunner's Battlegear' instead of 'Windrunner's Pursuit' for the 2 Piece T9 bonus. The Buff's name don't seem to be 'changed'
* Aligned the calculations of the 2pT9 effect.
* Rearranged calculations for Chimera Shot.
RotationTest.cs
* Updated to wait for Rapid Fire if close to cooldown in Rotationtest
CalculationOptionsPanelHunter.cs
* Added an option to be able to set the CooldownCutoff from the GUI
CalculationOptionsHunter.cs
* Changed the default cooldownCutoff to 30 (from 15) to align wiht spreadsheet
ShotPriority.cs
* Updated the calculation of critSpecialShotPerSecond to align
PetCalculations.cs
* Updated the focusRegen calculations to align with Spreadsheet.
* Added armor effect to pet calculations
* Updated name of Windrunner Buff for the 4pT9 effect. (Battlegear not Pursuit)
* Corrected Kill Command calculations
CharacterCalculationsHunter.cs
* set the initiation of shots to set Serpent sting to be able to crit.
HunterEnums.cs
* Updated some static constants to align with numbers in spreadsheet (including new armor pen value=
* Created a new constant for the Pet Cirt damage multiplier
CalculationsOptionsPanelHunter.cs
* made GUI of the update cooldownCutoff
- Applied Patch 3887: Updated Hunter Gemming to default to Epic gems instead of Rare gems
- Applied Patch 4304:
  RotationTest.cs
   * Changed a couple of initiation values to align with the spreadsheet
   * Removed a check for GCD to align with spreadsheet
   * Added special case for currentTime == 0
   * Changed to using LastAutoShotCheck value instead of the currentTime (align with spreadsheet)
   * Fixed a boundary error of the use of the rand.Next function
  CalculationsHunter.cs
   * Adding the Mangle/Trauma buff effect to the Piercing Shots
  CalculationOptionPanelHunter.cs
   * Added a checkbox for Randomizing Procs on Rotation Test
Rawr.HunterSE: Adding this model so Devs can do a verification for 2.2.25 release (will replace current Hunter model).
Rawr.HunterSE:
- Updated the Optimization Requirements to use Percentages instead of Ratings and added a couple more options
- Added requirement to Buff Relevance that it must be obtainable by a Hunter (E.g.- a DK only buff won't show up in the set bonuses)
Rawr.HunterSE:
- Cleaned out the PvP set bonuses (there is only one since they all share)
- Set the Buffs to filter out if not available for this class
- Fixed a Crit from AGI tooltip
- Migrated a couple of back end items to use SpecialEffects for Uptime instead of a private Hunter function (which is now deleted)

Rawr.Hunter:
- Turned the Debug Shot Rotation setup off (kills performance)

Rawr.HunterSE:
- Added Hide Bad Items setup (similar to DPSWarr). Can hide Spell gear (based on Spell Power and Spell Penetration) and/or PvP gear (Based on Resilience)
- Added Hide Enchants Based on Profession (non-engineers won't see Hyperspeed Accelerators, etc)
- Renamed a lot of the Interface Objects on the Options Pane for better naming convention
- Cleaned up the Interface scaling on several sections of the Interface
- Added Piercing Shots talent work by Drizz (was supposed to be in last commit but apparently I missed it)

Rawr.HunterSE:
- Refactored Pet Talents into a new class and changed all references to that
- Fixed window being too small so interface items are hidden issue (added scrollbars where applicable)

Rawr.HunterSE:
- Adjustments to reduce DPS differences between Hunter and HunterSE
- Reformatted Damage for shots on the Stats pane to 'Damage : DPS'

Rawr.HunterSE:
- Added Freq to the stats pane for shots
- Chimera shot's primary issues fixed (couple of the multipliers were bad)

Rawr.HunterSE:
- Fixed Auto-Shot DPS (haste wasn't set correctly)

Rawr.HunterSE:
- Steady Shot Damage fixed (DPS still off)

Rawr.HunterSE:
- Fixed Steady Shot DPS (Haste was set wrong, have now checked everything that involves haste to fix all those breaks)

Rawr.HunterSE:
- Some more float conversion cleanups
- Resorted and added summaries to the Hunter Glyphs (users may need to reselect their glyphs)

***** OCT 31st, 2009: Removing old Hunter, Hunter SE is now Hunter *****

- Rawr3: Fix for compile issue (function doesn't have same call in Silverlight)
- Couple more changes to prevent compile errors
");
            PNStuff.Add(
            "v2.2.24 (Oct 24, 2009 18:00)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.23 (Oct 15, 2009 03:15)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.22 (Oct 13, 2009 05:16)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.21 (Oct 07, 2009 20:18)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.20 (Oct 06, 2009 19:22)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.19 (Sep 23, 2009 06:22)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.18 (Sep 23, 2009 04:36)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.17 (Sep 23, 2009 04:36)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.16 (Sep 13, 2009 21:51)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.15 (Sep 07, 2009 08:25)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.14 (Aug 30, 2009 07:18)",
            @"- use Readiness in rapid fire cooldown calculation
- make Readiness trigger a GCD
- fixed effect of Readiness on Kill Command cooldown
- implemented heroism effect and added heroism usage option
- the options pane now scrolls when needed
- fixed tab ordering on all options panes
- made items/enchants with use-effects appear correctly
- implemented the 5 proc haste trinkets/enchants on the spreadsheet
- implemented the 2 proc crit trinkets on the spreadsheet
- implemented improved steady shot (mana reduction & damage adjustment)
- fixed default pet rotation for pets without two of dive/dash/charge/swoop
- calculate pet specials crit frequency soon enough for Ferocious Inspiraion calcs
- show more pet crit detail in stats pane
- use heroism for pets too
- don't let the pet dodge effect be beneficial
- calculate mana regen from roar of recovery
- calculate mana regen from invigoration
- adjust pet ap for animal handler
- calculate the t7 pet bonus correctly
- calculate the beastial wrath effect for pets
- show full (correct) haste calculations in stats pane
- correctly adjust mana for glyph of arcane shot
- calculate mana and damage effects from improved steady shot
- calculate mana regen when viper is up 100%
- added cobra strikes calculation
- made the utility calculation functions static
- implemented t7 2-piece bonus
- implemented t9 2-piece & 4-piece bonus
- correctly display set bonuses on the buffs tabs
- apply Aspect of the Beast AP bonus correctly to pet
- apply the correct number of Rake dots (based on frequency)
- implemented rabid effect
- implemented pet glancing blows
- implemented feeding frenzy
- use debuff calculations everywhere they're needed
- show AP from debuffs in tooltip
- apply AP from call of the wild correctly to the hunter
- calculate aspect uptimes correctly when using AotB during BW
- rounding tweak for serpent sting damage
- implemented the missing +AP proc trinkets
- implemented the missing +ArPen proc trinkets
- implemented the missing +Damage proc trinkets/enchants
- implemented the missing +Agi proc trinkets
- hunters are interested in direct-damage fire effects
- tweaked troll berserk and orc blood fury effects
- spirit does not affect our dps
- melee-triggered special effects are useless to hunters
- no DK runes pls
- use MP5 from stats and gear
- implemented Darkmoon Card: Crusade
- updated base AP from Swordguard Embroidery
- mjolnir trinket now 665 ArP
- implemented trinkets with simple +haste use effects
- removed emulations for many spreadsheet bugs fixed in 91b:
* wrong trinkets being counted as alchemist stones
* dust cloud, snatch and tendon rip can no longer trigger crit procs
* spore cloud debuff uses the correct freq
* acid spit debuff uses the correct freq
* monstrous bite proc uses the correct freq
* scorpid poison uses correct AP scaling
* spirit strike uses correct AP scaling
* poison spit multiplies damage correctly by number of dots
* scorpid poison damage uses the correct freq
- now compatible with spreadsheet 91b
- beastial wrath correctly reduce the mana cost of kill command
- Bestial Wrath can only be used if you have the talent
- PriorityRotation uses the options object directly - moved some constants in there
- implemented Scorpid Sting
- implemented Viper Sting
- implemented Immolation Trap
- added a rotation test (disabled by default)
- calculate static haste before dynamic haste
- modified priorityRotation to correctly calculate specials pet second and the crit ratio sum
- don't calculate shot frequencies the old way when we're using the rotation test
- make sure we initialize the timings (time_used & rotation_cooldown) regardless of rotation test (and after we know duration!)
- always calculate crits_ratio for every shot, even when its ratio is 0
- correctly recalculate crit ratios in rotation test mode, once we know steady shot cast time
- readme update
- replaced the old custom chart with 4 useful ones
- added rotation DPS and MPS charts and cleaned up the others a little
- most buffs now working correctly (hunter & pet)
- removed second dodge adjustment from pet white damage
- crit tooltip tweak
- tweaked which buffs to offer
- recalculate armor, agility & base crit
- implemented JoW
- corrected damage adjustment for BA (it's not physical)
- corrected intellect base calculation
- calculated JoW correctly given that it's stored as a 1% restore
- hopefully the last JoW change for a while
- added an options tab for pet buffs
- removed pet buffs from hunter buffs filter
- added PetBuffSelector control
- fixed LotP/Rampage spreadsheet bug for pets
- store pet buffs in options xml in the same way the main xml stores character buffs
- pet dodge uses the hunter's non-rounded hit
- hide worthless foods
- added a note about possible MP5 stacking bug (posted on EJ)
- emulate the spreadsheet Mp5 stacking issue
- updated spreadsheet emulation to 91c
- remove untalented silencing shot correctly in rotation-test mode
- leave untalented silencing shot in frequency mode when emulating bugs
- added LALProcChance as an options (not set-able yet)
- rotation test is now on by default
- special shots per second should use final_freq rather than freq - refreshed shots count as shots
- shot better tooltips for refreshed shots
- emulate Serpent/Chimera spreadsheet rounding bug
- updated to match spreadsheet 91c(r2)
- another bug that got fixed in 91c(r2)
- show mana usage/regen details in stats output
- show a lot more detail for pet ap/hit/hit in stats output
- compatibility with spreadsheet 91e
- fix for rotation test - don't wait for non-gcd abilities
- automatically set frequency of (some) zero focus pet skills to their cooldown
- sting debuff uses real frequency for uptime calculation
- roar of recovery can use frequency in any mode since it now equals cooldown
- call of the wild can use frequency in any mode since it now equals cooldown
- updated partial resist formula for pet to match hunter
- troll berserking uses real frequency
- updated calculation for crit from racial to match bug in spreadsheet
- calculate pet timings before debuffs, else sting doesn't get factored in correctly
- changed T9 2-piece bonus crit damage for serpent sting
- fixed killShot sub 20 calcs to factor in viper for each shot instead of on the delta");
            PNStuff.Add(
            "v2.2.13 (Aug 12, 2009 02:28)",
            @"- Correctly calculate autos per sec, specials per sec and crittable specials per sec
- Correctly calculate crit bonus from master tactician
- Calculate cumulative percentage bonuses correctly
- Added agility bonus from Hunting Party
- Include crit chances in shot data
- Calculate composite crit ratios
- Show AP from expose weakness in tooltip
- Correctly compound static and dynamic haste
- Calculate Expose Weakness AP gain correctly, using critting shots per second & crit composite sum
- Calculate base agility correctly
- AP from Hunter vs Wild needs to be rounded down
- Fixed meta gem crit boost - was previously getting 0 and then compounding it incorrectly
- fixed wild quiver calculation
- split out wild quiver from autoshot dps
- cleaned up displayed stats list (and added placeholders)
- Mana regen overhaul
- General refactoring continues
- show AP from procs on tooltip
- show crit talent breakdown in tooltip
- recalculate frequencies before calculating shots per second (since we have final haste & steady shot speed)
- round crit correctly (or at least so that we agree with the spreadsheet)
- calculate correct noxious sting bonus for non-sting shots
- adjust serpent sting damage correctly and apply correct number of ticks
- adjust black arrow damage correctly
- calculate stamina from gear/race correctly
- Calculate LAL procs as soon as we know frequencies (frequencies will not need correcting at the end)
- Fixed expose weakness calculation (0.33,0.66, not 1/3,2/3)
- Added a few test trinkets (mine, of course ;)
- Updated hunting party to 3.2 values (2% per second, not 2.5%)
- replenishment buffs override hunting party
- calculate & display bonus damage from kill shot at sub-20% HP
- added all of the armor set bonuses (tiers 4-9, seasons 1-6)
- don't crash when character is not wearing trinkets or back enchant
- updated Thrill of the Hunt mana adjustment for 3.2
- removed Thrill of the Hunt mana adjustment from black arrow
- round MPS in the same way the spreadsheet does
- break out mps calculation from dps, so we can determine aspect uptime for dps
- added several new options (main aspect, viper usage, mana pots, phase timings and more)
- calculate and display aspect uptimes (based on mana rotation and needed viper)
- corrected aspect of the hawk AP calculation
- tweaked base mana calculation to match spreadsheet
- rearranged the option panels
- correctly save and load fight duration option
- removed some debugging
- explicitly mark regions where we're copying bugs from the spreadsheet (w/ a global toggle)
- fixed thread safety
- removed more cruft
- implemented quick shots (improved aspect of the hawk proc)
- Started overhauling the pet calculations
- Added options to emulate spreadsheet bugs & oddities
- Lots of cleanup and minor calculation adjustments
- Show more detail for hit and haste in tooltips
- Silenced all the warnings
- implemented first pet skill
- micro rounding adjustments to match spreadsheet
- my profile is now within 0.00007 DPS of the spreadsheet
- more pet skills implemented (bat-hyena)
- started on target debuff effects - armor debuffs from pet are working
- fixed bug with pet AP rounding
- calculate savage rend effects
- more pet skills implemented (bat-spore bat)
- emulate all spreadsheet KC crit bugs (some non-damaing abilities are counted as critting)
- implemented sting and acid spit armor debuffs
- implemented correct furious howl ap effect
- emulate spreadsheet cooldown bugs in acid spit and monstrous bite uptime calcs
- implemented every focus-using rotation pet skill that's in the spreadsheet (thunderstomp is missing)
- clear the pet family list before re-populating it (so it doesn't grow with each profile load)
- include the full/correct skill list for pet rotation pick-lists
- correctly load pet skill priority instead of overriding it with the default
- allow setting up to 7 pet skills in the priority list (up from 4)
- renamed the ferocity talent to 'Charge/Swoop'
- Fixed incorrect base health for 3 of the 7 hunter races
- added 3 more pet priority slots
- added Frenzy effects
- added Ferocious Inspiraion effects
- added ability to set pet type to none
- added cooldowns & durations for all the unimplemented hunter skills (so rotation timing is correct)
- calculate spell crit (used for trinkets)
- added AP from aspect mastery
- implemented some more trinkets: Grim Toll & Bandit's Insignia
- correctly calculate Beast Within effects
- added t8-2 piece bonus for serpent sting
- fixed aimed shot damage when weapon isn't 2.8
- calculate base armor correctly
- calculate base health correctly
- updated sources/credits block
- fixed hunter T7 2-piece bonus
- better formatting for some hunter stats");
            PNStuff.Add(
            "v2.2.12 (Aug 06, 2009 05:01)",
            @"- Replaced old broken rotation code with priority-based shot rotation
- Use a 2-stage rotation calculator to first get frequencies for uptimes, LAL procs, etc
- Show shot details in tooltips, with frequencies, DPS and reasons shots are skipped
- Deal with chimera shot refreshing serpent & viper stings
- Calculate proper LAL frequencies
- Added debugger for shot rotation table
- Removed all of the old unused shot results from the CharacterCalculation class
- Added 'bad uptime calculation' mode to mirror the spreadsheet
- Correctly calculate rapid fire & black arrow uptimes based on rotation frequency
- Set critProc & useGCD correctly when creating the ShotData objects
- Added ShotData model for Rapid Fire
- Use latency from options in frequency calculations
- Fixed possible divide-by-zero error in uptime calculations
- Added all the unimplemented shots/traps/effects
- Cleaned up the displayed calculations list
- Correctly apply rotation options when choosing a preset rotation
- Allow setting of every pet talent
");
            PNStuff.Add(
            "v2.2.11 (Aug 05, 2009 05:40)",
            @"- Better formatting for haste
- WotLK boss armor is 10643, not 13100
- Show 2dp for haste calculations.
- Rapid fire base haste is 40%, not 50%
- Corrected base mana and hp calculations
- Updated autoshot dps calculations
- Break autoshot dps down into base autoshot and wild quiver
- Removed a couple of unused/redundant regions
- Corrected steady shot damage calculation
- Corrected serpent sting dps
- Fixed aimed shot calculation
- Started refactoring of damage, crit chance and crit damage adjustments
- Corrected calculations for explosive shot
- Corrected calculations for chimera shot
- Corrected calculations for Arcane shot
- Corrected calculations for multi shot, kill shot and black arrow");
            PNStuff.Add(
            "v2.2.10 (Jul 17, 2009 22:27)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.09 (Jul 02, 2009 03:02)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.08 (Jun 30, 2009 11:29)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.07 (Jun 21, 2009 22:54)",
            @"- Updated Hunter Calculations: Approximately 80% complete, gives accurate numbers for dps and stat values. Mork work to come soon
- Rotations in Hunter finished, DPS for both hunter and pet is 95%-98% accurate
- Updates to the Hunter module for the move to Silverlight
- Couple of minor changes as Rawr.Hunter readies for Silverlight
- Took out ammo damage in Hunter for now until we can sort out a null value issue");
            PNStuff.Add(
            "v2.2.06 (Jun 06, 2009 01:34)",
            @"- No commits for this release");
            PNStuff.Add(
            "v2.2.05 (May 25, 2009 16:25)",
@"- First step to get Hunter module caught up to 3.x:
* Adding a txt file to track my todos and give the current state of expected support for the module.
* Added Glyphs based on what I could find on wowhead.
* Adjusted the relative stat value graph to be Agility based.
* Started adding pet talents.
-  	Hunter Pet DPS section updated to 3.1.2
- File missing from previous build, PetSkills.cs now present");

            CB_Version.Items.Add("All");
            string[] arr = new string[PNStuff.Keys.Count];
            PNStuff.Keys.CopyTo(arr, 0);
            foreach (string s in arr) { CB_Version.Items.Add(s); }
            CB_Version.SelectedIndex = 0;
            CB_Version_SelectedIndexChanged(null, null);
        }
        private void SetUpOther()
        {
            RTB_Welcome.Text = @"Welcome to Rawr.Hunter!

This module is designed for Hunters hoping to fulfill the DPS role in a raid, regardless of their specific spec (MM, BM, Surv).

To begin we assume you have already imported your character from either Character Profiler or the WoW Armory. If you would like to check your numbers against information from the next patch (at present, this is 3.3) you can check that item.

... More to come, these instructions have to be rewritten for Rawr3";
            RTB_Advanced.Text = @"This section is for advanced users only, most players do not need to concern themselves with these settings.

Since you have gotten your feet wet, looked at your gear, maybe even run an optimization or two, now you must be hungry for more. Fear not, there's plenty more you can tweak with your character.

The Boss Handler Tab

We will be adding functionality to maintain damage taken, for survivability and for the additional rage generated from damage taken at some point, but time takes time.

The Situational Boxes on The Fight Info Tab

This tab holds information regarding how often in a fight your toon is in that particular situation. Presently, there are five options with individual settings for each:
- Standing in Back: You spend at least x% of the fight standing behind the target. Mobs are unable to parry or block attacks from behind so the ~13% of attacks that could normally be parried are no longer on the table. If you are not standing behind the mob during for any portion of the fight, Expertise will have additional value due to it preventing Parries.
- Multiple Targets: Your encounter has additional mobs within melee striking distance for x% amount of time. This provides usefulness for abilities such as Cleave, Bladestorm, Whirlwind, Sweeping Strikes to start 'doing their thing' and allowing you to hit the additional targets for a greater overall DPS. Boss fights like Patchwerk however, do not have additional targets. There is a cap for the number of targets placed so that abilities such as Whirlwind do not go for 4 targets worth of damage when there are only 2 targets.
- Moving Targets: Your encounter has a target that moves out of melee striking distance for x% amount of time. This provides usefulness for abilities such as Charge and Intercept and talents which enable these abilities. This also provides effectiveness for Move Speed Enchants like Cat's Swiftness. A good example of this situation is 'Archavon the Stone Watcher' in the 'Vault of Archavon (VoA)'. [Currently this functionality is only active for Arms, not Fury, Charge, etc abilities are not yet modeled]
- Stunning Targets: Your encounter has a target that either stuns just yourself or your entire raid  x times over the fight duration for y milliseconds (1000 = 1 second). This provides usefulness for abilities such as Every man for Himself (Humans) and the talent Iron Will. You can change the values of both boxes by changing one of them. E.g.- Set Percentage to 25% and it will change the seconds box to match and vice-versa. [Currently this functionality is only active for Arms, not Fury]
- Disarming Targets: Your encounter has a target that disarms your characters' weapon periodically in combat. This provides usefulness for things like Titanium Weapon Chain and the talent Weapon Mastery. Most bosses do not do this, but there are several groups of trash (namely in Karazhan) that will disarm players. [Currently, this functionality has not been implemented, though it will be coming soon.]
Additional Situations to manage will be coming soon.

The Ability Maintenance Tab

Select additional abilities to watch how they affect your DPS. Thunder Clap applies a debuff to bosses as do Sunder Armor, Demoralizing Shout, Shattering Throw, etc.";
        }
        private void CB_FAQ_Questions_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
                string text = "";
                if ((string)CB_FAQ_Questions.SelectedItem == "All") {
                    int Iter = 1;
                    text += "== CONTENTS ==" + "\r\n";
                    foreach (string s in FAQStuff.Keys) {
                    text += Iter.ToString("00") + "Q. " + s + "\r\n"; // Question
                    Iter++;
                    } Iter = 1;
                    text += "\r\n";
                    text += "== READ ON ==" + "\r\n";
                    foreach (string s in FAQStuff.Keys) {
                    string a = "invalid";
                    text += Iter.ToString("00") + "Q. " + s + "\r\n"; // Question
                    bool ver = FAQStuff.TryGetValue(s, out a);
                    text += Iter.ToString("00") + "A. " + (ver ? a : "An error occurred calling the string") + "\r\n"; // Answer
                    text += "\r\n" + "\r\n";
                    Iter++;
                    } Iter = 1;
                    RTB_FAQ.Text = text;
                } else {
                    string s = "";//CB_FAQ_Questions.Text;
                    string a = "invalid";
                    bool ver = FAQStuff.TryGetValue(s, out a);
                    text += s + "\r\n";
                    text += "\r\n";
                    text += (ver ? a : "An error occurred calling the string");
                    RTB_FAQ.Text = text;
                    RTB_FAQ.SelectAll();
                    //RTB_FAQ.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
                    RTB_FAQ.Select(0, RTB_FAQ.Text.IndexOf('\n'));
                    //RTB_FAQ.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
                }
        }
        private void CB_Version_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = "";
            if ((string)CB_Version.SelectedItem == "All")
            {
                int Iter = 1;
                text += "== CONTENTS ==" + "\r\n";
                foreach (string s in PNStuff.Keys)
                {
                    text += s + "\r\n";
                    Iter++;
                } Iter = 1;
                text += "\r\n";
                text += "== READ ON ==" + "\r\n";
                foreach (string s in PNStuff.Keys)
                {
                    string a = "invalid";
                    text += s + "\r\n";
                    bool ver = PNStuff.TryGetValue(s, out a);
                    text += (ver ? a : "An error occurred calling the string") + "\r\n";
                    text += "\r\n" + "\r\n";
                    Iter++;
                } Iter = 1;
                RTB_Version.Text = text;
            } else {
                string s = (string)CB_Version.SelectedItem;
                string a = "invalid";
                bool ver = PNStuff.TryGetValue(s, out a);
                text += s + "\r\n";
                text += "\r\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_Version.Text = text;
                RTB_Version.SelectAll();
                //RTB_Version.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
                RTB_Version.Select(0, RTB_Version.Text.IndexOf('\n'));
                //RTB_Version.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            }
        }
        #endregion

        #region ICalculationOptionsPanel Members
        public UserControl PanelControl { get { return this; } }

        CalculationOptionsHunter CalcOpts = null;

        private Character character;
        public Character Character
        {
            get { return character; }
            set {
                // Kill any old event connections
                if (character != null && character.CalculationOptions != null && character.CalculationOptions is CalculationOptionsHunter) {
                    ((CalculationOptionsHunter)character.CalculationOptions).PropertyChanged
                        -= new PropertyChangedEventHandler(CalculationOptionsPanelHunter_PropertyChanged);
                    character.TalentChangedEvent -= new System.EventHandler(CharTalents_Changed);
                }
                // Apply the new character
                character = value;
                // Load the new CalcOpts
                LoadCalculationOptions();
                // Model Specific Code
                // Set the Data Context
                LayoutRoot.DataContext = CalcOpts;
                // Add new event connections
                CalcOpts.PropertyChanged += new PropertyChangedEventHandler(CalculationOptionsPanelHunter_PropertyChanged);
                character.TalentChangedEvent += new System.EventHandler(CharTalents_Changed);
                // Run it once for any special UI config checks
                CalculationOptionsPanelHunter_PropertyChanged(null, new PropertyChangedEventArgs(""));
            }
        }

        private bool _loadingCalculationOptions = false;
        public void LoadCalculationOptions()
        {
            string info = "";
            _loadingCalculationOptions = true;
            try {
                if (Character != null && Character.CalculationOptions == null) {
                    // If it's broke, make a new one with the defaults
                    Character.CalculationOptions = new CalculationOptionsHunter();
                    _loadingCalculationOptions = true;
                }
                else if (Character == null) { return; }
                CalcOpts = Character.CalculationOptions as CalculationOptionsHunter;
                ThePetTalentPicker.Character = character;
                PetBuffs.Character = Character;
                PopulateArmoryPets();
                PopulatePetAbilities();
                CB_PriorityDefaults.SelectedIndex = ShotRotationIndexCheck();
                if (ShotRotationFunctions.ShotRotationIsntSet(CalcOpts)) {
                    _loadingCalculationOptions = false;
                    CB_PriorityDefaults.SelectedIndex = ShotRotationFunctions.ShotRotationGetRightSpec(Character);
                    _loadingCalculationOptions = true;
                }
                // Bad Item Hiding
                CalculationsHunter.HidingBadStuff_Spl = CalcOpts.HideBadItems_Spl;
                CalculationsHunter.HidingBadStuff_PvP = CalcOpts.HideBadItems_PvP;
                ItemCache.OnItemsChanged();
            } catch (Exception ex) {
                new ErrorBox("Error in loading the Hunter Options Pane",
                    ex.Message, "LoadCalculationOptions()", info, ex.StackTrace);
            }
            _loadingCalculationOptions = false;
        }

        public void CalculationOptionsPanelHunter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_loadingCalculationOptions) { return; }
            // Change abilities if stance changes
            /*if (e.PropertyName == "FuryStance")
            {
                bool Checked = calcOpts.FuryStance;
                // Fury
                CK_M_F_WW.IsChecked = Checked;
                CK_M_F_BS.IsChecked = Checked;
                CK_M_F_BT.IsChecked = Checked;
                // Fury Special
                CK_M_F_DW.IsChecked = calcOpts.M_DeathWish && Checked;
                CK_M_F_RK.IsChecked = calcOpts.M_Recklessness && Checked;
                // Arms
                CK_M_A_BLS.IsChecked = !Checked;
                CK_M_A_MS.IsChecked = !Checked;
                CK_M_A_RD.IsChecked = !Checked;
                CK_M_A_OP.IsChecked = !Checked;
                CK_M_A_TB.IsChecked = !Checked;
                CK_M_A_SD.IsChecked = !Checked;
                CK_M_A_SL.IsChecked = !Checked;
                // Arms Special
                CK_M_A_TH.IsChecked = calcOpts.M_ThunderClap && !Checked;
                CK_M_A_ST.IsChecked = calcOpts.M_ShatteringThrow && !Checked;
                CK_M_A_SW.IsChecked = calcOpts.M_SweepingStrikes && !Checked;
            }*/
            //
            if (Character != null) { Character.OnCalculationsInvalidated(); }
        }
        #endregion

        #region Rotations
        private void CB_PriorityDefaults_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // only do anything if we weren't set to 0
            if (_loadingCalculationOptions || CB_PriorityDefaults.SelectedIndex == 0) return;

            _loadingCalculationOptions = true;

            int i = 0;
            int[] _prioIndxs = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
            if (CB_PriorityDefaults.SelectedIndex == (int)Specs.BeastMaster)
            {
                foreach (ComboBox cb in ShotPriorityBoxes) {
                    //cb.SelectedItem = CalculationOptionsHunter.BeastMaster.ShotList[i];
                    _prioIndxs[i] = CalculationOptionsHunter.BeastMaster.ShotList[i].Index;
                    i++;
                }
            }
            else if (CB_PriorityDefaults.SelectedIndex == (int)Specs.Marksman) {
                foreach (ComboBox cb in ShotPriorityBoxes) {
                    //cb.SelectedItem = CalculationOptionsHunter.Marksman.ShotList[i];
                    _prioIndxs[i] = CalculationOptionsHunter.Marksman.ShotList[i].Index;
                    i++;
                }
            }
            else if (CB_PriorityDefaults.SelectedIndex == (int)Specs.Survival) {
                foreach (ComboBox cb in ShotPriorityBoxes) {
                    //cb.SelectedItem = CalculationOptionsHunter.Survival.ShotList[i];
                    _prioIndxs[i] = CalculationOptionsHunter.Survival.ShotList[i].Index;
                    i++;
                }
            }

            /* I want to do a conglomerate one:
                CB_ShotPriority_01.SelectedIndex = CalculationOptionsHunter.RapidFire.Index;
                CB_ShotPriority_02.SelectedIndex = CalculationOptionsHunter.BestialWrath.Index;
                CB_ShotPriority_03.SelectedIndex = CalculationOptionsHunter.Readiness.Index;
                CB_ShotPriority_04.SelectedIndex = CalculationOptionsHunter.SerpentSting.Index;
                CB_ShotPriority_05.SelectedIndex = CalculationOptionsHunter.ChimeraShot.Index;
                CB_ShotPriority_06.SelectedIndex = CalculationOptionsHunter.KillShot.Index;
                CB_ShotPriority_07.SelectedIndex = CalculationOptionsHunter.ExplosiveShot.Index;
                CB_ShotPriority_08.SelectedIndex = CalculationOptionsHunter.BlackArrow.Index;
                CB_ShotPriority_09.SelectedIndex = CalculationOptionsHunter.AimedShot.Index;
                CB_ShotPriority_10.SelectedIndex = CalculationOptionsHunter.SilencingShot.Index;
                CB_ShotPriority_11.SelectedIndex = CalculationOptionsHunter.ArcaneShot.Index;
                CB_ShotPriority_12.SelectedIndex = CalculationOptionsHunter.SteadyShot.Index;
             * But this requires 2 extra slots minimum
             * Gotta add even more for volley and traps, etc.
             * So frack it, I'll just forget it until we have the
             * new rotation setup where this stuff doesn't matter
             */
            _loadingCalculationOptions = false;

            CalcOpts.PriorityIndexes = _prioIndxs;

            Character.OnCalculationsInvalidated();
        }
        /// <summary>
        /// This is to figure out which of the default rotations (if any) are in use
        /// </summary>
        /// <returns>The combobox index to use</returns>
        private int ShotRotationIndexCheck() {
            int specIndex = 0;

            List<Shot> list = new List<Shot>() { };
            foreach (ComboBox cb in ShotPriorityBoxes) {
                list.Add(CalculationOptionsHunter.ShotList[cb.SelectedIndex < 0 ? 0 : cb.SelectedIndex]);
            }
            ShotGroup current = new ShotGroup("Custom", list);

            if (current == CalculationOptionsHunter.BeastMaster) { specIndex = (int)Specs.BeastMaster; }
            else if (current == CalculationOptionsHunter.Marksman) { specIndex = (int)Specs.Marksman; }
            else if (current == CalculationOptionsHunter.Survival) { specIndex = (int)Specs.Survival; }
            
            return specIndex;
        }

        private void CharTalents_Changed(object sender, EventArgs e) {
            if (_loadingCalculationOptions) return;
            //CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter;
            //ErrorBox eb = new ErrorBox("Event fired", "yay!", "CharTalents_Changed");
            int rightSpec = ShotRotationFunctions.ShotRotationGetRightSpec(Character);
            if (ShotRotationFunctions.ShotRotationIsntSet(CalcOpts)) {
                 // No Shot Priority set up, use a default based on talent spec
                CB_PriorityDefaults.SelectedIndex = ShotRotationFunctions.ShotRotationGetRightSpec(Character);
            } else if (rightSpec != 0 && CurrentSpec != 0 && CurrentSpec != rightSpec) {
                // The rotation setup needs to change, user has changed to a totally different spec
                CB_PriorityDefaults.SelectedIndex = rightSpec;
            }
            CurrentSpec = CB_PriorityDefaults.SelectedIndex;
        }
        #endregion
        #region Pet
        private void PopulatePetAbilities() {
            // the abilities should be in this order:
            //
            // 1) focus dump (bite / claw / smack)
            // 2) dash / dive / none
            // 3) charge / swoop / none
            // 4) family skill (the one selected by default)
            // 5) second family skill (not selected by default)
            //   
            // only Cat and SpiritBeast currently have a #5!
            //

            switch (CalcOpts.PetFamily)
            {
                case PetFamily.Bat:         familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.None, PetAttacks.SonicBlast }; break;
                case PetFamily.Bear:        familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.None, PetAttacks.Charge, PetAttacks.Swipe }; break;
                case PetFamily.BirdOfPrey:  familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.Dive, PetAttacks.None, PetAttacks.Snatch }; break;
                case PetFamily.Boar:        familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.None, PetAttacks.Charge, PetAttacks.Gore }; break;
                case PetFamily.CarrionBird: familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.Swoop, PetAttacks.DemoralizingScreech }; break;
                case PetFamily.Cat:         familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.Rake, PetAttacks.Prowl }; break;
                case PetFamily.Chimaera:    familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.None, PetAttacks.FroststormBreath }; break;
                case PetFamily.CoreHound:   familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.LavaBreath }; break;
                case PetFamily.Crab:        familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.None, PetAttacks.Charge, PetAttacks.Pin }; break;
                case PetFamily.Crocolisk:   familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.None, PetAttacks.Charge, PetAttacks.BadAttitude }; break;
                case PetFamily.Devilsaur:   familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.MonstrousBite }; break;
                case PetFamily.Dragonhawk:  familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.None, PetAttacks.FireBreath }; break;
                case PetFamily.Gorilla:     familyList = new PetAttacks[] { PetAttacks.Smack, PetAttacks.None, PetAttacks.Charge, PetAttacks.Pummel }; break;
                case PetFamily.Hyena:       familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.TendonRip }; break;
                case PetFamily.Moth:        familyList = new PetAttacks[] { PetAttacks.Smack, PetAttacks.Dive, PetAttacks.Swoop, PetAttacks.SerenityDust }; break;
                case PetFamily.NetherRay:   familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.None, PetAttacks.NetherShock }; break;
                case PetFamily.Raptor:      familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.SavageRend }; break;
                case PetFamily.Ravager:     familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.None, PetAttacks.Ravage }; break;
                case PetFamily.Rhino:       familyList = new PetAttacks[] { PetAttacks.Smack, PetAttacks.None, PetAttacks.Charge, PetAttacks.Stampede }; break;
                case PetFamily.Scorpid:     familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.None, PetAttacks.Charge, PetAttacks.ScorpidPoison }; break;
                case PetFamily.Serpent:     familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.None, PetAttacks.PoisonSpit }; break;
                case PetFamily.Silithid:    familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.Dash, PetAttacks.None, PetAttacks.VenomWebSpray }; break;
                case PetFamily.Spider:      familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.None, PetAttacks.Web }; break;
                case PetFamily.SpiritBeast: familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.SpiritStrike, PetAttacks.Prowl }; break;
                case PetFamily.SporeBat:    familyList = new PetAttacks[] { PetAttacks.Smack, PetAttacks.Dive, PetAttacks.None, PetAttacks.SporeCloud }; break;
                case PetFamily.Tallstrider: familyList = new PetAttacks[] { PetAttacks.Claw, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.DustCloud }; break;
                case PetFamily.Turtle:      familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.None, PetAttacks.Charge, PetAttacks.ShellShield }; break;
                case PetFamily.WarpStalker: familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.Warp }; break;
                case PetFamily.Wasp:        familyList = new PetAttacks[] { PetAttacks.Smack, PetAttacks.Dive, PetAttacks.Swoop, PetAttacks.Sting }; break;
                case PetFamily.WindSerpent: familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.None, PetAttacks.LightningBreath }; break;
                case PetFamily.Wolf:        familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dash, PetAttacks.Charge, PetAttacks.FuriousHowl }; break;
                case PetFamily.Worm:        familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.None, PetAttacks.Charge, PetAttacks.AcidSpit }; break;
            }

            List<PetAttacks> toPost = new List<PetAttacks>();

            toPost.Add(PetAttacks.None);

            int family_mod = 0;

            if (CalcOpts.PetFamily != PetFamily.None)
            {
                toPost.Add(PetAttacks.Growl);
                toPost.Add(PetAttacks.Cower);

                foreach (PetAttacks A in familyList)
                {
                    if (A == PetAttacks.None) {
                        family_mod++;
                    } else {
                        toPost.Add(A);
                        //CB_PetPrio_01.Items.Add(A);
                    }
                }

                PetFamilyTree family = getPetFamilyTree();

                if (family == PetFamilyTree.Cunning) {
                    //toPost.Add(PetAttacks.RoarOfRecovery);
                    toPost.Add(PetAttacks.RoarOfSacrifice);
                    toPost.Add(PetAttacks.WolverineBite);
                    //toPost.Add(PetAttacks.Bullheaded);
                }

                if (family == PetFamilyTree.Ferocity) {
                    toPost.Add(PetAttacks.LickYourWounds);
                    //toPost.Add(PetAttacks.CallOfTheWild);
                    //toPost.Add(PetAttacks.Rabid);
                }

                if (family == PetFamilyTree.Tenacity) {
                    toPost.Add(PetAttacks.Thunderstomp);
                    toPost.Add(PetAttacks.LastStand);
                    toPost.Add(PetAttacks.Taunt);
                    toPost.Add(PetAttacks.RoarOfSacrifice);
                }
            }

            CB_PetPrio_01.ItemsSource = toPost;
            CB_PetPrio_02.ItemsSource = toPost;
            CB_PetPrio_03.ItemsSource = toPost;
            CB_PetPrio_04.ItemsSource = toPost;
            CB_PetPrio_05.ItemsSource = toPost;
            CB_PetPrio_06.ItemsSource = toPost;
            CB_PetPrio_07.ItemsSource = toPost;

            if (CalcOpts.PetFamily != PetFamily.None) {
                CB_PetPrio_01.SelectedIndex = 6 - family_mod; // family skill 1
                CB_PetPrio_02.SelectedIndex = 3; // focus dump
            } else {
                CB_PetPrio_01.SelectedIndex = 0; // none
                CB_PetPrio_02.SelectedIndex = 0; // none
            }

            CB_PetPrio_03.SelectedIndex = 0; // none
            CB_PetPrio_04.SelectedIndex = 0; // none
            CB_PetPrio_05.SelectedIndex = 0; // none
            CB_PetPrio_06.SelectedIndex = 0; // none
            CB_PetPrio_07.SelectedIndex = 0; // none
        }
        private void CB_PetFamily_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadingCalculationOptions) updateTalentDisplay();

            if (!_loadingCalculationOptions && CB_PetFamily.SelectedItem != null)
            {
                PopulatePetAbilities();
                updateTalentDisplay();
                resetTalents();
                _loadingCalculationOptions = false; // force it
                Character.OnCalculationsInvalidated();
            }
        }
        private PetFamilyTree getPetFamilyTree()
        {
            if (CB_PetFamily.SelectedItem == null) return PetFamilyTree.None;
            return CalculationOptionsHunter.PetFamilyToPetFamilyTree((PetFamily)CB_PetFamily.SelectedItem);
        }
        private void updateTalentDisplay() { updateTalentDisplay(CalcOpts.PetTalents); }
        private void updateTalentDisplay(PetTalents newtalents)
        {
            PetFamilyTree tree = getPetFamilyTree();
            //if (newtalents != CalcOpts.PetTalents) {
                ThePetTalentPicker.Tree1.Talents = newtalents;
                ThePetTalentPicker.Tree2.Talents = newtalents;
                ThePetTalentPicker.Tree3.Talents = newtalents;
                ThePetTalentPicker.RefreshSpec();
            //}
            ThePetTalentPicker.TreeTab1.Visibility = (tree == PetFamilyTree.None || tree == PetFamilyTree.Cunning ? Visibility.Visible : Visibility.Collapsed);
            ThePetTalentPicker.TreeTab2.Visibility = (tree == PetFamilyTree.None || tree == PetFamilyTree.Ferocity ? Visibility.Visible : Visibility.Collapsed);
            ThePetTalentPicker.TreeTab3.Visibility = (tree == PetFamilyTree.None || tree == PetFamilyTree.Tenacity ? Visibility.Visible : Visibility.Collapsed);

            switch(tree){
                case PetFamilyTree.Cunning : { ThePetTalentPicker.TreeTab1.IsSelected = true; break; }
                case PetFamilyTree.Ferocity: { ThePetTalentPicker.TreeTab2.IsSelected = true; break; }
                case PetFamilyTree.Tenacity: { ThePetTalentPicker.TreeTab3.IsSelected = true; break; }
                default: { ThePetTalentPicker.TreeTab1.IsSelected = true; break; }
            }
        }
        private void resetTalents() { CalcOpts.PetTalents = new Hunter.PetTalents(); }
        private void CB_ArmoryPets_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadingCalculationOptions) return;
            try { if ((string)CB_ArmoryPets.SelectedItem == "No Armory Pets Loaded") return; } catch (Exception) { } // ignore any exceptions
            // Save the Index
            ArmoryPet CurrentPet = (ArmoryPet)CB_ArmoryPets.SelectedItem;
            if (CurrentPet == null) return; // Don't continue and crash
            // Populate the Pet Family
            _loadingCalculationOptions = true;
            CB_PetFamily.SelectedItem = (PetFamily)int.Parse(ArmoryPet.PetFamilyNameToPetFamilyId(CurrentPet.Family));
            _loadingCalculationOptions = false;
            // Convert the ArmoryPet spec to our spec
            PetTalents pt = PetTalents.FromArmoryPet(CurrentPet);
            // Populate the Pet Specs box
            {
                CalcOpts.PetTalents = pt;
                CalcOpts.petTalents = CalcOpts.PetTalents.ToString();
                updateTalentDisplay();
            }
            // Populate the Abilities
            PopulatePetAbilities();
        }
        private void PopulateArmoryPets() {
            if (Character.ArmoryPets != null && Character.ArmoryPets.Count > 0)
            {
                _loadingCalculationOptions = false;
                if (CB_ArmoryPets.Items.Count == 1) { CB_ArmoryPets.Items.Clear(); }
                CB_ArmoryPets.ItemsSource = Character.ArmoryPets.ToArray();
                CB_ArmoryPets.SelectedIndex = CalcOpts.SelectedArmoryPet;
                _loadingCalculationOptions = true;
            }
        }
        #endregion

        #region Stat Graph
        private Stats[] BuildStatsList()
        {
            List<Stats> statsList = new List<Stats>();
            if (CK_StatsAgility.IsChecked.GetValueOrDefault(true)) { statsList.Add(new Stats() { Agility = 1f }); }
            if (CK_StatsAP.IsChecked.GetValueOrDefault(true)) { statsList.Add(new Stats() { AttackPower = 1f }); }
            if (CK_StatsCrit.IsChecked.GetValueOrDefault(true)) { statsList.Add(new Stats() { CritRating = 1f }); }
            if (CK_StatsHit.IsChecked.GetValueOrDefault(true)) { statsList.Add(new Stats() { HitRating = 1f }); }
            if (CK_StatsHaste.IsChecked.GetValueOrDefault(true)) { statsList.Add(new Stats() { HasteRating = 1f }); }
            if (CK_StatsArP.IsChecked.GetValueOrDefault(true)) { statsList.Add(new Stats() { ArmorPenetrationRating = 1f }); }
            return statsList.ToArray();
        }
        private void BT_StatsGraph_Click(object sender, EventArgs e)
        {
            CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter;
            Stats[] statsList = BuildStatsList();
            Graph graph = new Graph();
            string explanatoryText = "This graph shows how adding or subtracting\nmultiples of a stat affects your dps.\n\nAt the Zero position is your current dps.\n" +
                         "To the right of the zero vertical is adding stats.\nTo the left of the zero vertical is subtracting stats.\n" +
                         "The vertical axis shows the amount of dps added or lost";
            graph.SetupStatsGraph(Character, statsList, calcOpts.StatsIncrement, explanatoryText, calcOpts.CalculationToGraph);
            //graph.Show();
        }
        //private void CK_StatsAgility_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[0] = CK_StatsAgility.IsChecked.GetValueOrDefault(true); }
        //private void CK_StatsAP_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts      = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[1] = CK_StatsAP.IsChecked.GetValueOrDefault(true); }
        //private void CK_StatsCrit_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts    = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[2] = CK_StatsCrit.IsChecked.GetValueOrDefault(true); }
        //private void CK_StatsHit_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts     = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[3] = CK_StatsHit.IsChecked.GetValueOrDefault(true); }
        //private void CK_StatsHaste_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts   = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[4] = CK_StatsHaste.IsChecked.GetValueOrDefault(true); }
        //private void CK_StatsArP_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts     = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[5] = CK_StatsArP.IsChecked.GetValueOrDefault(true); }
        private void CB_CalculationToGraph_SelectedIndexChanged(object sender, EventArgs e) {
            CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter;
            calcOpts.CalculationToGraph = (string)CB_CalculationToGraph.SelectedItem;
        }
        private void NUD_StatsIncrement_ValueChanged(object sender, EventArgs e) {
            CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter;
            calcOpts.StatsIncrement = (int)NUD_StatsIncrement.Value;
        }
        #endregion
    }
    public static class ShotRotationFunctions
    {
        public static int ShotRotationGetRightSpec(Character character)
        {
            int specIndex = 0;
            int Iter = 0;
            int SpecTalentCount_BM = 0; for (Iter = 00; Iter < 26; Iter++) { SpecTalentCount_BM += character.HunterTalents.Data[Iter]; }
            int SpecTalentCount_MM = 0; for (Iter = 26; Iter < 53; Iter++) { SpecTalentCount_MM += character.HunterTalents.Data[Iter]; }
            int SpecTalentCount_SV = 0; for (Iter = 53; Iter < 81; Iter++) { SpecTalentCount_SV += character.HunterTalents.Data[Iter]; }
            // No Shot Priority set up, use a default based on talent spec
            if (SpecTalentCount_BM > SpecTalentCount_MM && SpecTalentCount_BM > SpecTalentCount_SV) { specIndex = (int)CalculationOptionsPanelHunter.Specs.BeastMaster; }
            if (SpecTalentCount_MM > SpecTalentCount_BM && SpecTalentCount_MM > SpecTalentCount_SV) { specIndex = (int)CalculationOptionsPanelHunter.Specs.Marksman; }
            if (SpecTalentCount_SV > SpecTalentCount_MM && SpecTalentCount_SV > SpecTalentCount_BM) { specIndex = (int)CalculationOptionsPanelHunter.Specs.Survival; }
            return specIndex;
        }
        public static int ShotRotationGetRightSpec(HunterTalents talents)
        {
            int specIndex = 0;
            int Iter = 0;
            int SpecTalentCount_BM = 0; for (Iter = 00; Iter < 26; Iter++) { SpecTalentCount_BM += talents.Data[Iter]; }
            int SpecTalentCount_MM = 0; for (Iter = 26; Iter < 53; Iter++) { SpecTalentCount_MM += talents.Data[Iter]; }
            int SpecTalentCount_SV = 0; for (Iter = 53; Iter < 81; Iter++) { SpecTalentCount_SV += talents.Data[Iter]; }
            // No Shot Priority set up, use a default based on talent spec
            if (SpecTalentCount_BM > SpecTalentCount_MM && SpecTalentCount_BM > SpecTalentCount_SV) { specIndex = (int)CalculationOptionsPanelHunter.Specs.BeastMaster; }
            if (SpecTalentCount_MM > SpecTalentCount_BM && SpecTalentCount_MM > SpecTalentCount_SV) { specIndex = (int)CalculationOptionsPanelHunter.Specs.Marksman; }
            if (SpecTalentCount_SV > SpecTalentCount_MM && SpecTalentCount_SV > SpecTalentCount_BM) { specIndex = (int)CalculationOptionsPanelHunter.Specs.Survival; }
            return specIndex;
        }
        public static bool ShotRotationIsntSet(CalculationOptionsHunter CalcOpts)
        {
            return ((CalcOpts.PriorityIndex1 + CalcOpts.PriorityIndex2 +
                     CalcOpts.PriorityIndex3 + CalcOpts.PriorityIndex4 +
                     CalcOpts.PriorityIndex5 + CalcOpts.PriorityIndex6 +
                     CalcOpts.PriorityIndex7 + CalcOpts.PriorityIndex8 +
                     CalcOpts.PriorityIndex9 + CalcOpts.PriorityIndex10)
                    == 0);
        }
    }
}

