using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtWarr
{
    public class ComparisonCalculationProtWarr : ComparisonCalculationBase
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

        private float[] _subPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        private float _overallPoints;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = Math.Min(value, float.MaxValue); }
        }

        public float SurvivalPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = Math.Min(value, float.MaxValue); OverallPoints = _subPoints[0] + _subPoints[1] + _subPoints[2]; }
        }

        public float MitigationPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = Math.Min(value, float.MaxValue); OverallPoints = _subPoints[0] + _subPoints[1] + _subPoints[2]; }
        }

        public float ThreatPoints
        {
            get { return _subPoints[2]; }
            set { _subPoints[2] = Math.Min(value, float.MaxValue); OverallPoints = _subPoints[0] + _subPoints[1] + _subPoints[2]; }
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
            return string.Format("{0}: ({1}O {2}M {3}S {4}T)", Name, Math.Round(OverallPoints), Math.Round(MitigationPoints), Math.Round(SurvivalPoints), Math.Round(ThreatPoints));
        }
    }
}