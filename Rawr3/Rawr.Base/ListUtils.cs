using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;

namespace Rawr
{
    public static class ListUtils
    {

        public static T Find<T>(this IEnumerable<T> source, Predicate<T> match)
        {
            return source.FirstOrDefault(t => match(t));
        }

        public static int FindLastIndex<T>(this IList<T> source, Predicate<T> match)
        {
            return FindLastIndex(source, source.Count - 1, source.Count, match);
        }

        public static int FindLastIndex<T>(this IList<T> source, int startIndex, Predicate<T> match)
        {
            return FindLastIndex(source, startIndex, startIndex + 1, match);
        }

        public static int FindLastIndex<T>(this IList<T> source, int startIndex, int count, Predicate<T> match)
        {
            int num = startIndex - count;
            for (int i = startIndex; i > num; i--)
            {
                if (match(source[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<TOutput> ConvertAll<TInput, TOutput>(this IEnumerable<TInput> source, Converter<TInput, TOutput> converter)
        {
            return new List<TOutput>((from item in source select converter(item)));
        }

        public static bool Exists<T>(this IList<T> source, Predicate<T> match)
        {
            return FindIndex(source, match) != -1;
        }

        private class CastListImpl<T> : IList<T>
        {
            private System.Collections.IList list;

            public CastListImpl(System.Collections.IList source)
            {
                list = source;
            }

            #region IList<T> Members

            int IList<T>.IndexOf(T item)
            {
                return list.IndexOf(item);
            }

            void IList<T>.Insert(int index, T item)
            {
                list.Insert(index, item);
            }

            void IList<T>.RemoveAt(int index)
            {
                list.RemoveAt(index);
            }

            T IList<T>.this[int index]
            {
                get
                {
                    return (T)list[index];
                }
                set
                {
                    list[index] = value;
                }
            }

            #endregion

            #region ICollection<T> Members

            void ICollection<T>.Add(T item)
            {
                list.Add(item);
            }

            void ICollection<T>.Clear()
            {
                list.Clear();
            }

            bool ICollection<T>.Contains(T item)
            {
                return list.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            int ICollection<T>.Count
            {
                get { return list.Count; }
            }

            bool ICollection<T>.IsReadOnly
            {
                get { return list.IsReadOnly; }
            }

            bool ICollection<T>.Remove(T item)
            {
                list.Remove(item);
                return true;
            }

            #endregion

            #region IEnumerable<T> Members

            private class CastEnumerator : IEnumerator<T>
            {
                private System.Collections.IEnumerator enumerator;

                public CastEnumerator(System.Collections.IEnumerator source)
                {
                    enumerator = source;
                }

                #region IEnumerator<T> Members

                T IEnumerator<T>.Current
                {
                    get { return (T)enumerator.Current; }
                }

                #endregion

                #region IDisposable Members

                void IDisposable.Dispose()
                {
                    enumerator = null;
                }

                #endregion

                #region IEnumerator Members

                object System.Collections.IEnumerator.Current
                {
                    get { return enumerator.Current; }
                }

                bool System.Collections.IEnumerator.MoveNext()
                {
                    return enumerator.MoveNext();
                }

                void System.Collections.IEnumerator.Reset()
                {
                    enumerator.Reset();
                }

                #endregion
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return new CastEnumerator(list.GetEnumerator());
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }

            #endregion
        }

        public static IList<T> CastList<T>(this System.Collections.IList source)
        {
            return new CastListImpl<T>(source);
        }

        public static int FindIndex<T>(this IList<T> source, Predicate<T> match)
        {
            return FindIndex(source, 0, source.Count, match);
        }

        public static int FindIndex<T>(this IList<T> source, int startIndex, Predicate<T> match)
        {
            return FindIndex(source, startIndex, source.Count - startIndex, match);
		}

        public static int FindIndex<T>(this IList<T> source, int startIndex, int count, Predicate<T> match)
        {
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (match(source[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int RemoveAll<T>(this List<T> list, Predicate<T> match)
        {
            int index = 0;
            while ((index < list.Count) && !match(list[index]))
            {
                index++;
            }
            if (index >= list.Count)
            {
                return 0;
            }
            int num2 = index + 1;
            while (num2 < list.Count)
            {
                while ((num2 < list.Count) && match(list[num2]))
                {
                    num2++;
                }
                if (num2 < list.Count)
                {
                    list[index++] = list[num2++];
                }
            }
            int num3 = list.Count - index;
            list.RemoveRange(index, list.Count - index);
            return num3;
        }

        public static List<T> FindAll<T>(this List<T> list, Predicate<T> match)
        {
           return new List<T>(list.Where(t => match(t)));
        }
    }
}
