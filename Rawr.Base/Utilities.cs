using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Rawr
{
    public static class Utilities
    {

        private const int BitsPerUInt32 = 32;


        public delegate bool EqualityComparison<T>(T x, T y);

        public delegate float FloatValueSource();


        /// <summary>
        /// Make a deep clone of the object
        /// </summary>
        /// <typeparam name="T">The type of object to be cloned</typeparam>
        /// <param name="obj">The object to be cloned</param>
        /// <returns>A clone of the object</returns>
        public static T Clone<T>(T obj)
        {
            T ret = default(T);
            if (obj != null)
            {
                //for deep cloning, this should be a binary serializer (which requires objects be marked as serializable; however,
                //we save our objects back out to disk with xml and don't store
                //any interal state that would be mised with xml serialization, so this should be fine.
                XmlSerializer cloner = new XmlSerializer(typeof(T));
                MemoryStream stream = new MemoryStream();
                cloner.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);
                ret = (T)cloner.Deserialize(stream);
            }
            return ret;
        }

        public static uint RotateLeft(uint value, int rotationBitCount)
        {
            int effectiveRotationBitCount = rotationBitCount % BitsPerUInt32;
            if (effectiveRotationBitCount < 0)
                effectiveRotationBitCount = BitsPerUInt32 - effectiveRotationBitCount;

            if (effectiveRotationBitCount == 0)
                return value;

            return (value << rotationBitCount) | (value >> (BitsPerUInt32 - rotationBitCount));
        }

        public static uint RotateRight(uint value, int rotationBitCount)
        {
            // We cannot just use -rotationBitCount to avoid overflow on int.MinValue
            return RotateLeft(value, -(rotationBitCount % BitsPerUInt32));
        }

        public static int RotateLeft(int value, int rotationBitCount)
        {
            return (int)RotateLeft((uint)value, rotationBitCount);
        }

        public static int RotateRight(int value, int rotationBitCount)
        {
            return (int)RotateRight((uint)value, rotationBitCount);
        }

        public static int CombineHashCodes(IEnumerable<int> hashCodes)
        {
            if (hashCodes == null)
                throw new ArgumentNullException("hashCodes");

            int combinedHashCode = 0;
            foreach (var hashCode in hashCodes)
                combinedHashCode = RotateLeft(combinedHashCode, 23) ^ hashCode;

            return combinedHashCode;
        }

        public static int CombineHashCodes(params int[] hashCodes)
        {
            return CombineHashCodes((IEnumerable<int>)hashCodes);
        }

        public static int GetCombinedHashCode(IEnumerable<object> objects)
        {
            if (objects == null)
                throw new ArgumentNullException("objects");

            List<int> hashCodes = new List<int>();
            foreach (object anObject in objects)
                hashCodes.Add(anObject == null ? 0 : anObject.GetHashCode());

            return CombineHashCodes(hashCodes);
        }

        public static int GetCombinedHashCode(params object[] objects)
        {
            return GetCombinedHashCode((IEnumerable<object>)objects);
        }

        public static int GetArrayHashCode<T>(T[] array)
        {
            if (array == null)
                return 0;

            object[] values = new object[array.Length];
            Array.Copy(array, values, array.Length);

            return GetCombinedHashCode(values);
        }

        public static bool AreArraysEqual<TElement>(TElement[] array1, TElement[] array2)
        {
            return AreArraysEqual(array1, array2, EqualityComparer<TElement>.Default);
        }

        public static bool AreArraysEqual<TElement>(
            TElement[] array1,
            TElement[] array2,
            IEqualityComparer<TElement> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            return AreArraysEqual(array1, array2, comparer.Equals);
        }

        public static bool AreArraysEqual<TElement>(
            TElement[] array1,
            TElement[] array2,
            EqualityComparison<TElement> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException("comparison");

            if (array1 == array2)
                return true;

            if ((array1 == null) || (array2 == null) || (array1.Length != array2.Length))
                return false;

            for (int elementIndex = 0; elementIndex < array1.Length; elementIndex++)
                if (!comparison(array1[elementIndex], array2[elementIndex]))
                    return false;

            return true;
        }

        /// <summary>
        /// Gets all permutations of a list of elements where all elements are considered different.
        /// </summary>
        /// <typeparam name="TElement">Element type</typeparam>
        /// <param name="elements">The list of elemnents to get permutations of. 
        /// All elements are considered different.</param>
        /// <returns></returns>
        public static IEnumerable<TElement[]> GetDifferentElementPermutations<TElement>(
            IList<TElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            int[] elementIndices = new int[elements.Count];
            for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
                elementIndices[elementIndex] = elementIndex;

            do yield return GetElementsByIndices(elements, elementIndices);
                while (TryMakeNextElementIndexPermutation(elementIndices));
        }

        /// <summary>
        /// Calculates weighted sum of two values.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="xWeight">The weight of the first value.</param>
        /// <param name="y">The second value.</param>
        /// <param name="yWeight">The weight of the second value.</param>
        /// <returns>The weighted sum.</returns>
        public static float GetWeightedSum(float x, float xWeight, float y, float yWeight)
        {
            if ((xWeight == 0) && (yWeight == 0))
                return 0;

            return (x * xWeight + y * yWeight) / (Math.Abs(xWeight) + Math.Abs(yWeight));
        }

        /// <summary>
        /// Calculates the weighted sum of any number of values.
        /// </summary>
        /// <param name="arguments">
        /// (val1, weight1, val2, weight2, ..., valN, weightN).
        /// </param>
        /// <returns>The weighted sum.</returns>
        public static float GetWeightedSum(params float[] arguments) {

            float numerator = 0f;
            float denominator = 0f;
            for (int i = 0; i < arguments.Length; i += 2) {
                numerator += arguments[i] * arguments[i + 1];
                denominator += Math.Abs(arguments[i + 1]);
            }
            return numerator / denominator;
        }

        /// <summary>
        /// Calculates the weighted sum of any number of values.
        /// </summary>
        /// <param name="arguments">
        /// (val1, weight1, val2, weight2, ..., valN, weightN).
        /// </param>
        /// <returns>The weighted sum.</returns>
        public static float GetWeightedSum(
            List<float> values, List<float> weights) {

            float numerator = 0f;
            float denominator = 0f;
            for (int i = 0; i < values.Count; ++i) {
                numerator += values[i] * weights[i];
                denominator += Math.Abs(weights[i]);
            }
            return numerator / denominator;
        }

        /// <summary>
        /// Calculates weighted sum of two values.
        /// The values are not directly provided.
        /// Delegates that calculate the values are provided instead.
        /// The values are not calculated if they have zero weight.
        /// </summary>
        /// <param name="x">The delegate that calculates the first value.</param>
        /// <param name="xWeight">The weight of the first value.</param>
        /// <param name="y">The delegate that calculates the second value.</param>
        /// <param name="yWeight">The weight of the second value.</param>
        /// <returns>The weighted sum.</returns>
        public static float GetWeightedSum(
            FloatValueSource x, 
            float xWeight, 
            FloatValueSource y, 
            float yWeight)
        {
            if (xWeight == 0)
            {
                if (yWeight == 0)
                    return 0;

                return y();
            }

            if (yWeight == 0)
                return x();

            return GetWeightedSum(x(), xWeight, y(), yWeight);
        }

        public static void SwapElements<T>(
            List<T> list,
            int index1,
            int index2) 
        {

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        private static TElement[] GetElementsByIndices<TElement>(
            IList<TElement> elements, 
            int[] elementIndices)
        {
            TElement[] result = new TElement[elements.Count];

            for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
                result[elementIndex] = elements[elementIndices[elementIndex]];

            return result;
        }

        private static bool TryMakeNextElementIndexPermutation(int[] elementIndices)
        {
            // Dijkstra lexicographical algorithm

            if (elementIndices.Length < 2)
                return false;

            // Find last index which precede a greater index
            int lastNotReversedIndexIndex = elementIndices.Length - 2;
            while (elementIndices[lastNotReversedIndexIndex] > elementIndices[lastNotReversedIndexIndex + 1])
            {
                lastNotReversedIndexIndex--;

                if (lastNotReversedIndexIndex < 0)
                    return false;
            }

            // Find last index which is greater than index at lastNotReversedIndexIndex.
            // It will be minimal index of all indices past lastNotReversedIndexIndex
            // which is greater than index at lastNotReversedIndexIndex.
            // Such index always exists.
            int lastGreaterIndexIndex = elementIndices.Length - 1;
            while (elementIndices[lastGreaterIndexIndex] < elementIndices[lastNotReversedIndexIndex])
                lastGreaterIndexIndex--;

            // Swap the two elements
            SwapElements(elementIndices, lastNotReversedIndexIndex, lastGreaterIndexIndex);

            // Reverse all elements at indices greater than lastNotReversedIndexIndex.
            // They become sorted.
            Array.Reverse(
                elementIndices,
                lastNotReversedIndexIndex + 1,
                elementIndices.Length - lastNotReversedIndexIndex - 1);

            return true;
        }

        private static void SwapElements<TElement>(TElement[] elements, int index1, int index2)
        {
            TElement element1 = elements[index1];
            elements[index1] = elements[index2];
            elements[index2] = element1;
        }

    }

}
