using LayerZero.Tools.Guard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Collections
{
    public static class SafeLinQ<T>
    {
        public static T? ElementAtSafe(IEnumerable<T> source, 
                                        int Index,
                                        [CallerArgumentExpression("source")] string sourceExpression = "")
        {
            source.NotNull();

            source.NotEmpty();

            source.HasIndex(Index);

            if (source is IList<T> list)
                return list[Index];

            var i = 0;
            foreach (var item in source)
                if(i++ == Index)
                    return item;

            return default;
                
        }

        public static Dictionary<TKey, T> ToDictionarySafe<TKey>(IEnumerable<T> source, Func<T, TKey> keySelector, bool Overwrite) where TKey : notnull
        {
            var dict = new Dictionary<TKey, T>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                if(Overwrite)
                    dict[key] = item;
                else if (!dict.ContainsKey(key))
                    dict[key] = item;
            }

            return dict;
        }

        public static IEnumerable<T> DistinctSafe(IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            return comparer != null ? source.Distinct(comparer) : source.Distinct();
        }

        public static IEnumerable<T> FallbackIfEmpty(IEnumerable<T> source, Func<T> fallbackFactory)
        {
            bool isEmpty = true;
            foreach (var item in source)
            {
                isEmpty = false;
                yield return item;
            }

            if (isEmpty)
                yield return fallbackFactory();
        }
    }
}
