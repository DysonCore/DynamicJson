using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DysonCore.DynamicJson
{
    internal static class CollectionUtils
    {
        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        internal static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this IList<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this IReadOnlyList<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this ReadOnlyCollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this List<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this T[] source)
        {
            return source == null || source.Length == 0;
        }

        internal static bool IsNullOrEmpty<T>(this Queue<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this Stack<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this HashSet<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<T>(this ISet<T> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return source == null || source.Count == 0;
        }

        internal static bool IsNullOrEmpty<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
        {
            return source == null || source.Count == 0;
        }
    }
}