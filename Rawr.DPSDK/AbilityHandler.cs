using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    class AbilityHandler
    {
        Stats stats;
        CombatTable combatTable;
        DeathKnightTalents talents;
        Character character;
        CalculationOptionsDPSDK calcOpts;
        public AbilityHandler(Stats s, CombatTable t, Character c, CalculationOptionsDPSDK opts)
        {
            stats = new Stats();
            stats.Accumulate(s);
            combatTable = t;
            character = c;
            calcOpts = opts;
            talents = character.DeathKnightTalents;
            initialize();
        }
        public void initialize()
        {
            BS = new BloodStrike(character, stats, calcOpts, combatTable, talents);
            DC = new DeathCoil(character, stats, calcOpts, combatTable, talents);
            IT = new IcyTouch(character, stats, calcOpts, combatTable, talents);
            PS = new PlagueStrike(character, stats, calcOpts, combatTable, talents);
            SS = new ScourgeStrike(character, stats, calcOpts, combatTable, talents);
            HB = new HowlingBlast(character, stats, calcOpts, combatTable, talents);
            FS = new FrostStrike(character, stats, calcOpts, combatTable, talents);
            OB = new Obliterate(character, stats, calcOpts, combatTable, talents);
            HS = new HeartStrike(character, stats, calcOpts, combatTable, talents);
            DS = new DeathStrike(character, stats, calcOpts, combatTable, talents);
            BB = new BloodBoil(character, stats, calcOpts, combatTable, talents);
            FF = new Disease(character, stats, calcOpts, combatTable, talents);
            BP = new Disease(character, stats, calcOpts, combatTable, talents);
            BS.DamageMod = 1;
            DC.DamageMod = 1;
            IT.DamageMod = 1;
            PS.DamageMod = 1;
            SS.DamageMod = 1;
            SS.SecondaryDamageMod = 1;
            HB.DamageMod = 1;
            FS.DamageMod = 1;
            OB.DamageMod = 1;
            HS.DamageMod = 1;
            DS.DamageMod = 1;
            BB.DamageMod = 1;
            FF.DamageMod = 1;
            BP.DamageMod = 1;

            double PhysicalMod = stats.BonusPhysicalDamageMultiplier;
            double FrostMod = stats.BonusFrostDamageMultiplier;
            double ShadowMod = stats.BonusShadowDamageMultiplier;
            if (character.ActiveBuffsContains("Crypt Fever"))
            {
                BP.DamageMod *= 1.3d;
                FF.DamageMod *= 1.3d;
            }
            else
            {
                BP.DamageMod *= 1d + talents.CryptFever * 0.1d;
                FF.DamageMod *= 1d + talents.CryptFever * 0.1d;
            }
            if (stats.DiseasesCanCrit > 0)
            {
                float DiseaseCritDmgMult = 0.5f * (2f + stats.BonusCritMultiplier);
                float DiseaseCrit = 1f + combatTable.spellCrits;
                BP.DamageMod *= DiseaseCrit * DiseaseCritDmgMult;
            }
            if (talents.WanderingPlague > 0)
            {
                float modifier = 1f + (combatTable.physCrits * (talents.WanderingPlague / 3f));
                BP.DamageMod *= modifier;
                FF.DamageMod *= modifier;
            }
            BS.DamageMod *=
                combatTable.physicalMitigation *
                PhysicalMod *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BloodOfTheNorth * .10 / 3d) *
                (1d + talents.BloodyStrikes * .05d) *
                (1d + talents.BloodyVengeance * .03d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.Hysteria * 0.2 / 180) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0));
            DC.DamageMod *=
                (1d + talents.BlackIce * .02d) *
                ShadowMod *
                .94 *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + (talents.GlyphofDarkDeath ? .15d : 0d)) *
                (1d + talents.Morbidity * .05d) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d) *
                (1d - combatTable.spellResist);
            IT.DamageMod *=
                (1d + talents.BlackIce * .02d) *
                FrostMod *
                .94 *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.GlacierRot * .2 / 3) *
                (1d + talents.ImprovedIcyTouch * .05d) *
                (1d + talents.MercilessCombat * .06 * 0.3) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d);
            PS.DamageMod *=
                combatTable.physicalMitigation *
                PhysicalMod *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BloodyVengeance * .03d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.Hysteria * .2d / 180) *
                (1d + talents.Outbreak * .1d) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0));
            SS.DamageMod *=
                combatTable.physicalMitigation *
                PhysicalMod *
                (1d + talents.Outbreak * 0.2d / 3) *
                (1d + talents.BloodyVengeance * .03d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.TwoHandedWeaponSpecialization * .02d) *
                (1d + stats.BonusScourgeStrikeMultiplier);
            SS.SecondaryDamageMod *=
                0.94d *// Partial Resist
                ShadowMod *
                (1d + (calcOpts.rotation.presence == CalculationOptionsDPSDK.Presence.Blood ? .15d : 0d) +
                    (talents.Desolation * .05d) +
                    (talents.BoneShield * .02d) +
                    (talents.BlackIce * .02d)) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0));
            HB.DamageMod *=
                FrostMod *
                .94 *
                (1d + talents.BlackIce * .02d) *
                (1d + talents.GlacierRot * .2 / 3) *
                (1d + talents.MercilessCombat * .06 * .3) *
                (1d + talents.TundraStalker * .03d) *
                (1d - combatTable.spellResist);
            FS.DamageMod *=
                FrostMod *
                .94 *
                (1d + talents.BlackIce * .02d) *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BloodOfTheNorth * .1/3) *
                (1d+ talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.GlacierRot * .2/3) *
                (1d + talents.MercilessCombat * .06 * .3) *
                (1d + talents.TundraStalker * .03d) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0)) *
                (1d - combatTable.missedSpecial - combatTable.dodgedSpecial);
            OB.DamageMod *=
                combatTable.physicalMitigation *
                PhysicalMod *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BloodyVengeance * .03d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.Hysteria * 0.2d / 180) *
                (1d + talents.MercilessCombat * .06 * .3) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0)) *
                (1d + stats.BonusObliterateMultiplier);
            HS.DamageMod *= 
                PhysicalMod *
                combatTable.physicalMitigation *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BloodyStrikes * .15d) *
                (1d + talents.BloodyVengeance * .03d) *
                (1d + talents.Hysteria * .2d / 180) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0)) *
                (1d + stats.BonusHeartStrikeMultiplier);
            DS.DamageMod *=
                PhysicalMod *
                combatTable.physicalMitigation *
                (1d + (talents.GlyphofDeathStrike ? 0.25 : 0)) *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BloodyVengeance * .03d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.Hysteria * 0.2d / 180) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d) *
                (1d + talents.ImprovedDeathStrike * .15d) *
                (1d + (!combatTable.DW ? talents.TwoHandedWeaponSpecialization * .02d : 0));
            BB.DamageMod *=
                ShadowMod *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BlackIce * .02d) *
                (1d + talents.BloodyStrikes * .1d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .02d);
            FF.DamageMod *=
                FrostMod *
                .94 *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BlackIce * .02d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + (talents.GlyphofIcyTouch ? 0.2 : 0)) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d);
            BP.DamageMod *=
                ShadowMod *
                .94 *
                (1d + talents.BloodGorged * .02d) *
                (1d + talents.BlackIce * .02d) *
                (1d + talents.BoneShield * .02d) *
                (1d + talents.Desolation * .01d) *
                (1d + talents.RageOfRivendare * .02d) *
                (1d + talents.TundraStalker * .03d);

        }
        public RuneAbility BS;
        public RuneAbility DC;
        public RuneAbility IT;
        public RuneAbility PS;
        public RuneAbility SS;
        public RuneAbility HB;
        public RuneAbility FS;
        public RuneAbility OB;
        public RuneAbility HS;
        public RuneAbility DS;
        public RuneAbility BB;
        public RuneAbility FF;
        public RuneAbility BP;
    }
}