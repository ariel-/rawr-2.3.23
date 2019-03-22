using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.Properties
{
    public class OptimizerSettings
    {
        
        static OptimizerSettings()
        {
            _default = new OptimizerSettings();

            _default.WarningsEnabled = true;
            _default.Thoroughness = 150;
            _default.OverrideRegem = false;
            _default.CalculationToOptimize = "";
            _default.OverrideReenchant = false;
            _default.OptimizationMethod = OptimizationMethod.GeneticAlgorithm;
            _default.TemplateGemsEnabled = true;
            _default.GreedyOptimizationMethod = GreedyOptimizationMethod.AllCombinations;
        }

        private static OptimizerSettings _default;
        public static OptimizerSettings Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public bool WarningsEnabled { get; set; }
        public int Thoroughness { get; set; }
        public bool OverrideRegem { get; set; }
        public string CalculationToOptimize { get; set; }
        public bool OverrideReenchant { get; set; }
        public Rawr.OptimizationMethod OptimizationMethod { get; set; }
        public bool TemplateGemsEnabled { get; set; }
        public Rawr.GreedyOptimizationMethod GreedyOptimizationMethod { get; set; }
        
    }
}