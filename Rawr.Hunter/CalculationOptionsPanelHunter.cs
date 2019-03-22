using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Rawr.Base;

namespace Rawr.Hunter
{
    public partial class CalculationOptionsPanelHunter : CalculationOptionsPanelBase
    {
        #region Instance Variables
        private bool isLoading = false;
        private CalculationOptionsHunter CalcOpts = null;
        private BossOptions BossOpts = null;
        private PetAttacks[] familyList = null;
        private List<ComboBox> ShotPriorityBoxes = new List<ComboBox>();
        #endregion

        #region Constructors
        public CalculationOptionsPanelHunter()
        {
            isLoading = true;
            LoadPetTalentSpecs();
            InitializeComponent();

            CB_Duration.Minimum = 0;
            CB_Duration.Maximum = 60 * 20; // 20 minutes

            #region Pet Talents
            // Cunning
            initTalentValues(CB_CunningCobraReflexes, 2);
            initTalentValues(CB_CunningDiveDash, 1);
            initTalentValues(CB_CunningGreatStamina, 3);
            initTalentValues(CB_CunningNaturalArmor, 2);
            initTalentValues(CB_CunningBoarsSpeed, 1);
            initTalentValues(CB_CunningMobility, 2);
            initTalentValues(CB_CunningSpikedCollar, 3);
            initTalentValues(CB_CunningCullingTheHerd, 3); // Add in 3.3
            initTalentValues(CB_CunningLionhearted, 2);
            initTalentValues(CB_CunningCarrionFeeder, 1);
            initTalentValues(CB_CunningGreatResistance, 3);
            initTalentValues(CB_CunningOwlsFocus, 2);
            initTalentValues(CB_CunningCornered, 2);
            initTalentValues(CB_CunningFeedingFrenzy, 2);
            initTalentValues(CB_CunningWolverineBite, 1);
            initTalentValues(CB_CunningRoarOfRecovery, 1);
            initTalentValues(CB_CunningBullheaded, 1);
            initTalentValues(CB_CunningGraceOfTheMantis, 2);
            initTalentValues(CB_CunningWildHunt, 2);
            initTalentValues(CB_CunningRoarOfSacrifice, 1);
            // Ferocity
            initTalentValues(CB_FerocityCullingTheHerd, 3); // Add in 3.3
            initTalentValues(CB_FerocityBloodthirsty, 2);
            initTalentValues(CB_FerocityBoarsSpeed, 1);
            initTalentValues(CB_FerocityCallOfTheWild, 1);
            initTalentValues(CB_FerocityChargeSwoop, 1);
            initTalentValues(CB_FerocityCobraReflexes, 2);
            initTalentValues(CB_FerocityDiveDash, 1);
            initTalentValues(CB_FerocityGreatResistance, 3);
            initTalentValues(CB_FerocityGreatStamina, 3);
            initTalentValues(CB_FerocityHeartOfThePhoenix, 1);
            initTalentValues(CB_FerocityImprovedCower, 2);
            initTalentValues(CB_FerocityLickYourWounds, 1);
            initTalentValues(CB_FerocityLionhearted, 2);
            initTalentValues(CB_FerocityNaturalArmor, 2);
            initTalentValues(CB_FerocityRabid, 1);
            initTalentValues(CB_FerocitySharkAttack, 2);
            initTalentValues(CB_FerocitySpidersBite, 3);
            initTalentValues(CB_FerocitySpikedCollar, 3);
            initTalentValues(CB_FerocityWildHunt, 2);
            // Tenacity
            initTalentValues(CB_TenacityCullingTheHerd, 3); // Add in 3.3
            initTalentValues(CB_TenacityBloodOfTheRhino, 2);
            initTalentValues(CB_TenacityBoarsSpeed, 1);
            initTalentValues(CB_TenacityCharge, 1);
            initTalentValues(CB_TenacityCobraReflexes, 2);
            initTalentValues(CB_TenacityGraceOfTheMantis, 2);
            initTalentValues(CB_TenacityGreatResistance, 3);
            initTalentValues(CB_TenacityGreatStamina, 3);
            initTalentValues(CB_TenacityGuardDog, 2);
            initTalentValues(CB_TenacityIntervene, 1);
            initTalentValues(CB_TenacityLastStand, 1);
            initTalentValues(CB_TenacityLionhearted, 2);
            initTalentValues(CB_TenacityNaturalArmor, 2);
            initTalentValues(CB_TenacityPetBarding, 2);
            initTalentValues(CB_TenacityRoarOfSacrifice, 1);
            initTalentValues(CB_TenacitySilverback, 2);
            initTalentValues(CB_TenacitySpikedCollar, 3);
            initTalentValues(CB_TenacityTaunt, 1);
            initTalentValues(CB_TenacityThunderstomp, 1);
            initTalentValues(CB_TenacityWildHunt, 2);
            #endregion

            PetBuffs.character = Character;

            // This should now be the only group of 10 lines, the rest are loop-capable
            ShotPriorityBoxes.Add(CB_ShotPriority_01);
            ShotPriorityBoxes.Add(CB_ShotPriority_02);
            ShotPriorityBoxes.Add(CB_ShotPriority_03);
            ShotPriorityBoxes.Add(CB_ShotPriority_04);
            ShotPriorityBoxes.Add(CB_ShotPriority_05);
            ShotPriorityBoxes.Add(CB_ShotPriority_06);
            ShotPriorityBoxes.Add(CB_ShotPriority_07);
            ShotPriorityBoxes.Add(CB_ShotPriority_08);
            ShotPriorityBoxes.Add(CB_ShotPriority_09);
            ShotPriorityBoxes.Add(CB_ShotPriority_10);

            foreach (ComboBox cb in ShotPriorityBoxes) { InitializeShotList(cb); }

            CB_CalculationToGraph.Items.AddRange(Graph.GetCalculationNames());

            UpdateSavedTalents();
            SavePetTalentSpecs();

            //initTalentImages();
            isLoading = false;
        }
        protected override void LoadCalculationOptions() {
            try {
                isLoading = true;
                if (Character != null && Character.CalculationOptions == null) {
                    // If it's broke, make a new one with the defaults
                    Character.CalculationOptions = new CalculationOptionsHunter();
                    isLoading = true;
                }
                CalcOpts = Character.CalculationOptions as CalculationOptionsHunter;

                Character.TalentChangedEvent += new System.EventHandler(this.CharTalents_Changed);

                for (int i = 0; i < CB_TargetLevel.Items.Count; i++) {
                    if (CB_TargetLevel.Items[i] as string == CalcOpts.TargetLevel.ToString()) {
                        CB_TargetLevel.SelectedItem = CB_TargetLevel.Items[i];
                        break;
                    }
                }
                for (int i = 0; i < CB_TargArmor.Items.Count; i++) {
                    if (CB_TargArmor.Items[i] as string == CalcOpts.TargetArmor.ToString()) {
                        CB_TargArmor.SelectedItem = CB_TargArmor.Items[i];
                        break;
                    }
                }
                isLoading = true;

                CB_ArmoryPets.Items.Clear();
                if (Character.ArmoryPets != null && Character.ArmoryPets.Count > 0) {
                    CB_ArmoryPets.Items.AddRange(Character.ArmoryPets.ToArray());
                    isLoading = false;
                    CB_ArmoryPets.SelectedIndex = CalcOpts.SelectedArmoryPet;
                    isLoading = true;
                }

                // Hiding Gear based on Bad Stats
                CK_HideSplGear.Checked = CalcOpts.HideBadItems_Spl; CalculationsHunter.HidingBadStuff_Spl = CalcOpts.HideBadItems_Spl;
                CK_HidePvPGear.Checked = CalcOpts.HideBadItems_PvP; CalculationsHunter.HidingBadStuff_PvP = CalcOpts.HideBadItems_PvP;

                NUD_SurvScale.Value = (decimal)CalcOpts.SurvScale;

                CK_PTRMode.Checked = CalcOpts.PTRMode;

                PetBuffs.character = Character;
                //PetBuffs.LoadBuffsFromOptions();

                NUD_Latency.Value = (decimal)(CalcOpts.Latency * 1000.0);

                // Special Effects Special Option
                CK_SE_UseDur.Checked = CalcOpts.SE_UseDur;

                CK_MultipleTargets.Checked = CalcOpts.MultipleTargets;
                NUD_MultiTargsUptime.Enabled = CalcOpts.MultipleTargets;
                NUD_MultiTargsUptime.Value = (int)(CalcOpts.MultipleTargetsPerc * 100f);
                isLoading = true;
                CB_PetFamily.Items.Clear();
                foreach (PetFamily f in Enum.GetValues(typeof(PetFamily))) CB_PetFamily.Items.Add(f);
                CB_PetFamily.SelectedItem = CalcOpts.PetFamily;
                isLoading = true;
                CB_Duration.Value = CalcOpts.Duration;
                NUD_Time20.Value = CalcOpts.TimeSpentSub20;
                NUD_35.Value = CalcOpts.TimeSpent35To20;
                NUD_BossHP.Value = (decimal)Math.Round(100 * CalcOpts.BossHPPerc);

                NUD_CDCutOff.Value = CalcOpts.CDCutoff;

                NUD_Time20.Maximum = CB_Duration.Value;
                NUD_35.Maximum = CB_Duration.Value;
                isLoading = true;
                if (CalcOpts.SelectedAspect == Aspect.None) CB_Aspect.SelectedIndex = 0;
                if (CalcOpts.SelectedAspect == Aspect.Beast) CB_Aspect.SelectedIndex = 1;
                if (CalcOpts.SelectedAspect == Aspect.Hawk) CB_Aspect.SelectedIndex = 2;
                if (CalcOpts.SelectedAspect == Aspect.Viper) CB_Aspect.SelectedIndex = 3;
                if (CalcOpts.SelectedAspect == Aspect.Monkey) CB_Aspect.SelectedIndex = 4;
                if (CalcOpts.SelectedAspect == Aspect.Dragonhawk) CB_Aspect.SelectedIndex = 5;

                if (CalcOpts.AspectUsage == AspectUsage.None) CB_AspectUsage.SelectedIndex = 0;
                if (CalcOpts.AspectUsage == AspectUsage.ViperToOOM) CB_AspectUsage.SelectedIndex = 1;
                if (CalcOpts.AspectUsage == AspectUsage.ViperRegen) CB_AspectUsage.SelectedIndex = 2;
                isLoading = true;
                CK_UseBeastDuringBW.Checked = CalcOpts.UseBeastDuringBestialWrath;
                CK_UseRotation.Checked = CalcOpts.UseRotationTest;
                CK_RandomProcs.Enabled = CK_UseRotation.Checked;
                CK_RandomProcs.Checked = CalcOpts.RandomizeProcs;
                isLoading = true;
                PopulateAbilities();
                isLoading = true;
                CB_PetPrio_01.SelectedItem = CalcOpts.PetPriority1;
                CB_PetPrio_02.SelectedItem = CalcOpts.PetPriority2;
                CB_PetPrio_03.SelectedItem = CalcOpts.PetPriority3;
                CB_PetPrio_04.SelectedItem = CalcOpts.PetPriority4;
                CB_PetPrio_05.SelectedItem = CalcOpts.PetPriority5;
                CB_PetPrio_06.SelectedItem = CalcOpts.PetPriority6;
                CB_PetPrio_07.SelectedItem = CalcOpts.PetPriority7;
                isLoading = true;
                CalcOpts.PetTalents = new PetTalentTreeData(CalcOpts.petTalents);
                populatePetTalentCombos();
                isLoading = true;
                // set up shot priorities
                int k = 0;
                foreach (ComboBox cb in ShotPriorityBoxes) { cb.SelectedIndex = CalcOpts.PriorityIndexes[k]; k++; }
                CB_PriorityDefaults.SelectedIndex = ShotRotationIndexCheck();
                if (ShotRotationFunctions.ShotRotationIsntSet(CalcOpts)) {
                    isLoading = false;
                    CB_PriorityDefaults.SelectedIndex = ShotRotationFunctions.ShotRotationGetRightSpec(Character);
                    isLoading = true;
                }
                //
                CK_StatsAgility.Checked = CalcOpts.StatsList[0];
                CK_StatsAP.Checked = CalcOpts.StatsList[1];
                CK_StatsCrit.Checked = CalcOpts.StatsList[2];
                CK_StatsHit.Checked = CalcOpts.StatsList[3];
                CK_StatsHaste.Checked = CalcOpts.StatsList[4];
                CK_StatsArP.Checked = CalcOpts.StatsList[5];
                NUD_StatsIncrement.Value = CalcOpts.StatsIncrement;
                CB_CalculationToGraph.Text = CalcOpts.CalculationToGraph;
                //
                isLoading = false;
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox(
                    "Error Loading Char File", ex.Message,
                    "LoadCalculationOptions", "No Additional Info", ex.StackTrace);
            }
        }

