using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    class ComparisonCalculationRetribution : ComparisonCalculationBase
    {
        private string _name = String.Empty;
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

        private float[] _subPoints = new float[] { 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DPSPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        private Item _item = null;
        public override Item Item
        {
            get { return _item; }
            set {  _item = value; }
		}

		private ItemInstance _itemInstance = null;
		public override ItemInstance ItemInstance
		{
			get { return _itemInstance; }
			set { _itemInstance = value; }
		}

        bool _equipped = false;
        public override bool Equipped
        {
            get { return _equipped; }
            set { _equipped = value; }
        }

        public override bool PartEquipped { get; set; }
    }
}
