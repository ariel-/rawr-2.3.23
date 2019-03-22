using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter
{

    public class RotationInfo
    {
        public double rotationDmg = 0;
        public double rotationTime = 0;

        public double DPS
        {
            get { return rotationDmg / rotationTime; }
        }
    }

    public class RotationShot
    {
        public double cooldown = 0.0;
        public double casttime = 1.5;
        public double nextcast = 0.0;
        public Shots type;
    }


    public class ShotRotationCalculator
    {
        Character character;
        CharacterCalculationsHunter calculatedStats;
        CalculationOptionsHunter options;
        double hawkRAPBonus;
        double totalStaticHaste;
        double effectiveRAPAgainstMob;
        double abilitiesCritDmgModifier;
        double yellowCritDmgModifier;
        double weaponDamageAverage;
        double ammoDamage;
        double talentModifiers;
        double armorReduction;
        double talentedArmorReduction;

        HunterRatings ratings;


        public ShotRotationCalculator(Character character, CharacterCalculationsHunter calculatedStats, CalculationOptionsHunter options, double totalStaticHaste, double effectiveRAPAgainstMob, double abilitiesCritDmgModifier, double yellowCritDmgModifier, double weaponDamageAverage, double ammoDamage, double talentModifiers)
        {
            ratings = new HunterRatings();

            this.character = character;
            this.calculatedStats = calculatedStats;
            this.options = options;
            this.hawkRAPBonus = ratings.HAWK_BONUS_AP * (1.0 + 0.5 * character.HunterTalents.AspectMastery);
            this.totalStaticHaste = totalStaticHaste;
            this.effectiveRAPAgainstMob = effectiveRAPAgainstMob;
            this.abilitiesCritDmgModifier = abilitiesCritDmgModifier;
            this.yellowCritDmgModifier = yellowCritDmgModifier;
            this.weaponDamageAverage = weaponDamageAverage;
            this.ammoDamage = ammoDamage;
            this.talentModifiers = talentModifiers;

			int targetArmor = options.TargetArmor;
			this.armorReduction = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor,
				calculatedStats.BasicStats.ArmorPenetration, 0f, calculatedStats.BasicStats.ArmorPenetrationRating);
			//double targetArmor = (options.TargetArmor - calculatedStats.BasicStats.ArmorPenetration) * (1.0 - calculatedStats.BasicStats.ArmorPenetrationRating / (ratings.ARP_RATING_PER_PERCENT * 100.0));
            //this.armorReduction = 1.0 - (targetArmor / (467.5 * options.TargetLevel + targetArmor - 22167.5));

            //reducedArmor *= (1f - character.HunterTalents.PiercingShots * 0.02f);
            
            this.talentedArmorReduction = 1f - StatConversion.GetArmorDamageReduction(character.Level, targetArmor,
				calculatedStats.BasicStats.ArmorPenetration, character.HunterTalents.PiercingShots * 0.02f,
				calculatedStats.BasicStats.ArmorPenetrationRating);
			//this.talentedArmorReduction = 1.0 - (targetArmor / (467.5 * options.TargetLevel + targetArmor - 22167.5));
        }


        public RotationInfo SteadyRotation()
        {
            RotationInfo info = new RotationInfo();
            ShotSteady(info);

            return info;
        }

        public RotationInfo ASSteadyRotation(int steadys)
        {
            RotationInfo info = new RotationInfo();

            ShotArcane(info, steadys);
            for (int i = 0; i < steadys; i++)
            {
                ShotSteady(info);
            }

            if (info.rotationTime < 6.0)
            {
                info.rotationTime = 6.0;
            }

            return info;
        }


        public RotationInfo ASSteadySerpentRotation()
        {
            RotationInfo info = new RotationInfo();

            ShotSerpentSting(info);
            ShotArcane(info, 3);
            ShotSteady(info);
            ShotSteady(info);
            ShotSteady(info);
            ShotArcane(info, 3);
            ShotSteady(info);
            ShotSteady(info);
            ShotSteady(info);

            return info;

        }

        public RotationInfo ExpSteadySerpRotation()
        {
            RotationInfo info = new RotationInfo();

            ShotExplosive(info);
            ShotSerpentSting(info);
            ShotSteady(info);
            ShotSteady(info);
            ShotExplosive(info);
            ShotSteady(info);
            ShotSteady(info);
            ShotSteady(info);

            if (info.rotationTime < 12.0)
            {
                info.rotationTime = 12.0;
            }


            return info;
        }

        public RotationInfo ChimASSteadyRotation()
        {
            RotationInfo info = new RotationInfo();

            //TODO: Update helptext
            double steadyTime = 10.0 - 1.5 - 1.5; // Chim CD - 2x GCD
            int steadyAmount = (int)Math.Ceiling(steadyTime/1.5);

            ShotChimera(info, steadyAmount);
            ShotArcane(info, 0);

            for (int i = 0; i < steadyAmount; i++)
            {
                ShotSteady(info);
            }

            if (info.rotationTime < 10.0)
            {
                info.rotationTime = 10.0;
            }

            return info;

        }

        protected void ShotChimera(RotationInfo info, int steadyshots)
        {
            double chimCritDmgModifier = yellowCritDmgModifier + 0.02 * character.HunterTalents.MarkedForDeath;
            double critHitModifier = (calculatedStats.BasicStats.PhysicalCrit * chimCritDmgModifier + 1.0) * calculatedStats.BasicStats.PhysicalHit;

            double chimeraDmg = weaponDamageAverage * 1.25;

            // Imp Steady Shot only affects Bullet Damage
            double impSteadyChance = 1.0 - Math.Pow(1.0 - character.HunterTalents.ImprovedSteadyShot * 0.05, steadyshots);
            chimeraDmg *= 1.0 + 0.15 * impSteadyChance;
            //chimeraDmg *= this.talentedArmorReduction; // Both dmg components seem to be nature damage


            double serpentStingDmg = (effectiveRAPAgainstMob + hawkRAPBonus) * ratings.STEADY_AP_SCALE + ratings.SERPENT_BONUS_DMG;
            double serpentTalentModifiers = 1.0 + character.HunterTalents.ImprovedStings * 0.10;
            serpentStingDmg *= serpentTalentModifiers;
            
            chimeraDmg += serpentStingDmg * 0.4;
            chimeraDmg *= critHitModifier * talentModifiers;

            info.rotationDmg += chimeraDmg + 2.0/3.0 * serpentStingDmg * talentModifiers; // Add Serpent Sting dmg for 10 sec
            info.rotationTime += 1.5 + options.Latency;

        }

        protected void ShotExplosive(RotationInfo info)
        {
            double explosiveCrit = calculatedStats.BasicStats.PhysicalCrit + 0.03 * character.HunterTalents.TNT + 0.02 * character.HunterTalents.SurvivalInstincts;
            double critHitModifier = (explosiveCrit * abilitiesCritDmgModifier + 1.0) * calculatedStats.BasicStats.PhysicalHit;

            double explosiveShotDmg = ratings.EXPLOSIVE_AP_SCALE * (effectiveRAPAgainstMob + hawkRAPBonus) + ratings.EXPLOSIVE_BONUS_DMG;

            explosiveShotDmg *= critHitModifier * talentModifiers;


            info.rotationDmg += explosiveShotDmg * 3.0;
            info.rotationTime += 1.5 + options.Latency;

        }

        protected void ShotSteady(RotationInfo info)
        {
            double steadyCritDmgModifier = abilitiesCritDmgModifier + 0.02 * character.HunterTalents.MarkedForDeath;
            double critHitModifier = (calculatedStats.BasicStats.PhysicalCrit * steadyCritDmgModifier + 1.0) * calculatedStats.BasicStats.PhysicalHit;
            double steadyShotDmg = weaponDamageAverage + ammoDamage + (effectiveRAPAgainstMob + hawkRAPBonus) * ratings.STEADY_AP_SCALE + ratings.STEADY_BONUS_DMG;

            steadyShotDmg *= critHitModifier * talentModifiers;

            double steadyShotCastTime = calculatedStats.SteadySpeed;

            info.rotationDmg += steadyShotDmg * talentedArmorReduction;
            info.rotationTime += (steadyShotCastTime > 1.5 ? steadyShotCastTime : 1.5) + options.Latency;
        }

        protected void ShotArcane(RotationInfo info, int steadyshots)
        {
            double arcaneShotCrit = calculatedStats.BasicStats.PhysicalCrit + 0.02 * character.HunterTalents.SurvivalInstincts;

            double critHitModifier = (arcaneShotCrit * abilitiesCritDmgModifier + 1.0) * calculatedStats.BasicStats.PhysicalHit;
            double arcaneShotDmg = (effectiveRAPAgainstMob + hawkRAPBonus) * ratings.ARCANE_AP_SCALE + ratings.ARCANE_BONUS_DMG;

            double asTalentModifiers = 1.0 + character.HunterTalents.ImprovedArcaneShot * 0.05;

            double impSteadyChance = 1.0 - Math.Pow(1.0 - character.HunterTalents.ImprovedSteadyShot * 0.05, steadyshots);
            asTalentModifiers *= 1.0 + 0.15 * impSteadyChance;

            arcaneShotDmg *= critHitModifier * talentModifiers * asTalentModifiers;
            info.rotationDmg += arcaneShotDmg;
            info.rotationTime += 1.5 + options.Latency;
        }

        protected void ShotSerpentSting(RotationInfo info)
        {
            double serpentStingDmg = (effectiveRAPAgainstMob + hawkRAPBonus) * ratings.SERPENT_AP_SCALE + ratings.SERPENT_BONUS_DMG;

            double serpentTalentModifiers = 1.0 + character.HunterTalents.ImprovedStings * 0.10;

            serpentStingDmg *= calculatedStats.BasicStats.PhysicalHit * talentModifiers * serpentTalentModifiers;

            info.rotationDmg += serpentStingDmg;
            info.rotationTime += 1.5 + options.Latency;
        }

        protected void ShotMulti(RotationInfo info)
        {
            double critHitModifier = ((calculatedStats.BasicStats.PhysicalCrit + character.HunterTalents.ImprovedBarrage * 0.04) * abilitiesCritDmgModifier + 1.0) * calculatedStats.BasicStats.PhysicalHit;

            double shotDmg = (weaponDamageAverage + ratings.MULTI_BONUS_DMG) * critHitModifier;
            shotDmg *= talentModifiers * (1.0 + character.HunterTalents.Barrage * 0.04);

            info.rotationDmg += shotDmg * armorReduction;
            info.rotationTime += 1.5 + options.Latency;
        }

        protected void ShotAimed(RotationInfo info)
        {
            double critHitModifier = ((calculatedStats.BasicStats.PhysicalCrit + character.HunterTalents.ImprovedBarrage * 0.04) * abilitiesCritDmgModifier + 1.0) * calculatedStats.BasicStats.PhysicalHit;

            double shotDmg = (weaponDamageAverage + ratings.AIMED_BONUS_DMG) * critHitModifier;
            shotDmg *= talentModifiers * (1.0 + character.HunterTalents.Barrage * 0.04);

            info.rotationDmg += shotDmg * talentedArmorReduction;
            info.rotationTime += 1.5 + options.Latency;

        }



        public RotationInfo createCustomRotation()
        {
            List<RotationShot> shots = new List<RotationShot>();
            List<double> timings = new List<double>();

            if (options.ShotPriority1 != Shots.None)
            {
                shots.Add(createRotationShot(options.ShotPriority1));
            }
            if (options.ShotPriority2 != Shots.None)
            {
                shots.Add(createRotationShot(options.ShotPriority2));
            }
            if (options.ShotPriority3 != Shots.None)
            {
                shots.Add(createRotationShot(options.ShotPriority3));
            }
            /*
            if (options.ShotPriority4 != Shots.None)
            {
                shots.Add(createRotationShot(options.ShotPriority4));
            }
            */
            shots.Add(createRotationShot(Shots.SteadyShot));

            double currentTime = 0.0;
            List<Shots> rotation = new List<Shots>();

            int cycleLength;
            for (cycleLength = 1; cycleLength < 20; cycleLength++)
            {
                #region Step
                foreach (RotationShot s in shots)
                {
                    if (s.nextcast <= currentTime)
                    {
                        timings.Add(currentTime);
                        s.nextcast = currentTime + s.cooldown;
                        currentTime += s.casttime;
                        rotation.Add(s.type);
                        break;
                    }
                }

                foreach (RotationShot s in shots)
                {
                    if (s.nextcast <= currentTime)
                    {
                        timings.Add(currentTime);
                        s.nextcast = currentTime + s.cooldown;
                        currentTime += s.casttime;
                        rotation.Add(s.type);
                        break;
                    }
                }
                #endregion

                if (rotation[0] == rotation[cycleLength])
                {
                    bool cycle = true;
                    for (int i = 0; i < cycleLength; i++)
                    {
                        if (rotation[i] != rotation[cycleLength + i])
                        {
                            cycle = false;
                            break;
                        }
                    }
                    if (cycle)
                    {
                        break;
                    }
                }
            }

            double cycleTime = timings[cycleLength];

            rotation.RemoveRange(cycleLength, rotation.Count - cycleLength);
            timings.RemoveRange(cycleLength, rotation.Count - cycleLength);

            RotationInfo info = new RotationInfo();

            StringBuilder builder = new StringBuilder();
            builder.Append("*");

            for(int i = 0; i < cycleLength; i++)
            {
                builder.Append(timings[i].ToString("F1"));
                builder.Append("s ");
                builder.AppendLine(rotation[i].ToString());
                switch (rotation[i])
                {
                    case Shots.SteadyShot:
                        ShotSteady(info);
                        break;

                    case Shots.SerpentSting:
                        ShotSerpentSting(info);
                        break;

                    case Shots.MultiShot:
                        ShotMulti(info);
                        break;

                    case Shots.ExplosiveShot:
                        ShotExplosive(info);
                        break;

                    case Shots.ChimeraShot_Serpent:
                        ShotChimera(info, 0); // TODO: Improved Steady Shot
                        break;

                    case Shots.ArcaneShot:
                        ShotArcane(info, 0); // TODO: Improved Steady Shot
                        break;

                    case Shots.AimedShot:
                        ShotAimed(info);
                        break;
                }
            }

            calculatedStats.CustomRotation = builder.ToString();
            return info;
        }


        protected RotationShot createRotationShot(Shots type)
        {
            RotationShot shot = new RotationShot();
            switch (type)
            {
                case Shots.SteadyShot:
                    shot.casttime = (calculatedStats.SteadySpeed < 1.5 ? 1.5 : calculatedStats.SteadySpeed) + options.Latency;
                    shot.cooldown = 0.0;
                    shot.type = Shots.SteadyShot;
                    break;

                case Shots.SerpentSting:
                    shot.casttime = 1.5 + options.Latency;
                    shot.cooldown = 15.0;
                    shot.type = Shots.SerpentSting;
                    break;

                case Shots.MultiShot:
                    shot.casttime = 1.5 + options.Latency;
                    shot.cooldown = 10.0;
                    shot.type = Shots.MultiShot;
                    break;

                case Shots.ExplosiveShot:
                    shot.casttime = 1.5 + options.Latency;
                    shot.cooldown = 6.0;
                    shot.type = Shots.ExplosiveShot;
                    break;

                case Shots.ChimeraShot_Serpent:
                    shot.casttime = 1.5 + options.Latency;
                    shot.cooldown = 10.0;
                    shot.type = Shots.ChimeraShot_Serpent;
                    break;

                case Shots.ArcaneShot:
                    shot.casttime = 1.5 + options.Latency;
                    shot.cooldown = 6.0;
                    shot.type = Shots.ArcaneShot;
                    break;

                case Shots.AimedShot:
                    shot.casttime = 1.5 + options.Latency;
                    shot.cooldown = 10.0;
                    shot.type = Shots.AimedShot;
                    break;
            }
            return shot;
        }
    }

}