        private static readonly string _SavedFilePath;
        static CalculationOptionsPanelHunter() {
            _SavedFilePath = 
                Path.Combine(
                    Path.Combine(
                        Path.GetDirectoryName(Application.ExecutablePath),
                        "Data"),
                    "PetTalents.xml");
        }

        private SavedPetTalentSpecList _savedPetTalents;
        private void LoadPetTalentSpecs() {
            try {
                if (File.Exists(_SavedFilePath)) {
                    using (StreamReader reader = new StreamReader(_SavedFilePath, Encoding.UTF8)) {
                        XmlSerializer serializer = new XmlSerializer(typeof(SavedPetTalentSpecList));
                        _savedPetTalents = (SavedPetTalentSpecList)serializer.Deserialize(reader);
                        reader.Close();
                    }
                }
            } catch (Exception) { }
            if (_savedPetTalents == null) { _savedPetTalents = new SavedPetTalentSpecList(10); }
        }
        private void SavePetTalentSpecs() {
            try {
                using (StreamWriter writer = new StreamWriter(_SavedFilePath, false, Encoding.UTF8)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(SavedPetTalentSpecList));
                    serializer.Serialize(writer, _savedPetTalents);
                    writer.Close();
                }
            } catch (Exception ex) { Log.Write(ex.Message); Log.Write(ex.StackTrace); }
        }

