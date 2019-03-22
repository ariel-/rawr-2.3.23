using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Rawr.Base;
using Rawr.Bosses;

/* Things to add:
 * 
 * Custom Rotation Priority
 * Threat Value/Weight
 * Pot Usage (Needs to pull GCDs)
 * Healing Recieved
 * Vigilance Threat pulling
 */

namespace Rawr.DPSWarr {
    public partial class CalculationOptionsPanelDPSWarr : CalculationOptionsPanelBase {
        private bool isLoading = false;
        private bool isLoadingBoss = false;
        private bool firstload = true;
        /// <summary>This Model's local bosslist</summary>
        private BossList bosslist = null;
        private BossHandler CustomBoss = new BossHandler();
        private Dictionary<string, string> FAQStuff = new Dictionary<string, string>();
        private Dictionary<string, string> PNStuff = new Dictionary<string, string>();
        // Constructors
        public CalculationOptionsPanelDPSWarr() {
            isLoading = true;
            try {
                InitializeComponent();
                SetUpFAQ();
                SetUpPatchNotes();
#if !DEBUG
                CK_Markov.Checked  = false;
                CK_Markov.Visible  = false;
#endif

                CTL_Maints.ExpandAll();
#region Tooltips
#region Boss Selector
LB_Under20Perc.ToolTipText =
@"Time where Execute Spamming Rotation occurs.

Only effective if Execute Spamming is active on the Ability Maintenance Tab";
CK_InBack.ToolTipText =
@"This affects how often the Boss can Parry your attacks

Note: No bosses in WotLK Block";
CK_MultiTargs.ToolTipText =
@"How much of the fight is spend where more than one target is within Melee Range, allowing the use of MultiTarget abilities like Cleave and normal abilities like WhirlWind to do extra damage.

Max is the highest number of targets over the fight, use to clip WhirlWind, etc. to 2 targets instead of the full 4.";
CK_StunningTargs.ToolTipText =
@"While stunned, you cannot DPS.
Iron Will and other Stun Duration reducing effects can lower the duration.
Abilities like Heroic Fury and Every Man for Himself (Humans) can negate Stuns after they occur if they are not on cooldown.";
CK_MovingTargs.ToolTipText =
@"Time spent in movement where you cannot DPS the Boss.
Either the boss has moved or you are in a situation where you can't stand where you are.
Movespeed buffs can reduce this time.";
CK_FearingTargs.ToolTipText =
@"While feared you cannot DPS.
Fear Duration reducing effects can lower the duration.
Abilities like Berserker Rage can negate Fears after they occur if they are not on cooldown.";
CK_RootingTargs.ToolTipText =
@"While Snared you cannot DPS (assume the target moves out of melee range).
Snare/Root Duration reducing effects can lower the duration.
Some abilities can negate Snares after they occur if they are not on cooldown.";
CK_DisarmTargs.ToolTipText =
@"This functionality is disabled as no bosses Disarm in WotLK";
CK_AoETargs.ToolTipText =
@"New testing for Rage Gains from Damage Taken.";
#endregion
#region Ability Maintenance
CK_Flooring.ToolTipText =
@"Flooring changes the way Rotations are calculated.
                
Normally, an ability can have 94.7 activates in a rotation, this allows a more smooth calc for things like Haste and Expertise (due to Overpower Procs).
Flooring forces any partial activate off the table, 94.7 becomes 94. This is to better simulate reality, however it isn't fully factored in everywhere that it should be.

Use Flooring at your own risk.";
#endregion
#region Misc
CK_PTRMode.ToolTipText =
@"Enables Patch 3.3.0 changes specific to DPSWarr.

Change List:
- None yet released that affect Rawr.DPSWarr";
CK_HideDefGear.ToolTipText =
@"This hides Items, Buffs, Gems, etc. that are irrelevant to DPSWarr on a Stats basis.

If the object has Defense Related Stats (Defense, Dodge, Parry, Block) the object will be removed from the lists.

Turn all three of these options off for normal behavior based solely on Item Type and having any kind of stat relevent to DPSWarr.";
CK_HideSplGear.ToolTipText =
@"This hides Items, Buffs, Gems, etc. that are irrelevant to DPSWarr on a Stats basis.

If the object has Casting Related Stats (Mp5, Spell Power, Mana, Spirit, Spell penetration) the object will be removed from the lists
(unless it has a specific reason not to be, like Powerful Stats still should be shown).

Turn all three of these options off for normal behavior based solely on Item Type and having any kind of stat relevent to DPSWarr.";
CK_HidePvPGear.ToolTipText =
@"This hides Items, Buffs, Gems, etc. that are irrelevant to DPSWarr on a Stats basis.

If the object has PvP Stats (Resilience) the object will be removed from the lists.

Turn all three of these options off for normal behavior based solely on Item Type and having any kind of stat relevent to DPSWarr.";
#endregion
#endregion
                #region Bosslist
                // Create our local Boss List
                if (bosslist == null) { bosslist = new BossList(); }
                // Populate the Boss List ComboBox
                if (CB_BossList.Items.Count < 1) { CB_BossList.Items.Add("Custom"); }
                if (CB_BossList.Items.Count < 2) { CB_BossList.Items.AddRange(bosslist.GetBetterBossNamesAsArray()); }
                // Set the default Filter Type
                if (CB_BL_FilterType.Text == "") { CB_BL_FilterType.Text = "Content"; }
                // Set the Default filter to All and Populate the list based upon the Filter Type
                // E.g.- If the type is Content, the Filter List will be { "All", "T7", "T7.5",... }
                // E.g.- If the type is Version, the Filter List will be { "All", "10 Man", "25 man", "25 Man Heroic",... }
                if (CB_BL_Filter.Items.Count < 1) { CB_BL_Filter.Items.Add("All"); }
                bosslist.GenCalledList(BossList.FilterType.Content, CB_BL_Filter.Text);
                if (CB_BL_Filter.Items.Count < 2) { CB_BL_Filter.Items.AddRange(bosslist.GetFilterListAsArray((BossList.FilterType)(CB_BL_FilterType.SelectedIndex))); }
                
                if (CB_BossList.Items.Count > 0) { CB_BossList.Items.Clear(); }
                CB_BossList.Items.Add("Custom");
                CB_BossList.Items.AddRange(bosslist.GetBetterBossNamesAsArray());
                #endregion
                
                CB_Duration.Minimum = 0;
                CB_Duration.Maximum = 60 * 20; // 20 minutes
                
                CB_CalculationToGraph.Items.AddRange(Graph.GetCalculationNames());
            } catch (Exception ex) {
                new ErrorBox("Error in creating the DPSWarr Options Pane",
                    ex.Message, "CalculationOptionsPanelDPSWarr()", "No Additional Info", ex.StackTrace);
            }
            isLoading = false;
        }
        protected override void LoadCalculationOptions() {
            string info = "";
            isLoading = true;
            CalculationOptionsDPSWarr calcOpts;
            try {
                if (Character != null && Character.CalculationOptions == null) {
                    // If it's broke, make a new one with the defaults
                    Character.CalculationOptions = new CalculationOptionsDPSWarr();
                    isLoading = true;
                }
                calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                CB_BossList.Text = calcOpts.BossName; info = calcOpts.TargetLevel.ToString(); isLoading = true;
                CB_TargLvl.Text = info; isLoading = true;// string.Format("{0}", calcOpts.TargetLevel);
                info = calcOpts.TargetArmor.ToString(); isLoading = true;
                CB_TargArmor.Text = calcOpts.TargetArmor.ToString("0"); isLoading = true;
                CB_Duration.Value = (decimal)calcOpts.Duration; isLoading = true;
                NUD_TargHP.Value = (decimal)calcOpts.TargetHP; isLoading = true;
                //RB_StanceArms.Checked = !comb calcOpts.FuryStance;  isLoading = true;
                CK_PTRMode.Checked = calcOpts.PTRMode; isLoading = true;
                CK_HideDefGear.Checked = calcOpts.HideBadItems_Def; CalculationsDPSWarr.HidingBadStuff_Def = calcOpts.HideBadItems_Def;
                CK_HideSplGear.Checked = calcOpts.HideBadItems_Spl; CalculationsDPSWarr.HidingBadStuff_Spl = calcOpts.HideBadItems_Spl;
                CK_HidePvPGear.Checked = calcOpts.HideBadItems_PvP; CalculationsDPSWarr.HidingBadStuff_PvP = calcOpts.HideBadItems_PvP;
                NUD_SurvScale.Value = (decimal)calcOpts.SurvScale;

                CustomBoss.Stuns = calcOpts.Stuns;
                CustomBoss.Moves = calcOpts.Moves;
                CustomBoss.Fears = calcOpts.Fears;
                CustomBoss.Roots = calcOpts.Roots;
                CustomBoss.Disarms = calcOpts.Disarms;

                // Save the new names
                CB_BL_FilterType.Text = calcOpts.FilterType;
                firstload = true;
                isLoading = false; CB_BL_FilterType_SelectedIndexChanged(null, null); isLoading = true;
                CB_BL_Filter.Text = calcOpts.Filter;
                isLoading = false; CB_BL_Filter_SelectedIndexChanged(null, null); isLoading = true;
                CB_BossList.Text = calcOpts.BossName;
                isLoading = false; CB_BossList_SelectedIndexChanged(null, null); isLoading = true;
                firstload = false;

                // Rotational Changes
                CK_InBack.Checked = calcOpts.InBack;
                CB_InBackPerc.Enabled = calcOpts.InBack;
                CB_InBackPerc.Value = calcOpts.InBackPerc;

                CK_MultiTargs.Checked = calcOpts.MultipleTargets;
                LB_Max.Enabled = calcOpts.MultipleTargets;
                //LB_MultiTargsPerc.Enabled = calcOpts.MultipleTargets;
                CB_MultiTargsPerc.Enabled = calcOpts.MultipleTargets;
                CB_MultiTargsMax.Enabled = calcOpts.MultipleTargets;
                CB_MultiTargsPerc.Value = calcOpts.MultipleTargetsPerc;
                CB_MultiTargsMax.Value = (int)calcOpts.MultipleTargetsMax;

                CK_StunningTargs.Checked = calcOpts.StunningTargets;
                BT_Stun.Enabled = calcOpts.StunningTargets;
                BT_Stun.Text = calcOpts.Stuns.Count > 0 ? CustomBoss.DynamicCompiler_Stun.ToString() : "None";

                CK_MovingTargs.Checked = calcOpts.MovingTargets;
                BT_Move.Enabled = calcOpts.MovingTargets;
                BT_Move.Text = calcOpts.Moves.Count > 0 ? CustomBoss.DynamicCompiler_Move.ToString() : "None";

                CK_FearingTargs.Checked = calcOpts.FearingTargets;
                BT_Fear.Enabled = calcOpts.FearingTargets;
                BT_Fear.Text = calcOpts.Fears.Count > 0 ? CustomBoss.DynamicCompiler_Fear.ToString() : "None";

                CK_RootingTargs.Checked = calcOpts.RootingTargets;
                BT_Root.Enabled = calcOpts.RootingTargets;
                BT_Root.Text = calcOpts.Roots.Count > 0 ? CustomBoss.DynamicCompiler_Root.ToString() : "None";

                CK_DisarmTargs.Checked = calcOpts.DisarmingTargets;
                BT_Disarm.Enabled = calcOpts.DisarmingTargets;
                BT_Disarm.Text = calcOpts.Disarms.Count > 0 ? CustomBoss.DynamicCompiler_Disarm.ToString() : "None";

                CK_AoETargs.Checked = calcOpts.AoETargets;
                NUD_AoEFreq.Enabled = calcOpts.AoETargets;
                NUD_AoEDMG.Enabled = calcOpts.AoETargets;
                NUD_AoEFreq.Value = (int)calcOpts.AoETargetsFreq;
                NUD_AoEDMG.Value = (int)calcOpts.AoETargetsDMG;

                // Abilities to Maintain
                CK_Flooring.Checked = calcOpts.AllowFlooring;
                LoadAbilBools(calcOpts);
                // Latency
                CB_Lag.Value = (int)calcOpts.Lag;
                CB_React.Value = (int)calcOpts.React;
                // Special Effects Special Option
                CK_SE_UseDur.Checked = calcOpts.SE_UseDur;
                //
                //calcOpts.FuryStance = (Character.WarriorTalents.TitansGrip > 0);
                //RB_StanceFury.Checked = calcOpts.FuryStance;
                //RB_StanceArms.Checked = !RB_StanceFury.Checked;
                //
                CK_Markov.Checked = calcOpts.UseMarkov;
                //
                CK_StatsStrength.Checked = calcOpts.StatsList[0];
                CK_StatsAgility.Checked  = calcOpts.StatsList[1];
                CK_StatsAP.Checked       = calcOpts.StatsList[2];
                CK_StatsCrit.Checked     = calcOpts.StatsList[3];
                CK_StatsHit.Checked      = calcOpts.StatsList[4];
                CK_StatsExp.Checked      = calcOpts.StatsList[5];
                CK_StatsHaste.Checked    = calcOpts.StatsList[6];
                CK_StatsArP.Checked      = calcOpts.StatsList[7];
                NUD_StatsIncrement.Value = calcOpts.StatsIncrement;
                CB_CalculationToGraph.Text = calcOpts.CalculationToGraph;
                //
                Character.OnCalculationsInvalidated();
            } catch (Exception ex) {
                new ErrorBox("Error in loading the DPSWarr Options Pane",
                    ex.Message, "LoadCalculationOptions()", info, ex.StackTrace);
            }
            ItemCache.OnItemsChanged();
            isLoading = false;
        }
        // Instructions Tab
        private void SetUpFAQ() {
FAQStuff.Add(
"Why is the Mortal Strike talent shown with negative DPS in the Talent Comparison Pane? The ability is doing x DPS.",
@"When the standard rotation abilities for Arms are active (including Slam and Heroic Strike) the large rage consumption of the Mortal Strike Ability tends to overshadow the rage left-over for Heroic Strikes. Basically, if you were to Slam instead of Mortal Strike on every time you would have otherwise, there would be more rage left over to Heroic Strike. In some cases, Rawr sees this as a DPS gain and wants you to drop Mortal Strike. Fully 25 Man raid buffed, Mortal Strike should have a higher DPS value than the rage to Heroic Strikes would provide."
);
FAQStuff.Add(
"Why does X talent/glyph not show any value in the comparison lists?",
@"Many talents cannot be valued by DPS gain or by Survivability Gain (which was recently added). Also, most Prot tree talents are not modeled as they are unnecessary, basically anything beyond 3rd tier. It's also possible that you do not have the Situation setting where the Talent/Glyph would have value. For example, If you are never Stunned, then Iron Will wouldn't have a value."
);
FAQStuff.Add(
"Why does X ability lower my DPS when I check it in the Ability Maintenance Tab?",
@"Abilities may not provide additional DPS, but they do absorb Global Cooldowns (GCDs). If the ability is replacing something that would otherwise cause damage and isn't providing at least a temporary buff, the total DPS will go down. This usually occurs when you check the Maintenance abilities. Commanding Shout doesn't add to DPS but takes GCDs to keep up."
);
FAQStuff.Add(
"Why is it when I run the Optimizer I don't end up hit capped and/or expertise capped? Shouldn't that be automatic?",
@"The optimizer, when run without any requirements, will attempt to find the highest possible Total DPS number. In many cases, this does not include being hit/expertise capped. This is an unfortunate calculational error that has been persistent throughout Rawr.DPSWarr's history. We have made great strides to correct this issue but a few points are still off. To ensure these caps are enforced, add the '% Chance to be Avoided <= 0' requirement before optimizing. You must also consider that in some cases, if another Hit Gem would put you over the cap by a large amount (you need 3 hit but gem would give you 8 so 5 wasted) the optimizer may find that leaving that 3 under the cap and giving an 8 STR gem in its place would be more beneficial overall."
);
FAQStuff.Add(
"Why does my toon do 0 DPS?",
@"There are a couple possible reasons this could occur.
1) You don't have a Main Hand Weapon, all DPS is tied to having a Main Hand Weapon.
2) Your Situational settings on the Fight Info tab are set such that you ave no ability to get any DPS out during the fight."
);
FAQStuff.Add(
"Why does the optimizer try and equip two of my weapon when I only have one?",
@"To restrict it to one item, right-click the item, select Edit then mark the Item as Unique. This will prevent it from putting the item in both MH and OH slots. The same goes for rings, trinkets, etc."
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
"What about <20% Target Health Execute Spamming?",
@"We don't model this yet, sorry."
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
            CB_FAQ_Questions.Items.AddRange(arr);
            CB_FAQ_Questions.SelectedIndex = 0;
            CB_FAQ_Questions_SelectedIndexChanged(null, null);
        }
        private void CB_FAQ_Questions_SelectedIndexChanged(object sender, EventArgs e) {
            string text = "";
            if (CB_FAQ_Questions.Text == "All") {
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
            }else{
                string s = CB_FAQ_Questions.Text;
                string a = "invalid";
                bool ver = FAQStuff.TryGetValue(s, out a);
                text += s + "\r\n";
                text += "\r\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_FAQ.Text = text;
                RTB_FAQ.SelectAll();
                RTB_FAQ.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
                RTB_FAQ.Select(0, RTB_FAQ.Text.IndexOf('\n'));
                RTB_FAQ.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            }
        }
        private void SetUpPatchNotes() {
PNStuff.Add(
"v2.3.3 (Unreleased)",
@"- Updated PvP set bonuses for Warriors
- Fixed the Warrior PvP 2 pc bonus (stupid typos)");
PNStuff.Add(
"v2.3.2 (Dec 12, 2009 14:39)",
@"No Significant Changes due to short period of time between releases.");
PNStuff.Add(
"v2.3.1 (Dec 10, 2009 03:47)",
@"- Fix for DW tick frequency: no longer using one per sec, as each crit will push back your next tick by 1sec. This caused the value of 2pT10 to decrease.");
PNStuff.Add(
"v2.3.0 (Dec 08, 2009 17:04)",
@"- Fixed the set threshold on T10 4Pc to actually need 4 pieces instead of 2
- Trying to get the SD proc change into place from T10 4P
- Support for 4p10 Arms - assumes that both executes have the 1.0sec GCD. If only one does, then its value is decreased roughly in half.
- Redid a couple of the Warrior set bonus descriptions
- Added PvP 4 pc set bonus to the Get Stats function, was missing by accident
- Fixed Improved Slam to provide positive DPS again
- Fixed bug causing your maintained BattleShout/CommandingShout (special effects don't like Use effects with CD=Duration)
- Fixed a bug with Second Wind providing DPS when you weren't being stunned/rooted, and while I was at it I fixed a bug where roots were being counted as fears and therefore not triggering SecondWind.");
PNStuff.Add(
"v2.2.28 (Dec 06, 2009 22:28)",
@"- Fixed a bug where some of the interface wasn't initially setting it's enable/disable
- Added the missing 7th WW hit for Bladestorm
- Modification to OP GCD usage (uses less when the CD is less than a base GCD)
- Same change to TfB
*** Ebs is arms now, so he's making arms fixes! ***
- MS activates are now also affected by Bladestorm
- Fix for issue 14830 (Overpower eating up slam GCDs)
- Added the 1sec Overpower logic to the execute spamming. Note that execute spamming is still not working properly
Beginning to migrate my 'big patch' over, which isn't so big anymore, as I had to scrap a lot of it :(
- Buff handling performance has been improved, a lot less strain on the garbage collector
- MessageBox changes when errors are caught
- Refactoring of GetCharacterStats
- Agility from Mongoose procs is now affected by kings
- Removed a lot of extraneous GetXOverDur calculations in the proc logic
- Options Panel: maint tree is handled by a small algorithm rather than hard-coded switch statement
- Moved the validated special effects of stats to Rotation
More changes migrated from my busted patch
- Created a Default Attack Table that we can reuse for abilities that don't have weird changes (increased crit chance from talents/glyphs, overpower, etc)
- Fixed a bug where white attacks were using yellow crit chances instead of white ones
- More errorbox fixes
- Removed a lot of private fields that were being used in Getters/Setters. The compiler can do this for us - cleaner code and faster performance, woo!
- Fury: Cleaned up the HS/Cleave calculations. Improved performance
- Skills: Removed InitializeA(CalcOpts) and changed InitializeB(CalcOpts) to not pass any parameters. Cleaner code, more efficiency
- Skills: Re-worked it so the only things that are overridden are Validated and Overrides. Not a bug-fix, but is consistent and makes more sense
- Arms will now get deep wounds damage from Heroic Strike
- Lowered DPS from Overpower after a dodged attack, because sometimes TfB has procced but you can't use it, or because you're keeping rend up, etc. As a result, Expertise will no longer be negative damage.
- Changed accessor of AddValidatedSpecialEffects from internal unsafe to public (gogo 'Generate Method Stub')
- Fix for 2-roll system on abilities with a bonus crit chance
- Removed some dead code
- Reverted DeepWounds change from arms, *mumbles something about spaghetti*
- Some whitespace changes, nothing significant

Refactoring of Rotation. It took a long time to untangle some of that spaghetti; hope I didn't miss anything.
Added to Rotation object:
- FightDuration param
- TimeLostGCDs for Boss Handling
- Pulled out the GCDs in the rotation, GCDs used and GCDs available to a property level
- New Methods: protected CalculateTimeLost, privates CalculateFear, CalculateMovement, CalculateStun, CalculateRoot.
Rotation.Arms:
- Took out maintenance from the SettleAll loop. These never change while looping
- Maintenance code is now generalized.
- Took out <20% looping, as it's been broken forever. The option's still there, it just does nothing. It can be hooked back up when we fix it
Misc Bug Fix: SecondWind/SweepingStrikes confusion in SecondWind's constructor.

- All models using Prof enchant hiding have been changed to use the global sets on the Stats Pane and Options > General Settings > Hide enchants based on professions. Models that were handling this manually have been edited to the new method, models that didn't have it are now on it as the back end changed
- Profession bonus Buffs (Toughness and the like) are now updated when you update your professions in all models. Models that were handling this manually have been edited to the new method, models that didn't have it are now on it as the back end changed
- Minor fix to Slam when way under on rage performance
- Implemented Stat Graph for DPSWarr (only in Rawr2, wanted to get the kinks out)
- Using the data shown on the charts for small to large numbers, I've implemented several stat caps and bottoms, some things wouldn't happen in real life, but they show directly on the Graph
- Made corrections to the HS/CL displays on the stats pane. The data used for display was garbage in certain situations, these now have good data coming in (this was a display issue only)
- Added a fix to Fury where the percentage of rage wasn't be distributed between HS and CL
- Added a fix to Fury where invalid HS/CL would have a NaN violation

Next batch of major refactoring of Rotation. If you're familiar with DPSWarr, please test this and report issues. Major refactoring is dangerous, but in the long run it will be much much easier to maintain
- Added AbilWrapper, which is a wrapper object for abilities, their activates, DPS, etc
- Rotation holds a list of AbilWrapper that can be iterated over for getting numAtksOverDur, building comparison charts, etc
- Added SwingsOffHand/SwingsPerActivate/UsesGCD to Ability to let us streamline special cases at the Ability level (whirlwind, bladestorm, etc)
- Abilities that give rage now have a negative rage cost for easier identification
- Moved the BossHandler fear/stun/move/root numbers to the Rotation base, which means Fury is able to use it. It probably needs some tweaking.
- Float fail and cache fail fixed

Arms tweaking to match Landsoul's sheet.
- Reaction Allowance lowered from 250ms to 200ms
- Expertise display now includes expertise from talents (was only a display issue)
- Flooring/Casting of stats removed due to jaggedness of charts; will re-add eventually
- More arms crowding added
- LatentGCD now includes Reaction Allowance
- GCDTime added to Ability object to facilitate knowledge of 1sec OP GCDs and 6sec Bladestorm GCDs at the Ability level
- Overpower now uses full reaction time and TFB now uses reaction allowance since TFB procs aren't random.
- Berserker Rage and Bloodrage now use full reaction time instead of reaction allowance time
- Ability.UseTime added, which factors in Lag/Reaction/GCD time to tell you how much time is used up when you activate the ability.
- OnAttacks are now default to not use the GCD (didn't change DPS but would have led to a bug later)

- Tweaks to BossHandling to support shared HF/EM
- Encounters with Movement will now support Intercept and Heroic Fury
- All abilities by default have a rage cost of 0 instead of refunding 1 rage
- Fixed a null exception error
- Added some XmlIgnores to cut down on redundant variables in the char file save
- Fixed Fears and Moves not recalling on char file load (We can now move forward on getting the rest of those moved to new method)
- Killed a warning about a variable not in use
- Added Descriptions to the abilities on the back end, so they can be used in our Custom Comp Charts as part of the tooltips
- Updated the remaining Impedances to use the new multi-imp handling method
- Moved our ErrorBox to Base (since I started referencing it a few times for stuff outside DPSWarr)
- Removed a check for FuryStance on Rampage in two places, since rampage works in all stances.
- Added base setups for T10 set bonuses (just the stats themselves, not the handling of those stats)
- Starter setup for value of T10 2 pc, Ebs needs to look at it
- The addition of 2pT10 set bonus has made me aware of a few problems in our stat multiplier and armor handling
- Completely revamped UpdateStatsAndAdd to work with stat multipliers.
- Removed some variables that weren't being used
- Armor was just way off, it's now much more accurate
- Berserking now interacts correctly with Armed to the Teeth
- PhysicalCrit from CritRating is now done in one place rather than all over the place
- 2pT10 is now supported and functional (unsure about if the actual items will activate the buffs, however). Still no value for 4pc
- Slight boo-boo with BaseArmorMultiplier vs BonusArmorMultiplier. Now using TotalArmor = (BaseArmor * BaseMult + BonusArmor)*BonusMult. This makes the Toughness talent a little worse

Fury's 4T10 has been implemented. Net result is slightly better than 2T9, so worth the upgrade.
- The rotation has changed. It is still a 'BT/WW-first, Slam only when possible' rotation, but slam is now limited by the 5sec duration of the Bloodsurge buff.
Without 4T10, BS now only procs from 1/2 your BTs and 5/8 of your HSes, due to the 5sec duration on the buff.
With 4T10, the number of procs you get is increased by 20%. You also get 20% of the BS procs that were not included in the 'Without 4T10', since they can be squeezed in the 1sec gap between BT#2 and WW. The remaining proc gets used if you wouldn't have had a proc otherwise.
New Rotation:
0.0: WW
1.5: BT
3.0: Slam procced at all? If not, leftover from 4T10proc slam used at 7.0)?
4.0: Bonus Slam from 4T10?
5.5: BT
7.0: 1s Slam from 4T10 proc? Leave second for use at 3.0
8.0: WW(repeat from 0.0)"); 
PNStuff.Add(
"v2.2.27 (Nov 10, 2009 03:45)",
@"- No commits for this release");
PNStuff.Add(
"v2.2.26 (Nov 09, 2009 01:53)",
@"- No commits for this release");
PNStuff.Add(
"v2.2.25 (Nov 08, 2009 20:15)",
@"- Fix for issue 14526 (Bladestorm eating all your GCDs in low rage settings). Also removed a reference to a variable in Rotation.cs that was never used
- Refactored ErrorBox to a new file
- Added a check for the iterator for Heroic Strikes/Cleaves to use the already set verification of (HS/CL)OK before attempting to loop, should imp perf a little for users not HS'g
- Added a Dev Only Accessible checkbox to enable Markov Rotation for Arms (working a new method that's more intelligent and better adapted for Interference modeling, it doesn't actually do anything as we haven't fully constructed the necessary files)
- Fix for Mongoose, fix for off-hand weapon enchants using MH speed in their uptime, some performance improvements
- Committing work on Markov method for arms rotation, note that it has absolutely no bearing on anything right now
- Updates for markov model
- Well Kavan and I got to working on it and it is generating states.... like 140,000 of them /facepalm. Turned it off for now so it doesn't affect anything
- Converted CalcOpts' variables to properties with an OnPropertyChanged event (to be used later in Rawr3)
- Rawr3: Some cleanup on the Options pane... but it isn't showing up in Rawr3 when run for some reason
- Rawr3: LOTS of work on the Options Pane and now it's not crashing (well, almost not) and it's functioning (selecting options actually does stuff)
- Rawr3: Updated the Options pane to reflect work from Rawr2 (still not done but definitely closer)
- More Markov work with Kavan
- Rawr3: Significant improvements to the Options pane, objects should line up correctly, enable/disable and tie to the correct parameters. There's still work to be done but we're getting close to a fully functional Rawr3 model.
- Added a verifier for bleed hit intervals (in Special Effects handling) to check if Rend is being maintained instead of assuming it is.
- Cleaned out some old commented code
- Added logfile creation for recording errors in DPSWarr that go through ErrorBox class. This is primarily for Rawr3 debugging.
- Ability Maintenance Tree now ties to abilities, but for some reason when you open that tab, it resets all of them to off (thus making user re-activate them on each character load). Once reactivated, it seems to work fine.
- Fixed the Ability Maintenance so it won't reset them all to not active (had to do away with the tree unfortunately)
- Got some of the ComboBoxes working
- Fixed the cooldown on Intercept and added PvP 4 pc set bonus to it
- Added comments to the Hunter and Warrior Set Bonus Stats");
PNStuff.Add(
"v2.2.24 (Oct 24, 2009 17:45)",
@"- Added new naming/colors for the custom chart so you know what each number actually means
- Fixed a bug with Sword Spec to show values when not using a Swordspec
- Fixed a bug with Needed Rage in Arms Rotation, making it visually double-dip
- Fixed a bug where in an Unlimited Rage scenario, Heroic Strikes/Cleaving would cause a NaN error
- Tied the Boss Does AoE Damage fields to the AoE Damage freq's in BossHandler
- Removed unnecessary float casts on Math.Max and Math.Min. Also removed string calculations when needsDisplay is false.
- Removed a lot of validation on return values that only serve to hide bugs that we don't have and should probably fix if we get them, rather than just say 'make it 0'. (ie return Math.Max(0f, Lag/1000f))
- Removed the cast to float on Math.Abs, as there's an overload for floats.
- Refactored some of the talents/special effects calculations so we're doing Talents/30 instead of Talents*10/3/100
- Fixed a bug where null MH item would cause a crash
- Improved the Extra Rage handling for Sudden Death/Execute
- Added Modeling for situations where you aren't generating enough rage for your rotation to slip ability activates (relatively evenly). When this happens, you no longer Slam or Heroic Strike/Cleave to conserve rage. Should only happen when unbuffed or at really low gear levels. Note: There's a side-effect with this that reduces the value of Expertise when at low gear levels, this is somewhat to be expected but could be a little bugged.
- Fixed a bug where Unbridled Wrath was only proccing on missed attacks instead of landed attacks
- Unbridled Wrath now procs on landed Heroic Strikes and Cleaves. The DPS change refunded to the rage cost of the effect. For those curious, from 0/5 to 5/5 UW went from +15dps to +21dps on a test fury character, so ~40% increase.
- Minor performance increases by removing parameterized methods and making unparameterized ones. Changes made were:
- ContainCritValue(bool isMH) to ContainCritValue_MH and ContainCritValue_OH
- AvgMhWeaponDmg(itemspeed) to AvgMhWeaponDmgUnhasted (never used with a different speed)
- GetDPRfromExp(float Expertise) to just use StatConversion (no need for the wrapper)
- LevelModifier() and NPC_CritChance() changed to properties with only a Get method (no params)
- Reworked the 'I don't have enough rage' Arms scenario that Jothay checked in, per his request.
- Added new Comparison Chart 'Rage Cost per Damage'. This chart shows the amount of damage per ability divided by the abilities rage cost. The second point shows the bonus damage that goes to Deep Wounds for that abilities crits per rage point spent. This gives extra value to abilities that have high crit rates as they are more likely to gen extra damage via Deep Wounds.
- Fixed a bug with Hide Enchants Based on Professions. It wasn't doing it's recall set until you unchecked/rechecked.
- Separated the PerRageFails for Above and Below Exec Spam points
- Added a WARNING! statement to the Total GCD Usage tooltip stating how much of your abilities are being lost to Rage Starvation
- Improved the Total GCD tooltip (better spacing and lining up of stuff)
- Added Every Man for Himself handling to break stuff besides just stuns. Limited to previous uses for other stuff
- Fixed handling of BonusRageGen stat, at some point it got dropped off
- Changed handling of Bloodrage for Arms and Fury, now properly adds to both sides for the appropriate amounts
- Fixed a cosmetic issue with Fury's display of Rage Gen Other (Anger Management, Blodrage etc rage gains)
- CombatFactors wasn't updating crit chances from special effects due to caching. Moved the _c_foo methods from readonly public to a property with {get;private set;} and had _c_foo called on InvalidateCache
- Added HealthRestore as a relevant Stat so Healing Pots and similar effects can be counted for Survivability
- Added new handling for Potions in DPSWarr, now the Cooldown is set to Fight Duration on our end to better show the value of the pot in the fight. This only affects users with 'Use Duration in SpecialEffects' not checked
- Added Profession to Buff handling. If you are a Miner it auto-enables the Toughness Buff, same for Master of anatomy and Skinning. If Engineer and using Runic Healing Injector, auto-enable the improvement bonus
- Bugfix for the new Bloodrage usage (wasn't counting health value for BR Glyph)
- There was a bug where the new potion code would affect the potion's duration until the character was re-created (ie, when changing models and their stats instead of creating a new character or loading an old one). Modified so we are creating a new buff instead.
- Fixed a null reference error
- Added some Default Talent Specs for Warriors (several mutations for Arms and 1 Fury, 1 Prot)
- Changed the Set Bonus filtering to use the new Class restrictions instead of by string comparison
- Updated Patch Notes on the Options pane to use the FAQ system (drop-down selects version to view DPSWarr notes for that version)
- Added a Target HP field to the Boss Tab, now Custom Bosses can have health besides 1 million.
- Added boxes to control individual stuns etc, it will average them out for now, will added handling per stun later
- Replaced the boxes for Movement Frequency and Duration with a Button that opens the dialog, all moves are now controlled by that system
- Changed the handling for Moves in Arms rotation to check each move individually, allows better handle for bosses with multiple moves of differing lengths, etc.
- Replaced the boxes for Fear Frequency and Duration with a Button that opens the dialog, all fears are now controlled by that system
- Changed the handling for Fears in Arms rotation to check each fear individually, allows better handle for bosses with multiple fears of differing lengths, etc.
Known Issue: The moves and fears are not recalling from saved files. The file stores the lists correctly but for some reason they don't populate on load.
- Rawr3: Provided some updated to prevent compile errors but until I can actually dev in Silverlight properly DPSWarr still doesn't work in Rawr3."
);
PNStuff.Add(
"v2.2.23 (Oct 15, 2009 03:15)",
@"- Some more backwards compatibility work added so we don't break character files. Only one method to update when calcopts changes, and we can call CalculationOptionsPanelDPSWarr.CheckSize(calcOpts) if there are other weird entry points for crashes down the line.
- Fixed stacking SpecialEffects
- Fixed the Expertise Issue for Fury (also affects Arms)
- Fixed a AverageStack/Uptime misconception we had
- Fixed it so the debug frame shouldn't show up in release mode (even though it has no data in release mode)
- Fix for a bug that came up with the new stats averaging. Trauma and similar effects weren't getting their eventual values passed properly."
);
PNStuff.Add(
"v2.2.22 (Oct 13, 2009 05:16)",
@"- More perf increases. Changed all stat operations to .Accumulate, except for one subtraction method in which I would have had to create a new stats object to use accumulate anyway (resulting in no perf change that I can tell)
- More perf fixes. CalcOpts is now being passed around like dancers at a strip club.
- Paragon will now add Agility if it is your highest stat (but if it is, you should re-roll). I almost added a MessageBox that says 'You're doing it wrong' when this code path is hit.
- Fixed a couple conflict issues, fixed the Error outside of range issue for setting boss variables in the Boss Selector Pane
- Fixed Berserker Rage needing to be maintained for Fear Breaks (can now activate without Maintenance Box Checked)
- Added Berserker Rage Maintenance reducs from BR's used for Fear Breaks (prevent double-activations)
- Change a couple references for Latency
- Added better variables and calc'g for Stunned GCDs, this fixed Iron Will giving neg dps value (which was just weird)
- Added Validated checks to AddAnItem (several maintenance abilities use this). Prevents those code blocks from running when ability isn't valid (saves processing time)
- Fixed bugs with the some activates where Reaction time wasn't being converted to millisec properly
- Further fixing to Side-Systems for Arms
- Added a debug/Rawr2 only stopwatch to help us monitor performance of our app in the future.
- Using cached values of Item.Speed and Item.Type in combatfactors (apparently we missed some a while back)
- Fix for issue 14401: Intellect shouldn't be blocking gear as it comes free on Mail and Leather items
- Preserving Rot object in CalculationsDPSWarr
- Replaced call for MaximumStats with BuffedStats and an iteration over SpecialEffects with ArP Rating, since we were only using that for Max Armor Pen (ie with procs)
- Removed statsGearEnchantsBuffs, and just pulled from statsGear and statsBuffs
- Replaced a MakeRotation() call with a doIterations() call for iterative special effects
- Lots of caching of CombatFactors values, lots of privatization of CombatFactors values. Cache is reset whenever Stats object is changed
- Updated more methods that were essentially { get { float a = foo(); return a; } }
- More unnecessary 'get { float a = foo(); return a; }' nonsense removed."
);
PNStuff.Add(
"v2.2.21 (Oct 07, 2009 20:18)",
@"- One bugfix where ArP trinkets were using boss-level regardless of what you set in options
- Lots of performance increases, especially for Arms (more to come later).
- Not looking up Unbuffed/Buffed stats right now since we aren't doing anything with them
- Switched GetXAtks() methods to properties
- Caching lots of the XAtks calls
- Caching CombatTable.AnyLand and AnyNotLand
- Arms iteration is now much more efficient (from 14 big iterations to 3-4 big iterations and 2-3 small ones). No real loss of precision
- Caching Ability.Validated
- Rage calculations streamlined
- Other misc changes"
);
PNStuff.Add(
"v2.2.20 (Oct 06, 2009 19:22)",
@"- Fury warriors can now only equip polearms in the mainhand, and only if there is no offhand equipped
- Fixed a bug where not having Commanding Presence, Maintaining Battle Shout, and having Battle Shout/BoMight would give you both buff effects
- Maintaining buffs will no longer disable their corresponding buff checkboxes; those buffs will now just not contribute to your stats. You no longer need to re-set your BoMight after un-checking Battle Shout from the maintenance tab
- Fixed a bug in where buff/debuff maintenances that can not miss (ie battle shout, enraged regeneration) were capable of missing.
- Deathwish and Recklessness will now use up GCDs
- Fixed a bug where we were double-dipping in AvoidanceStreaks when calculating Bloodsurge procs.
- Hit will no longer randomly jump above strength when changing buffs or maintenance abilities (fixed from the double-dip and enraged-regen-miss bugs listed above). If you still see this occur, please open an Issue and attach your character file.
- PTR Mode is now disabled and anything requiring it is now set as standard instead of the option
- Updated the ArP tooltip to the new caps and added T9 breakout into that
- White-space reformatting in some areas (no calc changes)
- Fix for issue 14264, older char files not able to load in new version due to abil tree failing to populate before a separate thread is launched that needs it pop'd
- Fixed a bug with Cleave DPS
- Updated to match new BossHandler methods
- Added Draenei Heroic Presence Buff enforcement if player is a Draenei, removed other conflicts with this method
- More bug-fix crunchin an' munchin'
- Framework for having Unbuffed/Buffed/Max Stats (not in tooltips yet)
- Special Effects now iterate over Crit/Haste/Hit bonuses until settled, then move on to non-Crit/Haste/Hit bonuses
- Mongoose and Executioner now properly trigger only from their weapon, and not all hits on one weapon
- Whirlwind will no longer be calculated as Glance-able
- Fixed the Hit 'Relative Stat Values' bug once and for all (was related to flooring the stats given by On-Hit effects like Berserking)
- Added Rage Gains from Damage Taken
- Added Boxes to Options Pane to control Damage Taken: Freq (in seconds, default every 20 sec) and Unmitigated Damage (default 5000)
- These come together to increase total rage generated over duration from Boss AoE attacks. Next step is to include damage types for damage reduction from Armor, Resists, etc and handling if that AoE is dodge-able, etc.
- These Damage Taken variables are not tied to BossHandler yet so they are set independently for the time being
- Added Movement abilities (Charge from Juggernaught only for now). Value is now given to Juggernaught, Improved Charge, Glyph of Charge and (if movement freq is high enough) Glyph of Rapid Charge
- This recovers a set amount of time (based on movement speed) when you have to move to keep up with a boss.
- Fixed a couple of Bugs with the other effects that get in the way of dealing damage (Stuns, etc.)
- Fixed a couple bugs that came up from last commit
- Added Fury Berserker Stance Damage taken penalty to Rage gen from damage taken
- Added Damage Taken Trigger handling in our SpecialEffects (Black Heart now gives us the armor, not that we really needed it though)
- Stupid new files not auto-adding themselves
- Rearranged the Options pane some to make room for new stuff
- Separated Hide Bad Items to 3 separate boxes: Def, Spell, PvP
- Removed Intellect from Spell so Mail gear doesn't get removed inadvertently
Minor accuracy updates 
- Fix for ReactionTime/Latency (now Reaction will only be used when you need to react, ie procs or fear/root breaks).
- Modified the AvoidanceStreak calcs to use a Rage Slip instead (credit to Landsoul's arms sheet). Abilities that do not cost rage are not affected (ie Bloodrage, Berserker Rage)
- Reverted the fix-but-not-really-a-fix on Fury's Slam being absolved from AvoidanceStreak. In this new formula, it's affected just as everything else is.
- Fury warriors take note: Hit post-cap has lost a little bit of value in the new checkin.
- Updated Rend Calcs
- Added Damage Taken Multipliers to affect damage for Rage Gains
- Added Rage Gains from Blessing of Sanctuary
- Fixed a bug with Slam trying to eat GCDs when not being Maintained
- Fixed some relevancy stuff
- Added Incoming Dodge/Parry stuff for better DamageAvoided accuracy(but didn't add those as relevant)
- How did I miss this bulletin on Dodged White attacks?
- Rage gained from a dodged/parried attack is now that of a normal hit (rather from 0)
- Avoidance Streak calculations only look at white miss chance, not white dodge or parry
- Added intelligent proc handling for ArP; Mjolnir Runestone and Grim Toll are now handled more intelligently, averaging out your Damage Reduction from ArP rather than the rating from ArP
- Added 'Max Armor Penetration' to the Armor Penetration tooltip, so you can track how close you are to the ArP cap
- Overhauled Sword Spec, new activates formula (to better sim the once every 6 sec), new attack table (to enforce glances), new tooltip (to show you the glances)
- Yellow attacks weren't using the 2-roll system after the optimizations we did a while back, but it wasn't obvious until the AvoidanceStreak re-tuning (thanks BrWarner)
- Deep Wounds was counting Heroic Strike/Cleaves twice. Oops.
- Added another minor fix to Sword Spec activates
- Incorporated Mortal Strike activation delays based on Landsoul's latest sheet
- Using new method for Sudden Death activates (similar to Sword Spec's new ones)
- Added intentional downtime to Rend
- Several parts of these changes put the Arms rotations noted activates closer to Landsoul's sheet for easier comparisons between the two. However, in order to set up like the spreadsheet, you have to turn off more than half the options available to Rawr and water it down to just a Patchwerk test. Total DPS variance for a BiS test was 3.2% (about 9k vs 9.2k)
- More Landsoul v Rawr work
- Moved Rend up in Priority
- Formulized more of the values used in MS Delays
- Improved the Performance of the new MS Delay (was causing heavy slowdowns, managed to reduce them some)
- Updated the Expertise tooltip on Stats Pane to show ratings caps against talents (nice grid for players that need to know it)
- Added Damage Taken Multiplier to Survivability, -10% damage (like Blessing of Sanc) increases Surv Value, +10% Damage decreases it
- Fixed the tooltips on the Options Pane, they now properly take their new character returns so they aren't just one line running off screen
- Added BossAttackPower as Relevant
- Added Demo Shout Handling for Maintenance vs Buffs to Prevent Double-Dipping
- Added BossAttackPower to Survivability (turns it into a dps incoming mod and adds that to Survivability, this gives value to Improved Demo Shout Talent)
- Added BossAttackPower modifier to the Rage Gained from Incoming Damage
- Fixed Demo Shout Duration's attachment to Booming Voice
- Added BossAttackSpeedMultiplier as relevant, store Thunder Clap value
- Added Thunder Clap logic to use ours instead of Buffs when being maintained
- Added BossAttackSpeedMultiplier to Survivability
- Added Sunder Armor Special Effect Based on Talents, etc
- Added Sunder Armor logic to use ours instead of Buffs when being maintained
- Fixed a bad relevant stat
- Turned on the 'Use Duration for Special Effects'. See below for description
- Fixed the rage gain from Blessing of Sanctuary; the calcs were a little wonky, and you were seeing rage gains from it whether you had the checkbox for AOE damage or not
- Description of 'Use Duration for Special Effects' option: There are two ways we can get the stats from a special effect such as Flurry, Death Wish, or Bloodlust. One way (the way most spreadsheets use) is to average it out over time, so that regardless of how long the fight is, the buff is worth the same amount of DPS. Take Bloodlust as an example: 30% haste for 40secs, 10min CD. You can average that out to 2% haste (0.3*40/600), but on a 2min fight, you get much more haste out of it (0.3*40/120 = 10%, it's up 33% of the time). When 'Use Duration for Special Effects' is checked, it will give you 10% for Bloodlust on a 120sec fight; if it's unchecked, it will give you 2%. Landsoul's sheet gives you 2%.
- Removed code that's still in development
- one final fix for BoSanc's 2-rage bonus on dodged/parried attacks. Should be correct now.
- Fixed an unreachable code warning
- Added Option to hide Enchants based on professions, E.g.- If you aren't an Engineer you won't see Hyperspeed Accelerators (this option is defaulted to off, users will need to activate it for the benefit)
- Added Taste for Blood separation from Overpower on Abil Maintenance Tree
- laid groundwork for a new comp chart for DPSWarr, but it wasn't working right so it will remain hidden until it does
- Fix for issue 14338, 14339"
);
PNStuff.Add(
"v2.2.19 (Sep 23, 2009 06:22)",
@"No Significant Changes due to short period of time between releases."
);
PNStuff.Add(
"v2.2.18 (Sep 23, 2009 04:36)",
@"- Partial fix for issue 14176, needed to check base.IsItemRelevent to hide items for other classes etc
- Fix to show Potion of Wild Magic as a relevant Buff
- Hid extraneous Set Bonus Buffs
- Added Fear and Root settings to the Options Pane, these work the same way the Stuns settings do
- Added Fear and Root handling to Arms
- Fixed an issue where some of the Boss Settings might not reload
- Added a LOT of tooltips to the Options pane so users can more easily know what each setting does
Special Rage Updates! 
- Added spell-miss changes for Demo/Thunderclap. Also added support for Misery in buffs
- Fixed a rage calc issue where it was using your hasted weapon speed as opposed to your unhasted weapon speed in the rage formula
- Fixed a rage calc where, when doing Heroic Strike/Cleave calculations, the rage cost of those abilities were effectively doubled. HS damage for Fury should be in line with actual performance (and landsoul's spreadsheet)
- Fixed the RageGen tooltips in the Fury model
- Moved a FreeRageOverDur from Rotation.base to Rotation.Fury, as it wasn't being used in Arms
- More tooltip corrections (White DPS tooltip now functions properly)
- Fixed T8 2pc set bonus for good this time
- Added new proc stats methodology that supports nested proc effects such as Victory's Call
- Added forced override for Nightmare Tear (and it's lesser counterparts) to be shown with Hide Bad Items enabled
- Updated the Options Pane to reflect recent changes in Rawr2
- Managed to attach some of the UI elements to functions (blend kept crashing on everything else). It now lets you select Fury vs Arms, which is the most important thing.
- Made the Under 20% HP Box on the options pane actually do something
- Tied the Under 20% HP handling to the box on the options pane
-  Fixed use of SpecialEffects (we were confusing using Frequency instead of Interval, and we were passing a critInterval with 100% chance, rather than hitInterval with crit% chance).  (Now, the Rampage talent gives <5% crit as it should (and will open up the chance for LotP to be a slight dps increase; need to support that though). This will also allow us to use charged buffs like Sweeping Strikes, Recklessness, and Flurry, more accurately.)
- Slight perf/accuracy improvement by making sure we don't constantly recreate CombatFactors/WhiteAttacks objects
- Migrated Flurry over to the Special Effects system. Rampage is not a Special Effect for now due to performance reasons
- Berserking uptime is now much more accurate
- Fixed a bug in the White DPS tooltip where DPS didn't match the stats pane
- Fixed a bug causing the White Damage on Hit to be Zero for Arms in the stats pane (visual issue only)
- Fixed a Typo in the White DPS tooltip
- Fixed a bug with the new SpecialEffects method that was causing things to have a >100% chance to proc (was throwing several calcs off, especially Victor's Call)
- Updated the Comparison Calcs statement to include Survivability"
);
PNStuff.Add(
"v2.2.17 (Sep 23, 2009 04:36)",
@"- Fixed a bug with Overpower Activates
- Added Support for StunDurReduc items and Migrated Iron Will to use that stat
- Added new Relevancy method for Items/Enchants/Buffs, hides items that have reduced usefuless (items that have Spell stats and/or Tanking Stats). This is to make the comparison lists smaller and more relevant and make the Build Upgrade List run faster (since there's a smaller list of items to have to choose from. This setting can be turned on/off from the top of the Options Pane.
- Laid the visual groundwork for '<20% Execute Spamming' (making it an option on Ability Maintenance tree, adding a line for DPS on the stats pane, adding back-end variables to store the DPS info)
- Added <20% HP Execute Spam handling
- More refactoring to remove dependencies between Arms and Fury
- improved performance by removing superfluous DoIterations calls. Began implementing a smarter rotation (not being called, so don't worry, no functional changes)
- removed some old code (bloodsurge rps stuff). 
- Changed accessability on a lot of Skills methods/properties from public to protected (helps w/intellisense, and we should minimize our public methods in general)
- commented out the call to the new method that was just eating up cycles and doing nothing relevent yet. Whoops!
- Finishing the commit that was supposed to go in last night
- Moved Exec Spamming into the Proc Settler
- Fixed the total rage generated stat on the pane so it's properly visible
- Reworked the Rage usage system on Execute and Sudden Death
- Reworked the activates on Taste for Blood for a better value per point
- Redunculous overhaul to Execute Spamming in Arms, trying to get it under better logic and ensure that you see a DPS gain with it activated. Meh, 1 of 2 ain't bad...
- Partial Fix for issue 14169: T8 2P Set Bonus for DPSWarr overvalued. Added the chance to Proc instead of 100%, corrected it's activates interval."
);
PNStuff.Add(
"v2.2.16 (Sep 13, 2009 21:51)",
@"- Added usage for BossHandler's filtered lists, can now filter by Content (T7, T9, etc), Instance (Naxx, EoE, etc), Version (10/25, etc) or by Boss Name.
- Added Saving/Recalling methods for Filtered lists and last boss selected
- Fixed NaN issue with arms with low stats (ie: no buffs). Fixed NaN issue when no weapon is equipped (which also affected the weapon lookup window when checking for upgrades)
- Fixed a number of bugs that led to offhand giving negative DPS
- Fix for issue 14106: Skills.Base.WhiteAttacks.OhActivates wasn't checking for attack speed of 0 (when no offhand was equipped). Also, removed 'Which' logic in some places, as it was no longer being used
- Removed the reference to GlobalSuppression.cs from the project file.
- Fixed Landed attacks per second calcs used in SpecialEffects
- Reworked SweepingStrikes using a new method that more accurately consumes charges
- Added a new verification check that ensure if MultiTargs Percent is set to 0, it will act as if MultiTargs isn't checked (so for sure no DPS change for on @ 0% vs off)
- Updated the Default Gemming Templates, there's now 24 (4 groups of 6, Uncommon, Rare, Epic, Jeweler). Only the 6 from Epic are defaulted to active. Note to users: I had to delete my GemmingTemplates.xml file so Rawr could auto-recreate it.
- Corrected a couple abilities' validations
- Fixed the Landed Attacks thing (did one thing backwards
- Added caps to durations for Sweeping Strikes and Recklessness so they can't go overboard with taking too long to consume the activates
- Enhanced the FAQ box's readability by linking it to a dropdown so a user can select the question then read only that question and the answer.
- Fixed the cap on Max Targets to it won't null exception with the number it's recieving is too high"
);
PNStuff.Add(
"v2.2.15 (Sep 07, 2009 08:25)",
@"- Partially applied a patch that updates talents to 3.2 (Rogue, Paladin and Warriors)
- Added a PTR Mode checkbox (currently activates 3.2.2 changes for SwordSpec) (added variable to Rawr3 so it can compile)
- Increased the loop limit on proc settling for Arms
- Working on Stun handling
- Working on Stun handling
- More work on Stun handling
- Reworked Multi-Targs and it's relation to Heroic Strike and Cleave. Now settles Cleaves used vs Heroic Strike with Multiple Targets Percent for Arms (Fury still needs to adopt this)
- Added Heroic Strike and Cleave to Verbose Tooltip
- Fixed the issue with Available Rage being shown on the panel as negative.
- Added Survivability Calcs: This is Health / 100 + Heals Per Sec (HPS) that comes from self abilities (Second Wind, Enraged Regen, etc.). You can scale the effect using the Options Panel (set to 0 to ignore Survivability).
- Made Changes to Fury Iter to settle Cleave in with HS and Bloodsurge. This was to fix a performance issue and Ebs needs to review
- Fixed the bug that makes the Titan's Grip talent show massive negative DPS. Needed to add a lot more checks for OH stuff to require TG.
- Added Enraged Regeneration to Ability Maintenance Tree.
- Fixed Jothay's attempt to put multipletargets in fury :P Multiple Targets should now work properly in both arms and fury.
- Oops, forgot to save a file :x
- Added better logic to Enchant Fits in Slot checks for MH/OH
- Changed the defaults for the Options panel to 100% in back, multitargs off and stunning off
- Sweeping Strikes wasn't playing nice with my changes earlier
- Fully removed the Trinket GCD system
- Changed Sweeping Strike's chance to new attack table
- Added Paragon support (Death's Verdict/Choice)
- Fixed Rampage Talent Handling
- Changed Trinket and other SpecialEffects trigger intervals to operate more on actual ability usage numbers
- Reworked Ability Proc'g to operate more on actual ability usage numbers
- Added Rage Gains from Sword Spec hits
- Added Rage Slip effect to a couple of abilities that weren't using it
- Adjusted Dual Wield Specialization Bonus application
- Remodeled the following abilities to operate as SpecialEffects: Flurry, Wrecking Crew, Rampage, Trauma. This increases their accuracy for bonus over uptime.
- Split handling of Cleave and Heroic Strike numbers where they were previously combined.
- Pushed a series of code changes to make more items act in relation to Duration of Fight verse Per Sec to simplify math
- Fix Bonus Targets for White Damage
- Modeled Bloodrage's Health usage and Glyph to negate it
- Removed some code that had become outdated
- Removed some bugged requirements for Second Wind
- Moved some of the extra SpecialEffects around to try and improve performance and accuracy.
- Broke T8 2P set bonus (working on a solution)
- Added OnAttack Abilities to Landed Attacks per sec (since they have different tables than white attacks)
- Simplified a lot of the rage calcs
- Reverted Flurry Change
- Found the bug in T8 2P for Arms, fixed
- Created FuryRotation and ArmsRotation as a subclass of Rotation, pulled all the fury/arms specific rotation logic into these files. Broke down Skills.cs to different partial classes to make it easier to navigate and see changes. I've tested both arms and fury for right now
- Fixed Ebs's commit (one of the files was conflicting on his attempt to commit)
- Fixed a bug with displayed information which occurred after rotation refactoring
- Fixed a bug with Bladestorm which occurred after rotation refactoring
- Simplified usage of use OH checks
- Added MovementSpeed as a relevant stat
- Fixed a slight bug with the new useOH method
- Enabled Moving Targets, change the box from a Percentage to a Number of Seconds. This box is clipped by Duration (can't set a higher value than what Duration is at). Making the necessary code for Arms to make use of this box next.
- Improved reliability of saved options recovering on the Panel by using an isLoading check found in several models.
- Added a Percentage box to Moving Targets time. This relates directly to the other box (adjust one and it adjusts the other). Also when Fight Duration is updated, adjusts Move Time (seconds) by Move Time (%)
- Added BossHandler usage: Users can now use the Boss Selection drop-down to change their Situational stuff (Moving, Stunning, Multiple Targets) to presets based upon boss fights or use one of the Special Bosses.
- Swapped name list to use BetterBossNames (better for sorting)
- Fixed a bug with Stunning targets where if freq is set to 0, you will get a NaN error.
- Added Boss info box, updates using GenInfoString. TODO: Make it gen a new string based upon custom settings
- Updated the Instructions tab to relate to all the recent changes
- Added 11 items to the FAQ and updated the 4 that were there
- Updated the Version Notes tab to 2.2.15
- Fixed a bug where players with Trauma Talent that have not selected a MH weapon would get a NaN error"
);
PNStuff.Add(
"v2.2.14 (Aug 30, 2009 07:18)",
@"- Fixed a couple of stat roundings
- Fix for issue 13873 'Target level does not affect yellow hit cap'. Hit cap is now affected by target level in the Options Pane. This is reflected both in calculations and in the Hit can free/more hit needed tooltip.
- Added a Bounds Check to ensure character files created pre-v2.2.12 can be loaded into current version
- Modified CombatFactors to use the new StatConversion for target level adjustment handling
- Applied patch 3631 (T9 set bonuses)
- Applied patch to fix Warrior T9 set bonuses
- Fix for Issue 13901, Bloodrage no longer pulls GCDs
- Reworked the Tier Set bonuses (7,8,9). This fixes the Haste proc, and the double-dipping for the T9 bonuses created by last commit.
- Little bit of code reorganizing
- Changed the perc optimizer requirements to translate percentages
- Fixed Armor calculations, was not properly using the difference between Base Armor and Bonus Armor and their respective multipliers.
- Fixed MultiTargs Handling: It was double-dipping in some places and not being consistently handled. Now if you have MultiTargs checked but at 0% you will see same numbers as if it were unchecked and it scales up to 100% correctly.
- Added Max Targets to MultiTargs Handling: This places a limit on how much extra damage goes out for hitting additional targets, otherwise it will use max (up to 8).
- Updated Glyph bonuses for Battle and Commanding Shout to 3.2
- Added Glyph of Command as Relevant for DPSWarr, it affects the GCD usage of Comm Shout
- Fixed Commanding Shout's naming, etc and it now pulls GCDs properly when maintained
- Added Stun Handling (Arms Only). The Selector on the Options Panel is now active and can be used as a percentage of the fight while stunned (recommend using low numbers as very few fights have more than a couple of stuns over the entire fight).
- Added Iron Will Handling (Reduces number of GCDs lost to a stun)
- Added Second Wind Handling (Arms Only) (pops when getting Stunned)
- Added Every Man for Himself Handling (Humans Only) (Arms Only). Recovers GCDs lost to Stuns.
- Added Heroic Fury Handling (Talent Only) (Arms Only). Recovers GCDs lost to Stuns.
- Fixed Reloading of Saved options from the Options Panel, grouped function calls was causing it to set the first item or two, then an event would trigger and break the rest of the settings.
- SpecialEffect procs now include hasted white attacks, not just unhasted.
- Fixed a bug where swordspec was giving negative DPS if you weren't equipping a sword -- should have been 0 dps change.
- Did some more work on the Verbose Tooltip, cleaning it up and having it hide parts of the table that are 0.
- Changed the defaulting values in the Target Armor box
- Added more abilities to the Verbose Tooltip (Bloodsurge, Bloodthirst, Whirlwind, Thunder Clap, Shattering Throw)
- Removed Block from the Attack Table until proof can be shown of bosses in WotLK that block player's attacks
- Added more verification for Swordspec to ensure it's not affecting anything without a Sword equipped.
- Added Fall-through logic for Cleave and Heroic Strike. If MultiTargs is active and Cleave is not being maintained, you will Heroic Strike instead. There is currently not a half-way option yet.
- Fixed Overpower so it cannot be Blocked
- Fixed Targets display for Bladestorm
- Fixed Targets for Hamstring, Shattering Throw, Retaliation
- Turned off GCD Usage for Trinkets with Trigger.Use Effects
- Fixed handling for internal Blood Frenzy (Buff Blood Frenzy stays the same)
- Added White Attacks to the Verbose Tooltip method
- Added Combat Table class to DPSWarr: Migrated all attack table related calls to this new class, now handles Crit cap against Glancing and other similar extreme ratings levels
- Fixed a bug where Crits were getting an extra 100% damage
- Fixed a bug in the Rage formula for Unbridled Wrath
- Fixed a couple of abilities that had Crit on their attack table when they aren't capable of Critting
- Fixed a bug in Sudden Death and Overpower abilities activating when not being maintained
- Reworked the Activates for Overpower, no longer breaks on extremes
- Reworked Rend activates to handle Rend Ability not landing. If it's dodged, etc on the application, player must redo Rend to get it on the target as Rend needs 100% uptime for Taste for Blood. (This adds value to hit and expertise)
- Fixed several BuffEffects to operate on Attack Tables. If the debuff 'misses' the ability is degraded. Will add reapplication checks (like Rend does now) later.
Note to DPSWarr Users: New testing in Arms shows an optimize for Max DPS without any requirements will now place you within a gem's reach of Hit and Expertise caps. I've seen it forgo 4 hit rating (from cap) in favor of 8 STR or ArP, same on Expertise. Higher Standing in Back increases the likelihood STR will override Exp. Whether this affects Fury the same way remains to be seen by Ebs.
- Now supports softcap with Grim Toll and Mjolnir Runestone (note: wearing BOTH trinkets is not yet supported).
- Performance improvements
- Reworked the tooltips for Hit and Expertise, you will now see better values and the differences in caps for 2h vs DW (hit) and Dodge vs Parry (Exp). Also takes better account for other bonuses towards the caps.
- Fixed Bonus Armor procs not providing AttT bonus. This allows handling for Indestructible Potion
- Attempt to correct issue with Options panel not showing same values as back end. Users may need to uncheck/recheck options in some cases to assert the values.
- Changed Allow Flooring Default to false.
- Possible fix for issue 13977. When some options, gear, etc are changed DPSWarr hangs. Added a loop counter to the OP/SD/SL/SS settler to stop it at 100 loop iterations if it hasn't already finished"
);
PNStuff.Add(
"v2.2.13 (Aug 12, 2009 02:28)",
@"- Updated the Tooltips for Warrior Abilities
- Updated Talent and Glyph trees for 3.2
- Updated Base Stats for Warriors
- Reworked the Optimizer fields to a smaller set that is more functional and reads more to what they actually do
- Fixed a Tooltip issue with Armored to the Teeth
- Fixed a bug with new Agility Calc "
);
PNStuff.Add(
"v2.2.12 (Aug 06, 2009 05:01)",
@"- No notes for this release for DPSWarr, many changes occurred for other models and for the Base and data file caches to fully prepare for WoW Patch 3.2"
);
PNStuff.Add(
"v2.2.11 (Aug 05, 2009 05:40)",
@"- Rearranged the options panel to a series of tabs. Will be using this to spread out all the new systems to be added later (including boss handling)
- Fixed a bug with HS's not using Incite correctly, also not capping the crit percentage from 0%-100%
- Fixed a bug with Slam freq being zero and causing a divide by zero in certain calculations
- Added Slam freq to Sudden Death proc handling (corrects white swing timer)
- Changed the Health and Stamina stat displays to a single line (with more verbose tool tip). Re-aligned the tool tips on several others to make them more clean
- Fixed the Strength, Stamina and Attack Power calculations. Over time these had become invalid and are now set properly to match in-game
- Changed the Buff system to operate a little differently. When you have the relevant talent it will override the buffs selected. Eg- If you have Trauma, it will remove the Trauma buff and use the Talents' calculated version. Same for Battle Shout, Blood Frenzy and Rampage.
- Trinkets that have Trigger.Use effects now pull GCDs (making them more accurate to how much dps they are actually adding)
- Added a new section to the Options Panel: Instructions. This has 4 sub-panels.
- - Basics: Basic information for immediate, simple setup of the DPSWarr module for your character
- - Advanced: Information about advanced tweaking that can be done to the module
- - F.A.Q.: A Frequently Asked Questions tab to help alleviate some of those head-scratchers
- - Version Notes: The Raw patch notes for DPSWarr commits
- Added Weapon Master Dodge Percentage line to the Expertise Tooltip
- Fixed -x DPS value for defensive talents when Rampage Buff is selected
- Changed the Overpower activates from dodges formula, should be technically more accurate than it was.
- Split Overpower from Taste for Blood procs to two separate display lines (and activate as two separate abilities)
- Changed the Expertise Tooltip to better display the values and corrected an issue where panel was showing SoA in one spot and not another
- Changed the Total DPS Tooltip to show only the GCDs used, not abilities that use 0 (the list was getting too big to keep up at all times)
- Changed the GCD usage of Overpower to absorb 2 GCDs instead of 1 to sim the attack that was used to activate it being dodged and doing no damage and then having to use another GCD to activate OP and do damage.
- Cleaned up CombatFactors to newer naming convention
- Cleaned up the Attack table for White attacks and reworked WhiteDPS to operate more like the yellow abilities do (in file and function structure, still uses all the white factors)
- Fixed Overpower so it's no longer Block-able (as it should be)
- Implemented a 3.2 Mode
- Added Health as a Relevant Stat so you can select Commanding Shout as a Buff
- First round of optimizations
- Made a couple corrections to Ebs's optimization code to prevent null exceptions and show proper info on tooltips
- Applied Astrylian's Special Effect Dual-Zerking changes, drastically improves optimize performace. Ebs needs to review for Fury.
- Cleaned up a couple more Tooltips
- Fixed bug in Astry's patch that was overvaluing non-Berserking Off-Hand enchants, and causing fury to crash while changing weapons. DPSWarr perf issues are officially considered fixed, thanks everyone!
- Changed the Activates for Arms Warriors to be Floored (100.85 becomes 100). This is more accurate to # of times activated over fight duration. It was previously not floored due to rotation constraints, which have been removed. Most characters will see a slight difference in DPS, not major difference. NOTE: Abilities like Shattering Throw need an extra second to activate (will create a fix for this later). Eg- 5 Min cd so 300 sec is fine, but 600 seconds isn't, need 601 for the 2nd activate
- Changed the Order of Text on the GCDs tooltip over Total DPS. This lines up better now and is easier to read
- Reworked the Ability Maintenance List to a CheckList TreeView, easier to use and more obvious functionality added (Can now select Commanding Shout vs Battle Shout, and it will only allow you to select one at a time. Selecting Fury Stance enables the Fury abilities automagically, and disables Arms. +Vice-versa)
- Added Fury abilities to maintenance (to function like the arms ones do, unchecking it means you never do that ability)
- Fixed the Ability activation of Commanding Shout and actually added it's stats to the character (oops). Also added it to the buff override system (if you are puttingup CS yourself, it will remove the 'Buff' CS and Warlock Imp Buff)
- Fixed the new Ability Maintenance Tree saving/loading issues (oops again)
- Fixed a bug where AvoidanceStreak was increasing DPS rather than decreasing it.
- Added a new option to the panel for Allowing the Flooring changed (happened in the last couple commits). You can now turn this on or off.
- Moved some more code to the Optimized method that Ebs was setting up (for uniformity). made the calculators for these private to enforce no outside use. Also did some optimization in the Skills.cs file to make some abilities that have sub-abilities take less memory.
- Reworked the Haste system, not sure if it made any calculational differences overall, but it does walk through all the additive vs multiplicative stacking before and after procs.
- Fixed some of the melee hit triggering for SpecialEffects, they will have less value now because they are affected by the hit table (landing vs not landing). Also verified that all SpecialEffects are using non-hasted values
- Broke apart AvgMhWeapDamage, there are now all 3 values (ItemSpeed, ItemSpeed Hasted, NormalizedItemSpeed)
- Migrated all weapon speed function calls to the same 2 in White Attacks to ensure uniformity on Slam_Freq adding to the swing timer in all things that need to know this
- Fixed Rend to work off of unhasted ItemSpeed (gives positive value to Haste where it hadn't been due to other changes)
- Migrated more functions to Probxxofhit/land to make some function calls easier to read and less intensive to work out.
- Cleaned up the Skills List in Rotation to list everythign correctly and prevent duplicates
- Added Sword Specialization, this is a new dps line and it procs from all landed attacks over duration of fight, must have 1h or 2h sword equipped and talent (of course). May be inaccurate for dual-wielding swords (but you shouldn't be in a spec that could do that anyways). Sword spec hits are white and should add rage but I have not added that in yet (coming soon). Please note that initial testing has shown SS still has far less total dps with equivalent items than the other 2 specs (Mace/Poleaxe), this was expected.
- Changed Sudden Death to include offhand white hit procs and sword spec procs as hits that can proc SD.
- Fixed a bug where Slam DPS was being added in place of SwordSpecDPS (counting Slam DPS twice)
- Changed the Slam wing timer delay mechanic, should operate on it's proper per second theory
- Fixed NormalizedWeaponDmg vs AvgWeaponDmg vs AvgWeaponDmgUnhasted for several abilities
- Added Sword Spec attacks to OP procs
- Added Armor as a relevant stat and buff (since it influences AttT)
- Added new Tooltip format for Arms abilities, very verbose now (will add to Fury shortly)
- Fixed a mistype with OH Blocks
- Reworked the Arms proc'ing system: OP, SD, SS, Slam now settle their activates instead of using generics
- Fixed BonusArmor to actually be added to calculations (Devo aura and other similar buffs now apply correctly).
- Removed all References to 3.2 Mode and marked those items as actual (Patch Released Aug 4th, 2009)
- Fixed a bug with the new Sudden Death Damage on Hit limitation"
);
PNStuff.Add(
"v2.2.10 (Jul 17, 2009 22:27)",
@"- Fix for issue 13616 - One-Handed weapons in the main hand with Titans Grip was causing UpgradeList to crash due to the damage modifier being set to 0
- Fix for issue 13607 regarding T8/T8.5 2pc bonus. The calculations still have inaccuracies as to the value of a 2pc set for arms
- Added Relevant Glyphs to Arms (narrows glyphs list of all protection only glyphs)
- Removed Rotation logic, replaced with only Priority Queues (Also changed Fury to a Priority Queue)
- Cleaned up the Primary Stat display
- Refactored Deep Wounds to calculate at a later time, correcting it's activates method. It is still only about 2/3's effective but better than 1/10
- Considerable changes to Latency, should factor in a lot better
- Cleaned up Jothay's changes to work with fury. Few random bugfixes (Rage Starvation now computed assuming you're HS/Cleaving, Deep Wounds triggers off of HS/Cleave, Recklessness was bugged and has been removed as it's a paltry DPS increase anyway, Glancing Blows can now push normal hits off of the table and dip into crits).
- Stat Display Pane: Changed the display method for some of the stats and change the tool tips on some as well. You now see both the % gain and the Rating number for stats like Crit.
- Made the Arms Rotation work a bit more dynamically (to prepare for custom rotation priorities)
- Changed Overpower to operate with it's activates separately (Taste For Blood will have separate activates from the Dodges/Parries). This allows it to calculate correctly in extremes
- Started Constructing some of the framework for incapacitation effects (back end only, you will not see this in calcs)
- Fixed a bug with Death Wish not being activate-able for Fury Warriors"
);
PNStuff.Add(
"v2.2.09 (Jul 02, 2009 03:02)",
@"- Added fury support for maintaining buffs/debuffs. Thunderclap isn't yet modeled, because TClap isn't usable in fury stance (additional work needed to calculate rage loss of switching back and forth). Possible that we'll just say Fury can't maintain TClap
- Replaced Maintenance Checks with a new list that encompasses all the Arms abilities. You can now customize your rotation by removing certain abilities (will make it re-orderable later)
- Combined a Couple of the Rage Details tooltips (will help prevent height issues)
- Reworked the landed attacks per sec to take into account gcd users with no melee hits (corrects activates for Sudden Death)
- Did heavy reworking on Latency for Arms, now operates almost like Landsoul's sheet instead of as a percent modifier (which was a faulty method)
- Heavily recoded Heroic Strike handling for Arms, it now activates based upon the actual rage remaining after what is gen'd and iterates the number of white attacks vs overridden ones (Heroic Strike or Cleave)
- Updated Rawr3.DPSWarr so that it can compile with the new changes
- Fixed bug in Maintaining debuffs, Minor optimizations"
);
PNStuff.Add(
"v2.2.08 (Jun 30, 2009 11:29)",
@"- We were double-dipping in glancing blows.
- Fury has a rage-starvation factor.
- Fixed a bug in racial calculations and PhysicalHit (all racial calculations now come from Base)
- Cleaned up rage info in arms rotation
- Fixed Darkmoon Card: Death
- Removed unneeded Damage Reduction field
- Fixed T8-2pc bonus
- Renamed 'CritPercBonus' to 'BonusCritChance' to be more clear what it is storing
- Many damage mods changed from Additive to Multiplicative
- Agility is now affected by Kings
- Yellow Attacks now use the 2-roll instead of 1-roll system
- Yellow attacks now properly have a different MH and OH crit chance with respect to their expertise
- Rage Starvation now impacts both arms and fury
- Fixed armor penetration to use player level and not target level
- Slight optimization in calculating hit/crit (don't pass weapon item on the fly, instead use MhCrit/OhCrit/MhHit/OhHit)
- Fixed some discrepancies in how hit rating works. At this point, there are no known/glaring issues with the output of DPSWarr.Fury"
);
PNStuff.Add(
"v2.2.07 (Jun 21, 2009 22:54)",
@"- Fixed Titansgrip having major negative dps for arms, was a miss chance issue
- Added Heroic Strike back into Arms
- Changed the Options panel for handling future edits
- Corrected a couple of Tooltip errors
- Fixed arpen calculations for arms, so Mace Spec and Battle Stance are counted as buffs instead of debuffs (additive towards arpen rating now)
- Fixed double-counting of Heroic Strikes in overall DPS calculations
- Added placements for additional war abils, not in use yet but placed
- Added Incite working
- Changed some things with HS for Arms
- Fixed Staffs showing up for Titan's Grip (TY Droidicus)
- Added T8 set bonuses (TY Droidicus)
- Added some base type changes to abilities so they can work a little more in sync
- Created a new Rotation object and pushed related functions and calls to it
- Fix in Deep Wounds calculations, and put back in the HS fix from before (merge issue removed it)
- Moved more stuff to Rot
- Added to Rawr3 (Options Panel not ported)
- Realized there was an issue with the port to v3 that broke existing v2 code
- Refactored a lot of things out of Ability and into Rotation, where they belong. Should improve performance (less creating of Ability objects). Fixed bug in DeepWounds where its DPS was 6x as high as it should be :-x New number seems low though, and needs to be investigated
- Added logic for maintaining buffs and debuffs (Thunderclap, Sunders, etc) these now pull GCDs off the Arms Rotation as needed. Also, if you are maintaining Sunder Armor, then the SetDefaults for buffs will enable Sunder as well (to show the dps boost). Still need to make it run the buff add(s) at time of change.
- Beginning to rework the rage calcs for arms to better work abilities based upon rage gen (esp HS and cleave)
- Added Cleave logic (if MultiTargsm use Cleave instead of HS, will create an option to *not* do this automatically later)
- Added Multi-target laogic, abilities now multiply their damage out for # of targets. Eg- MS has 1 for base + ~.13 in a 600 sec Duration for Sweeping Strikes. Next step will be to add Sword Spec logic using this process
- Added Latency (mimicks Landsoul's spreadsheet)
- Added Inback check for Parry
- Changed the Optmizer options, Can now set a lot more control on the optimizer and use 'makes sense' options like 'Chance to be Avoided %' which if set to '<=0' will make sure you are hit/exp capped including your racial and buff bonuses
- Added a GCD Usage check for Arms, mouse over the TotalDPS for it's tooltip to read how your GCDs are being consumed
- Lined up the COmbatFactors file to better distinguish the ind. parts of the attack table
- Changed the DW calcs to work more off of the actual activates than the theoretical ones
- Fixed a bug in OH Whtie DPS Calcs
- Ported a lot of functionality to Rawr3, but have not activated it yet
- Fixed SuddenDeath activates (latency handled wrong)
- Fixed Hit Rating (Had it adding to Miss rate instead of reducing it)
- Fixed a bug in Slam causing it to generate rage instead of use it
- Minor fixes
- Fixed a few bugs, and moved a lot of the GetFoo()-type methods to properties to help while debugging
- Added Parry factoring, affected by the Standing in Back option on the Panel
- Fixed/Added Parry and Block handling for the attack table
- Added more info to the Hit Rating Tooltip
- Added more Rage Calc info and corrected some of it's usage
- Fixed Precision addage"
);
PNStuff.Add(
"v2.2.06 (Jun 06, 2009 01:34)",
@"- Added stamina as a relevant stat
- Fixed PoleaxeSpecialization (was applying regardless of having talent)
- Fixed TasteforBlood so the talent shows it's DPS gain in talents comparison
- Changed Hit Rating ToolTip to show amount of hit can be freed
- Activated background usage of abilities like Deathwish, Recklessness, Shattering Throw
- Arms rotation takes GCDs off for above abilities being used
- Added 10643 armor as default in options, fixed bug in DeathWish, removed Recklessness for now (can't be modeled as a SpecialEffect)
- BloodFrenzy now provides 5/10% haste (up from 3/6%), removed double-dipping of death wish
- Default Stance Recognition
- Better Berserking Logic (needs optimization)
- Default target set to a level 83 boss with 13083 armor (configureable through options)
- Fury Rotation changed due to BT's new cooldown/ragecost
- Added Imp Berserker Stance to strength modifier
- Procs now take into account misses/dodges/parries
- Format issue in fury rotation
- Rage formula modified based on blue post
- Removed overpower calculations due to expertise temporarily because it was causing crashes in some situations
- Known Issue: DeathWish is being counted twice"
);
PNStuff.Add(
"v2.2.05 (May 25, 2009 16:25)",
@"- Rage Generation bugfix (below expertise cap will now calculate properly)
- Deep Wounds bugfix (damage dealt is based off of number of abilities/white attacks done, still needs more work but is giving fairly accurate results)
- More work on Arms, now does reasonable approximate dps counts. (ind skills may be off but ttl dps should be pretty close).
- Rend now factors for the dmg difference for target hp level
- Corrected DW and some issues with negative rage available killing SuddenDeath's damage (added a minimum freerage given since it was already reserved by the GCD process)
- Minor modifications
- Lots of bugfixes
- WhiteMissChance was being calculated incorrectly.
- Slight issue with double-dipping on damage modifiers for some abilities.
- WhiteDmg displayed in character details is now the average damage for each MainHand swing (previously was unmitigated non-crit hit).
- Reworked bloodsurge to be much more 'smart', resulting in a lower number of procs per rotation. The old system was allowing slams as if it didn't share the GCD with any other ability. The new system won't try to slam until you have a free GCD.
- Fixed OP
- Changed Rotation length based upon Rend Glyph so it consumes GCDs correctly
- Fixed some bugs in GetDamage on a few abilities
- Corrected white-damage glance/miss issue
- Enforced activation of buffs Trauma, Rampage and Blood Frenzy if you have the talents
- Formulaic fixes and some Talent/Glyph improvements"
);
CB_Version.Items.Add("All");
            string[] arr = new string[PNStuff.Keys.Count];
            PNStuff.Keys.CopyTo(arr, 0);
            CB_Version.Items.AddRange(arr);
            CB_Version.SelectedIndex = 0;
            CB_Version_SelectedIndexChanged(null, null);
        }
        private void CB_Version_SelectedIndexChanged(object sender, EventArgs e) {
            string text = "";
            if (CB_Version.Text == "All") {
                int Iter = 1;
                text += "== CONTENTS ==" + "\r\n";
                foreach (string s in PNStuff.Keys) {
                    text += s + "\r\n";
                    Iter++;
                } Iter = 1;
                text += "\r\n";
                text += "== READ ON ==" + "\r\n";
                foreach (string s in PNStuff.Keys) {
                    string a = "invalid";
                    text += s + "\r\n";
                    bool ver = PNStuff.TryGetValue(s, out a);
                    text += (ver ? a : "An error occurred calling the string") + "\r\n";
                    text += "\r\n" + "\r\n";
                    Iter++;
                } Iter = 1;
                RTB_Version.Text = text;
            }else{
                string s = CB_Version.Text;
                string a = "invalid";
                bool ver = PNStuff.TryGetValue(s, out a);
                text += s + "\r\n";
                text += "\r\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_Version.Text = text;
                RTB_Version.SelectAll();
                RTB_Version.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
                RTB_Version.Select(0, RTB_Version.Text.IndexOf('\n'));
                RTB_Version.SelectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            }
        }
        // Boss Handler
        private void CB_BL_FilterType_SelectedIndexChanged(object sender, EventArgs e) {
            if (!isLoading && !isLoadingBoss) {
                isLoadingBoss = true;
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                // Use Filter Type Box to adjust Filter Box
                if (CB_BL_Filter.Items.Count > 0) { CB_BL_Filter.Items.Clear(); }
                if (CB_BL_Filter.Items.Count < 1) { CB_BL_Filter.Items.Add("All"); }
                CB_BL_Filter.Text = "All";
                BossList.FilterType ftype = (BossList.FilterType)(CB_BL_FilterType.SelectedIndex);
                bosslist.GenCalledList(ftype, CB_BL_Filter.Text);
                CB_BL_Filter.Items.AddRange(bosslist.GetFilterListAsArray(ftype));
                CB_BL_Filter.Text = "All";
                // Now edit the Boss List to the new filtered list of bosses
                if (CB_BossList.Items.Count > 0) { CB_BossList.Items.Clear(); }
                CB_BossList.Items.Add("Custom");
                CB_BossList.Items.AddRange(bosslist.GetBetterBossNamesAsArray());
                CB_BossList.Text = "Custom";
                // Save the new names
                if (!firstload) {
                    calcOpts.FilterType = CB_BL_FilterType.Text;
                    calcOpts.Filter = CB_BL_Filter.Text;
                    calcOpts.BossName = CB_BossList.Text;
                }
                //
                Character.OnCalculationsInvalidated();
                isLoadingBoss = false;
            }
        }
        private void CB_BL_Filter_SelectedIndexChanged(object sender, EventArgs e) {
            if (!isLoading && !isLoadingBoss) {
                isLoadingBoss = true;
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                // Use Filter Type Box to adjust Filter Box
                BossList.FilterType ftype = (BossList.FilterType)(CB_BL_FilterType.SelectedIndex);
                bosslist.GenCalledList(ftype, CB_BL_Filter.Text);
                // Now edit the Boss List to the new filtered list of bosses
                if (CB_BossList.Items.Count > 0) { CB_BossList.Items.Clear(); }
                CB_BossList.Items.Add("Custom");
                CB_BossList.Items.AddRange(bosslist.GetBetterBossNamesAsArray());
                CB_BossList.Text = "Custom";
                // Save the new names
                if (!firstload) {
                    calcOpts.FilterType = CB_BL_FilterType.Text;
                    calcOpts.Filter = CB_BL_Filter.Text;
                    calcOpts.BossName = CB_BossList.Text;
                }
                //
                Character.OnCalculationsInvalidated();
                isLoadingBoss = false;
            }
        }
        private void CB_BossList_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                if (!isLoading && !isLoadingBoss) {
                    CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                    CalculationsDPSWarr calcs = new CalculationsDPSWarr();
                    if (CB_BossList.Text != "Custom") {
                        isLoadingBoss = true;
                        // Get Values
                        BossHandler boss = bosslist.GetBossFromBetterName(CB_BossList.Text);
                        calcOpts.TargetLevel = boss.Level;
                        calcOpts.TargetArmor = (int)boss.Armor;
                        calcOpts.TargetHP = boss.Health;
                        calcOpts.Duration = boss.BerserkTimer;
                        calcOpts.InBack = ((calcOpts.InBackPerc = (int)(boss.InBackPerc_Melee * 100f)) != 0);
                        //calcOpts.MultipleTargets = ((calcOpts.MultipleTargetsPerc = (int)(boss.MultiTargsPerc * 100f)) > 0);
                        //calcOpts.MultipleTargetsMax = Math.Min((float)CB_MultiTargsMax.Maximum, (float)boss.MaxNumTargets);
                        calcOpts.StunningTargets = boss.Stuns.Count > 0;
                        calcOpts.MovingTargets = boss.Moves.Count > 0;
                        calcOpts.FearingTargets = boss.Fears.Count > 0;
                        calcOpts.RootingTargets = boss.Roots.Count > 0;
                        calcOpts.DisarmingTargets = boss.Disarms.Count > 0;
                        calcOpts.AoETargets = ((calcOpts.AoETargetsFreq = (int)(boss.AoETargsFreq)) <= calcOpts.Duration * 0.99f && calcOpts.AoETargetsFreq != 0f);
                        calcOpts.AoETargetsDMG = boss.AoETargsDMG;

                        // Set Controls to those Values
                        CB_TargLvl.Text = calcOpts.TargetLevel.ToString();
                        CB_TargArmor.Text = calcOpts.TargetArmor.ToString();
                        CB_Duration.Value = (int)calcOpts.Duration;
                        NUD_TargHP.Value = (int)calcOpts.TargetHP;

                        CK_InBack.Checked = calcOpts.InBack;
                        CB_InBackPerc.Value = calcOpts.InBackPerc;

                        CK_MultiTargs.Checked = calcOpts.MultipleTargets;
                        LB_Max.Enabled = calcOpts.MultipleTargets;
                        //LB_MultiTargsPerc.Enabled = calcOpts.MultipleTargets;
                        CB_MultiTargsPerc.Enabled = calcOpts.MultipleTargets;
                        CB_MultiTargsMax.Enabled = calcOpts.MultipleTargets;
                        CB_MultiTargsPerc.Value = calcOpts.MultipleTargetsPerc;
                        CB_MultiTargsMax.Value = Math.Max(CB_MultiTargsMax.Minimum, Math.Min(CB_MultiTargsMax.Maximum, (int)calcOpts.MultipleTargetsMax));

                        CK_StunningTargs.Checked = calcOpts.StunningTargets;
                        BT_Stun.Enabled = calcOpts.StunningTargets;
                        BT_Stun.Text = boss.Stuns.Count == 0 ? "None" : boss.DynamicCompiler_Stun.ToString();
                        calcOpts.Stuns = boss.Stuns;

                        CK_MovingTargs.Checked = calcOpts.MovingTargets;
                        BT_Move.Enabled = calcOpts.MovingTargets;
                        BT_Move.Text = boss.Moves.Count == 0 ? "None" : boss.DynamicCompiler_Move.ToString();
                        calcOpts.Moves = boss.Moves;

                        CK_FearingTargs.Checked = calcOpts.FearingTargets;
                        BT_Fear.Enabled = calcOpts.FearingTargets;
                        BT_Fear.Text = boss.Fears.Count == 0 ? "None" : boss.DynamicCompiler_Fear.ToString();
                        calcOpts.Fears = boss.Fears;

                        CK_RootingTargs.Checked = calcOpts.RootingTargets;
                        BT_Root.Enabled = calcOpts.RootingTargets;
                        BT_Root.Text = boss.Roots.Count == 0 ? "None" : boss.DynamicCompiler_Root.ToString();
                        calcOpts.Roots = boss.Roots;

                        CK_DisarmTargs.Checked = calcOpts.DisarmingTargets;
                        BT_Disarm.Enabled = calcOpts.DisarmingTargets;
                        BT_Disarm.Text = boss.Disarms.Count == 0 ? "None" : boss.DynamicCompiler_Disarm.ToString();
                        calcOpts.Disarms = boss.Disarms;

                        CK_AoETargs.Checked = calcOpts.AoETargets;
                        NUD_AoEFreq.Enabled = calcOpts.AoETargets;
                        NUD_AoEDMG.Enabled = calcOpts.AoETargets;
                        NUD_AoEFreq.Value = (int)Math.Max(NUD_AoEFreq.Minimum, Math.Min(NUD_AoEFreq.Maximum, calcOpts.AoETargetsFreq));
                        NUD_AoEDMG.Value = (int)Math.Max(NUD_AoEDMG.Minimum, Math.Min(NUD_AoEDMG.Maximum, (decimal)calcOpts.AoETargetsDMG));

                        Stats stats = calcs.GetCharacterStats(Character, null);
                        TB_BossInfo.Text = boss.GenInfoString(
                            0, // The Boss' Damage bonuses against you (meaning YOU are debuffed)
                            StatConversion.GetArmorDamageReduction(calcOpts.TargetLevel, stats.Armor,0,0,0), // Your Armor's resulting Damage Reduction
                            StatConversion.GetDRAvoidanceChance(Character, stats, HitResult.Miss , calcOpts.TargetLevel), // Your chance for Boss to Miss you
                            StatConversion.GetDRAvoidanceChance(Character, stats, HitResult.Dodge, calcOpts.TargetLevel), // Your chance Dodge
                            StatConversion.GetDRAvoidanceChance(Character, stats, HitResult.Parry, calcOpts.TargetLevel), // Your chance Parry
                            0,  // Your Chance to Block
                            0); // How much you Block when you Block
                        // Save the new names
                        if (!firstload) {
                            calcOpts.FilterType = CB_BL_FilterType.Text;
                            calcOpts.Filter = CB_BL_Filter.Text;
                            calcOpts.BossName = CB_BossList.Text;
                        }
                        isLoadingBoss = false;
                        CustomBoss = boss.Clone();
                    } else {
                        isLoadingBoss = true;
                        BossHandler boss = CustomBoss != null ? CustomBoss : new BossHandler();
                        //
                        boss.Name               = "Custom";
                        boss.Health             = (float)NUD_TargHP.Value;
                        boss.Level              = int.Parse(CB_TargLvl.Text);
                        boss.Armor              = int.Parse(CB_TargArmor.Text == "" ? "10643" : CB_TargArmor.Text);
                        boss.BerserkTimer       = (int)CB_Duration.Value;
                        boss.InBackPerc_Melee   = ((float)CB_InBackPerc.Value / 100f);
                        //boss.MaxNumTargets      = (float)CB_MultiTargsMax.Value;
                        //boss.MultiTargsPerc     = ((float)CB_MultiTargsPerc.Value / 100f);
                        calcOpts.Stuns = boss.Stuns;
                        calcOpts.Moves = boss.Moves;
                        calcOpts.Fears = boss.Fears;
                        calcOpts.Roots = boss.Roots;
                        calcOpts.Disarms = boss.Disarms;
                        calcOpts.BossName = boss.Name;
                        //
                        TB_BossInfo.Text = boss.GenInfoString();
                        isLoadingBoss = false;
                        CustomBoss = boss;
                    }
                    Character.OnCalculationsInvalidated();
                }
                isLoadingBoss = false;
            } catch (Exception ex) {
                new ErrorBox("Error in setting DPSWarr Character settings from Boss",
                    ex.Message,"CB_BossList_SelectedIndexChanged()", "No Additional Info", ex.StackTrace);
            }
        }
        // Basics
        private void CK_HideBadItems_Def_CheckedChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                calcOpts.HideBadItems_Def = CK_HideDefGear.Checked;
                CalculationsDPSWarr.HidingBadStuff_Def = calcOpts.HideBadItems_Def;
                ItemCache.OnItemsChanged();
                Character.OnCalculationsInvalidated();
            }
        }
        private void CK_HideBadItems_Spl_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            calcOpts.HideBadItems_Spl = CK_HideSplGear.Checked;
            CalculationsDPSWarr.HidingBadStuff_Spl = calcOpts.HideBadItems_Spl;
            ItemCache.OnItemsChanged();
            Character.OnCalculationsInvalidated();
        }
        private void CK_HideBadItems_PvP_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            calcOpts.HideBadItems_PvP = CK_HidePvPGear.Checked;
            CalculationsDPSWarr.HidingBadStuff_PvP = calcOpts.HideBadItems_PvP;
            ItemCache.OnItemsChanged();
            Character.OnCalculationsInvalidated();
        }
        private void CK_PTRMode_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            calcOpts.PTRMode = CK_PTRMode.Checked;
            Character.OnCalculationsInvalidated();
        }
        private void RB_StanceFury_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            CB_BossList.Text = "Custom";
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            //calcOpts.FuryStance = RB_StanceFury.Checked;
            CTL_Maints.Nodes[3].Nodes[0].Checked = true;
            CTL_Maints.Nodes[3].Nodes[1].Checked = true;
            Character.OnCalculationsInvalidated();
        }
        // Boss
        private void CB_ArmorBosses_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CB_BossList.Text = "Custom";
            int targetArmor = int.Parse(CB_TargArmor.Text);

            if (Character != null && Character.CalculationOptions != null) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                calcOpts.TargetArmor = targetArmor;
                Character.OnCalculationsInvalidated();
            }
        }
        private void CB_TargetLevel_SelectedIndexChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CB_BossList.Text = "Custom";
                if (Character != null && Character.CalculationOptions != null) {
                    CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                    calcOpts.TargetLevel = int.Parse(CB_TargLvl.Text);
                    Character.OnCalculationsInvalidated();
                }
            }
        }
        private void NUD_TargHP_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CB_BossList.Text = "Custom";
                if (Character != null && Character.CalculationOptions != null) {
                    CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                    calcOpts.TargetHP = (float)NUD_TargHP.Value;
                    CB_BossList_SelectedIndexChanged(null, null);
                    Character.OnCalculationsInvalidated();
                }
            }
        }
        private void CB_Duration_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CB_BossList.Text = "Custom";
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                calcOpts.Duration = (float)CB_Duration.Value;
                //NUD_MoveFreq.Maximum = CB_Duration.Value;
                Character.OnCalculationsInvalidated();
            }
        }
        // Rotational Changes
        private void NUD_Under20Perc_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.Under20Perc = (float)NUD_Under20Perc.Value / 100f;
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_InBack_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.InBack = CK_InBack.Checked;
                CB_InBackPerc.Enabled = calcOpts.InBack;
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Multi_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.MultipleTargets = CK_MultiTargs.Checked;
                CB_MultiTargsMax.Enabled = calcOpts.MultipleTargets;
                CB_MultiTargsPerc.Enabled = calcOpts.MultipleTargets;
                LB_Max.Enabled = calcOpts.MultipleTargets;
                //LB_MultiTargsPerc.Enabled = calcOpts.MultipleTargets;
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Stun_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                bool Checked             = CK_StunningTargs.Checked;
                calcOpts.StunningTargets = Checked;
                BT_Stun.Enabled          = Checked;
                BT_Stun.Text = CustomBoss.Stuns.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Stun.ToString();
                CB_BossList.Text         = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Move_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                bool Checked            = CK_MovingTargs.Checked;
                calcOpts.MovingTargets  = Checked;
                BT_Move.Enabled         = Checked;
                BT_Move.Text = CustomBoss.Moves.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Move.ToString();
                CB_BossList.Text        = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Disarm_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                bool Checked              = CK_DisarmTargs.Checked;
                calcOpts.DisarmingTargets = Checked;
                BT_Disarm.Enabled         = Checked;
                BT_Disarm.Text = CustomBoss.Disarms.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Disarm.ToString();
                CB_BossList.Text          = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Fear_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                bool Checked            = CK_FearingTargs.Checked;
                calcOpts.FearingTargets = Checked;
                BT_Fear.Enabled         = Checked;
                BT_Fear.Text = CustomBoss.Fears.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Fear.ToString();
                CB_BossList.Text        = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Root_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                bool Checked             = CK_RootingTargs.Checked;
                calcOpts.RootingTargets  = Checked;
                BT_Root.Enabled          = Checked;
                BT_Root.Text = CustomBoss.Roots.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Root.ToString();
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_AoE_ChecksChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                bool Checked        = CK_AoETargs.Checked;
                calcOpts.AoETargets = Checked;
                NUD_AoEFreq.Enabled = Checked;
                NUD_AoEDMG.Enabled  = Checked;
                //CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_InBack_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.InBackPerc = (int)CB_InBackPerc.Value;
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_Multi_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.MultipleTargetsPerc = (int)CB_MultiTargsPerc.Value;
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void RotChanges_MultiMax_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.MultipleTargetsMax = (float)CB_MultiTargsMax.Value;
                CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void NUD_AoEFreq_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.AoETargetsFreq = (int)NUD_AoEFreq.Value;
                //CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void NUD_AoEDmg_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.AoETargetsDMG = (float)NUD_AoEDMG.Value;
                //CB_BossList.Text = "Custom";
                //
                Character.OnCalculationsInvalidated();
            }
        }
        // Customize Impedence
        private void BT_Stun_Click(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            DG_BossSitChanges Box = new DG_BossSitChanges(CustomBoss.Stuns, DG_BossSitChanges.Flags.Stun);
            Box.ShowDialog(this);
            if (Box.DialogResult != DialogResult.Cancel) {
                CustomBoss.Stuns = Box.TheList;
                calcOpts.Stuns = CustomBoss.Stuns;
                BT_Stun.Text = CustomBoss.Stuns.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Stun.ToString();
                CB_BossList_SelectedIndexChanged(null, null);
            }
        }
        private void BT_Move_Click(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            DG_BossSitChanges Box = new DG_BossSitChanges(CustomBoss.Moves, DG_BossSitChanges.Flags.Move);
            Box.ShowDialog(this);
            if (Box.DialogResult != DialogResult.Cancel) {
                CustomBoss.Moves = Box.TheList;
                calcOpts.Moves = CustomBoss.Moves;
                BT_Move.Text = CustomBoss.Moves.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Move.ToString();
                CB_BossList_SelectedIndexChanged(null, null);
            }
        }
        private void BT_Fear_Click(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            DG_BossSitChanges Box = new DG_BossSitChanges(CustomBoss.Fears, DG_BossSitChanges.Flags.Fear);
            Box.ShowDialog(this);
            if (Box.DialogResult != DialogResult.Cancel) {
                CustomBoss.Fears = Box.TheList;
                calcOpts.Fears = CustomBoss.Fears;
                BT_Fear.Text = CustomBoss.Fears.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Fear.ToString();
                CB_BossList_SelectedIndexChanged(null, null);
            }
        }
        private void BT_Root_Click(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            DG_BossSitChanges Box = new DG_BossSitChanges(CustomBoss.Roots, DG_BossSitChanges.Flags.Root);
            Box.ShowDialog(this);
            if (Box.DialogResult != DialogResult.Cancel) {
                CustomBoss.Roots = Box.TheList;
                calcOpts.Roots = CustomBoss.Roots;
                BT_Root.Text = CustomBoss.Roots.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Root.ToString();
                CB_BossList_SelectedIndexChanged(null, null);
            }
        }
        private void BT_Disarm_Click(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            DG_BossSitChanges Box = new DG_BossSitChanges(CustomBoss.Disarms, DG_BossSitChanges.Flags.Disarm);
            Box.ShowDialog(this);
            if (Box.DialogResult != DialogResult.Cancel) {
                CustomBoss.Disarms = Box.TheList;
                calcOpts.Disarms = CustomBoss.Disarms;
                BT_Disarm.Text = CustomBoss.Disarms.Count == 0 ? "None" : CustomBoss.DynamicCompiler_Disarm.ToString();
                CB_BossList_SelectedIndexChanged(null, null);
            }
        }
        // Abilities to Maintain Changes
        private void CK_Flooring_CheckedChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                //
                calcOpts.AllowFlooring = CK_Flooring.Checked;
                //
                Character.OnCalculationsInvalidated();
            }
        }
        private void CTL_Maints_AfterCheck(object sender, TreeViewEventArgs e) {
            if (!isLoading) {
                CTL_Maints.AfterCheck -= new TreeViewEventHandler(CTL_Maints_AfterCheck);
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                // Work special changes for the tree
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode n in e.Node.Nodes)
                    {
                        n.Checked = e.Node.Checked;
                    }
                }
                TreeNode p = e.Node.Parent;
                while (p != null)
                {
                    bool hasChecked = false;
                    foreach (TreeNode c in p.Nodes)
                    {
                        if (c.Checked)
                        {
                            hasChecked = true;
                            break;
                        }
                    }
                    if (hasChecked) p.Checked = true;
                    p = p.Parent;
                }
                // Assign the new values to the program
                setAbilBools(calcOpts);
                // Run a new dps calc
                Character.OnCalculationsInvalidated();
                CTL_Maints.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(CTL_Maints_AfterCheck);
            }
        }
        public static void CheckSize(CalculationOptionsDPSWarr calcOpts)
        {
            if (calcOpts.Maintenance.Length != (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.HeroicStrike_ + 1)
            {
                bool[] newArray = new bool[] {
                        true,  // == Rage Gen ==
                            false,  // Berserker Rage
                            true,   // Bloodrage
                        false, // == Maintenance ==
                            false, // Shout Choice
                                false, // Battle Shout
                                false, // Commanding Shout
                            false, // Demoralizing Shout
                            false, // Sunder Armor
                            false, // Thunder Clap
                            false, // Hamstring
                        true,  // == Periodics ==
                            true,  // Shattering Throw
                            true,  // Sweeping Strikes
                            true,  // DeathWish
                            true,  // Recklessness
                            false,  // Enraged Regeneration
                        true,  // == Damage Dealers ==
                            true,  // Fury
                                true,  // Whirlwind
                                true,  // Bloodthirst
                                true,  // Bloodsurge
                            true,  // Arms
                                true,  // Bladestorm
                                true,  // Mortal Strike
                                true,  // Rend
                                true,  // Overpower
                                true,  // Taste for Blood
                                true,  // Sudden Death
                                true,  // Slam
                            true,  // <20% Execute Spamming
                        true,  // == Rage Dumps ==
                            true,  // Cleave
                            true   // Heroic Strike
                    };
                calcOpts.Maintenance = newArray;
            }
        }
        private void setAbilBools(CalculationOptionsDPSWarr calcOpts) {
            //CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            CalculationOptionsPanelDPSWarr.CheckSize(calcOpts);

            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._RageGen__]        = CTL_Maints.Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.BerserkerRage_]    = CTL_Maints.Nodes[0].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bloodrage_]        = CTL_Maints.Nodes[0].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._Maintenance__]    = CTL_Maints.Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ShoutChoice_]      = CTL_Maints.Nodes[1].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.BattleShout_]      = CTL_Maints.Nodes[1].Nodes[0].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.CommandingShout_]  = CTL_Maints.Nodes[1].Nodes[0].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.DemoralizingShout_]= CTL_Maints.Nodes[1].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SunderArmor_]      = CTL_Maints.Nodes[1].Nodes[2].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ThunderClap_]      = CTL_Maints.Nodes[1].Nodes[3].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Hamstring_]        = CTL_Maints.Nodes[1].Nodes[4].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._Periodics__]      = CTL_Maints.Nodes[2].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ShatteringThrow_]  = CTL_Maints.Nodes[2].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SweepingStrikes_]  = CTL_Maints.Nodes[2].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.DeathWish_]        = CTL_Maints.Nodes[2].Nodes[2].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Recklessness_]     = CTL_Maints.Nodes[2].Nodes[3].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.EnragedRegeneration_]=CTL_Maints.Nodes[2].Nodes[4].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._DamageDealers__]  = CTL_Maints.Nodes[3].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Fury_]             = CTL_Maints.Nodes[3].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Whirlwind_]        = CTL_Maints.Nodes[3].Nodes[0].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bloodthirst_]      = CTL_Maints.Nodes[3].Nodes[0].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bloodsurge_]       = CTL_Maints.Nodes[3].Nodes[0].Nodes[2].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Arms_]             = CTL_Maints.Nodes[3].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bladestorm_]       = CTL_Maints.Nodes[3].Nodes[1].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.MortalStrike_]     = CTL_Maints.Nodes[3].Nodes[1].Nodes[1].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Rend_]             = CTL_Maints.Nodes[3].Nodes[1].Nodes[2].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Overpower_]        = CTL_Maints.Nodes[3].Nodes[1].Nodes[3].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.TasteForBlood_]    = CTL_Maints.Nodes[3].Nodes[1].Nodes[4].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SuddenDeath_]      = CTL_Maints.Nodes[3].Nodes[1].Nodes[5].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Slam_]             = CTL_Maints.Nodes[3].Nodes[1].Nodes[6].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ExecuteSpam_]      = CTL_Maints.Nodes[3].Nodes[2].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._RageDumps__]      = CTL_Maints.Nodes[4].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Cleave_]           = CTL_Maints.Nodes[4].Nodes[0].Checked;
            calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.HeroicStrike_]     = CTL_Maints.Nodes[4].Nodes[1].Checked;
        }
        private void LoadAbilBools(CalculationOptionsDPSWarr calcOpts) {
            try {
                CTL_Maints.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(CTL_Maints_AfterCheck);
                // Bounds Check
                CalculationOptionsPanelDPSWarr.CheckSize(calcOpts);
                //
                CTL_Maints.Nodes[0].Checked                     = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._RageGen__];
                CTL_Maints.Nodes[0].Nodes[0].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.BerserkerRage_];
                CTL_Maints.Nodes[0].Nodes[1].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bloodrage_];
                CTL_Maints.Nodes[1].Checked                     = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._Maintenance__];
                CTL_Maints.Nodes[1].Nodes[0].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ShoutChoice_];
                CTL_Maints.Nodes[1].Nodes[0].Nodes[0].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.BattleShout_];
                CTL_Maints.Nodes[1].Nodes[0].Nodes[1].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.CommandingShout_];
                CTL_Maints.Nodes[1].Nodes[1].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.DemoralizingShout_];
                CTL_Maints.Nodes[1].Nodes[2].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SunderArmor_];
                CTL_Maints.Nodes[1].Nodes[3].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ThunderClap_];
                CTL_Maints.Nodes[1].Nodes[4].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Hamstring_];
                CTL_Maints.Nodes[2].Checked                     = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._Periodics__];
                CTL_Maints.Nodes[2].Nodes[0].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ShatteringThrow_];
                CTL_Maints.Nodes[2].Nodes[1].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SweepingStrikes_];
                CTL_Maints.Nodes[2].Nodes[2].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.DeathWish_];
                CTL_Maints.Nodes[2].Nodes[3].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Recklessness_];
                CTL_Maints.Nodes[2].Nodes[4].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.EnragedRegeneration_];
                CTL_Maints.Nodes[3].Checked                     = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._DamageDealers__];
                CTL_Maints.Nodes[3].Nodes[0].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Fury_];
                CTL_Maints.Nodes[3].Nodes[0].Nodes[0].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Whirlwind_];
                CTL_Maints.Nodes[3].Nodes[0].Nodes[1].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bloodthirst_];
                CTL_Maints.Nodes[3].Nodes[0].Nodes[2].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bloodsurge_];
                CTL_Maints.Nodes[3].Nodes[1].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Arms_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[0].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Bladestorm_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[1].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.MortalStrike_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[2].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Rend_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[3].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Overpower_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[4].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.TasteForBlood_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[5].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SuddenDeath_];
                CTL_Maints.Nodes[3].Nodes[1].Nodes[6].Checked   = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Slam_];
                CTL_Maints.Nodes[3].Nodes[2].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ExecuteSpam_];
                CTL_Maints.Nodes[4].Checked                     = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances._RageDumps__];
                CTL_Maints.Nodes[4].Nodes[0].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Cleave_];
                CTL_Maints.Nodes[4].Nodes[1].Checked            = calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.HeroicStrike_];
                //
                this.CTL_Maints.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.CTL_Maints_AfterCheck);
            } catch (Exception ex) {
                new ErrorBox("Error in loading the DPSWarr Ability Bool Set",
                    ex.Message, "LoadAbilBools()", "No Additional Info", ex.StackTrace);
            }
        }
        // Latency
        private void CB_Latency_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                calcOpts.Lag = (int)CB_Lag.Value;
                calcOpts.React = (int)CB_React.Value;
                Character.OnCalculationsInvalidated();
            }
        }
        // Survival
        private void NUD_SurvScale_ValueChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                calcOpts.SurvScale = (float)NUD_SurvScale.Value;
                Character.OnCalculationsInvalidated();
            }
        }
        // Special Effects Modifier
        private void CK_SE_UseDur_CheckedChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                calcOpts.SE_UseDur = CK_SE_UseDur.Checked;
                Character.OnCalculationsInvalidated();
            }
        }
        // Markov
        private void CK_Markov_CheckedChanged(object sender, EventArgs e) {
            if (!isLoading) {
                CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
                bool Checked = CK_Markov.Checked;
                calcOpts.UseMarkov = Checked;
                Character.OnCalculationsInvalidated();
            }
        }
        // Stat Graph
        private Stats[] BuildStatsList() {
            List<Stats> statsList = new List<Stats>();
            if (CK_StatsStrength.Checked) { statsList.Add(new Stats() { Strength = 1f }); }
            if (CK_StatsAgility.Checked ) { statsList.Add(new Stats() { Agility = 1f }); }
            if (CK_StatsAP.Checked      ) { statsList.Add(new Stats() { AttackPower = 2f }); }
            if (CK_StatsCrit.Checked    ) { statsList.Add(new Stats() { CritRating = 1f }); }
            if (CK_StatsHit.Checked     ) { statsList.Add(new Stats() { HitRating = 1f }); }
            if (CK_StatsExp.Checked     ) { statsList.Add(new Stats() { ExpertiseRating = 1f }); }
            if (CK_StatsHaste.Checked   ) { statsList.Add(new Stats() { HasteRating = 1f }); }
            if (CK_StatsArP.Checked     ) { statsList.Add(new Stats() { ArmorPenetrationRating = 1f }); }
            return statsList.ToArray();
        }
        private void BT_StatsGraph_Click(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            Stats[] statsList = BuildStatsList();
            Graph graph = new Graph();
            string explanatoryText = "This graph shows how adding or subtracting\nmultiples of a stat affects your dps.\n\nAt the Zero position is your current dps.\n" +
                         "To the right of the zero vertical is adding stats.\nTo the left of the zero vertical is subtracting stats.\n" +
                         "The vertical axis shows the amount of dps added or lost";
            graph.SetupStatsGraph(Character, statsList, calcOpts.StatsIncrement, explanatoryText, calcOpts.CalculationToGraph);
            graph.Show();
        }
        private void CK_StatsStrength_CheckedChanged(object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[0] = CK_StatsStrength.Checked; }
        private void CK_StatsAgility_CheckedChanged( object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[1] = CK_StatsAgility.Checked; }
        private void CK_StatsAP_CheckedChanged(      object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[2] = CK_StatsAP.Checked; }
        private void CK_StatsCrit_CheckedChanged(    object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[3] = CK_StatsCrit.Checked; }
        private void CK_StatsHit_CheckedChanged(     object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[4] = CK_StatsHit.Checked; }
        private void CK_StatsExp_CheckedChanged(     object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[5] = CK_StatsExp.Checked; }
        private void CK_StatsHaste_CheckedChanged(   object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[6] = CK_StatsHaste.Checked; }
        private void CK_StatsArP_CheckedChanged(     object sender, EventArgs e) { CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr; calcOpts.StatsList[7] = CK_StatsArP.Checked; }
        private void CB_CalculationToGraph_SelectedIndexChanged(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            calcOpts.CalculationToGraph = (string)CB_CalculationToGraph.SelectedItem;
        }
        private void NUD_StatsIncrement_ValueChanged(object sender, EventArgs e) {
            CalculationOptionsDPSWarr calcOpts = Character.CalculationOptions as CalculationOptionsDPSWarr;
            calcOpts.StatsIncrement = (int)NUD_StatsIncrement.Value;
        }
    }
}
