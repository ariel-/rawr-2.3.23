using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class FormOptimizeResult : Form 
    {
        public FormOptimizeResult(Character before, Character after)
        {
            InitializeComponent();
            paperDollBefore.Character = before.Clone();
            paperDollAfter.Character = after.Clone();
            RemoveUnchangedItems();
        }

        private void RemoveUnchangedItems()
        {
            Enchant noEnchant = Enchant.FindEnchant(0, ItemSlot.None, paperDollBefore.Character);
            paperDollBefore.ShirtHidden = true;
            paperDollBefore.TabardHidden = true;
            paperDollAfter.ShirtHidden = true;
            paperDollAfter.TabardHidden = true;

            if (paperDollBefore.Head.Equals(paperDollAfter.Head))
            {
                paperDollBefore.HeadHidden = true;
                paperDollAfter.HeadHidden = true;
            }
            if (paperDollBefore.Neck.Equals(paperDollAfter.Neck))
            {
                paperDollBefore.NeckHidden = true;
                paperDollAfter.NeckHidden = true;
            }
            if (paperDollBefore.Shoulders.Equals(paperDollAfter.Shoulders))
            {
                paperDollBefore.ShouldersHidden = true;
                paperDollAfter.ShouldersHidden = true;
            }
            if (paperDollBefore.Back.Equals(paperDollAfter.Back))
            {
                paperDollBefore.BackHidden = true;
                paperDollAfter.BackHidden = true;
            }
            if (paperDollBefore.Chest.Equals(paperDollAfter.Chest))
            {
                paperDollBefore.ChestHidden = true;
                paperDollAfter.ChestHidden = true;
            }
            if (paperDollBefore.Wrist.Equals(paperDollAfter.Wrist))
            {
                paperDollBefore.WristHidden = true;
                paperDollAfter.WristHidden = true;
            }
            if (paperDollBefore.MainHand.Equals(paperDollAfter.MainHand))
            {
                paperDollBefore.MainHandHidden = true;
                paperDollAfter.MainHandHidden = true;
            }
            if (paperDollBefore.OffHand.Equals(paperDollAfter.OffHand))
            {
                paperDollBefore.OffHandHidden = true;
                paperDollAfter.OffHandHidden = true;
            }
            if (paperDollBefore.Ranged.Equals(paperDollAfter.Ranged))
            {
                paperDollBefore.RangedHidden = true;
                paperDollAfter.RangedHidden = true;
            }
            if (paperDollBefore.Projectile.Equals(paperDollAfter.Projectile))
            {
                paperDollBefore.ProjectileHidden = true;
                paperDollAfter.ProjectileHidden = true;
            }
            if (paperDollBefore.ProjectileBag.Equals(paperDollAfter.ProjectileBag))
            {
                paperDollBefore.ProjectileBagHidden = true;
                paperDollAfter.ProjectileBagHidden = true;
            }
            if (paperDollBefore.Hands.Equals(paperDollAfter.Hands))
            {
                paperDollBefore.HandsHidden = true;
                paperDollAfter.HandsHidden = true;
            }
            if (paperDollBefore.Waist.Equals(paperDollAfter.Waist))
            {
                paperDollBefore.WaistHidden = true;
                paperDollAfter.WaistHidden = true;
            }
            if (paperDollBefore.Legs.Equals(paperDollAfter.Legs))
            {
                paperDollBefore.LegsHidden = true;
                paperDollAfter.LegsHidden = true;
            }
            if (paperDollBefore.Feet.Equals(paperDollAfter.Feet))
            {
                paperDollBefore.FeetHidden = true;
                paperDollAfter.FeetHidden = true;
            }
            if (paperDollBefore.Finger1.Equals(paperDollAfter.Finger1))
            {
                paperDollBefore.Finger1Hidden = true;
                paperDollAfter.Finger1Hidden = true;
            }
            if (paperDollBefore.Finger2.Equals(paperDollAfter.Finger2))
            {
                paperDollBefore.Finger2Hidden = true;
                paperDollAfter.Finger2Hidden = true;
            }
            if (paperDollBefore.Trinket1.Equals(paperDollAfter.Trinket1))
            {
                paperDollBefore.Trinket1Hidden = true;
                paperDollAfter.Trinket1Hidden = true;
            }
            if (paperDollBefore.Trinket2.Equals(paperDollAfter.Trinket2))
            {
                paperDollBefore.Trinket2Hidden = true;
                paperDollAfter.Trinket2Hidden = true;
            }

            // now check swapped slot rings & trinkets
            if (paperDollBefore.Finger1.Equals(paperDollAfter.Finger2))
            {
                paperDollBefore.Finger1Hidden = true;
                paperDollAfter.Finger2Hidden = true;
            }
            if (paperDollBefore.Finger2.Equals(paperDollAfter.Finger1))
            {
                paperDollBefore.Finger2Hidden = true;
                paperDollAfter.Finger1Hidden = true;
            }
            if (paperDollBefore.Trinket1.Equals(paperDollAfter.Trinket2))
            {
                paperDollBefore.Trinket1Hidden = true;
                paperDollAfter.Trinket2Hidden = true;
            }
            if (paperDollBefore.Trinket2.Equals(paperDollAfter.Trinket1))
            {
                paperDollBefore.Trinket2Hidden = true;
                paperDollAfter.Trinket1Hidden = true;
            }

            // also hide offhands if they're not actually used
            if (!Calculations.IncludeOffHandInCalculations(paperDollBefore.Character) && !Calculations.IncludeOffHandInCalculations(paperDollAfter.Character))
            {
                paperDollBefore.OffHandHidden = true;
                paperDollAfter.OffHandHidden = true;
            }
        }

        public void SetOptimizerScores(float scoreBefore, float scoreAfter)
        {
            paperDollBefore.SetResultText(string.Format("Score Before Optimization :\r\n{0}", scoreBefore));
            if (scoreAfter >= 0)
                paperDollAfter.SetResultText(string.Format("Score After Optimization :\r\n{0}", scoreAfter));
            else
                paperDollAfter.SetResultText("The Optimizer was not able to meet all the requirements. Would you like to equip the gear that is closest to meeting them?");
        }
    }
}