        private PetTalentTreeData _pettalents = null;
        public PetTalentTreeData PetTalents {
            get { return _pettalents ?? (CalcOpts.PetTalents != null ? new PetTalentTreeData(CalcOpts.PetTalents.ToString()) : new PetTalentTreeData()); }
            set {
                _pettalents = value;
                CalcOpts.PetTalents = _pettalents;
                //if (_character != null) _character.CurrentTalents = value;
                UpdatePetTrees();
            }
        }

        public SavedPetTalentSpec CustomPetSpec { get; set; }
        public List<SavedPetTalentSpec> SpecsFor(PetFamilyTree petClass) {
            List<SavedPetTalentSpec> classTalents = new List<SavedPetTalentSpec>();
            foreach (SavedPetTalentSpec spec in _savedPetTalents) {
                //if (spec.Class == _character.Class) {
                    classTalents.Add(spec);
                //}
            }
            if (((SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem).Spec == null) {
                CustomPetSpec = new SavedPetTalentSpec("Custom", _pettalents, petClass, _treeCount);
                classTalents.Add(CustomPetSpec);
            }
            return classTalents;
        }

        public SavedPetTalentSpec CurrentPetSpec() {
            if (CB_PetTalentsSpecSwitcher.SelectedItem == null) return CustomPetSpec;
            else if (((SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem).Spec == null) return CustomPetSpec;
            else return (SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem;
        }

        private int _treeCount;

        private void UpdatePetTrees() { populatePetTalentCombos(); }

        private bool _updateSaved = false;
        private void UpdateSavedTalents() {
            //if (_character != null) {
                List<SavedPetTalentSpec> classTalents = new List<SavedPetTalentSpec>();
                SavedPetTalentSpec current = null;
                foreach (SavedPetTalentSpec spec in _savedPetTalents) {
                    //if (spec.Class == _character.Class) {
                        classTalents.Add(spec);
                        if (spec.Equals(_pettalents)) current = spec;
                    //}
                }
                if (current == null) {
                    current = new SavedPetTalentSpec("Custom", null, (PetFamilyTree)CB_PetFamily.SelectedIndex, _treeCount);
                    classTalents.Add(current);
                }
                _updateSaved = true;
                CB_PetTalentsSpecSwitcher.DataSource = classTalents;
                CB_PetTalentsSpecSwitcher.SelectedItem = current;
                _updateSaved = false;
            //}
        }
        #endregion

        #region Event Handlers
        private void CalculationOptionsPanelHunter_Resize(object sender, EventArgs e)
        {
            Tabs.Height = Tabs.Parent.Height - 5;
        }
        #endregion

        #region Basics
        private void CB_TargetLevel_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.TargetLevel = int.Parse(CB_TargetLevel.SelectedItem.ToString());
            Character.OnCalculationsInvalidated();
        }
        private void CB_TargArmor_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.TargetArmor = int.Parse(CB_TargArmor.SelectedItem.ToString());
            Character.OnCalculationsInvalidated();
        }
        private void NUD_Latency_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.Lag = (float)NUD_Latency.Value;
            Character.OnCalculationsInvalidated();
        }
        private void NUD_CDCutOff_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.CDCutoff = (int)NUD_CDCutOff.Value;
            Character.OnCalculationsInvalidated();
        }
        private void NUD_Duration_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.Duration = (int)CB_Duration.Value;
            NUD_Time20.Maximum = CB_Duration.Value; // don't allow these two to be 
            NUD_35.Maximum = CB_Duration.Value; // longer than than the fight!
            Character.OnCalculationsInvalidated();
        }
        private void NUD_Time20_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.TimeSpentSub20 = (int)NUD_Time20.Value;
            Character.OnCalculationsInvalidated();
        }
        private void NUD_Time35_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.TimeSpent35To20 = (int)NUD_35.Value;
            Character.OnCalculationsInvalidated();
        }
        private void NUD_BossHP_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.BossHPPerc = (float)(NUD_BossHP.Value / 100);
            Character.OnCalculationsInvalidated();
        }
        private void CB_Aspect_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoading) return;
            if (CB_Aspect.SelectedIndex == 0) CalcOpts.SelectedAspect = Aspect.None;
            if (CB_Aspect.SelectedIndex == 1) CalcOpts.SelectedAspect = Aspect.Beast;
            if (CB_Aspect.SelectedIndex == 2) CalcOpts.SelectedAspect = Aspect.Hawk;
            if (CB_Aspect.SelectedIndex == 3) CalcOpts.SelectedAspect = Aspect.Viper;
            if (CB_Aspect.SelectedIndex == 4) CalcOpts.SelectedAspect = Aspect.Monkey;
            if (CB_Aspect.SelectedIndex == 5) CalcOpts.SelectedAspect = Aspect.Dragonhawk;
            Character.OnCalculationsInvalidated();
        }
        private void CB_AspectUsage_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoading) return;
            if (CB_AspectUsage.SelectedIndex == 0) CalcOpts.AspectUsage = AspectUsage.None;
            if (CB_AspectUsage.SelectedIndex == 1) CalcOpts.AspectUsage = AspectUsage.ViperToOOM;
            if (CB_AspectUsage.SelectedIndex == 2) CalcOpts.AspectUsage = AspectUsage.ViperRegen;
            Character.OnCalculationsInvalidated();
        }
        private void CK_UseBeastDuringBW_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.UseBeastDuringBestialWrath = CK_UseBeastDuringBW.Checked;
            Character.OnCalculationsInvalidated();
        }
        private void CK_UseRotation_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.UseRotationTest = CK_UseRotation.Checked;
            CK_RandomProcs.Enabled = CK_UseRotation.Checked;
            Character.OnCalculationsInvalidated();
        }
        private void CK_RandomProcs_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.RandomizeProcs = CK_RandomProcs.Checked;
            Character.OnCalculationsInvalidated();
        }
        private void CK_HideBadItems_Spl_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.HideBadItems_Spl = CK_HideSplGear.Checked;
            CalculationsHunter.HidingBadStuff_Spl = CalcOpts.HideBadItems_Spl;
            ItemCache.OnItemsChanged();
            Character.OnCalculationsInvalidated();
        }
        private void CK_HideBadItems_PvP_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.HideBadItems_PvP = CK_HidePvPGear.Checked;
            CalculationsHunter.HidingBadStuff_PvP = CalcOpts.HideBadItems_PvP;
            ItemCache.OnItemsChanged();
            Character.OnCalculationsInvalidated();
        }
        private void CK_PTRMode_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.PTRMode = CK_PTRMode.Checked;
            Character.OnCalculationsInvalidated();
        }
        private void NUD_MultiTargsUptime_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.MultipleTargetsPerc = (float)NUD_MultiTargsUptime.Value / 100f;
            Character.OnCalculationsInvalidated();
        }
        private void CK_MultipleTargets_CheckedChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.MultipleTargets = CK_MultipleTargets.Checked;
            NUD_MultiTargsUptime.Enabled = CalcOpts.MultipleTargets;
            Character.OnCalculationsInvalidated();
        }
        private void NUD_SurvScale_ValueChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.SurvScale = (float)NUD_SurvScale.Value;
            Character.OnCalculationsInvalidated();
        }
        // Special Effects Modifier, not in use yet
        private void CK_SE_UseDur_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter;
            calcOpts.SE_UseDur = CK_SE_UseDur.Checked;
            Character.OnCalculationsInvalidated();
        }
        #endregion
        #region Rotations
        private void CB_PriorityDefaults_SelectedIndexChanged(object sender, EventArgs e) {
            // only do anything if we weren't set to 0
            if (isLoading || CB_PriorityDefaults.SelectedIndex == 0) return;

            isLoading = true;

            int i = 0;
            if (CB_PriorityDefaults.SelectedIndex == (int)Specs.BeastMaster) { foreach (ComboBox cb in ShotPriorityBoxes) { cb.SelectedIndex = CalculationOptionsHunter.BeastMaster.ShotList[i].Index; i++; } }
            else if (CB_PriorityDefaults.SelectedIndex == (int)Specs.Marksman) { foreach (ComboBox cb in ShotPriorityBoxes) { cb.SelectedIndex = CalculationOptionsHunter.Marksman.ShotList[i].Index; i++; } }
            else if (CB_PriorityDefaults.SelectedIndex == (int)Specs.Survival) { foreach (ComboBox cb in ShotPriorityBoxes) { cb.SelectedIndex = CalculationOptionsHunter.Survival.ShotList[i].Index; i++; } }

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
            isLoading = false;

            int j = 0;
            int[] _prioIndxs = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
            foreach (ComboBox cb in ShotPriorityBoxes) { _prioIndxs[j] = cb.SelectedIndex; j++; }
            CalcOpts.PriorityIndexes = _prioIndxs;

            Character.OnCalculationsInvalidated();
        }
        private void PrioritySelectedIndexChanged(object sender, EventArgs e) {
            // this is called whenever any of the priority dropdowns are modified
            if (isLoading) return;

            int j = 0;
            int[] k = new int[10];
            foreach (ComboBox cb in ShotPriorityBoxes) { k[j] = cb.SelectedIndex; j++; }

            CalcOpts.PriorityIndexes = k;

            CB_PriorityDefaults.SelectedIndex = 0;

            Character.OnCalculationsInvalidated();
        }
        private void InitializeShotList(ComboBox cb) { foreach (Shot s in CalculationOptionsHunter.ShotList) { cb.Items.Add(s); } }
        private void PopulateAbilities() {
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
                case PetFamily.Chimaera:     familyList = new PetAttacks[] { PetAttacks.Bite, PetAttacks.Dive, PetAttacks.None, PetAttacks.FroststormBreath }; break;
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

            CB_PetPrio_01.Items.Clear();
            CB_PetPrio_01.Items.Add(PetAttacks.None);

            int family_mod = 0;

            if (CalcOpts.PetFamily != PetFamily.None)
            {
                CB_PetPrio_01.Items.Add(PetAttacks.Growl);
                CB_PetPrio_01.Items.Add(PetAttacks.Cower);

                foreach (PetAttacks A in familyList)
                {
                    if (A == PetAttacks.None) {
                        family_mod++;
                    } else {
                        CB_PetPrio_01.Items.Add(A);
                    }
                }

                PetFamilyTree family = getPetFamilyTree();

                if (family == PetFamilyTree.Cunning) {
                    //comboBoxPet1.Items.Add(PetAttacks.RoarOfRecovery);
                    CB_PetPrio_01.Items.Add(PetAttacks.RoarOfSacrifice);
                    CB_PetPrio_01.Items.Add(PetAttacks.WolverineBite);
                    //comboBoxPet1.Items.Add(PetAttacks.Bullheaded);
                }

                if (family == PetFamilyTree.Ferocity) {
                    CB_PetPrio_01.Items.Add(PetAttacks.LickYourWounds);
                    //comboBoxPet1.Items.Add(PetAttacks.CallOfTheWild);
                    //comboBoxPet1.Items.Add(PetAttacks.Rabid);
                }

                if (family == PetFamilyTree.Tenacity) {
                    CB_PetPrio_01.Items.Add(PetAttacks.Thunderstomp);
                    CB_PetPrio_01.Items.Add(PetAttacks.LastStand);
                    CB_PetPrio_01.Items.Add(PetAttacks.Taunt);
                    CB_PetPrio_01.Items.Add(PetAttacks.RoarOfSacrifice);
                }
            }

            object[] attacks_picklist = new object[CB_PetPrio_01.Items.Count];
            CB_PetPrio_01.Items.CopyTo(attacks_picklist, 0);

            CB_PetPrio_02.Items.Clear();
            CB_PetPrio_03.Items.Clear();
            CB_PetPrio_04.Items.Clear();
            CB_PetPrio_05.Items.Clear();
            CB_PetPrio_06.Items.Clear();
            CB_PetPrio_07.Items.Clear();

            CB_PetPrio_02.Items.AddRange(attacks_picklist);
            CB_PetPrio_03.Items.AddRange(attacks_picklist);
            CB_PetPrio_04.Items.AddRange(attacks_picklist);
            CB_PetPrio_05.Items.AddRange(attacks_picklist);
            CB_PetPrio_06.Items.AddRange(attacks_picklist);
            CB_PetPrio_07.Items.AddRange(attacks_picklist);

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
        /// <summary>
        /// This is to figure out which of the default rotations (if any) are in use
        /// </summary>
        /// <returns>The combobox index to use</returns>
        private int ShotRotationIndexCheck() {
            int specIndex = 0;

            List<Shot> list = new List<Shot>() { };
            foreach (ComboBox cb in ShotPriorityBoxes) { list.Add(CalculationOptionsHunter.ShotList[cb.SelectedIndex]); }
            ShotGroup current = new ShotGroup("Custom", list);

            if (current == CalculationOptionsHunter.BeastMaster) { specIndex = (int)Specs.BeastMaster; }
            else if (current == CalculationOptionsHunter.Marksman) { specIndex = (int)Specs.Marksman; }
            else if (current == CalculationOptionsHunter.Survival) { specIndex = (int)Specs.Survival; }
            
            return specIndex;
        }

        private int _CurrentSpec;
        private int CurrentSpec {
            get { return _CurrentSpec; }
            set { _CurrentSpec = value; }
        }
        public enum Specs { BeastMaster=1, Marksman, Survival }

        private void CharTalents_Changed(object sender, EventArgs e) {
            if (isLoading) return;
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
        private void CB_Pets_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoading) return;
            CalcOpts.PetPriority1 = CB_PetPrio_01.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_01.SelectedItem;
            CalcOpts.PetPriority2 = CB_PetPrio_02.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_02.SelectedItem;
            CalcOpts.PetPriority3 = CB_PetPrio_03.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_03.SelectedItem;
            CalcOpts.PetPriority4 = CB_PetPrio_04.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_04.SelectedItem;
            CalcOpts.PetPriority5 = CB_PetPrio_05.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_05.SelectedItem;
            CalcOpts.PetPriority6 = CB_PetPrio_06.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_06.SelectedItem;
            CalcOpts.PetPriority7 = CB_PetPrio_07.SelectedItem == null ? PetAttacks.None : (PetAttacks)CB_PetPrio_07.SelectedItem;
            Character.OnCalculationsInvalidated();
        }
        private void CB_PetFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) updateTalentDisplay();

            if (!isLoading && CB_PetFamily.SelectedItem != null)
            {
                CalcOpts.PetFamily = (PetFamily)CB_PetFamily.SelectedItem;
                PopulateAbilities();
                updateTalentDisplay();
                resetTalents();
                isLoading = false; // force it
                Character.OnCalculationsInvalidated();
            }
        }
        private void initTalentValues(ComboBox cmbBox, int max) {
            cmbBox.Items.Clear();
            for (int i = 0; i <= max; i++) {
                cmbBox.Items.Add(i);
            }
            cmbBox.SelectedIndex = 0;
        }
        private PetFamilyTree getPetFamilyTree()
        {
            if (CB_PetFamily.SelectedItem == null) return PetFamilyTree.None;
            switch ((PetFamily)CB_PetFamily.SelectedItem)
            {
                case PetFamily.Bat:
                case PetFamily.BirdOfPrey:
                case PetFamily.Chimaera:
                case PetFamily.Dragonhawk:
                case PetFamily.NetherRay:
                case PetFamily.Ravager:
                case PetFamily.Serpent:
                case PetFamily.Silithid:
                case PetFamily.Spider:
                case PetFamily.SporeBat:
                case PetFamily.WindSerpent:
                    return PetFamilyTree.Cunning;

                case PetFamily.Bear:
                case PetFamily.Boar:
                case PetFamily.Crab:
                case PetFamily.Crocolisk:
                case PetFamily.Gorilla:
                case PetFamily.Rhino:
                case PetFamily.Scorpid:
                case PetFamily.Turtle:
                case PetFamily.WarpStalker:
                case PetFamily.Worm:
                    return PetFamilyTree.Tenacity;

                case PetFamily.CarrionBird:
                case PetFamily.Cat:
                case PetFamily.CoreHound:
                case PetFamily.Devilsaur:
                case PetFamily.Hyena:
                case PetFamily.Moth:
                case PetFamily.Raptor:
                case PetFamily.SpiritBeast:
                case PetFamily.Tallstrider:
                case PetFamily.Wasp:
                case PetFamily.Wolf:
                    return PetFamilyTree.Ferocity;
            }

            // hmmm!
            return PetFamilyTree.None;
        }
        private void updateTalentDisplay()
        {
            PetFamilyTree tree = getPetFamilyTree();
            GB_PetTalents_Cunning.Visible = tree == PetFamilyTree.Cunning;
            GB_PetTalents_Ferocity.Visible = tree == PetFamilyTree.Ferocity;
            GB_PetTalents_Tenacity.Visible = tree == PetFamilyTree.Tenacity;
        }
        private void resetTalents() {
            CalcOpts.PetTalents.Reset();
            populatePetTalentCombos();
        }
        private void BT_PetTalentSpecButton_Click(object sender, EventArgs e) {
            if (((SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem).Spec == null) {
                List<SavedPetTalentSpec> classTalents = new List<SavedPetTalentSpec>();
                foreach (SavedPetTalentSpec spec in _savedPetTalents) {
                    /*if (spec.Class == Character.Class)*/ classTalents.Add(spec);
                }
                FormSavePetTalentSpec form = new FormSavePetTalentSpec(classTalents);
                if (form.ShowDialog(this) == DialogResult.OK) {
                    SavedPetTalentSpec spec = form.PetTalentSpec();
                    String specName = form.PetTalentSpecName();
                    if (spec == null) {
                        spec = new SavedPetTalentSpec(specName, _pettalents, spec.Class, _treeCount);
                        _savedPetTalents.Add(spec);
                    }
                    else spec.Spec = PetTalents.ToString();
                    UpdateSavedTalents();
                    SavePetTalentSpecs();
                    Character.OnCalculationsInvalidated();
                }
                form.Dispose();
            } else {
                _savedPetTalents.Remove((SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem);
                UpdateSavedTalents();
                SavePetTalentSpecs();
                Character.OnCalculationsInvalidated();
            }
        }

        private void CB_PetTalentSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem).Spec == null) {
                BT_PetTalentsSaveDel.Text = "Save";
            } else {
                BT_PetTalentsSaveDel.Text = "Delete";
                if (!_updateSaved) PetTalents = ((SavedPetTalentSpec)CB_PetTalentsSpecSwitcher.SelectedItem).TalentSpec();
            }
            populatePetTalentCombos();
            if(Character != null) Character.OnCalculationsInvalidated();
        }

        private void petTalentCombo_Changed(object sender, EventArgs e)
        {
            if (isLoading) { return; }
            // one of the (many) talent combo boxes has been updated
            // so we need to update the options

            PetTalentTreeData pt = CalcOpts.PetTalents;
            PetFamilyTree tree = getPetFamilyTree();
            int currentId = 0;

            try {
                pt.Reset();
                if (tree == PetFamilyTree.Cunning)
                {
                    currentId = pt.CobraReflexes.ID; pt.TalentTree[currentId].Value = CB_CunningCobraReflexes.SelectedIndex; LB_CunningCobraReflexes.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.DiveDash.ID; pt.TalentTree[currentId].Value = CB_CunningDiveDash.SelectedIndex; LB_CunningDiveDash.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GreatStamina.ID; pt.TalentTree[currentId].Value = CB_CunningGreatStamina.SelectedIndex; LB_CunningGreatStamina.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.NaturalArmor.ID; pt.TalentTree[currentId].Value = CB_CunningNaturalArmor.SelectedIndex; LB_CunningNaturalArmor.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.BoarsSpeed.ID; pt.TalentTree[currentId].Value = CB_CunningBoarsSpeed.SelectedIndex; LB_CunningBoarsSpeed.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Mobility.ID; pt.TalentTree[currentId].Value = CB_CunningMobility.SelectedIndex; LB_CunningMobility.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.SpikedCollar.ID; pt.TalentTree[currentId].Value = CB_CunningSpikedCollar.SelectedIndex; LB_CunningSpikedCollar.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.CullingTheHerd.ID; pt.TalentTree[currentId].Value = CB_CunningCullingTheHerd.SelectedIndex; LB_CunningCullingTheHerd.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];// Add in 3.3
                    currentId = pt.Lionhearted.ID; pt.TalentTree[currentId].Value = CB_CunningLionhearted.SelectedIndex; LB_CunningLionHearted.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.CarrionFeeder.ID; pt.TalentTree[currentId].Value = CB_CunningCarrionFeeder.SelectedIndex; LB_CunningCarrionFeeder.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GreatResistance.ID; pt.TalentTree[currentId].Value = CB_CunningGreatResistance.SelectedIndex; LB_CunningGreatResistance.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.OwlsFocus.ID; pt.TalentTree[currentId].Value = CB_CunningOwlsFocus.SelectedIndex; LB_CunningOwlsFocus.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Cornered.ID; pt.TalentTree[currentId].Value = CB_CunningCornered.SelectedIndex; LB_CunningCornered.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.FeedingFrenzy.ID; pt.TalentTree[currentId].Value = CB_CunningFeedingFrenzy.SelectedIndex; LB_CunningFeedingFrenzy.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.WolverineBite.ID; pt.TalentTree[currentId].Value = CB_CunningWolverineBite.SelectedIndex; LB_CunningWolverineBite.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.RoarOfRecovery.ID; pt.TalentTree[currentId].Value = CB_CunningRoarOfRecovery.SelectedIndex; LB_CunningRoarOfRecovery.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Bullheaded.ID; pt.TalentTree[currentId].Value = CB_CunningBullheaded.SelectedIndex; LB_CunningBullheaded.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GraceOfTheMantis.ID; pt.TalentTree[currentId].Value = CB_CunningGraceOfTheMantis.SelectedIndex; LB_CunningGraceOfTheMantis.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.WildHunt.ID; pt.TalentTree[currentId].Value = CB_CunningWildHunt.SelectedIndex; LB_CunningWildHunt.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.RoarOfSacrifice.ID; pt.TalentTree[currentId].Value = CB_CunningRoarOfSacrifice.SelectedIndex; LB_CunningRoarofSacrifice.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                }
                if (tree == PetFamilyTree.Ferocity)
                {
                    currentId = pt.CobraReflexes.ID; pt.TalentTree[currentId].Value = CB_FerocityCobraReflexes.SelectedIndex; LB_FerocityCobraReflexes.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.DiveDash.ID; pt.TalentTree[currentId].Value = CB_FerocityDiveDash.SelectedIndex; LB_FerocityDiveDash.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.ChargeSwoop.ID; pt.TalentTree[currentId].Value = CB_FerocityChargeSwoop.SelectedIndex; LB_FerocityChargeSwoop.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GreatStamina.ID; pt.TalentTree[currentId].Value = CB_FerocityGreatStamina.SelectedIndex; LB_FerocityGreatStamina.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.NaturalArmor.ID; pt.TalentTree[currentId].Value = CB_FerocityNaturalArmor.SelectedIndex; LB_FerocityNaturalArmor.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.BoarsSpeed.ID; pt.TalentTree[currentId].Value = CB_FerocityBoarsSpeed.SelectedIndex; LB_FerocityBoarsSpeed.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.SpikedCollar.ID; pt.TalentTree[currentId].Value = CB_FerocitySpikedCollar.SelectedIndex; LB_FerocitySpikedCollar.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.ImprovedCower.ID; pt.TalentTree[currentId].Value = CB_FerocityImprovedCower.SelectedIndex; LB_FerocityImprovedCower.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Bloodthirsty.ID; pt.TalentTree[currentId].Value = CB_FerocityBloodthirsty.SelectedIndex; LB_FerocityBloodthirsty.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.CullingTheHerd.ID; pt.TalentTree[currentId].Value = CB_FerocityCullingTheHerd.SelectedIndex; LB_FerocityCullingTheHerd.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];// Add in 3.3
                    currentId = pt.Lionhearted.ID; pt.TalentTree[currentId].Value = CB_FerocityLionhearted.SelectedIndex; LB_FerocityLionHearted.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GreatResistance.ID; pt.TalentTree[currentId].Value = CB_FerocityGreatResistance.SelectedIndex; LB_FerocityGreatResistance.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.HeartOfThePhoenix.ID; pt.TalentTree[currentId].Value = CB_FerocityHeartOfThePhoenix.SelectedIndex; LB_FerocityHeartOfThePheonix.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.SpidersBite.ID; pt.TalentTree[currentId].Value = CB_FerocitySpidersBite.SelectedIndex; LB_FerocitySpidersBite.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Rabid.ID; pt.TalentTree[currentId].Value = CB_FerocityRabid.SelectedIndex; LB_FerocityRabid.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.LickYourWounds.ID; pt.TalentTree[currentId].Value = CB_FerocityLickYourWounds.SelectedIndex; LB_FerocityLickYourWounds.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.CallOfTheWild.ID; pt.TalentTree[currentId].Value = CB_FerocityCallOfTheWild.SelectedIndex; LB_FerocityCalloftheWild.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.SharkAttack.ID; pt.TalentTree[currentId].Value = CB_FerocitySharkAttack.SelectedIndex; LB_FerocitySharkAttack.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.WildHunt.ID; pt.TalentTree[currentId].Value = CB_FerocityWildHunt.SelectedIndex; LB_FerocityWildHunt.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                }
                if (tree == PetFamilyTree.Tenacity)
                {
                    currentId = pt.CobraReflexes.ID; pt.TalentTree[currentId].Value = CB_TenacityCobraReflexes.SelectedIndex; LB_TenacityCobraReflexes.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.ChargeSwoop.ID; pt.TalentTree[currentId].Value = CB_TenacityCharge.SelectedIndex; LB_TenacityCharge.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GreatStamina.ID; pt.TalentTree[currentId].Value = CB_TenacityGreatStamina.SelectedIndex; LB_TenacityGreatStamina.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.NaturalArmor.ID; pt.TalentTree[currentId].Value = CB_TenacityNaturalArmor.SelectedIndex; LB_TenacityNaturalArmor.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.BoarsSpeed.ID; pt.TalentTree[currentId].Value = CB_TenacityBoarsSpeed.SelectedIndex; LB_TenacityBoarsSpeed.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.SpikedCollar.ID; pt.TalentTree[currentId].Value = CB_TenacitySpikedCollar.SelectedIndex; LB_TenacitySpikedCollar.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.BloodOfTheRhino.ID; pt.TalentTree[currentId].Value = CB_TenacityBloodOfTheRhino.SelectedIndex; LB_TenacityBloodOfTheRhino.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.PetBarding.ID; pt.TalentTree[currentId].Value = CB_TenacityPetBarding.SelectedIndex; LB_TenacityPetBarding.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.CullingTheHerd.ID; pt.TalentTree[currentId].Value = CB_TenacityCullingTheHerd.SelectedIndex; LB_TenacityCullingTheHerd.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];// Add in 3.3
                    currentId = pt.Lionhearted.ID; pt.TalentTree[currentId].Value = CB_TenacityLionhearted.SelectedIndex; LB_TenacityLionHearted.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GuardDog.ID; pt.TalentTree[currentId].Value = CB_TenacityGuardDog.SelectedIndex; LB_TenacityGuardDog.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Thunderstomp.ID; pt.TalentTree[currentId].Value = CB_TenacityThunderstomp.SelectedIndex; LB_TenacityThunderstomp.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GreatResistance.ID; pt.TalentTree[currentId].Value = CB_TenacityGreatResistance.SelectedIndex; LB_TenacityGreatResistance.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.GraceOfTheMantis.ID; pt.TalentTree[currentId].Value = CB_TenacityGraceOfTheMantis.SelectedIndex; LB_TenacityGraceOfTheMantis.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.LastStand.ID; pt.TalentTree[currentId].Value = CB_TenacityLastStand.SelectedIndex; LB_TenacityLastStand.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Taunt.ID; pt.TalentTree[currentId].Value = CB_TenacityTaunt.SelectedIndex; LB_TenacityTaunt.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Intervene.ID; pt.TalentTree[currentId].Value = CB_TenacityIntervene.SelectedIndex; LB_TenacityIntervene.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.WildHunt.ID; pt.TalentTree[currentId].Value = CB_TenacityWildHunt.SelectedIndex; LB_TenacityWildHunt.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.RoarOfSacrifice.ID; pt.TalentTree[currentId].Value = CB_TenacityRoarOfSacrifice.SelectedIndex; LB_TenacityRoarOfSacrifice.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                    currentId = pt.Silverback.ID; pt.TalentTree[currentId].Value = CB_TenacitySilverback.SelectedIndex; LB_TenacitySilverback.ToolTipText = pt.TalentTree[currentId].Desc[pt.TalentTree[currentId].Value];
                }

                if (tree == PetFamilyTree.None) { CalcOpts.PetTalents.Reset(); }

                //initTalentImages();
                CalcOpts.petTalents = CalcOpts.PetTalents.ToString();

                {
                    //ComboBox item = sender as ComboBox;
                    _treeCount = pt.TotalPoints();
                    //if (item != null) PetTalents.Data[item.Index] = item.CurrentRank;
                    UpdateSavedTalents();
                    SavePetTalentSpecs();
                }

                Character.OnCalculationsInvalidated();
            }
            catch (Exception ex)
            {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox(
                    "Error Setting Pet Talents after a Change", ex.Message,
                    "talentComboChanged", "Current ID: " + currentId.ToString() + "\r\nCurrent Talent: " + pt.TalentTree[currentId].Name, ex.StackTrace);
                eb.Show();
            }
        }
        private void populatePetTalentCombos()
        {
            // called when options are loaded to allow us to set the selected indexes
            int line = 0;
            try
            {
                isLoading = true;
                PetFamilyTree tree = getPetFamilyTree();
                // Cunning
                CB_CunningCullingTheHerd.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.CullingTheHerd.Value : 0; line++; // Add in 3.3
                CB_CunningBoarsSpeed.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.BoarsSpeed.Value : 0; line++;
                CB_CunningBullheaded.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.Bullheaded.Value : 0; line++;
                CB_CunningCarrionFeeder.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.CarrionFeeder.Value : 0; line++;
                CB_CunningCobraReflexes.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.CobraReflexes.Value : 0; line++;
                CB_CunningCornered.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.Cornered.Value : 0; line++;
                CB_CunningDiveDash.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.DiveDash.Value : 0; line++;
                CB_CunningFeedingFrenzy.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.FeedingFrenzy.Value : 0; line++;
                CB_CunningGraceOfTheMantis.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.GraceOfTheMantis.Value : 0; line++;
                CB_CunningGreatResistance.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.GreatResistance.Value : 0; line++;
                CB_CunningGreatStamina.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.GreatStamina.Value : 0; line++;
                CB_CunningLionhearted.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.Lionhearted.Value : 0; line++;
                CB_CunningMobility.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.Mobility.Value : 0; line++;
                CB_CunningNaturalArmor.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.NaturalArmor.Value : 0; line++;
                CB_CunningOwlsFocus.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.OwlsFocus.Value : 0; line++;
                CB_CunningRoarOfRecovery.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.RoarOfRecovery.Value : 0; line++;
                CB_CunningRoarOfSacrifice.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.RoarOfSacrifice.Value : 0; line++;
                CB_CunningSpikedCollar.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.SpikedCollar.Value : 0; line++;
                CB_CunningWildHunt.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.WildHunt.Value : 0; line++;
                CB_CunningWolverineBite.SelectedIndex = (tree == PetFamilyTree.Cunning) ? CalcOpts.PetTalents.WolverineBite.Value : 0; line++;
                // Ferocity
                CB_FerocityCullingTheHerd.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.CullingTheHerd.Value : 0; line++;  // Add in 3.3
                CB_FerocityBloodthirsty.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.Bloodthirsty.Value : 0; line++;
                CB_FerocityBoarsSpeed.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.BoarsSpeed.Value : 0; line++;
                CB_FerocityCallOfTheWild.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.CallOfTheWild.Value : 0; line++;
                CB_FerocityChargeSwoop.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.ChargeSwoop.Value : 0; line++;
                CB_FerocityCobraReflexes.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.CobraReflexes.Value : 0; line++;
                CB_FerocityDiveDash.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.DiveDash.Value : 0; line++;
                CB_FerocityGreatResistance.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.GreatResistance.Value : 0; line++;
                CB_FerocityGreatStamina.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.GreatStamina.Value : 0; line++;
                CB_FerocityHeartOfThePhoenix.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.HeartOfThePhoenix.Value : 0; line++;
                CB_FerocityImprovedCower.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.ImprovedCower.Value : 0; line++;
                CB_FerocityLickYourWounds.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.LickYourWounds.Value : 0; line++;
                CB_FerocityLionhearted.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.Lionhearted.Value : 0; line++;
                CB_FerocityNaturalArmor.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.NaturalArmor.Value : 0; line++;
                CB_FerocityRabid.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.Rabid.Value : 0; line++;
                CB_FerocitySharkAttack.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.SharkAttack.Value : 0; line++;
                CB_FerocitySpidersBite.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.SpidersBite.Value : 0; line++;
                CB_FerocitySpikedCollar.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.SpikedCollar.Value : 0; line++;
                CB_FerocityWildHunt.SelectedIndex = (tree == PetFamilyTree.Ferocity) ? CalcOpts.PetTalents.WildHunt.Value : 0; line++;
                // Tenacity Tree
                CB_TenacityCullingTheHerd.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.CullingTheHerd.Value : 0; line++;  // Add in 3.3
                CB_TenacityBloodOfTheRhino.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.BloodOfTheRhino.Value : 0; line++;
                CB_TenacityBoarsSpeed.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.BoarsSpeed.Value : 0; line++;
                CB_TenacityCharge.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.ChargeSwoop.Value : 0; line++;
                CB_TenacityCobraReflexes.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.CobraReflexes.Value : 0; line++;
                CB_TenacityGraceOfTheMantis.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.GraceOfTheMantis.Value : 0; line++;
                CB_TenacityGreatResistance.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.GreatResistance.Value : 0; line++;
                CB_TenacityGreatStamina.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.GreatStamina.Value : 0; line++;
                CB_TenacityGuardDog.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.GuardDog.Value : 0; line++;
                CB_TenacityIntervene.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.Intervene.Value : 0; line++;
                CB_TenacityLastStand.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.LastStand.Value : 0; line++;
                CB_TenacityLionhearted.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.Lionhearted.Value : 0; line++;
                CB_TenacityNaturalArmor.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.NaturalArmor.Value : 0; line++;
                CB_TenacityPetBarding.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.PetBarding.Value : 0; line++;
                CB_TenacityRoarOfSacrifice.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.RoarOfSacrifice.Value : 0; line++;
                CB_TenacitySilverback.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.Silverback.Value : 0; line++;
                CB_TenacitySpikedCollar.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.SpikedCollar.Value : 0; line++;
                CB_TenacityTaunt.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.Taunt.Value : 0; line++;
                CB_TenacityThunderstomp.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.Thunderstomp.Value : 0; line++;
                CB_TenacityWildHunt.SelectedIndex = (tree == PetFamilyTree.Tenacity) ? CalcOpts.PetTalents.WildHunt.Value : 0; line++;
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox(
                    "Error Populating Pet Talent ComboBoxes", ex.Message,
                    "populatePetTalentCombos", "Line: " + line.ToString(), ex.StackTrace);
            }
            isLoading = false;
        }

        private void CB_ArmoryPets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter;
            // Save the Index
            calcOpts.SelectedArmoryPet = CB_ArmoryPets.SelectedIndex;
            ArmoryPet CurrentPet = (ArmoryPet)CB_ArmoryPets.SelectedItem;
            // Populate the Pet Family
            isLoading = true;
            CB_PetFamily.Text = CurrentPet.Family;
            isLoading = false;
            // Convert the ArmoryPet spec to our spec
            PetTalentTreeData pt = PetTalentTreeData.FromArmoryPet(CurrentPet);
            // Populate the Pet Specs box
            {
                CalcOpts.PetTalents = pt;
                CalcOpts.petTalents = CalcOpts.PetTalents.ToString();
                //ComboBox item = sender as ComboBox;
                _treeCount = pt.TotalPoints();
                //if (item != null) PetTalents.Data[item.Index] = item.CurrentRank;
                UpdateSavedTalents();
                SavePetTalentSpecs();
            }
            CB_PetTalentSpec_SelectedIndexChanged(null, null);
        }
        #endregion
        #region Details Tab
        private void BT_Calculate_Click(object sender, EventArgs e)
        {
            RotationShotInfo[] myShotsTable;
            CalculationsHunter calcs = Character.CurrentCalculations as CalculationsHunter;
            CharacterCalculationsHunter charCalcs = calcs.GetCharacterCalculations(Character) as CharacterCalculationsHunter;
            charCalcs.collectSequence = true;
            RotationTest rTest = new RotationTest(Character, charCalcs, CalcOpts, BossOpts);
            rTest.RunTest();
            myShotsTable = rTest.getRotationTable();
            TB_Rotation.Text = charCalcs.sequence;
            TB_Shots.Text = "";
            for (int i = 0; i < myShotsTable.Length; i++)
            {
                if (myShotsTable[i] != null)
                    TB_Shots.Text += String.Format("{0,-13} : {1,4}", myShotsTable[i].type, myShotsTable[i].countUsed) + Environment.NewLine;
            }
        }
        private void TB_Rotation_TextChanged(object sender, EventArgs e) { }
        private void TB_Shots_TextChanged(object sender, EventArgs e) { }
        #endregion
        #region Stat Graph
        private Stats[] BuildStatsList()
        {
            List<Stats> statsList = new List<Stats>();
            if (CK_StatsAgility.Checked) { statsList.Add(new Stats() { Agility = 1f }); }
            if (CK_StatsAP.Checked) { statsList.Add(new Stats() { AttackPower = 1f }); }
            if (CK_StatsCrit.Checked) { statsList.Add(new Stats() { CritRating = 1f }); }
            if (CK_StatsHit.Checked) { statsList.Add(new Stats() { HitRating = 1f }); }
            if (CK_StatsHaste.Checked) { statsList.Add(new Stats() { HasteRating = 1f }); }
            if (CK_StatsArP.Checked) { statsList.Add(new Stats() { ArmorPenetrationRating = 1f }); }
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
            graph.Show();
        }
        private void CK_StatsAgility_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[0] = CK_StatsAgility.Checked; }
        private void CK_StatsAP_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[1] = CK_StatsAP.Checked; }
        private void CK_StatsCrit_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[2] = CK_StatsCrit.Checked; }
        private void CK_StatsHit_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[3] = CK_StatsHit.Checked; }
        private void CK_StatsHaste_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[4] = CK_StatsHaste.Checked; }
        private void CK_StatsArP_CheckedChanged(object sender, EventArgs e) { CalculationOptionsHunter calcOpts = Character.CalculationOptions as CalculationOptionsHunter; calcOpts.StatsList[5] = CK_StatsArP.Checked; }
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
    public static class ShotRotationFunctions {
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