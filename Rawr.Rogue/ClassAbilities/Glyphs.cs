using System.Collections.Generic;

namespace Rawr.Rogue.ClassAbilities {
    public class Glyphs {
        public static void Initialize(RogueTalents talents) { _talents = talents; }
        private static RogueTalents _talents;

        public static bool GlyphOfBackstab          { get { return _talents.GlyphOfBackstab; } }
        public static bool GlyphOfEviscerate        { get { return _talents.GlyphOfEviscerate; } }
        public static bool GlyphOfMutilate          { get { return _talents.GlyphOfMutilate; } }
        public static bool GlyphOfHungerforBlood    { get { return _talents.GlyphOfHungerforBlood; } }
        public static bool GlyphOfKillingSpree      { get { return _talents.GlyphOfKillingSpree; } }
        public static bool GlyphOfVigor             { get { return _talents.GlyphOfVigor; } }
        public static bool GlyphOfFanOfKnives       { get { return _talents.GlyphOfFanOfKnives; } }
        public static bool GlyphOfExposeArmor       { get { return _talents.GlyphOfExposeArmor; } }
        public static bool GlyphOfSinisterStrike    { get { return _talents.GlyphOfSinisterStrike; } }
        public static bool GlyphOfSliceandDice      { get { return _talents.GlyphOfSliceandDice; } }
        public static bool GlyphOfFeint             { get { return _talents.GlyphOfFeint; } }
        public static bool GlyphOfGhostlyStrike     { get { return _talents.GlyphOfGhostlyStrike; } }
        public static bool GlyphOfRupture           { get { return _talents.GlyphOfRupture; } }
        public static bool GlyphOfBladeFlurry       { get { return _talents.GlyphOfBladeFlurry; } }
        public static bool GlyphOfAdrenalineRush    { get { return _talents.GlyphOfAdrenalineRush; } }
        public static bool GlyphOfEvasion           { get { return _talents.GlyphOfEvasion; } }
        public static bool GlyphOfGarrote           { get { return _talents.GlyphOfGarrote; } }
        public static bool GlyphOfGouge             { get { return _talents.GlyphOfGouge; } }
        public static bool GlyphOfSap               { get { return _talents.GlyphOfSap; } }
        public static bool GlyphOfSprint            { get { return _talents.GlyphOfSprint; } }
        public static bool GlyphOfAmbush            { get { return _talents.GlyphOfAmbush; } }
        public static bool GlyphOfCripplingPoison   { get { return _talents.GlyphOfCripplingPoison; } }
        public static bool GlyphOfHemorrhage        { get { return _talents.GlyphOfHemorrhage; } }
        public static bool GlyphOfPreparation       { get { return _talents.GlyphOfPreparation; } }
        public static bool GlyphOfShadowDance       { get { return _talents.GlyphOfShadowDance; } }
        public static bool GlyphOfDeadlyThrow       { get { return _talents.GlyphOfDeadlyThrow; } }
        public static bool GlyphOfCloakOfShadows    { get { return _talents.GlyphOfCloakOfShadows; } }
        public static bool GlyphOfTricksOfTheTrade  { get { return _talents.GlyphOfTricksOfTheTrade; } }

        //minor glyphs
        public static bool GlyphOfBlurredSpeed      { get { return _talents.GlyphOfBlurredSpeed; } }
        public static bool GlyphOfPickPocket        { get { return _talents.GlyphOfPickPocket; } }
        public static bool GlyphOfPickLock          { get { return _talents.GlyphOfPickLock; } }
        public static bool GlyphOfDistrict          { get { return _talents.GlyphOfDistrict; } }
        public static bool GlyphOfVanish            { get { return _talents.GlyphOfVanish; } }
        public static bool GlyphOfSafeFall          { get { return _talents.GlyphOfSafeFall; } }
    }
}