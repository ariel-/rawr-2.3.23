#region Old Enchants
/*                   
            if (stats.MongooseProc > 0 | stats.BerserkingProc > 0)
            {
                if (character.MainHandEnchant != null)
                {
                    float whiteAttacksPerSecond = swingsPerSMHMelee * (1f - chanceWhiteMiss - chanceDodge);
                    if (character.MainHandEnchant.Id == 2673) // Mongoose Enchant
                    {
                        float timeBetweenMongooseProcs = 60f / (whiteAttacksPerSecond + yellowAttacksPerSecond);
                        float mongooseUptime = 15f / timeBetweenMongooseProcs;
                        float mongooseAgility = 120f * mongooseUptime * (1 + stats.BonusAgilityMultiplier);
                        chanceCrit = Math.Min(0.75f, chanceCrit + StatConversion.GetCritFromAgility(mongooseAgility, character.Class));
                        attackPower += mongooseAgility * (1 + stats.BonusAttackPowerMultiplier);
                        basecs.HastedMHSpeed /= 1f + (0.02f * mongooseUptime);
                    }
                    if (character.MainHandEnchant.Id == 3789) // Berserker Enchant
                    {
                        float timeBetweenBerserkingProcs = 45f / (whiteAttacksPerSecond + yellowAttacksPerSecond);
                        float berserkingUptime = 15f / timeBetweenBerserkingProcs;
                        attackPower += 400f * berserkingUptime * (1 + stats.BonusAttackPowerMultiplier);
                    }
                }
                if (character.OffHandEnchant != null && character.ShamanTalents.DualWield == 1)
                {
                    float whiteAttacksPerSecond = swingsPerSOHMelee * (1f - chanceWhiteMiss - chanceDodge);
                    if (character.OffHandEnchant.Id == 2673)  // Mongoose Enchant
                    {
                        float timeBetweenMongooseProcs = 60f / (whiteAttacksPerSecond + yellowAttacksPerSecond);
                        float mongooseUptime = 15f / timeBetweenMongooseProcs;
                        float mongooseAgility = 120f * mongooseUptime * (1 + stats.BonusAgilityMultiplier);
                        chanceCrit = Math.Min(0.75f, chanceCrit + StatConversion.GetCritFromAgility(mongooseAgility, character.Class));
                        attackPower += mongooseAgility * (1 + stats.BonusAttackPowerMultiplier);
                        basecs.HastedOHSpeed /= 1f + (0.02f * mongooseUptime);
                    }
                    if (character.OffHandEnchant.Id == 3789) // Berserker Enchant
                    {
                        float timeBetweenBerserkingProcs = 45f / (whiteAttacksPerSecond + yellowAttacksPerSecond);
                        float berserkingUptime = 15f / timeBetweenBerserkingProcs;
                        attackPower += 400f * berserkingUptime * (1 + stats.BonusAttackPowerMultiplier);
                    }
                }
            }
 */
#endregion

        #region Get Race Stats
/*
        private Stats GetRaceStats(Character character)
        {
            Stats statsRace = new Stats()
            {
                Mana = 4116f,
                AttackPower = 140f,
                SpellCrit = 0.0220f, 
                PhysicalCrit = 0.0292f
            };

            switch (character.Race)
            {
                case CharacterRace.Draenei:
                    statsRace.Health = 6305f;
                    statsRace.Strength = 121f;
                    statsRace.Agility = 71f;
                    statsRace.Stamina = 135f;
                    statsRace.Intellect = 129f;
                    statsRace.Spirit = 145f;
                    break;

                case CharacterRace.Tauren:
                    statsRace.Health = 6313f;
                    statsRace.BonusStaminaMultiplier = .05f;
                    statsRace.Strength = 125f;
                    statsRace.Agility = 69f;
                    statsRace.Stamina = 138f;
                    statsRace.Intellect = 123f;
                    statsRace.Spirit = 145f;
                    break;

                case CharacterRace.Orc:
                    statsRace.Health = 6305f;
                    statsRace.Strength = 123f;
                    statsRace.Agility = 71f;
                    statsRace.Stamina = 138f;
                    statsRace.Intellect = 125f;
                    statsRace.Spirit = 146f;
                    break;

                case CharacterRace.Troll:
                    statsRace.Health = 6305f;
                    statsRace.Strength = 121f;
                    statsRace.Agility = 76f;
                    statsRace.Stamina = 137f;
                    statsRace.Intellect = 124f;
                    statsRace.Spirit = 144f;
                    break;
            }
            return statsRace;
        }
*/
        #endregion

