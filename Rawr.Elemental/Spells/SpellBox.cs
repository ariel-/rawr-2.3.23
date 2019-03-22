using System;

namespace Rawr.Elemental.Spells
{
    public enum SpellIndex
    {
        LightningBolt,
        ChainLightning,
        LavaBurst,
        LavaBurstFS,
        FlameShock,
        EarthShock,
        FrostShock,
        Thunderstorm,
        FireNova,
        SearingTotem,
        MagmaTotem,
        FireElemental
    }

    /// <summary>
    /// A container class to hold an instance of every spell that can be used in a rotation. It also supplies methods to update these spells to new stats and talents.
    /// The idea is to save memory by reusing the spells.
    /// </summary>
    public class SpellBox
    {
        private Spell[] spells;
        private bool EMapplied = false;

        public SpellBox(ISpellArgs args)
        {
            spells = new Spell[12];
            spells[(int)SpellIndex.LightningBolt] = new LightningBolt(args);
            spells[(int)SpellIndex.ChainLightning] = new ChainLightning(args);
            spells[(int)SpellIndex.LavaBurst] = new LavaBurst(args, 0);
            spells[(int)SpellIndex.LavaBurstFS] = new LavaBurst(args, 1);
            spells[(int)SpellIndex.FlameShock] = new FlameShock(args);
            spells[(int)SpellIndex.EarthShock] = new EarthShock(args);
            spells[(int)SpellIndex.FrostShock] = new FrostShock(args);
            spells[(int)SpellIndex.Thunderstorm] = new Thunderstorm(args);
            spells[(int)SpellIndex.FireNova] = new FireNova(args);
            spells[(int)SpellIndex.SearingTotem] = new SearingTotem(args);
            spells[(int)SpellIndex.MagmaTotem] = new MagmaTotem(args);
            spells[(int)SpellIndex.FireElemental] = new FireElemental(args);
        }

        public void Update(ISpellArgs args)
        {
            foreach (Spell s in spells)
            {
                if (s != null)
                    s.Update(args);
            }
            EMapplied = false;
        }

        public void ApplyEM(float modifier)
        {
            if (EMapplied)
                return;

            foreach (Spell s in spells)
            {
                if (s != null)
                    s.ApplyEM(modifier);
            }
            EMapplied = true;
        }

        public Spell Get(SpellIndex spellIndex)
        {
            return spells[(int)spellIndex];
        }

        /// <summary>
        /// The underlying spell array.
        /// This is a reference, not a copy.
        /// </summary>
        public Spell[] Array
        {
            get { return spells; }
        }

        #region Properties for typed access
        public LightningBolt LB
        {
            get { return (LightningBolt)spells[(int)SpellIndex.LightningBolt]; }
        }

        public ChainLightning CL
        {
            get { return (ChainLightning)spells[(int)SpellIndex.ChainLightning]; }
        }

        public LavaBurst LvB
        {
            get { return (LavaBurst)spells[(int)SpellIndex.LavaBurst]; }
        }

        public LavaBurst LvBFS
        {
            get { return (LavaBurst)spells[(int)SpellIndex.LavaBurstFS]; }
        }

        public FlameShock FS
        {
            get { return (FlameShock)spells[(int)SpellIndex.FlameShock]; }
        }

        public EarthShock ES
        {
            get { return (EarthShock)spells[(int)SpellIndex.EarthShock]; }
        }

        public FrostShock FrS
        {
            get { return (FrostShock)spells[(int)SpellIndex.FrostShock]; }
        }
        
        public Thunderstorm TS
        {
            get { return (Thunderstorm)spells[(int)SpellIndex.Thunderstorm]; }
        }

        public FireNova FN
        {
            get { return (FireNova)spells[(int)SpellIndex.FireNova]; }
        }

        public SearingTotem ST
        {
            get { return (SearingTotem)spells[(int)SpellIndex.SearingTotem]; }
        }

        public MagmaTotem MT
        {
            get { return (MagmaTotem)spells[(int)SpellIndex.MagmaTotem]; }
        }

        public FireElemental FE
        {
            get { return (FireElemental)spells[(int)SpellIndex.FireElemental]; }
        }
        #endregion
    }
}
