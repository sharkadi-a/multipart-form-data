using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    internal static class EnumerableHelper
    {
        public static bool In<T>(this T value, params T[] enumerable)
        {
            foreach (var item in enumerable)
            {
                if (item.Equals(value)) return true;
            }
            return false;
        }

        public static bool ContainsAny(this string value, params string[] substrings)
        {
            foreach (var item in substrings)
            {
                if (value.Contains(item)) return true;
            }
            return false;
        } 
    }
}
