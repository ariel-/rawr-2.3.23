using Rawr.Rogue;

namespace Rawr.UnitTests.Rogue
{
    internal class RogueTestCharacter : Character
    {
        public RogueTestCharacter():this(new RogueTalents()){}
        public RogueTestCharacter(RogueTalents talents)
        {
            Class = CharacterClass.Rogue;
            RogueTalents = talents;
            CurrentModel = "Rogue";
            CalculationOptions = new CalculationOptionsRogue();
        }
    }
}