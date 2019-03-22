using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rawr.Enhance
{
    public class FireElemental
    {
        private float _ap = 0f;
        private float _sp = 0f;
        private CombatStats _cs;
        private float _critbuffs = 0f;
        private float _petMeleeMissRate = 0f;
        private float _petMeleeMultipliers = 0f;
        private float _petSpellMissRate = 0f;
        private float _petSpellMultipliers = 0f;

        private float meleeTotaldps = 0f;
        private float novaTotaldps = 0f;
        private float blastTotaldps = 0f;
        private float shieldTotaldps = 0f;
        private float totaldps = 0f;
        private float totalMana = 0f;
        private int novaUses = 0;
        private int blastUses = 0;
        private int meleeUses = 0;
        
        public FireElemental(float ap, float sp, float intellect, CombatStats cs, float critbuffs,
                             float petMeleeMissRate, float petMeleeMultipliers, float petSpellMissRate, float petSpellMultipliers)
        {
            _ap = ap; 
            _sp = sp;
            _cs = cs;
            _petMeleeMissRate = petMeleeMissRate;
            _petMeleeMultipliers = petMeleeMultipliers;
            _petSpellMissRate = petSpellMissRate;
            _petSpellMultipliers = petSpellMultipliers;

            meleeTotaldps = 0f;
            novaTotaldps = 0f;
            blastTotaldps = 0f;
            shieldTotaldps = 0f;
            totaldps = 0f;
            totalMana = 4000 + 15 * (0.3f * intellect);
            CalculateUses();
            CalculateDamage();
        }

        public float getDPS()
        {
            return totaldps;
        }

        public string getDPSOutput()
        {
            return string.Format("{0}*{1} Melee dps\n{2} Fire Blast dps\n{3} Fire Nova dps\n{4} Fire Shield dps",
                totaldps.ToString("F2", CultureInfo.InvariantCulture), 
                meleeTotaldps.ToString("F2", CultureInfo.InvariantCulture),
                blastTotaldps.ToString("F2", CultureInfo.InvariantCulture),
                novaTotaldps.ToString("F2", CultureInfo.InvariantCulture),
                shieldTotaldps.ToString("F2", CultureInfo.InvariantCulture));
        }

        private void CalculateDamage()
        {
            float meleeBasedps = (180 + .6f * _sp + (2f / 14f) * _ap) * meleeUses / 120f; //subject to glancing blows 
            float blastBasedps = (775f + .2f * _sp) * blastUses / 120f;
            float novaBasedps = (987.5f + .5f * _sp) * novaUses / 120f;
            float shieldBasedps = (96 + .015f * _sp) / 2f;

            float meleeNormal = meleeBasedps * (1 - _petMeleeMissRate - _critbuffs - _cs.GlancingRate);
            float meleeCrits = meleeBasedps * _critbuffs * _cs.CritMultiplierMelee;
            float meleeGlances = meleeBasedps * _cs.GlancingHitModifier;
            meleeTotaldps = (meleeNormal + meleeCrits + meleeGlances) * _petMeleeMultipliers * _cs.FireElementalUptime;

            float blastNormal = blastBasedps * (1 - _petSpellMissRate - _critbuffs);
            float blastCrits = blastBasedps * _critbuffs * _cs.CritMultiplierSpell;
            blastTotaldps = (blastNormal + blastCrits) * _petSpellMultipliers * _cs.FireElementalUptime;

            float novaNormal = novaBasedps * (1 - _petSpellMissRate - _critbuffs);
            float novaCrits = novaBasedps * _critbuffs * _cs.CritMultiplierSpell;
            novaTotaldps = (novaNormal + novaCrits) * _petSpellMultipliers * _cs.FireElementalUptime * _cs.MultiTargetMultiplier;

            float shieldNormal = shieldBasedps * (1 - _petSpellMissRate - _critbuffs);
            float shieldCrits = shieldBasedps * _critbuffs * _cs.CritMultiplierSpell;
            shieldTotaldps = (shieldNormal + shieldCrits) * _petSpellMultipliers * _cs.FireElementalUptime * _cs.MultiTargetMultiplier;

            totaldps = meleeTotaldps + novaTotaldps + blastTotaldps + shieldTotaldps;
        }

        private void CalculateUses()
        {
            float time = 2f; // he tends to take a couple of seconds to get in range and start
            int sequence = 1;
            novaUses = 0;
            blastUses = 0;
            meleeUses = 0;
            while (totalMana > 0 && time < 120f)
            {
                switch (sequence)
                {
                    case (int)AbilitySequence.nova:
                        if (totalMana > 207)
                        {
                            novaUses++;
                            totalMana -= 207;
                            time += 2.5f;
                        }
                        break;
                    case (int)AbilitySequence.blast:
                        if (totalMana > 276)
                        {
                            blastUses++;
                            totalMana -= 276;
                            time += 4f;
                        }
                        break;
                    case (int)AbilitySequence.melee:
                        meleeUses++;
                        time += 2f;
                        break;
                }
                sequence++;
                if (sequence == 4) sequence = 1;
            }
        }

        private enum AbilitySequence
        {
            nova = 1,
            blast = 2,
            melee = 3,
        }
    }
}
