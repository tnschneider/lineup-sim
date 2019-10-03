using System.Collections.Generic;
using System.Linq;

namespace System
{
    static class Extensions
    {

        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                return false;

            return items.Contains(item);
        }

        public static List<List<T>> GetPermutations<T>(this List<T> list, int length)
        {
            if (length == 1) return list.Select(t => new List<T> { t }).ToList();

            var result = GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new List<T> { t2 }).ToList()).ToList();

            return result;
        }

    }
}