using System;

namespace Rawr.Rogue
{
    internal class ComparisonCalculationsRogue : ComparisonCalculationBase
    {
        private string _name = string.Empty;
        private float[] _subPoints = new[] {0f};

        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _desc = string.Empty;
        public override string Description
        {
            get { return _desc; }
            set { _desc = value; }
        }

        public override float OverallPoints { get; set; }

        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DpsPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

		public override Item Item { get; set; }

        public override ItemInstance ItemInstance { get; set; }

        public override bool Equipped { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: ({1}O {2}D)", Name, Math.Round(OverallPoints), Math.Round(DpsPoints));
        }
    }
}