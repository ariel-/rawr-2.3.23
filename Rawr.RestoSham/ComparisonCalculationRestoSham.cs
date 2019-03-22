using System;
using System.Collections.Generic;

namespace Rawr.RestoSham
{
    class ComparisonCalculationRestoSham : ComparisonCalculationBase
    {
        //
        // Constructors:
        //
        public ComparisonCalculationRestoSham()
            : base()
        {
        }

        public ComparisonCalculationRestoSham(string szName)
            : base()
        {
            this.Name = szName;
        }

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

        private float _overallPoints = 0.0f;
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


        private bool _bEquipped;
        public override bool Equipped
        {
            get { return _bEquipped; }
            set { _bEquipped = value; }
        }

        public override bool PartEquipped { get; set; }
    }
}
