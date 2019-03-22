using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace Rawr.UI
{
    public partial class DG_BuffsByRaidMembers : ChildWindow
    {
        public DG_BuffsByRaidMembers()
        {
            InitializeComponent();
            // Set up the dialog
            CB_RaidSize.SelectedIndex = 1;
            CB_Class2Add.SelectedIndex = 9;
            #region Individual Class Work
            // Hunter
            RB_Hunter_MM.IsChecked = true;
            // Mage
            RB_Mage_Arcane.IsChecked = true;
            // Priest
            RB_Priest_H.IsChecked = true;
            // Warrior
            RB_Warrior_Tank.IsChecked = true;
            #endregion
        }

        #region List Editing
        public static Color FromKnownColor(string colorName)
        {
            Line lne = (Line)XamlReader.Load("<Line xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Fill=\"" + colorName + "\" />");
            return (Color)lne.Fill.GetValue(SolidColorBrush.ColorProperty);
        }
        private void RaidSizeCheck()
        {
            // Disable the Add button if we've hit raid size
            if (List_Classes.Items.Count >= (Int32)CB_RaidSize.SelectedItem)
            { BT_Add.IsEnabled = false; } else { BT_Add.IsEnabled = true; }
            // Disable the Delete Button if there is no one in the Raid
            if (List_Classes.Items.Count == 0)
            { BT_Delete.IsEnabled = false; } else { BT_Delete.IsEnabled = true; }
            if (List_Classes.Items.Count == 0)
            { BT_DeleteAll.IsEnabled = false; } else { BT_DeleteAll.IsEnabled = true; }
        }
        private void CB_RaidSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaidSizeCheck();
        }
        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            #region Individual Class Work
            string toAdd = "";
            string toAddsColor = "White";
            switch ((string)CB_Class2Add.SelectedItem)
            {
                case "Death Knight": { break; }
                case "Druid": { break; }
                #region Hunter
                case "Hunter": {
                    toAddsColor = "Green";
                    string spec = RB_Hunter_BM.IsChecked.GetValueOrDefault(false) ? "BM" :
                                  RB_Hunter_MM.IsChecked.GetValueOrDefault(false) ? "MM" :
                                  RB_Hunter_SV.IsChecked.GetValueOrDefault(false) ? "SV" : "MM";
                    // Class & Spec
                    toAdd += "Hunter (" + spec + ")";
                    // Spec Specific
                    if (spec == "BM") {
                        toAdd += "\n - Buff: Ferocious Inspiration (Damage)";
                    } else if (spec == "MM") {
                        toAdd += "\n - Buff: Trueshot Aura (AP)";
                    } else if (spec == "SV") {
                        toAdd += "\n - Buff: Hunting Party (Mana Regen)";
                    }
                    // Pet
                    if ((string)CB_Hunter_Pet.SelectedItem != "None" && (string)CB_Hunter_Pet.SelectedItem != "Other") {
                        toAdd += "\n - Buff: " + (string)CB_Hunter_Pet.SelectedItem;
                    }
                    // Sting
                    if ((string)CB_Hunter_Sting.SelectedItem != "None" && (string)CB_Hunter_Sting.SelectedItem != "Other") {
                        toAdd += "\n - Debuff: " + (string)CB_Hunter_Sting.SelectedItem;
                    }
                    // Hunter's Mark
                    if ((string)CB_Hunter_Mark.SelectedItem != "None") {
                        toAdd += "\n - Debuff: " + (string)CB_Hunter_Mark.SelectedItem
                            + (spec == "MM" && CK_Hunter_Mark.IsChecked.GetValueOrDefault(false) ? " (Imp)" : "");
                    }
                    // Aspect
                    if ((string)CB_Hunter_Aspect.SelectedItem != "None" && (string)CB_Hunter_Aspect.SelectedItem != "Other") {
                        toAdd += "\n - Debuff: " + (string)CB_Hunter_Aspect.SelectedItem;
                    }
                    // Cleanup
                    toAdd = toAdd.Trim();
                    break;
                }
                #endregion
                #region Mage
                case "Mage": {
                    toAddsColor = "LightBlue";
                    string spec = RB_Mage_Frost.IsChecked.GetValueOrDefault(false) ? "Frost" :
                                  RB_Mage_Fire.IsChecked.GetValueOrDefault(false) ? "Fire" :
                                  RB_Mage_Arcane.IsChecked.GetValueOrDefault(false) ? "Arcane" : "Arcane";
                    // Class & Spec
                    toAdd += "Mage (" + spec + ")";
                    toAdd += "\n - Buff: Arcane Intellect (Intellect)"
                           + "\n - Debuff: Amplify Magic (SpellPower)";
                    // Spec Specific
                    if (spec == "Frost") {
                        toAdd += "\n - Buff: Enduring Winter (Mana Regen)";
                        toAdd += "\n - Debuff: Winter's Chill (SpellCrit Debuff)";
                    } else if (spec == "Fire") {
                        toAdd += "\n - Debuff: Improved Scorch (SpellCrit Debuff)";
                    } else if (spec == "Arcane") {
                        toAdd += "\n - Buff: Arcane Empowerment (Bonus Damage)";
                    }
                    // Focus Magic
                    if ((string)CB_Mage_Focus.SelectedItem != "None") {
                        toAdd += "\n - Buff: " + (string)CB_Mage_Focus.SelectedItem;
                    }
                    // Magic Attunement
                    if ((string)CB_Mage_MA.SelectedItem != "None") {
                        toAdd += "\n - Buff: " + (string)CB_Mage_MA.SelectedItem;
                    }
                    // Cleanup
                    toAdd = toAdd.Trim();
                    break;
                }
                #endregion
                case "Paladin": { break; }
                #region Priest
                case "Priest": {
                    toAddsColor = "LightGray";
                    string spec = RB_Priest_D.IsChecked.GetValueOrDefault(false) ? "Disc" :
                                  RB_Priest_H.IsChecked.GetValueOrDefault(false) ? "Holy" :
                                  RB_Priest_S.IsChecked.GetValueOrDefault(false) ? "Shadow" : "Holy";
                    // Class & Spec
                    toAdd += "Priest (" + spec + ")";
                    toAdd += "\n - Buff: Divine Spirit (Spirit)"
                           + "\n - Buff: Power Word Fortitude (Imp) (Stamina)"
                           + "\n - Buff: Shadow Protection (Shadow Resist)";
                    // Spec Specific
                    if (spec == "Disc") {
                        toAdd += "\n - Buff: Renewed Hope (Damage Taken Reduc)";
                    } else if (spec == "Holy") {
                        toAdd += "\n - Buff: Inspiration (Damage Taken Reduc)"
                               + "\n - Buff: Hymn of Hope (Burst Mana Regen)";
                    } else if (spec == "Shadow") {
                        toAdd += "\n - Buff: Vampiric Touch (Mana Regen)"
                               + "\n - Buff: Misery (Spell Hit)";
                    }
                    // Cleanup
                    toAdd = toAdd.Trim();
                    break;
                }
                #endregion
                case "Rogue": { break; }
                case "Shaman": { break; }
                case "Warlock": { break; }
                #region Warrior
                case "Warrior": {
                    toAddsColor = "BurlyWood";
                    string spec = RB_Warrior_Arms.IsChecked.GetValueOrDefault(false) ? "Arms" :
                                  RB_Warrior_Fury.IsChecked.GetValueOrDefault(false) ? "Fury" :
                                  RB_Warrior_Tank.IsChecked.GetValueOrDefault(false) ? "Prot" : "Arms";
                    // Class & Spec
                    toAdd += "Warrior (" + spec + ")";
                    // Spec Specific
                    if (spec == "Prot") {
                    } else if (spec == "Arms") {
                        toAdd += "\n - Debuff: Trauma (Bleed Bonus)";
                        toAdd += "\n - Debuff: Blood Frenzy (Phys DMG Bonus)";
                    } else if (spec == "Fury") {
                        toAdd += "\n - Buff: Rampage (5% Crit)";
                    }
                    // Buff Shout
                    if ((string)CB_Warrior_BuffShout.SelectedItem != "None") {
                        string temp = (string)CB_Warrior_BuffShout.SelectedItem;
                        toAdd += "\n - Buff: " + (string)CB_Warrior_BuffShout.SelectedItem
                            + (CK_Warrior_BuffShout.IsChecked.GetValueOrDefault(false) ? " (Imp)" : "")
                            + (temp == "Battle Shout" ? " (AP)" : " (Health)");
                    }
                    // Debuff Shout
                    if ((string)CB_Warrior_DebuffShout.SelectedItem != "None") {
                        toAdd += "\n - Debuff: " + (string)CB_Warrior_DebuffShout.SelectedItem
                            + (CK_Warrior_DebuffShout.IsChecked.GetValueOrDefault(false) ? " (Imp)" : "")
                            + " (Target AP Reduc)";
                    }
                    // Thunderclap
                    if (spec == "Prot" || (spec == "Arms" && (string)CB_Warrior_Thunderclap.SelectedItem != "None")) {
                        toAdd += "\n - Debuff: " + (string)CB_Warrior_Thunderclap.SelectedItem
                            + (spec == "Prot" || (CK_Warrior_Thunderclap.IsChecked.GetValueOrDefault(false)) ? " (Imp)" : "")
                            + " (Target AtkSpd Reduc)";
                    }
                    // Sunder Armor
                    if (spec == "Prot" || ((spec == "Arms" || spec == "Fury") && (string)CB_Warrior_Sunder.SelectedItem != "None")) {
                        toAdd += "\n - Debuff: " + (string)CB_Warrior_Sunder.SelectedItem
                            +" (Target Armor Reduc)";
                    }
                    // Cleanup
                    toAdd = toAdd.Trim();
                    break;
                }
                #endregion
                default: { break; }
            }
            #endregion
            // Add if not blank
            if (toAdd != "") {
                ListBoxItem newAdd = new ListBoxItem();
                newAdd.Content = toAdd;
                newAdd.Background = new SolidColorBrush(FromKnownColor(toAddsColor));
                newAdd.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop() { Color = FromKnownColor("White"), Offset = 0 },
                    new GradientStop() { Color = FromKnownColor(toAddsColor), Offset = 1 }
                }, 0);
                List_Classes.Items.Add(newAdd);
            }
            // Verify we can add more people after this
            RaidSizeCheck();
        }
        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure there is something to Remove
            if (List_Classes.SelectedItem == null) return;
            // Remove the Listing
            List_Classes.Items.Remove(List_Classes.SelectedItem);
            // Finish off
            RaidSizeCheck();
        }
        private void BT_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            // Remove all of the Listings
            List_Classes.Items.Clear();
            // Finish off
            RaidSizeCheck();
        }
        #endregion

        private void CB_Class2Add_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LO_Warrior.Visibility = Visibility.Collapsed;
            LO_DeathKnight.Visibility = Visibility.Collapsed;
            LO_Druid.Visibility = Visibility.Collapsed;
            LO_Hunter.Visibility = Visibility.Collapsed;
            LO_Mage.Visibility = Visibility.Collapsed;
            LO_Priest.Visibility = Visibility.Collapsed;
            LO_Paladin.Visibility = Visibility.Collapsed;
            LO_Rogue.Visibility = Visibility.Collapsed;
            LO_Shaman.Visibility = Visibility.Collapsed;
            LO_Warlock.Visibility = Visibility.Collapsed;
            LO_Warrior.Visibility = Visibility.Collapsed;

            switch ((string)CB_Class2Add.SelectedItem)
            {
                case "Death Knight": { LO_DeathKnight.Visibility = Visibility.Visible; break; }
                case "Druid": { LO_Druid.Visibility = Visibility.Visible; break; }
                case "Hunter": { LO_Hunter.Visibility = Visibility.Visible; break; }
                case "Mage": { LO_Mage.Visibility = Visibility.Visible; break; }
                case "Priest": { LO_Priest.Visibility = Visibility.Visible; break; }
                case "Paladin": { LO_Paladin.Visibility = Visibility.Visible; break; }
                case "Rogue": { LO_Rogue.Visibility = Visibility.Visible; break; }
                case "Shaman": { LO_Shaman.Visibility = Visibility.Visible; break; }
                case "Warlock": { LO_Warlock.Visibility = Visibility.Visible; break; }
                case "Warrior": { LO_Warrior.Visibility = Visibility.Visible; break; }
                default: { break; }
            }
        }

        #region Individual Class Work
        // Hunter
        private void CK_HunterSpec_Changed(object sender, RoutedEventArgs e)
        {
            if (RB_Hunter_BM.IsChecked.GetValueOrDefault(false)) {
                // Pet: Optional
                LB_Hunter_Pet.Visibility = Visibility.Visible;
                CB_Hunter_Pet.Visibility = Visibility.Visible;
                // Sting
                LB_Hunter_Sting.Visibility = Visibility.Visible;
                CB_Hunter_Sting.Visibility = Visibility.Visible;
                // Hunter's Mark
                LB_Hunter_Mark.Visibility = Visibility.Visible;
                CB_Hunter_Mark.Visibility = Visibility.Visible;
                CK_Hunter_Mark.Visibility = Visibility.Collapsed;
                // Aspect
                LB_Hunter_Aspect.Visibility = Visibility.Visible;
                CB_Hunter_Aspect.Visibility = Visibility.Visible;
            } else if (RB_Hunter_MM.IsChecked.GetValueOrDefault(false)) {
                // Pet: Optional
                LB_Hunter_Pet.Visibility = Visibility.Visible;
                CB_Hunter_Pet.Visibility = Visibility.Visible;
                // Sting
                LB_Hunter_Sting.Visibility = Visibility.Visible;
                CB_Hunter_Sting.Visibility = Visibility.Visible;
                // Hunter's Mark
                LB_Hunter_Mark.Visibility = Visibility.Visible;
                CB_Hunter_Mark.Visibility = Visibility.Visible;
                CK_Hunter_Mark.Visibility = Visibility.Visible;
                // Aspect
                LB_Hunter_Aspect.Visibility = Visibility.Visible;
                CB_Hunter_Aspect.Visibility = Visibility.Visible;
            } else if (RB_Hunter_SV.IsChecked.GetValueOrDefault(false)) {
                // Pet: Optional
                LB_Hunter_Pet.Visibility = Visibility.Visible;
                CB_Hunter_Pet.Visibility = Visibility.Visible;
                // Sting
                LB_Hunter_Sting.Visibility = Visibility.Visible;
                CB_Hunter_Sting.Visibility = Visibility.Visible;
                // Hunter's Mark
                LB_Hunter_Mark.Visibility = Visibility.Visible;
                CB_Hunter_Mark.Visibility = Visibility.Visible;
                CK_Hunter_Mark.Visibility = Visibility.Collapsed;
                // Aspect
                LB_Hunter_Aspect.Visibility = Visibility.Visible;
                CB_Hunter_Aspect.Visibility = Visibility.Visible;
            }
        }
        // Mage
        private void CK_MageSpec_Changed(object sender, RoutedEventArgs e)
        {
            // Do nothing because there are no specialties to swap around
        }
        // Priest
        private void CK_PriestSpec_Changed(object sender, RoutedEventArgs e)
        {
            // Do nothing because there are no specialties to swap around
        }
        // Warrior
        private void CK_WarriorSpec_Changed(object sender, RoutedEventArgs e)
        {
            // Buff Shouts
            LB_Warrior_BuffShout.Visibility = Visibility.Collapsed;
            CB_Warrior_BuffShout.Visibility = Visibility.Collapsed;
            CK_Warrior_BuffShout.Visibility = Visibility.Collapsed;
            // Debuff Shouts
            LB_Warrior_DebuffShout.Visibility = Visibility.Collapsed;
            CB_Warrior_DebuffShout.Visibility = Visibility.Collapsed;
            CK_Warrior_DebuffShout.Visibility = Visibility.Collapsed;
            // Thunderclap
            LB_Warrior_Thunderclap.Visibility = Visibility.Collapsed;
            CB_Warrior_Thunderclap.Visibility = Visibility.Collapsed;
            CK_Warrior_Thunderclap.Visibility = Visibility.Collapsed;
            // Sunder Armor
            LB_Warrior_Sunder.Visibility = Visibility.Collapsed;
            CB_Warrior_Sunder.Visibility = Visibility.Collapsed;

            if        (RB_Warrior_Tank.IsChecked.GetValueOrDefault(false)) {
                // Buff Shout: Optional
                LB_Warrior_BuffShout.Visibility = Visibility.Visible;
                CB_Warrior_BuffShout.Visibility = Visibility.Visible;
                CK_Warrior_BuffShout.Visibility = Visibility.Visible;
                // Debuff Shout: Optional
                LB_Warrior_DebuffShout.Visibility = Visibility.Visible;
                CB_Warrior_DebuffShout.Visibility = Visibility.Visible;
                CK_Warrior_DebuffShout.Visibility = Visibility.Visible;
                // Thunderclap: Always
                // Sunder: Always
            }
            else if (RB_Warrior_Arms.IsChecked.GetValueOrDefault(false))
            {
                // Buff Shout: Optional
                LB_Warrior_BuffShout.Visibility = Visibility.Visible;
                CB_Warrior_BuffShout.Visibility = Visibility.Visible;
                CK_Warrior_BuffShout.Visibility = Visibility.Visible;
                // Debuff Shout: Optional
                LB_Warrior_DebuffShout.Visibility = Visibility.Visible;
                CB_Warrior_DebuffShout.Visibility = Visibility.Visible;
                CK_Warrior_DebuffShout.Visibility = Visibility.Visible;
                // Thunderclap: Optional
                LB_Warrior_Thunderclap.Visibility = Visibility.Visible;
                CB_Warrior_Thunderclap.Visibility = Visibility.Visible;
                CK_Warrior_Thunderclap.Visibility = Visibility.Visible;
                // Sunder: Optional
                LB_Warrior_Sunder.Visibility = Visibility.Visible;
                CB_Warrior_Sunder.Visibility = Visibility.Visible;
            }
            else if (RB_Warrior_Fury.IsChecked.GetValueOrDefault(false))
            {
                // Buff Shout: Optional
                LB_Warrior_BuffShout.Visibility = Visibility.Visible;
                CB_Warrior_BuffShout.Visibility = Visibility.Visible;
                CK_Warrior_BuffShout.Visibility = Visibility.Visible;
                // Debuff Shout: Optional
                LB_Warrior_DebuffShout.Visibility = Visibility.Visible;
                CB_Warrior_DebuffShout.Visibility = Visibility.Visible;
                CK_Warrior_DebuffShout.Visibility = Visibility.Visible;
                // Thunderclap: Unavailable
                // Sunder: Optional
                LB_Warrior_Sunder.Visibility = Visibility.Visible;
                CB_Warrior_Sunder.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Dialog Exiting
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion
    }
}
