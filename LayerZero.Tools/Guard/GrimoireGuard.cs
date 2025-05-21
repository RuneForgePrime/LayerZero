using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Guard
{
    /// <summary>
    /// Collections/sets/maps
    /// </summary>
    public static class GrimoireGuard
    {
        public static IEnumerable<T> NotEmpty<T>(this IEnumerable<T> source,
                                        [CallerArgumentExpression("source")] string sourceExpression = "")
        {
            if (!source.Any())
                throw new ArgumentException($"{sourceExpression} collection must not be empty.");

            return source;
        }

        public static IReadOnlyCollection<T> Unique<T>(this IReadOnlyCollection<T> source,
                                        [CallerArgumentExpression("source")] string sourceExpression = "")
        {
            if (source.Count != source.Distinct().Count())
                throw new ArgumentException($"{sourceExpression} contains duplicates.");

            return source;
        }
    }
}
