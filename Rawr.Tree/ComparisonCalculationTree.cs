
namespace Rawr.Tree
{
    public class ComparisonCalculationTree : ComparisonCalculationBase
    {
        public override string Name { get; set;}

        private string _desc = string.Empty;
        public override string Description
        {
            get { return _desc; }
            set { _desc = value; }
        }

        public override float OverallPoints { get; set; }

        private float[] subPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints
        {
            get { return subPoints; }
            set { subPoints = value; }
        }

        public float BurstPoints
        {
            get { return subPoints[0]; }
            set { subPoints[0] = value; }
        }

        public float SustainedPoints
        {
            get { return subPoints[1]; }
            set { subPoints[1] = value; }
        }

        public float SurvivalPoints
        {
            get { return subPoints[2]; }
            set { subPoints[2] = value; }
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
        
        public override bool Equipped { get; set; }
        public override bool PartEquipped { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: ({1:0.0}O {2:0.0}H {3:0.0}M {4:0.0}S )", Name, OverallPoints, BurstPoints, SustainedPoints, SurvivalPoints);
        }
    }
}
