using System;
using System.Collections.Generic;

namespace Rawr
{
	public static class ArrayUtils
	{
		public enum CompareResult { LessThan, Equal, Unequal, GreaterThan }

        public static CompareResult And(CompareResult a, CompareResult b)
        {
            if(a==b)
            {
                return a;
            }
            if(a==CompareResult.Equal)
            {
                return b;
            }
            if(b==CompareResult.Equal)
            {
                return a;
            }
            return CompareResult.Unequal;
        }
     

		[Flags]
		public enum CompareOption { LessThan = 1, Equal = 2, GreaterThan = 4 }

		public static bool AllEqual(float[] lhs, float[] rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
			if (lhs.Length != rhs.Length) return false;
			
			for (int i = 0; i < lhs.Length; i++)
			{
				if (lhs[i] != rhs[i]) return false;
			}
			return true;
		}

		public static bool AllCompare(float[] lhs, float[] rhs, CompareOption comparison)
		{
			CompareResult compareResult;
			return AllCompare(lhs, rhs, comparison, out compareResult);
		}
		public static CompareResult AllCompare(float[] lhs, float[] rhs)
		{
			CompareResult compareResult;
			AllCompare(lhs, rhs, CompareOption.Equal | CompareOption.LessThan |
				CompareOption.GreaterThan, out compareResult);
			return compareResult;
		}
		public static bool AllCompare(float[] lhs, float[] rhs, CompareOption comparison, out CompareResult compareResult)
		{
			compareResult = CompareResult.Equal;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
			if (lhs.Length != rhs.Length) return false;

			bool allowLessThan = (comparison & CompareOption.LessThan) == CompareOption.LessThan;
			bool allowEqual = (comparison & CompareOption.Equal) == CompareOption.Equal;
			bool allowGreaterThan = (comparison & CompareOption.GreaterThan) == CompareOption.GreaterThan;

			if (allowEqual && ReferenceEquals(lhs, rhs)) return true;

			bool haveGreaterThan = false, haveLessThan = false;
			for (int i = 0; i < lhs.Length; i++)
			{
				int val = lhs[i].CompareTo(rhs[i]);
				if (val < 0)
				{
					haveLessThan = true;
					if (!allowLessThan) return false;
                    if (haveGreaterThan)
                    {
                        compareResult = CompareResult.Unequal;
                        return allowGreaterThan || allowLessThan || allowEqual;
                    }
                }
				else if (val > 0)
				{
					haveGreaterThan = true;
					if (!allowGreaterThan) return false;
                    if (haveLessThan)
                    {
                        compareResult = CompareResult.Unequal;
                        return allowGreaterThan || allowLessThan || allowEqual;
                    }
                }
			}
			if (haveGreaterThan && haveLessThan) compareResult = CompareResult.Unequal;
			else if (haveGreaterThan) compareResult = CompareResult.GreaterThan;
			else if (haveLessThan) compareResult = CompareResult.LessThan;
			else compareResult = CompareResult.Equal;

			return (allowGreaterThan && haveGreaterThan) ||
				(allowLessThan && haveLessThan) || allowEqual;
		}
	}
}
