using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    class ComparisonCalculationMage : ComparisonCalculationBase
    {
        private string _name = string.Empty;
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

        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DpsRating
        {
            get
            {
                return _subPoints[0];
            }
        }

        public float SurvivabilityRating
        {
            get
            {
                return _subPoints[1];
            }
        }

        private Item _item = null;
        public override Item Item
        {
            get { return _item; }
            set { _item = value; }
		}

		private ItemInstance _itemInstance = null;
		public override ItemInstance ItemInstance
		{
			get { return _itemInstance; }
			set { _itemInstance = value; }
		}

        private bool _equipped = false;
        public override bool Equipped
        {
            get { return _equipped; }
            set { _equipped = value; }
        }

        public override bool PartEquipped { get; set; }

        public override string ToString()
        {
            if (_subPoints.Length == 2)
            {
                return string.Format("{0:F}: ({1:F} Dps, {2:F} Survivability)", Name, Math.Round(DpsRating), Math.Round(SurvivabilityRating));
            }
            return null;
        }
    }
}
