using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter
{
    public class ComparisonCalculationHunter : ComparisonCalculationBase
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

        private float[] _subPoints = new float[] { 0f, 0f, 0f, 0f };
        private float _overallPoints = 0f;

        public override float[] SubPoints { get { return _subPoints; } set { _subPoints = value; } }

        public float HunterDPSPoints  { get { return _subPoints[0]; } set { _subPoints[0] = value; } }
        public float PetDPSPoints     { get { return _subPoints[1]; } set { _subPoints[1] = value; } }
        public float HunterSurvPoints { get { return _subPoints[2]; } set { _subPoints[2] = value; } }
        public float PetSurvPoints    { get { return _subPoints[3]; } set { _subPoints[3] = value; } }

        public override float OverallPoints { get { return _overallPoints; } set { _overallPoints = value; } }

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

        public override string ToString() {
            return string.Format("{0}: ({1}O {2}HD {3}PD {4}HS {5}PS)",
                Name, Math.Round(OverallPoints), Math.Round(HunterDPSPoints ), Math.Round(PetDPSPoints ),
                                                 Math.Round(HunterSurvPoints), Math.Round(PetSurvPoints));
        }
    }
}
