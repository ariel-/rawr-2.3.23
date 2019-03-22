using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rawr.Warlock {

    public class SimulatedStat {

        public List<float> Values = new List<float>();
        public List<float> Weights = new List<float>();
        private float Result;
        private bool Solved;

        public void AddSample(float value, float weight) {

            Debug.Assert(!Solved);
            Values.Add(value);
            Weights.Add(weight);
        }

        public float GetValue() {

            if (!Solved) {
                Result = Utilities.GetWeightedSum(Values, Weights);
                Solved = true;
            }
            return Result;
        }

        public float GetTotalWeight() {

            float sum = 0f;
            foreach (float w in Weights) {
                sum += w;
            }
            return sum;
        }
    }
}
