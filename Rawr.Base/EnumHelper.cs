using System;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
#if RAWR3
using System.Linq;
#endif

namespace Rawr
{
    public static class EnumHelper
    {

#if SILVERLIGHT
        public static T[] GetValues<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<T> values = new List<T>();

            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                object value = field.GetValue(enumType);
                values.Add((T)value);
            }

            return values.ToArray();
        }

        public static object[] GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<object> values = new List<object>();

            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                object value = field.GetValue(enumType);
                values.Add(value);
            }

            return values.ToArray();
        }

        public static string[] GetNames(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<string> values = new List<string>();

            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                values.Add(field.Name);
            }

            return values.ToArray();
        }

        public static int GetCount(Type enumType)
        {
            return enumType.GetFields(BindingFlags.Public | BindingFlags.Static).Length;
        }

#else
        public static T[] GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static Array GetValues(Type enumType)
        {
            return Enum.GetValues(enumType);
        }

        public static string[] GetNames(Type enumType)
        {
            return Enum.GetNames(enumType);
        }

        public static int GetCount(Type enumType)
        {
            return Enum.GetNames(enumType).Length;
        }

#endif

    }

}
