namespace Rawr.Rogue.ClassAbilities
{
    public class TalentsAndGlyphs
    {
        public static void Initialize( RogueTalents talents)
        {
            Glyphs.Initialize(talents);
            Talents.Initialize(talents);
        }
    }
}