using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Warlock {

    /// <summary>
    /// Data container class for the results of a comparison of two 
    /// CharacterCalculationsWarlock objects
    /// </summary>
    public class ComparisonCalculationWarlock : ComparisonCalculationBase {

        public override string Name { get; set; }

        public override string Description { get; set; }

        public override float OverallPoints { get; set; }

        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DpsPoints {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float PetDPSPoints {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public override Item Item { get; set; }

        public override ItemInstance ItemInstance { get; set; }

        public override bool Equipped { get; set; }
        public override bool PartEquipped { get; set; }
    }
}
//3456789 223456789 323456789 423456789 523456789 623456789 723456789 8234567890