/*

        private Stats ApplyTalents(Character character, Stats stats, Stats gear) // also includes basic class benefits
        {
            if (gear != null)
            {
                int AK = character.ShamanTalents.AncestralKnowledge;
                float intBase = (float)Math.Floor((float)(stats.Intellect * (1 + stats.BonusIntellectMultiplier) * (1 + .02f * AK))); // added fudge factor because apparently Visual Studio can't multiply 125 * 1.04 to get 130.
                float intBonus = (float)Math.Floor((float)(gear.Intellect * (1 + gear.BonusIntellectMultiplier) * (1 + .02f * AK)));
                stats += gear;
                stats.Intellect = (float)Math.Floor((float)(intBase + intBonus));
            }
            
            stats.Mana += 15f * stats.Intellect;
            stats.Health += 10f * stats.Stamina;
            stats.Expertise += 3 * character.ShamanTalents.UnleashedRage;
            
            int MQ = character.ShamanTalents.MentalQuickness;
            stats.AttackPower += AddAPFromStrAgiInt(character, stats.Strength, stats.Agility, stats.Intellect); 
            stats.AttackPower = (float)Math.Floor((float)(stats.AttackPower * (1f + stats.BonusAttackPowerMultiplier)));
            stats.SpellPower = (float)Math.Floor((float)(stats.SpellPower + (stats.AttackPower * .1f * MQ * (1f + stats.BonusSpellPowerMultiplier))));
            return stats;
        }
*/

// old WF model - aka Flat Windfury Society
/* 
float windfuryTimeToFirstHit = hastedMHSpeed - (3 % hastedMHSpeed);
//later -- //windfuryTimeToFirstHit = hasted
wfProcsPerSecond = 1f / (3f + windfuryTimeToFirstHit + ((avgHitsToProcWF - 1) * hitsThatProcWFPerS));
*/
/*
                // new WF model - slighly curved Windfury Society
                float maxExpectedWFPerFight = hitsThatProcWFPerS * chanceToProcWFPerHit * fightLength;
                float ineligibleSeconds = maxExpectedWFPerFight * (3f - hastedMHSpeed);
                float expectedWFPerFight = hitsThatProcWFPerS * chanceToProcWFPerHit * (fightLength - ineligibleSeconds);
                wfProcsPerSecond = expectedWFPerFight / fightLength;
                hitsPerSWF = 2f * wfProcsPerSecond * (1f - chanceYellowMissMH);
*/
/*
                // new Stationary Distribution WF model - with Markov Chains - idea inspired by Kavan
                float avTimeforWFHit = hastedMHSpeed < 1.5f ? 
                        (1 / (1 + chanceToProcWFPerHit)) * hastedMHSpeed + 2 * (chanceToProcWFPerHit / (1 + chanceToProcWFPerHit)) * hastedMHSpeed :
                        (1 / (1 + chanceToProcWFPerHit)) * hastedMHSpeed +     (chanceToProcWFPerHit / (1 + chanceToProcWFPerHit)) * hastedMHSpeed;
                wfProcsPerSecond = avTimeforWFHit == 0 ? 0f : hitsThatProcWFPerS / (avTimeforWFHit * (hastedMHSpeed < 1.5f ? 4 : 3));
                hitsPerSWF = 2f * wfProcsPerSecond * (1f - chanceYellowMissMH);

/*
CombatStats.cs
        private void CalculateAbilities()
        {
            _gcd = Math.Max(1.0f, 1.5f * (1f - StatConversion.GetSpellHasteFromRating(_stats.HasteRating)));
            int deadTimes = 0;
            string name = "";
            for (float timeElapsed = 0f; timeElapsed < FightLength; timeElapsed += _gcd)
            {
                bool abilityUsed = false;
                foreach (Ability ability in abilities)
                {
                    if (ability.OffCooldown(timeElapsed))
                    {
                        ability.AddUse(timeElapsed, _calcOpts.AverageLag / 1000f);
                        abilityUsed = true;
                        name = ability.Name;
                        break;
                    }
                }
                if (!abilityUsed)
                {
                    deadTimes++;
                    name = "Deadtime";
                }
                System.Diagnostics.Debug.Print("Time: {0} - FS {1}, {2} - LB {3}, {4} - SS {5}, {6} - ES {7}, {8} - LL {9}, {10} - LS {11}, {12} - MT {13}, {14} - used {15}",
                    timeElapsed, 
                    abilities[0].Uses, abilities[0].CooldownOver,
                    abilities[1].Uses, abilities[1].CooldownOver,
                    abilities[2].Uses, abilities[2].CooldownOver,
                    abilities[3].Uses, abilities[3].CooldownOver,
                    abilities[4].Uses, abilities[4].CooldownOver,
                    abilities[5].Uses, abilities[5].CooldownOver,
                    abilities[6].Uses, abilities[6].CooldownOver, name); 
            }
            // at this stage abilities now contains the number of procs per fight for each ability.
        }
*/

