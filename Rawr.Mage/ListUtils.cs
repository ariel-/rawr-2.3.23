using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
	public static class ListUtils
	{
		public static List<T> Intersect<T>(List<T> left, List<T> right)
		{
			List<T> intersect = new List<T>();
			foreach (T item in left)
				if (right.Contains(item))
					intersect.Add(item);
			return intersect;
		}

        public static List<T> RemoveDuplicates<T>(List<T> inputList) { 
            Dictionary<T, int> uniqueStore = new Dictionary<T, int>();
            List<T> finalList = new List<T>();
            foreach (T currValue in inputList) 
            { 
                if (!uniqueStore.ContainsKey(currValue)) 
                {
                    uniqueStore.Add(currValue, 0);
                    finalList.Add(currValue);
                }
            }
            return finalList;
        }
	}
}
