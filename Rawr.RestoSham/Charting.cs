using System.Collections.Generic;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif

namespace Rawr.RestoSham
{
    /// <summary>
    /// Delegate function that lets us specify the calculator method for a chart
    /// </summary>
    /// <param name="character">Character object to chart</param>
    /// <param name="baseCalculations">The calculation object for the character</param>
    /// <returns>ICollection of the calculation comparison values</returns>
    delegate ICollection<ComparisonCalculationBase> ChartCalculator(Character character, CalculationsRestoSham baseCalculations);

    /// <summary>
    /// Class used by stat relative weights custom chart.
    /// </summary>
    sealed class StatRelativeWeight
    {
        public StatRelativeWeight(string name, Stats stat)
        {
            this.Stat = stat;
            this.Name = name;
            this.Change = 0f;
        }

        public Stats Stat { get; set; }
        public string Name { get; set; }
        public float Change { get; set; }
    }

    /// <summary>
    /// Class used to describe custom charts
    /// </summary>
    sealed class CustomChart
    {
        public string ChartName { get; set; }
        public ChartCalculator ChartCalculatorMethod { get; set; }
        public override string ToString()
        {
            return this.ChartName;
        }
    }

    /// <summary>
    /// Class used to define the available custom charts
    /// </summary>
    static class CustomCharts
    {
        static CustomChart[] Charts = {
            new CustomChart() { ChartName = "Mana Available per Second", ChartCalculatorMethod = ChartCalculators.CalculateMAPSChart },
            new CustomChart() { ChartName = "Healing Sequences", ChartCalculatorMethod = ChartCalculators.CalculateSequencesChart }
        };

#if !RAWR3
        public static string[] CustomRenderedChartNames = { 
            "Burst Stats Graph",
            "Sustained Stats Graph",
            "Overall Rating Stats Graph"
        };
#else
        public static string[] CustomRenderedChartNames = new string[0];
#endif

        public static Stats[] StatsGraphStatsList = new Stats[] {
            new Stats() { SpellPower = 1 },
            new Stats() { Mp5 = 1 },
            new Stats() { CritRating = 1 },
            new Stats() { HasteRating = 1 },
            new Stats() { Intellect = 1 }
        };
        public static Color[] StatsGraphColors = new Color[] { 
            Color.FromArgb(255, 255, 0, 0), 
            Color.FromArgb(255, 0, 0, 156), // DarkBlue doesnt work in Rawr3, using RGB instead
            Color.FromArgb(255, 255, 165, 0), 
            Color.FromArgb(255, 128, 128, 0), // Olive Doesn't work in Rawr3, using RGB instead
            Color.FromArgb(255, 154, 205, 50)
        };

        public static string[] CustomChartNames
        {
            get
            {
#if RAWR3
                return new string[0];
#else
                if (Charts == null || Charts.Length == 0)
                    return null;

                string[] chartNames = new string[Charts.Length];
                for (int i = 0; i < Charts.Length; i++)
                {
                    chartNames[i] = Charts[i].ToString();
                }
                return chartNames;
#endif
            }
        }
        public static ChartCalculator GetChartCalculator(string chartName)
        {
            foreach (CustomChart c in Charts)
            {
                if (c.ChartName == chartName)
                    return c.ChartCalculatorMethod;
            }
            return null;
        }
    }

    /// <summary>
    /// Class used to define the various calculator methods for the custom charts
    /// </summary>
    static class ChartCalculators
    {
        #region Chart Calculator Methods
        public static ICollection<ComparisonCalculationBase> CalculateMAPSChart(Character character, CalculationsRestoSham baseCalculations)
        {
            List<ComparisonCalculationBase> list = new List<ComparisonCalculationBase>();
            CharacterCalculationsRestoSham calc = baseCalculations.GetCharacterCalculations(character) as CharacterCalculationsRestoSham;
            if (calc == null)
                calc = new CharacterCalculationsRestoSham();

            StatRelativeWeight[] stats = new StatRelativeWeight[] {
                      new StatRelativeWeight("Intellect", new Stats() { Intellect = 10f }),
                      new StatRelativeWeight("Haste Rating", new Stats() { HasteRating = 10f }),
                      new StatRelativeWeight("Spellpower", new Stats() { SpellPower = 10f}),
                      new StatRelativeWeight("MP5", new Stats() { Mp5 = 10f }),
                      new StatRelativeWeight("Crit Rating", new Stats() { CritRating = 10f })};

            // Get the percentage total healing is changed by a change in a single stat:
            foreach (StatRelativeWeight weight in stats)
            {
                CharacterCalculationsRestoSham statCalc = (CharacterCalculationsRestoSham)baseCalculations.GetCharacterCalculations(character, null, weight.Stat);
                weight.Change = (statCalc.MAPS - calc.MAPS);
            }

            // Create the chart data points:
            foreach (StatRelativeWeight weight in stats)
            {
                ComparisonCalculationRestoSham comp = new ComparisonCalculationRestoSham(weight.Name);
                comp.OverallPoints = weight.Change;
                comp.SubPoints = new float[] { 0f, weight.Change, 0f };
                comp.Description = string.Format("If you added 10 more {0}.", weight.Name);
                list.Add(comp);
            }

            return list;
        }
        public static ICollection<ComparisonCalculationBase> CalculateSequencesChart(Character character, CalculationsRestoSham baseCalculations)
        {
            List<ComparisonCalculationBase> list = new List<ComparisonCalculationBase>();
            CalculationOptionsRestoSham originalOptions = character.CalculationOptions as CalculationOptionsRestoSham;
            if (originalOptions == null)
                originalOptions = new CalculationOptionsRestoSham();

            CalculationOptionsRestoSham opts = originalOptions;
            string[] styles = new string[] { "CH Spam", "HW Spam", "LHW Spam", "RT+HW", "RT+CH", "RT+LHW" };
            string[] descs = new string[] {
                        "All chain heal, all the time.  \nMana available for use per minute added to sustained.",
                        "All healing wave, all the time  \nMana available for use per minute added to sustained.",
                        "All lesser healing wave, all the time  \nMana available for use per minute added to sustained.",
                        "Riptide + Healing Wave.  \nMana available for use per minute added to sustained.", 
                        "Riptide + Chain Heal.  \nMana available for use per minute added to sustained.", 
                        "Riptide + Lesser Healing Wave.\nMana available for use per minute added to sustained." 
                    };
            for (int i = 0; i < styles.Length; i++)
            {
                opts.SustStyle = styles[i];
                opts.BurstStyle = styles[i];
                character.CalculationOptions = opts;
                CharacterCalculationsRestoSham statCalc = (CharacterCalculationsRestoSham)baseCalculations.GetCharacterCalculations(character);

                // normalize the mana a bit to make a better chart
                float mana = statCalc.ManaUsed / (opts.FightLength);

                ComparisonCalculationRestoSham hsComp = new ComparisonCalculationRestoSham(styles[i]);
                hsComp.OverallPoints = statCalc.BurstHPS + statCalc.SustainedHPS + mana;
                hsComp.SubPoints = new float[] { statCalc.BurstHPS, statCalc.SustainedHPS + mana, 0f };
                hsComp.Description = descs[i];
                list.Add(hsComp);
            }

            return list;
        }
        #endregion
    }
}