/*
        public List<GemmingTemplate> addJewelerTemplates(int metagem, bool enabled)
        {
            return new List<GemmingTemplate>() { 
            	new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Expertise
					RedId = precise[3], YellowId = precise[3], BlueId = precise[3], PrismaticId = precise[3], MetaId = metagem },

            	new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Hit
					RedId = rigid[3], YellowId = rigid[3], BlueId = rigid[3], PrismaticId = rigid[3], MetaId = metagem },

            	new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Attack Power
					RedId = bright[3], YellowId = bright[3], BlueId = bright[3], PrismaticId = bright[3], MetaId = metagem },

                new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Agility
					RedId = delicate[3], YellowId = delicate[3], BlueId = delicate[3], PrismaticId = delicate[3], MetaId = metagem },

                new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Crit
					RedId = smooth[3], YellowId = smooth[3], BlueId = smooth[3], PrismaticId = smooth[3], MetaId = metagem },

              	new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Haste
					RedId = quick[3], YellowId = quick[3], BlueId = quick[3], PrismaticId = quick[3], MetaId = metagem },

              	new GemmingTemplate() { Model = "Enhance", Group = "Jeweler", Enabled = enabled, //Max Armour Penetration
					RedId = fractured[3], YellowId = fractured[3], BlueId = fractured[3], PrismaticId = fractured[3], MetaId = metagem },
             };
        }
*/

/* Priorities

        public void CalculateAbilities()
        {
            float gcd = 1.5f;
            string name = "";
            int deadtimes = 0;
            int totalAbilityUses = 0;
            float timeElapsed = 0f;
            float averageLag = _calcOpts.AverageLag / 1000f;
            PriorityQueue<Ability> queue = new PriorityQueue<Ability>();
            foreach (Ability ability in _abilities)
                queue.Enqueue(ability);
            while (queue.Count > 0)
            {
                Ability ability = queue.Dequeue();
                if (ability.MissedCooldown(timeElapsed))
                {   // we missed a cooldown so set new cooldown to current time
                    ability.UpdateCooldown(timeElapsed);
                    deadtimes++;
                    name = "deadtime for " + ability.Name;
                }
                else
                {
                    // do something to ability
                    ability.Use(timeElapsed); // 
                    gcd = ability.GCD;
                    name = ability.Name;
                    timeElapsed += gcd + averageLag;
                    totalAbilityUses++;
                }
                if (ability.CooldownOver < fightLength)
                {  // adds ability back into queue if its available again before end of fight
                    queue.Enqueue(ability);
                }
     //           DebugPrint(_abilities, timeElapsed - gcd - averageLag, name);
            }
            // at this stage abilities now contains the number of procs per fight for each ability as a whole number
            // to avoid big stepping problems work out the fraction of the ability use based on how long until next 
            // use beyond fight duration.
            foreach (Ability ability in _abilities)
            {
                float overrun = ability.Duration - (ability.CooldownOver - fightLength);
                ability.AddUses(overrun / ability.Duration);
            }
         //   DebugPrint(_abilities, timeElapsed - gcd - averageLag, "Final uses");
        }
        
        public void OldCalculateAbilities()
        {
            float gcd = 1.5f;
            string name = "";
            int totalAbilityUses = 0;
            int deadtimes = 0;
            float averageLag = _calcOpts.AverageLag / 1000f;
            for (float timeElapsed = 0f; timeElapsed < fightLength; timeElapsed += gcd) 
            {
                gcd = 0.1f; // set GCD to small value step for dead time as dead time doesn't use a GCD its just waiting time
                name = "deadtime";
                foreach (Ability ability in _abilities)
                {
                    if (ability.OffCooldown(timeElapsed))
                    {
                        ability.Use(timeElapsed);
                        gcd = ability.GCD;
                        name = ability.Name;
                        totalAbilityUses++;
                        timeElapsed += averageLag;
                        break;
                    }
                }
                if (name.Equals("deadtime")) deadtimes++;
                DebugPrint(_abilities, timeElapsed, name);
            }
            // at this stage abilities now contains the number of procs per fight for each ability as a whole number
            // to avoid big stepping problems work out the fraction of the ability use based on how long until next 
            // use beyond fight duration.
            foreach (Ability ability in _abilities)
            {
                float overrun = ability.Duration - (ability.CooldownOver - fightLength);
                ability.AddUses(overrun / ability.Duration);
            }
            DebugPrint(_abilities, fightLength, "Final uses");
        }
*/